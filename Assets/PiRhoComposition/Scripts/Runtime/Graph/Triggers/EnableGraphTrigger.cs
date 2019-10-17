using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "enable-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Enable Graph Trigger")]
	public sealed class EnableGraphTrigger : GraphTrigger
	{
		void OnEnable()
		{
			Run();
		}
	}
}
