using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class InstructionGraph : Instruction
	{
		private const string _processFailedError = "(CCIGPF) Failed to process Node '{0}': the Node yielded a value other than null or IEnumerator";

		[Tooltip("The list of all nodes in this graph")]
		[ListDisplay(AllowAdd = false, AllowRemove = false, AllowReorder = false, EmptyText = "Use the Instruction Graph Window to add nodes to this graph")]
		[SerializeField] [HideInInspector] // editor draws this manually so it shows up at the bottom for derived classes
		private InstructionGraphNodeList _nodes = new InstructionGraphNodeList();

		private Stack<NodeFrame> _callstack = new Stack<NodeFrame>();
		private InstructionStore _rootStore;
		private NodeFrame _nextNode;
		private bool _shouldBreak = false;

		public InstructionGraphNodeList Nodes => _nodes; // _nodes is private with a getter so it isn't found by node data reflection

		protected override void GetInputs(IList<VariableDefinition> inputs)
		{
			foreach (var node in Nodes)
				node.GetInputs(inputs);
		}

		protected override void GetOutputs(IList<VariableDefinition> outputs)
		{
			foreach (var node in Nodes)
				node.GetOutputs(outputs);
		}

		protected IEnumerator Run(InstructionStore variables, InstructionGraphNode root, string source)
		{
			_rootStore = variables;

			StartRunning(root, source);
			GoTo(root, source);

			while (ShouldContinue())
			{
				var frame = SetupFrame(_nextNode);
				_nextNode.Reset();

				if (frame.Node != null)
				{
					_callstack.Push(frame);

					yield return ProcessFrame(frame);
				}

				if (_shouldBreak)
				{
					HandleBreak();
					_shouldBreak = false;
				}
			}

			_callstack.Clear();
			_nextNode.Reset();
		}

		#region Traversal

		public void GoTo(InstructionGraphNode node, string name)
		{
			switch (node)
			{
				case ILoopNode loop: _nextNode.Type = NodeType.Loop; break;
				case ISequenceNode sequence: _nextNode.Type = NodeType.Sequence; break;
				default: _nextNode.Type = NodeType.Normal; break;
			}

			_nextNode.Node = node;
			_nextNode.Source = name;
		}

		public void GoTo(InstructionGraphNode node, string name, int index)
		{
			var source = string.Format("{0} {1}", name, index);
			GoTo(node, source);
		}

		public void GoTo(InstructionGraphNode node, string name, string key)
		{
			var source = string.Format("{0} {1}", name, key);
			GoTo(node, source);
		}

		public void Break()
		{
			_shouldBreak = true;
		}

		#endregion

		#region Playback

		private enum NodeType
		{
			Normal,
			Sequence,
			Loop
		}

		private struct NodeFrame
		{
			public NodeType Type;
			public int Iteration;
			public InstructionGraphNode Node;
			public string Source;

			public NodeFrame Increment()
			{
				var frame = this;
				frame.Iteration++;
				return frame;
			}

			public NodeFrame Break()
			{
				var frame = this;
				frame.Type = NodeType.Normal;
				return frame;
			}

			public void Reset()
			{
				Iteration = 0;
				Node = null;
			}
		}

#if UNITY_EDITOR

		private string _currentBranch;

		private void StartRunning(InstructionGraphNode root, string source)
		{
			_currentBranch = source;

			DebugState = PlaybackState.Running;

			if (IsDebugLoggingEnabled)
				Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: running branch '{2}'", Time.frameCount, name, source);
		}

		private bool ShouldContinue()
		{
			if (IsDebugLoggingEnabled)
			{
				if (DebugState == PlaybackState.Stopped)
					Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: halting branch '{2}'", Time.frameCount, name, _currentBranch);
				else if (_callstack.Count == 0 && _nextNode.Node == null)
					Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: finished running branch '{2}'", Time.frameCount, name, _currentBranch);
			}

			return DebugState != PlaybackState.Stopped && (_callstack.Count > 0 || _nextNode.Node != null);
		}

		private IEnumerator ProcessFrame(NodeFrame frame)
		{
			if (frame.Node.IsBreakpoint && IsDebugBreakEnabled)
			{
				DebugState = PlaybackState.Paused;
				OnBreakpointHit?.Invoke(this, frame.Node);
			}

			if (DebugState == PlaybackState.Paused && IsDebugLoggingEnabled)
				Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: pausing at node '{2}'", Time.frameCount, name, frame.Node.Name);

			while (DebugState == PlaybackState.Paused)
				yield return null;

			if (DebugState == PlaybackState.Stopped)
				yield break;

			if (IsDebugLoggingEnabled)
			{
				if (frame.Iteration > 0)
					Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: running iteration {2} of node '{3}' ", Time.frameCount, name, frame.Iteration + 1, frame.Node.Name);
				else
					Debug.LogFormat(this, "(Frame {0}) Instruction Graph {1}: following '{2}' to node '{3}'", Time.frameCount, name, frame.Source, frame.Node.Name);
			}

			yield return frame.Node.Run(this, _rootStore, frame.Iteration);

			if (DebugState == PlaybackState.Step)
				DebugState = PlaybackState.Paused;
		}

#else

		private void StartRunning(InstructionGraphNode root, string source)
		{
		}

		private bool ShouldContinue()
		{
			return _callstack.Count > 0 || _nextNode.Node != null;
		}

		private IEnumerator ProcessFrame(NodeFrame frame)
		{
			if (IsImmediate(frame.Node))
				RunEnumerator(frame.Node, frame.Node.Run(this, _rootStore, frame.Iteration));
			else
				yield return frame.Node.Run(this, _rootStore, frame.Iteration);
		}

#endif

		private NodeFrame SetupFrame(NodeFrame node)
		{
			if (node.Node == null)
			{
				// the current frame should never continue if there is no next

				if (_callstack.Count > 0)
					_callstack.Pop();

				// check if there is a sequence or loop node in the call stack to iterate

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Type != NodeType.Normal)
						return frame.Increment();
				}
			}
			else if (IsNodeInStack(node.Node))
			{
				// the node is already in the call stack, so retreat to the existing entry rather than adding a new one
				// and re-use its original variables - any loop or sequence nodes on the way are bypassed which is
				// probably the intended behavior

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Node == node.Node)
						return frame.Increment();
				}
			}

			return node;
		}

		private bool IsNodeInStack(InstructionGraphNode node)
		{
			foreach (var frame in _callstack)
			{
				if (frame.Node == node)
					return true;
			}

			return false;
		}

		private void HandleBreak()
		{
			while (_callstack.Count > 0)
			{
				var frame = _callstack.Pop();

				if (frame.Type == NodeType.Loop)
				{
					_callstack.Push(frame.Break());
					GoTo(null, string.Empty);
					break;
				}
			}
		}

		#endregion

		#region Debugging

