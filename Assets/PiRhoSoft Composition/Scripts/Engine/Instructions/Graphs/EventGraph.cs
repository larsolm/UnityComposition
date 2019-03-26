using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateAssetMenu(fileName = nameof(EventGraph), menuName = "PiRho Soft/Graphs/Events", order = 102)]
	[HelpURL(Composition.DocumentationUrl + "event-graph")]
	public class EventGraph : InstructionGraph
	{
		private const string _invalidEventWarning = "(CIGEGIE) Unable to run event graph '{0}': the event '{1}' could not be found";

		[NonSerialized] public string CurrentEvent;

		[Tooltip("The root nodes")]
		[DictionaryDisplay(AllowCollapse = false, EmptyText = "Add nodes that will be triggered based on event name")]
		public InstructionGraphNodeDictionary Events = new InstructionGraphNodeDictionary();

		protected override IEnumerator Run(InstructionStore variables)
		{
			if (Events.TryGetValue(CurrentEvent, out var node))
				yield return Run(variables, node, CurrentEvent);
			else
				Debug.LogWarningFormat(this, _invalidEventWarning, name, CurrentEvent);
		}
	}
}
