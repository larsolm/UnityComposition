﻿using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "enable-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Enable Graph Trigger")]
	public sealed class EnableGraphTrigger : GraphTrigger
	{
		void OnEnable()
		{
			Run();
		}
	}
}