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

		public InstructionGraphNodeList Nodes => _nodes; // _nodes is private with a getter so it isn't found by node data reflection

		public override bool IsExecutionImmediate
		{
			get
			{
				foreach (var node in Nodes)
				{
					if (!node.IsExecutionImmediate)
						return false;
				}

				return true;
			}
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

		public void GoTo(InstructionGraphNode node)
		{
			GoTo(node, _rootStore.This);
		}

		public void GoTo(InstructionGraphNode node, IVariableStore thisStore)
		{
			_nextNode.Node = node;
			_nextNode.Caller = thisStore;
			_nextNode.Iteration = 0;
			_nextNode.Break = false;
		}

		public void BreakTo(InstructionGraphNode node)
		{
			Break();
			GoTo(node);
		}

		private void Break()
		{
			// can't just peek - NodeFrame is a struct
			var caller = _callstack.Pop();

			if (caller.Node.ExecutionMode == InstructionGraphExecutionMode.Loop)
				caller.Break = true;
			else
				Break();

			_callstack.Push(caller);
		}

		protected IEnumerator Run(InstructionStore variables, InstructionGraphNode root)
		{
			_rootStore = variables;

			GoTo(root);

			while (_callstack.Count > 0 || _nextNode.Node != null)
			{
				var node = SetupNode(_nextNode);
				_nextNode.Node = null;

				if (node.Node != null)
				{
					_nextNode.Node = null;
					_callstack.Push(node);
					_rootStore.ChangeThis(node.Caller);

					if (node.Node.IsExecutionImmediate)
						Process(node.Node, node.Node.Run(this, _rootStore, node.Iteration));
					else
						yield return node.Node.Run(this, _rootStore, node.Iteration);
				}
			}
		}

		private NodeFrame SetupNode(NodeFrame node)
		{
			if (node.Node == null)
			{
				// check if there is a sequence or loop node in the call stack to iterate

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Node.ExecutionMode != InstructionGraphExecutionMode.Normal && !frame.Break)
						return new NodeFrame { Node = frame.Node, Caller = frame.Caller, Iteration = frame.Iteration + 1, Break = false };
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
						return new NodeFrame { Node = frame.Node, Caller = frame.Caller, Iteration = frame.Iteration + 1, Break = false };
				}
			}

			return node;
		}

		private void Process(InstructionGraphNode node, IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
					case null: break;
					case IEnumerator child: Process(node, child); break;
					default: Debug.LogErrorFormat(node, _processFailedError, node.Name); break;
				}
			}
		}

		private struct NodeFrame
		{
			public InstructionGraphNode Node;
			public IVariableStore Caller;
			public int Iteration;
			public bool Break;
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
