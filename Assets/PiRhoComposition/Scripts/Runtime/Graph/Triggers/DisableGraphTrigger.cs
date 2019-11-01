using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "disable-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Disable Graph Trigger")]
	public class DisableGraphTrigger : GraphTrigger
	{
		void OnDisable()
		{
			Run();
		}
	}
}
