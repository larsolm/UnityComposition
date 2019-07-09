using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewConnector : IEdgeConnectorListener
	{
		private readonly GraphViewNodeProvider _provider;
		private readonly GraphViewChange _graphViewChange;

		public GraphViewConnector(GraphViewNodeProvider provider)
		{
			_provider = provider;
			_graphViewChange.edgesToCreate = new List<Edge>();
			_graphViewChange.elementsToRemove = new List<GraphElement>();
		}

		public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
		{
			_graphViewChange.edgesToCreate.Add(edge);

			if (edge.input.capacity == Port.Capacity.Single)
			{
				foreach (var connection in edge.input.connections)
				{
					if (connection != edge)
						_graphViewChange.elementsToRemove.Add(connection);
				}
			}

			foreach (Edge connection in edge.output.connections)
			{
				if (connection != edge)
					_graphViewChange.elementsToRemove.Add(connection);
			}

			graphView.graphViewChanged(_graphViewChange);

			_graphViewChange.elementsToRemove.Clear();
			_graphViewChange.edgesToCreate.Clear();
		}

		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
		}
	}
}
