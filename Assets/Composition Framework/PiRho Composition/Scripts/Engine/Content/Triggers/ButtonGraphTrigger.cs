﻿using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "button-graph-trigger")]
	[AddComponentMenu("PiRho Soft/Composition/Button Graph Trigger")]
	public sealed class ButtonGraphTrigger : GraphTrigger
	{
		[Tooltip("The name of the Input Button that will trigger the graph")]
		public string Button;

		void Update()
		{
			if (InputHelper.GetWasButtonPressed(Button))
				Run();
		}
	}
}