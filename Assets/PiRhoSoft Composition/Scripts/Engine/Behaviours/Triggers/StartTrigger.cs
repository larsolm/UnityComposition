using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "start-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Start Trigger")]
	public class StartTrigger : GraphRunner
	{
		void Start()
		{
			Run();
		}
	}
}
