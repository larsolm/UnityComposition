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
		public enum ObjectPositioning
		{
			Absolute,
			RelativeToObject,
			ChildOfParent
		}

		private const string _missingObjectWarning = "(COMCONMO) Unable to create object for {0}: the specified object could not be found";
		private const string _missingParentWarning = "(COMCONMP) Unable to assign parent object for {0}: the parent '{1}' could not be found";
		private const string _missingRelativeWarning = "(COMCONMR) Unable to create object for {0}: the relative '{1}' could not be found";
		private const string _missingNameWarning = "(COMCONMN) Unable to assign name for {0}: the specified name could not could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The prefab object to spawn")]
		[InlineDisplay(PropagateLabel = true)]
		public GameObjectVariableSource Prefab = new GameObjectVariableSource();

		[Tooltip("The name of the new object")]
		[InlineDisplay(PropagateLabel = true)]
		public StringVariableSource ObjectName = new StringVariableSource("Spawned Object");

		[Tooltip("How to create and position the object, with an exact position, relative to another object, or as a child of another object")]
		public ObjectPositioning Positioning = ObjectPositioning.Absolute;

		[Tooltip("The object to position the created object relative to")]
		[ConditionalDisplaySelf(nameof(Positioning), EnumValue = (int)ObjectPositioning.RelativeToObject)]
		public VariableReference Object = new VariableReference();

		[Tooltip("The parent object to make the created object a child of")]
		[ConditionalDisplaySelf(nameof(Positioning), EnumValue = (int)ObjectPositioning.ChildOfParent)]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at")]
		public Vector3 Position;

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Prefab.GetInputs(inputs);
			ObjectName.GetInputs(inputs);

			if (Positioning == ObjectPositioning.ChildOfParent && InstructionStore.IsInput(Parent))
				inputs.Add(VariableDefinition.Create<GameObject>(Parent.RootName));

			if (Positioning == ObjectPositioning.RelativeToObject && InstructionStore.IsInput(Object))
				inputs.Add(VariableDefinition.Create<GameObject>(Object.RootName));
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve(variables, Prefab, out var prefab))
			{
				GameObject spawned = null;

				if (Positioning == ObjectPositioning.Absolute)
				{
					spawned = Instantiate(prefab, Position, Quaternion.identity);
				}
				else if (Positioning == ObjectPositioning.RelativeToObject)
				{
					if (Object.GetValue(variables).TryGetObject<GameObject>(out var obj))
						spawned = Instantiate(prefab, obj.transform.position + Position, Quaternion.identity);
					else
						Debug.LogWarningFormat(this, _missingRelativeWarning, Name, Object);
				}
				else if (Positioning == ObjectPositioning.ChildOfParent)
				{
					if (Parent.GetValue(variables).TryGetObject<GameObject>(out var parent))
						spawned = Instantiate(prefab, parent.transform.position + Position, Quaternion.identity, parent.transform);
					else
						Debug.LogWarningFormat(this, _missingParentWarning, Name, Parent);
				}

				if (spawned && Resolve(variables, ObjectName, out var objectName))
					spawned.name = objectName;
				else
					Debug.LogWarningFormat(this, _missingNameWarning, Name, Parent);

				graph.ChangeRoot(spawned);
				graph.GoTo(Next, nameof(Next));
			}
			else
			{
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);
				graph.GoTo(Next, nameof(Next));
			}

			yield break;
		}
	}
}
