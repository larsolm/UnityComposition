using PiRhoSoft.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Parallel", 11)]
	[HelpURL(Configuration.DocumentationUrl + "parallel-node")]
	public class ParallelNode : GraphNode
	{
		[Tooltip("The nodes to visit all at the same time")]
		[List]
		public GraphNodeList Nodes = new GraphNodeList();

		private List<NodeState> _states = new List<NodeState>();

		public override Color NodeColor => Colors.Sequence;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			for (var i = 0; i < Nodes.Count; i++)
			{
				var node = Nodes[i];

				if (node != null)
				{
					var state = new NodeState();
					var enumerator = Run(graph, variables, node, GetConnectionName(nameof(Nodes), i), state);
					var coroutine = CompositionManager.Instance.RunEnumerator(enumerator);

					_states.Add(state);
				}
			}

			while (_states.Count > 0)
			{
				for (var i = 0; i < _states.Count; i++)
				{
					var state = _states[i];

					if (state.IsFinished)
						_states.RemoveAt(i--);
				}

				yield return null;
			}
		}

		private class NodeState
		{
			public bool IsFinished = false;
		}

		private IEnumerator Run(IGraphRunner graph, IVariableCollection variables, GraphNode node, string source, NodeState state)
		{
			yield return graph.Run(node, variables, source);
			state.IsFinished = true;
		}
	}
}
