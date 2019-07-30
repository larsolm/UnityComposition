using PiRhoSoft.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Create Game Object", 0)]
	[HelpURL(Configuration.DocumentationUrl + "create-game-object-node")]
	public class CreateGameObjectNode : GraphNode
	{
		public enum ObjectPositioning
		{
			Absolute,
			Relative,
			Child
		}

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The prefab object to spawn")]
		[Inline]
		public GameObjectVariableSource Prefab = new GameObjectVariableSource();

		[Tooltip("The name of the new object")]
		[Inline]
		public StringVariableSource ObjectName = new StringVariableSource("Spawned Object");

		[Tooltip("A variable reference to assign the created object to so that it can be referenced later")]
		public VariableReference ObjectVariable = new VariableReference();

		[Tooltip("How to create and position the object, with an exact position, relative to another object, or as a child of another object")]
		[EnumButtons]
		public ObjectPositioning Positioning = ObjectPositioning.Absolute;

		[Tooltip("The object to position the created object relative to")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Relative)]
		public VariableReference Object = new VariableReference();

		[Tooltip("The parent object to make the created object a child of")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Child)]
		public VariableReference Parent = new VariableReference();

		[Tooltip("The position to spawn the object at")]
		[Inline]
		public Vector3VariableSource Position = new Vector3VariableSource();

		[Tooltip("The rotation to spawn the object at")]
		[Inline]
		public Vector3VariableSource Rotation = new Vector3VariableSource();

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			Prefab.GetInputs(inputs);
			ObjectName.GetInputs(inputs);
			Position.GetInputs(inputs);
			Rotation.GetInputs(inputs);

			if (Positioning == ObjectPositioning.Child && GraphStore.IsInput(Parent))
				inputs.Add(new VariableDefinition(Parent.RootName, new ObjectConstraint(typeof(GameObject))));

			if (Positioning == ObjectPositioning.Relative && GraphStore.IsInput(Object))
				inputs.Add(new VariableDefinition(Object.RootName, new ObjectConstraint(typeof(GameObject))));
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (GraphStore.IsOutput(ObjectVariable))
				outputs.Add(new VariableDefinition(ObjectVariable.RootName, new ObjectConstraint(typeof(GameObject))));
		}

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (ResolveObject(variables, Prefab, out GameObject prefab))
			{
				GameObject spawned = null;

				Resolve(variables, Position, out var position);
				Resolve(variables, Rotation, out var rotation);

				if (Positioning == ObjectPositioning.Absolute)
				{
					spawned = Instantiate(prefab, position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Relative)
				{
					if (ResolveObject(variables, Object, out GameObject obj))
						spawned = Instantiate(prefab, obj.transform.position + position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Child)
				{
					if (ResolveObject(variables, Parent, out GameObject parent))
						spawned = Instantiate(prefab, parent.transform.position + position, Quaternion.Euler(rotation), parent.transform);
				}

				if (spawned)
				{
					if (Resolve(variables, ObjectName, out var objectName) && !string.IsNullOrEmpty(objectName))
						spawned.name = objectName;

					if (ObjectVariable.IsAssigned)
						Assign(variables, ObjectVariable, Variable.Object(spawned));
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
