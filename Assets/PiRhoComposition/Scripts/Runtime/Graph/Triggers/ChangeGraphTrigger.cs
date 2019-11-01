using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "change-graph-trigger")]
	[AddComponentMenu("PiRho Composition/Graph Triggers/Change Graph Trigger")]
	public class ChangeGraphTrigger : GraphTrigger
	{
		[Tooltip("The Variable to watch for changes on")]
		public VariableLookupReference Variable = new VariableLookupReference();

		private Variable _value;

		void Start()
		{
			_value = Variable.GetValue(_variables);
		}

		void Update()
		{
			var value = Variable.GetValue(_variables);
			if (!VariableHandler.IsEqual(value, _value).GetValueOrDefault(false))
			{
				_value = value;
				Run();
			}
		}
	}
}
