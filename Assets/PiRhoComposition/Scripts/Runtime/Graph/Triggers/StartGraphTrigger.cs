using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "start-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Start Graph Trigger")]
	public class StartGraphTrigger : GraphTrigger
	{
		void Start()
		{
			Run();
		}
	}
}
