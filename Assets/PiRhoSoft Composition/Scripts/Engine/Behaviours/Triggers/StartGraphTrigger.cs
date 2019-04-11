using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "start-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Start Graph Trigger")]
	public class StartGraphTrigger : GraphTrigger
	{
		void Start()
		{
			Run();
		}
	}
}
