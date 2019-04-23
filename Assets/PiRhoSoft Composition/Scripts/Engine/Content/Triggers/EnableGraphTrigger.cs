﻿using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "enable-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Enable Graph Trigger")]
	public class EnableGraphTrigger : InstructionTrigger
	{
		void OnEnable()
		{
			Run();
		}
	}
}