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
		[ClassDisplay(ClassDisplayType.Propogated)]
		public ObjectVariableSource Target = new ObjectVariableSource();

		[Tooltip("The reference to store the returned value in")]
		public VariableReference Output = new VariableReference();

		[Tooltip("The values to pass as parameters to the method")]
		public List<VariableValueSource> Parameters = new List<VariableValueSource>();

		public string TargetTypeName;
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
				var cast = ComponentHelper.GetAsComponent(TargetType, target);
				if (cast)
				{
					ResolveParameters(variables);
					var obj = Method.Invoke(cast, _parameters);

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
					_parameters[i] = value.GetBoxedValue();
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
