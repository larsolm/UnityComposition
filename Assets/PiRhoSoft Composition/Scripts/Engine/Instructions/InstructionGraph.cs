using PiRhoSoft.UtilityEngine;
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
			StartRunning();

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

					yield return ProcessNode(frame.Node, frame.Iteration);
				}

				if (_shouldBreak)
				{
					ProcessBreak();
					_shouldBreak = false;
				}
			}

			_callstack.Clear();
			variables.ChangeThis(rootThis);
		}

		private void ProcessBreak()
		{
			while (_callstack.Count > 0)
			{
				var loop = _callstack.Pop();

				if (loop.Type == NodeType.Loop)
				{
					var target = (loop.Node as ILoopNode).GetBreakNode();

					_callstack.Push(new NodeFrame { Type = NodeType.Normal, Node = loop.Node, Source = loop.Source, This = loop.This, Iteration = loop.Iteration });
					GoTo(target.Node, loop.This, target.Name);
					break;
				}
			}
		}

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

		private bool IsNodeInStack(InstructionGraphNode node)
		{
			foreach (var frame in _callstack)
			{
				if (frame.Node == node)
					return true;
			}

			return false;
		}

		#region Playback Interface

		private IEnumerator RunNode(InstructionGraphNode node, int iteration)
		{
			if (IsImmediate(node))
				RunEnumerator(node, node.Run(this, _rootStore, iteration));
			else
				yield return node.Run(this, _rootStore, iteration);
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

#if UNITY_EDITOR

		public void StartRunning()
		{
			DebugState = PlaybackState.Running;
		}

		public bool ShouldContinue()
		{
			return DebugState != PlaybackState.Stopped && (_callstack.Count > 0 || _nextNode.Node != null);
		}

		public IEnumerator ProcessNode(InstructionGraphNode node, int iteration)
		{
			if (node.IsBreakpoint && IsDebugBreakEnabled)
				DebugState = PlaybackState.Paused;

			while (DebugState == PlaybackState.Paused)
				yield return null;

			if (DebugState == PlaybackState.Stopped)
				yield break;

			yield return RunNode(node, iteration);

			if (DebugState == PlaybackState.Step)
				DebugState = PlaybackState.Paused;
		}

#else

		public void StartRunning()
		{
		}

		public bool IsRunning()
		{
			return _callstack.Count > 0 || _nextNode.Node != null;
		}

		public IEnumerator ProcessNode(InstructionGraphNode node, int iteration)
		{
			yield return RunNode(node, iteration);
		}

#endif

		#endregion

		#region Editor Interface

		public enum PlaybackState
		{
			Running,
			Paused,
			Step,
			Stopped
		}

		[HideInInspector] public Vector2 StartPosition;

		public PlaybackState DebugState { get; private set; }

		public virtual void GetConnections(InstructionGraphNode.NodeData data)
		{
			data.AddConnections(this);
		}

		public virtual void SetConnection(InstructionGraphNode.ConnectionData connection, InstructionGraphNode target)
		{
			connection.ApplyConnection(this, target);
		}

#if UNITY_EDITOR

		public static bool IsDebugBreakEnabled = true;
		public static bool IsDebugLoggingEnabled = true;

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

		public bool IsInCallStack(InstructionGraphNode node)
		{
			if (_callstack.Count > 0)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node)
						return true;
				}
			}

			return false;
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
	}
}
