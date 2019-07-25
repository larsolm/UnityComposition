using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Create Scriptable Object", 1)]
	[HelpURL(Composition.DocumentationUrl + "create-scriptable-object-node")]
	public class CreateScriptableObjectNode : GraphNode
	{
		private const string _invalidTypeError = "(CCSONIT) Failed to create object in node '{0}': the type '{1}' could not be found";
		private const string _invalidObjectError = "(CCSONIO) Failed to create object in node '{0}': an object of type '{1}' could not be instantiated";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The ScriptableObject type to create an instance of")]
		[TypePicker(typeof(ScriptableObject), false)]
		public string ScriptableObjectType;

		[Tooltip("A variable reference to assign the created object to so that it can be referenced later")]
		public VariableReference ObjectVariable = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (GraphStore.IsOutput(ObjectVariable))
				outputs.Add(new VariableDefinition(ObjectVariable.RootName, new ObjectConstraint(typeof(GameObject))));
		}

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			Type type;

			try
			{
				type = Type.GetType(ScriptableObjectType, false); // still throws in some cases
			}
			catch
			{
				type = null;
			}

			if (type != null)
			{
				var obj = CreateInstance(type);

				if (obj != null)
				{
					if (ObjectVariable.IsAssigned)
						Assign(variables, ObjectVariable, Variable.Object(obj));
				}
				else
				{
					Debug.LogErrorFormat(this, _invalidObjectError, name, ScriptableObjectType);
				}
			}
			else
			{
				Debug.LogErrorFormat(this, _invalidTypeError, name, ScriptableObjectType);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
