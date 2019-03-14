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
			Relative,
			Child
		}

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The prefab object to spawn")]
		[InlineDisplay(PropagateLabel = true)]
		public GameObjectVariableSource Prefab = new GameObjectVariableSource();

		[Tooltip("The name of the new object")]
		[InlineDisplay(PropagateLabel = true)]
		public StringVariableSource ObjectName = new StringVariableSource("Spawned Object");

		[Tooltip("A variable reference to assign the created object to so that it can be referenced later")]
		[ConditionalDisplaySelf(nameof(Positioning), EnumValue = (int)ObjectPositioning.Child)]
		public VariableReference ObjectVariable = new VariableReference();

		[Tooltip("How to create and position the object, with an exact position, relative to another object, or as a child of another object")]
		[EnumButtons]
		public ObjectPositioning Positioning = ObjectPositioning.Absolute;

		[Tooltip("The object to position the created object relative to")]
		[ConditionalDisplaySelf(nameof(Positioning), EnumValue = (int)ObjectPositioning.Relative)]
		public VariableReference Object = new VariableReference();

		[Tooltip("The parent object to make the created object a child of")]
		[ConditionalDisplaySelf(nameof(Positioning), EnumValue = (int)ObjectPositioning.Child)]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at")]
		public Vector3 Position;

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			Prefab.GetInputs(inputs);
			ObjectName.GetInputs(inputs);

			if (Positioning == ObjectPositioning.Child && InstructionStore.IsInput(Parent))
				inputs.Add(VariableDefinition.Create<GameObject>(Parent.RootName));

			if (Positioning == ObjectPositioning.Relative && InstructionStore.IsInput(Object))
				inputs.Add(VariableDefinition.Create<GameObject>(Object.RootName));
		}

		public override void GetOutputs(List<VariableDefinition> outputs)
		{
			if (InstructionStore.IsOutput(ObjectVariable))
				outputs.Add(VariableDefinition.Create<GameObject>(ObjectVariable.RootName));
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
				else if (Positioning == ObjectPositioning.Relative)
				{
					if (Resolve<GameObject>(variables, Object, out var obj))
						spawned = Instantiate(prefab, obj.transform.position + Position, Quaternion.identity);
				}
				else if (Positioning == ObjectPositioning.Child)
				{
					if (Resolve<GameObject>(variables, Parent, out var parent))
						spawned = Instantiate(prefab, parent.transform.position + Position, Quaternion.identity, parent.transform);
				}

				if (spawned)
				{
					if (Resolve(variables, ObjectName, out var objectName) && !string.IsNullOrEmpty(objectName))
						spawned.name = objectName;

					if (ObjectVariable.IsAssigned)
						Assign(variables, ObjectVariable, VariableValue.Create(spawned));
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
