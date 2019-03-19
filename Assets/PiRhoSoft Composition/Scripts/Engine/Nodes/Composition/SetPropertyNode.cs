using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Set Property", 1)]
	[HelpURL(Composition.DocumentationUrl + "set-property-node")]
	public class SetPropertyNode : InstructionGraphNode, ISerializationCallbackReceiver
	{
		private const string _invalidComponentTypeWarning = "(CNCSPNICT) Unable to set property for {0}: the Target '{1}' was not of type '{2}'";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Component to set the property on")]
		[VariableConstraint(typeof(Component))]
		public VariableReference Target = new VariableReference();

		[Tooltip("Whether to set the value to a reference or a specified value")]
		public VariableSourceType SourceType;

		[Tooltip("The target value reference to set the property to")]
		public VariableReference ValueReference = new VariableReference();

		[Tooltip("The target value to set the property to")]
		public VariableValue Value;

		public string ComponentTypeName;
		public string PropertyName;

		public Type ComponentType { get; set; }
		public Type PropertyType { get; set; }
		public FieldInfo Field { get; set; }
		public PropertyInfo Property { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve<Component>(variables, Target, out var component))
			{
				if (component.GetType() == ComponentType)
				{
					var value = SourceType == VariableSourceType.Reference ? ValueReference.GetValue(variables) : Value;
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
						Field?.SetValue(component, obj);
						Property?.SetValue(component, obj);
					}
				}
				else
				{
					Debug.LogWarningFormat(this, _invalidComponentTypeWarning, Name, Target, ComponentTypeName);
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnAfterDeserialize()
		{
			ComponentType = Type.GetType(ComponentTypeName);
			Field = ComponentType?.GetField(PropertyName);
			Property = ComponentType?.GetProperty(PropertyName);
			PropertyType = (Field?.FieldType) ?? (Property?.PropertyType);
		}

		public void OnBeforeSerialize()
		{
			ComponentTypeName = ComponentType?.AssemblyQualifiedName;
			PropertyName = (Field?.Name) ?? (Property?.Name);
		}

		#endregion
	}
}
