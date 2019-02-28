using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Create Object", 0)]
	[HelpURL(Composition.DocumentationUrl + "create-object-node")]
	public class CreateObjectNode : InstructionGraphNode, IImmediate
	{
		private const string _objectNotFoundWarning = "(COMCOONF) Unable to create object for {0}: the object could not be found";
		private const string _parentNotFoundWarning = "(COMCOONF) Unable to assign parent object for {0}: the specified object '{1}' could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The prefab object to spawn")]
		public GameObject Prefab;

		[Tooltip("The name of the new object")]
		public string ObjectName;

		[Tooltip("The position to spawn the object at - in local space if parent is set")]
		public Vector2 Position;

		[Tooltip("The parent object to attach the object to (optional) - if set position will be in local space")]
		public VariableReference Parent = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Parent))
				inputs.Add(VariableDefinition.Create<GameObject>(Parent.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Prefab)
			{
				Transform parent = null;

				if (Parent.IsAssigned && !Parent.GetValue(variables).TryGetObject(out parent))
					Debug.LogWarningFormat(this, _parentNotFoundWarning, Name, Parent);

				var spawned = parent ? Instantiate(Prefab, parent.position + (Vector3)Position, Quaternion.identity, parent.transform) : Instantiate(Prefab, Position, Quaternion.identity);
				spawned.name = ObjectName;

				graph.GoTo(Next, spawned, nameof(Next));
			}
			else
			{
				Debug.LogWarningFormat(this, _objectNotFoundWarning, Name);
				graph.GoTo(Next, variables.This, nameof(Next));
			}

			yield break;
		}
	}
}
