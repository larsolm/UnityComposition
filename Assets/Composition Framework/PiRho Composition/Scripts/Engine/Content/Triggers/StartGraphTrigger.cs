﻿using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "start-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Start Graph Trigger")]
	public sealed class StartGraphTrigger : GraphTrigger
	{
		void Start()
		{
			Run();
		}
	}
}