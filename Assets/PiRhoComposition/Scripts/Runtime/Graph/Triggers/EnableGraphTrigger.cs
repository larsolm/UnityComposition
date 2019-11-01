using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "enable-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Enable Graph Trigger")]
	public class EnableGraphTrigger : GraphTrigger
	{
		void OnEnable()
		{
			Run();
		}
	}
}
