﻿using PiRhoSoft.Utilities;
using System.Collections;
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
		public VariableAssignmentReference ObjectVariable = new VariableAssignmentReference();

		[Tooltip("How to create and position the object, with an exact position, relative to another object, or as a child of another object")]
		[EnumButtons]
		public ObjectPositioning Positioning = ObjectPositioning.Absolute;

		[Tooltip("The object to position the created object relative to")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Relative)]
		public VariableLookupReference Object = new VariableLookupReference();

		[Tooltip("The parent object to make the created object a child of")]
		[Conditional(nameof(Positioning), (int)ObjectPositioning.Child)]
		public VariableLookupReference Parent = new VariableLookupReference();

		[Tooltip("The position to spawn the object at")]
		[Inline]
		public Vector3VariableSource Position = new Vector3VariableSource();

		[Tooltip("The rotation to spawn the object at")]
		[Inline]
		public Vector3VariableSource Rotation = new Vector3VariableSource();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Prefab, out GameObject prefab))
			{
				GameObject spawned = null;

				variables.Resolve(this, Position, out var position);
				variables.Resolve(this, Rotation, out var rotation);

				if (Positioning == ObjectPositioning.Absolute)
				{
					spawned = Instantiate(prefab, position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Relative)
				{
					if (variables.ResolveObject(this, Object, out GameObject obj))
						spawned = Instantiate(prefab, obj.transform.position + position, Quaternion.Euler(rotation));
				}
				else if (Positioning == ObjectPositioning.Child)
				{
					if (variables.ResolveObject(this, Parent, out GameObject parent))
						spawned = Instantiate(prefab, parent.transform.position + position, Quaternion.Euler(rotation), parent.transform);
				}

				if (spawned)
				{
					if (variables.Resolve(this, ObjectName, out var objectName) && !string.IsNullOrEmpty(objectName))
						spawned.name = objectName;

					if (ObjectVariable.IsAssigned)
						variables.Assign(this, ObjectVariable, Variable.Object(spawned));
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
