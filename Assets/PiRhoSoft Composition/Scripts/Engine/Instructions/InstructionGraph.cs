using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class InstructionGraph : Instruction
	{
		public const string _processFailedError = "(CCIGPF) Failed to process Node '{0}': the Node yielded a value other than null or IEnumerator";

		[Tooltip("The list of all nodes in this graph")]
		[ListDisplay(AllowAdd = false, AllowCollapse = false, AllowRemove = false, AllowReorder = false, EmptyText = "Use the Instruction Graph Window to add nodes to this graph")]
		[SerializeField] [HideInInspector] // editor draws this manually so it shows up at the bottom for derived classes
		private InstructionGraphNodeList _nodes = new InstructionGraphNodeList();

		private Stack<NodeFrame> _callstack = new Stack<NodeFrame>();
		private InstructionStore _rootStore;
		private NodeFrame _nextNode;
		private bool _shouldBreak = false;

		public InstructionGraphNodeList Nodes => _nodes; // _nodes is private with a getter so it isn't found by node data reflection

		public bool IsExecutionImmediate
		{
			get
			{
				foreach (var node in Nodes)
				{
					if (!IsImmediate(node))
						return false;
				}

				return true;
			}
		}

		public bool IsImmediate(InstructionGraphNode node)
		{
			if (node is IImmediate)
				return true;
			else if (node is IIsImmediate isImmediate)
				return isImmediate.IsExecutionImmediate;
			else
				return false;
		}

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			foreach (var node in Nodes)
			{
				if (InstructionStore.IsInput(node.This))
					inputs.Add(VariableDefinition.Create(node.This.RootName, VariableType.Store));

				node.GetInputs(inputs);
			}
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			foreach (var node in Nodes)
				node.GetOutputs(outputs);
		}

		public void GoTo(InstructionGraphNode node, object thisObject, string name)
		{
			switch (node)
			{
				case ILoopNode loop: _nextNode.Type = NodeType.Loop; break;
				case ISequenceNode sequence: _nextNode.Type = NodeType.Sequence; break;
				default: _nextNode.Type = NodeType.Normal; break;
			}

			_nextNode.Iteration = 0;
			_nextNode.Node = node;
			_nextNode.This = thisObject;
			_nextNode.Source = name;
		}

		public void GoTo(InstructionGraphNode node, object thisObject, string name, int index)
		{
			var source = string.Format("{0} {1}", name, index);
			GoTo(node, thisObject, source);
		}

		public void GoTo(InstructionGraphNode node, object thisObject, string name, string key)
		{
			var source = string.Format("{0} {1}", name, key);
			GoTo(node, thisObject, source);
		}

		public void Break()
		{
			_shouldBreak = true;
		}

		protected IEnumerator Run(InstructionStore variables, InstructionGraphNode root, string source)
		{
			StartRunning(root, source);

			var rootThis = variables.This;
			_rootStore = variables;
			GoTo(root, _rootStore.This, source);

			while (ShouldContinue())
			{
				var frame = SetupFrame(_nextNode);
				_nextNode.Node = null;

				if (frame.Node != null)
				{
					_callstack.Push(frame);
					_rootStore.ChangeThis(frame.This);

					yield return ProcessFrame(frame);
				}

				if (_shouldBreak)
				{
					HandleBreak();
					_shouldBreak = false;
				}
			}

			_callstack.Clear();
			variables.ChangeThis(rootThis);
		}

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
			public object This;
			public string Source;
		}

#if UNITY_EDITOR

		private string _currentBranch;

		private void StartRunning(InstructionGraphNode root, string source)
		{
			_currentBranch = source;

			DebugState = PlaybackState.Running;

			if (IsDebugLoggingEnabled)
				Debug.LogFormat(this, "Instruction Graph {0}: running branch '{1}'", name, source);
		}

		private bool ShouldContinue()
		{
			if (IsDebugLoggingEnabled)
			{
				if (DebugState == PlaybackState.Stopped)
					Debug.LogFormat(this, "Instruction Graph {0}: halting branch '{1}'", name, _currentBranch);
				else if (_callstack.Count == 0 && _nextNode.Node == null)
					Debug.LogFormat(this, "Instruction Graph {0}: finished running branch '{1}'", name, _currentBranch);
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
				Debug.LogFormat(this, "Instruction Graph {0}: pausing at node '{1}'", name, frame.Node.Name);

			while (DebugState == PlaybackState.Paused)
				yield return null;

			if (DebugState == PlaybackState.Stopped)
				yield break;

			if (IsDebugLoggingEnabled)
			{
				if (frame.Iteration > 0)
					Debug.LogFormat(this, "Instruction Graph {0}: running iteration {1} of node '{2}' ", name, frame.Iteration + 1, frame.Node.Name);
				else
					Debug.LogFormat(this, "Instruction Graph {0}: following '{1}' to node '{2}'", name, frame.Source, frame.Node.Name);
			}

			if (IsImmediate(frame.Node))
				RunEnumerator(frame.Node, frame.Node.Run(this, _rootStore, frame.Iteration));
			else
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
				// check if there is a sequence or loop node in the call stack to iterate

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Type != NodeType.Normal)
						return new NodeFrame { Type = frame.Type, Iteration = frame.Iteration + 1, Node = frame.Node, Source = frame.Source, This = frame.This };
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
						return new NodeFrame { Type = frame.Type, Iteration = frame.Iteration + 1, Node = frame.Node, Source = frame.Source, This = frame.This };
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

		private void RunEnumerator(InstructionGraphNode node, IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
					case null: break;
					case IEnumerator child: RunEnumerator(node, child); break;
					default: Debug.LogErrorFormat(node, _processFailedError, node.Name); break;
				}
			}
		}

		private void HandleBreak()
		{
			while (_callstack.Count > 0)
			{
				var loop = _callstack.Pop();

				if (loop.Type == NodeType.Loop)
				{
					_callstack.Push(new NodeFrame { Type = NodeType.Normal, Node = loop.Node, Source = loop.Source, This = loop.This, Iteration = loop.Iteration });
					GoTo(null, null, "");
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
