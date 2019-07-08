using PiRhoSoft.Composition.Engine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewConnector : IEdgeConnectorListener
	{
		private readonly GraphViewNodeProvider _provider;
		private readonly GraphViewChange _graphViewChange;
		private readonly List<Edge> _edgesToCreate = new List<Edge>();
		private readonly List<GraphElement> _edgesToDelete = new List<GraphElement>();

		public GraphViewConnector(GraphViewNodeProvider provider)
		{
			_provider = provider;
			_graphViewChange.edgesToCreate = _edgesToCreate;
		}

		public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
		{
			_edgesToDelete.Clear();
			_edgesToCreate.Clear();
			_edgesToCreate.Add(edge);

			if (edge.input.capacity == Port.Capacity.Single)
			{
				foreach (var connection in edge.input.connections)
				{
					if (connection != edge)
						_edgesToDelete.Add(connection);
				}
			}

			if (edge.output.capacity == Port.Capacity.Single)
			{
				foreach (Edge connection2 in edge.output.connections)
				{
					if (connection2 != edge)
						_edgesToDelete.Add(connection2);
				}
			}

			if (_edgesToDelete.Count > 0)
				graphView.DeleteElements(_edgesToDelete);

			graphView.graphViewChanged(_graphViewChange);
		}

		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			// Create node?
		}
	}
}