#if UNITY_EDITOR

		public enum PlaybackState
		{
			Running,
			Paused,
			Step,
			Stopped
		}

		public PlaybackState DebugState { get; private set; }

		public static bool IsDebugBreakEnabled = true;
		public static bool IsDebugLoggingEnabled = false;
		public static Action<InstructionGraph, InstructionGraphNode> OnBreakpointHit;

		public bool CanDebugPlay => IsRunning && DebugState == PlaybackState.Paused;
		public bool CanDebugPause => IsRunning && DebugState == PlaybackState.Running;
		public bool CanDebugStep => IsRunning && DebugState == PlaybackState.Paused;
		public bool CanDebugStop => IsRunning && DebugState != PlaybackState.Stopped;

		public void DebugPlay()
		{
			if (CanDebugPlay)
				DebugState = PlaybackState.Running;
		}

		public void DebugPause()
		{
			if (CanDebugPause)
				DebugState = PlaybackState.Paused;
		}

		public void DebugStep()
		{
			if (CanDebugStep)
				DebugState = PlaybackState.Step;
		}

		public void DebugStop()
		{
			if (CanDebugStop)
				DebugState = PlaybackState.Stopped;
		}

		public int IsInCallStack(InstructionGraphNode node)
		{
			if (_callstack.Count > 0)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node)
					{
						if (frame.Type != NodeType.Normal)
							return frame.Iteration + 1;
						else
							return 0;
					}
				}
			}

			return -1;
		}

		public bool IsInCallStack(InstructionGraphNode node, string source)
		{
			if (_callstack.Count > 0)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node && frame.Source == source)
						return true;
				}
			}

			return false;
		}

		public bool IsExecuting(InstructionGraphNode node)
		{
			return _callstack.Count > 0 && _callstack.Peek().Node == node;
		}

#endif

		#endregion

		#region Editor Interface

		[HideInInspector] public Vector2 StartPosition;

		public virtual void GetConnections(InstructionGraphNode.NodeData data)
		{
			data.AddConnections(this);
		}

		public virtual void SetConnection(InstructionGraphNode.ConnectionData connection, InstructionGraphNode target)
		{
			connection.ApplyConnection(this, target);
		}

		#endregion
	}
}
