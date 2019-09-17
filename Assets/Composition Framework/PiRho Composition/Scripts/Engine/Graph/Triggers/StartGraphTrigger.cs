using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "start-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Start Graph Trigger")]
	public sealed class StartGraphTrigger : GraphTrigger
	{
		void Start()
		{
			Run();
		}
	}
}
