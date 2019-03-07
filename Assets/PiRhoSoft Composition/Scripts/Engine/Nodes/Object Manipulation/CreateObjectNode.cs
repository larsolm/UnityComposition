using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Create Object", 0)]
	[HelpURL(Composition.DocumentationUrl + "create-object-node")]
	public class CreateObjectNode : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(COMCONMO) Unable to create object for {0}: the specified object could not be found";
		private const string _missingParentWarning = "(COMCONMP) Unable to assign parent object for {0}: the specified object '{1}' could not be found";
		private const string _missingNameWarning = "(COMCONMN) Unable to assign name for {0}: the specified name could not could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The prefab object to spawn")]
		[InlineDisplay(PropagateLabel = true)]
		public GameObjectVariableSource Prefab = new GameObjectVariableSource();

		[Tooltip("The name of the new object")]
		[InlineDisplay(PropagateLabel = true)]
		public StringVariableSource ObjectName = new StringVariableSource("Spawned Object");

		[Tooltip("The parent object to attach the object to (optional) - if set, position will be in local space")]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at - in local space if parent is set")]
		public Vector2 Position;

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Prefab.GetInputs(inputs);
			ObjectName.GetInputs(inputs);

			if (InstructionStore.IsInput(Parent))
				inputs.Add(VariableDefinition.Create<GameObject>(Parent.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Prefab.TryGetValue(variables, this, out var prefab))
			{
				GameObject parent = null;

				if (Parent.IsAssigned && !Parent.GetValue(variables).TryGetObject(out parent))
					Debug.LogWarningFormat(this, _missingParentWarning, Name, Parent);

				var spawned = parent ? Instantiate(prefab, parent.transform.position + (Vector3)Position, Quaternion.identity, parent.transform) : Instantiate(prefab, Position, Quaternion.identity);
				if (ObjectName.TryGetValue(variables, this, out var objectName))
					spawned.name = objectName;
				else
					Debug.LogWarningFormat(this, _missingNameWarning, Name, Parent);

				graph.GoTo(Next, spawned, nameof(Next));
			}
			else
			{
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);
				graph.GoTo(Next, variables.This, nameof(Next));
			}

			yield break;
		}
	}
}
