using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Get Property", 10)]
	[HelpURL(Composition.DocumentationUrl + "get-property-node")]
	public class GetPropertyNode : InstructionGraphNode, ISerializationCallbackReceiver
	{
		private const string _invalidObjectTypeWarning = "(CNCGPNIOT) Unable to get property for {0}: the Target '{1}' was not of type '{2}'";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Object to get the property of")]
		[InlineDisplay(PropagateLabel = true)]
		public ObjectVariableSource Target = new ObjectVariableSource();

		[Tooltip("The reference to store the retreived value in")]
		public VariableReference Output = new VariableReference();

		[Tooltip("The Type of the target object to get the property of")]
		public string TargetTypeName;

		[Tooltip("The property to get")]
		public string PropertyName;

		public Type TargetType { get; set; }
		public Type PropertyType { get; set; }
		public FieldInfo Field { get; set; }
		public PropertyInfo Property { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve(variables, Target, out var target))
			{
				if (target.GetType() == TargetType)
				{
					var obj = Field?.GetValue(target) ?? Property?.GetValue(target);
					var value = VariableValue.CreateValue(obj);

					Assign(variables, Output, value);
				}
				else
				{
					Debug.LogWarningFormat(this, _invalidObjectTypeWarning, Name, Target, TargetTypeName);
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnAfterDeserialize()
		{
			TargetType = Type.GetType(TargetTypeName);
			Field = TargetType?.GetField(PropertyName);
			Property = TargetType?.GetProperty(PropertyName);
			PropertyType = (Field?.FieldType) ?? (Property?.PropertyType);
		}

		public void OnBeforeSerialize()
		{
			TargetTypeName = TargetType?.AssemblyQualifiedName;
			PropertyName = (Field?.Name) ?? (Property?.Name);
		}

		#endregion
	}
}
