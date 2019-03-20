using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Set Property", 11)]
	[HelpURL(Composition.DocumentationUrl + "set-property-node")]
	public class SetPropertyNode : InstructionGraphNode, ISerializationCallbackReceiver
	{
		private const string _invalidComponentTypeWarning = "(CNCSPNICT) Unable to set property for {0}: the Target '{1}' was not of type '{2}'";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Component to set the property on")]
		[VariableConstraint(typeof(Component))]
		public VariableReference Target = new VariableReference();

		[Tooltip("The target value to set the property to")]
		public VariableValueSource Value = new VariableValueSource();

		[Tooltip("The Type of the target object to set the property for")]
		public string TargetTypeName;

		[Tooltip("The name of the Property to set")]
		public string PropertyName;

		public Type TargetType { get; set; }
		public Type PropertyType { get; set; }
		public FieldInfo Field { get; set; }
		public PropertyInfo Property { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve<Component>(variables, Target, out var target))
			{
				if (target.GetType() == TargetType)
				{
					ResolveOther(variables, Value, out var value);
					object obj = null;

					switch (value.Type)
					{
						case VariableType.Bool: obj = value.Bool; break;
						case VariableType.Int: obj = value.Int; break;
						case VariableType.Float: obj = value.Float; break;
						case VariableType.Int2: obj = value.Int2; break;
						case VariableType.Int3: obj = value.Int3; break;
						case VariableType.IntRect: obj = value.IntRect; break;
						case VariableType.IntBounds: obj = value.IntBounds; break;
						case VariableType.Vector2: obj = value.Vector2; break;
						case VariableType.Vector3: obj = value.Vector3; break;
						case VariableType.Vector4: obj = value.Vector4; break;
						case VariableType.Quaternion: obj = value.Quaternion; break;
						case VariableType.Rect: obj = value.Rect; break;
						case VariableType.Bounds: obj = value.Bounds; break;
						case VariableType.Color: obj = value.Color; break;
						case VariableType.String: obj = value.String; break;
						case VariableType.Object: obj = value.Object; break;
						case VariableType.Store: obj = value.Store; break;
						case VariableType.Other: obj = value.Reference; break;
					}

					if (obj != null)
					{
						Field?.SetValue(target, obj);
						Property?.SetValue(target, obj);
					}
				}
				else
				{
					Debug.LogWarningFormat(this, _invalidComponentTypeWarning, Name, Target, TargetTypeName);
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
