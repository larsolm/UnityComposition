using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ConnectionChangeGraphViewNode : DefaultGraphViewNode
	{
		public ConnectionChangeGraphViewNode(GraphNode node, GraphViewConnector nodeConnector, bool isStart) : base(node, nodeConnector, isStart)
		{
		}
	}
}
