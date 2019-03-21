using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Call Method", 12)]
	[HelpURL(Composition.DocumentationUrl + "call-method-node")]
	public class CallMethodNode : InstructionGraphNode, ISerializationCallbackReceiver
	{
		private const string _invalidObjectTypeWarning = "(CNCCMNIOT) Unable to call method for {0}: the Target '{1}' was not of type '{2}'";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target Object to call the method on")]
		[InlineDisplay(PropagateLabel = true)]
		public ObjectVariableSource Target = new ObjectVariableSource();

		[Tooltip("The reference to store the returned value in")]
		public VariableReference Output = new VariableReference();

		[Tooltip("The values to pass as parameters to the method")]
		public List<VariableValueSource> Parameters = new List<VariableValueSource>();

		[Tooltip("The Type of the target to call the method on")]
		public string TargetTypeName;

		[Tooltip("The name of the method to call")]
		public string MethodName;

		public string[] ParameterTypeNames;

		public Type TargetType { get; set; }
		public MethodInfo Method { get; set; }
		public Type[] ParameterTypes { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		private object[] _parameters;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out var target))
			{
				if (target.GetType() == TargetType)
				{
					ResolveParameters(variables);
					var obj = Method.Invoke(target, _parameters);

					if (obj != null)
					{
						var value = VariableValue.CreateValue(obj);
						Assign(variables, Output, value);
					}
				}
				else
				{
					Debug.LogWarningFormat(this, _invalidObjectTypeWarning, Name, Target, TargetTypeName);
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void ResolveParameters(InstructionStore variables)
		{
			for (var i = 0; i < Parameters.Count; i++)
			{
				if (Resolve(variables, Parameters[i], out var value))
					_parameters[i] = GetBoxedValue(value);
			}
		}

		private object GetBoxedValue(VariableValue value)
		{
			switch (value.Type)
			{
				case VariableType.Bool: return value.Bool;
				case VariableType.Int: return value.Int;
				case VariableType.Float: return value.Float;
				case VariableType.Int2: return value.Int2;
				case VariableType.Int3: return value.Int3;
				case VariableType.IntRect: return value.IntRect;
				case VariableType.IntBounds: return value.IntBounds;
				case VariableType.Vector2: return value.Vector2;
				case VariableType.Vector3: return value.Vector3;
				case VariableType.Vector4: return value.Vector4;
				case VariableType.Quaternion: return value.Quaternion;
				case VariableType.Rect: return value.Rect;
				case VariableType.Bounds: return value.Bounds;
				case VariableType.Color: return value.Color;
				case VariableType.String: return value.String;
				case VariableType.Object: return value.Object;
				default: return null;
			}
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnAfterDeserialize()
		{
			TargetType = Type.GetType(TargetTypeName);
			ParameterTypes = ParameterTypeNames?.Select(name => Type.GetType(name)).ToArray();
			Method = TargetType?.GetMethod(MethodName, ParameterTypes);

			_parameters = new object[Parameters.Count];
		}

		public void OnBeforeSerialize()
		{
			TargetTypeName = TargetType?.AssemblyQualifiedName;
			ParameterTypeNames = ParameterTypes?.Select(type => type.AssemblyQualifiedName).ToArray();
			MethodName = Method?.Name;
		}

		#endregion
	}
}
