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

		private List<IEnumerator> _nodes = new List<IEnumerator>();

		public override Color NodeColor => Colors.Sequence;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			for (var i = 0; i < Nodes.Count; i++)
			{
				var node = Nodes[i];

				if (node != null)
				{
					var enumerator = graph.Run(node, variables, GetConnectionName(nameof(Nodes), i));
					_nodes.Add(enumerator);
				}
			}

			while (_nodes.Count > 0)
			{
				yield return null;

				for (var i = 0; i < _nodes.Count; i++)
				{
					// TODO: somehow respect Current (i.e WaitForSeconds, etc)? might need to run these on the
					// CompositionManager

					if (!_nodes[i].MoveNext())
						_nodes.RemoveAt(i--);
				}
			}
		}
	}
}