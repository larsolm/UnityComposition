﻿using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

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

		public string TargetTypeName;
		public string PropertyName;

		public Type TargetType { get; set; }
		public FieldInfo Field { get; set; }
		public PropertyInfo Property { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		private Getter _getter;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out var target))
			{
				if (target.GetType() == TargetType)
				{
					if (_getter != null)
					{
						var value = _getter.Get(target);
						Assign(variables, Output, value);
					}
					else if (Field != null)
					{
						var obj = Field?.GetValue(target);
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

		#region ISerializationCallbackReceiver Implementation

		public void OnAfterDeserialize()
		{
			TargetType = Type.GetType(TargetTypeName);
			Field = TargetType?.GetField(PropertyName);
			Property = TargetType?.GetProperty(PropertyName);

			_getter = Getter.Create(TargetType, Property);
		}

		public void OnBeforeSerialize()
		{
			TargetTypeName = TargetType?.AssemblyQualifiedName;
			PropertyName = (Field?.Name) ?? (Property?.Name);
		}

		#endregion

		private abstract class Getter
		{
			public static Getter Create(Type objectType, PropertyInfo property)
			{
				if (objectType != null || property == null)
					return null;

				var getter = Create(objectType, property.PropertyType);
				var getMethod = property.GetGetMethod();

				getter.Setup(getMethod);

				return getter;
			}

			private static Getter Create(Type componentType, Type propertyType)
			{
				var open = typeof(Getter<,>);
				var closed = open.MakeGenericType(componentType, propertyType);

				return Activator.CreateInstance(closed) as Getter;
			}

			public abstract VariableValue Get(Object obj);

			protected abstract void Setup(MethodInfo getMethod);
		}

		private class Getter<ObjectType, PropertyType> : Getter where ObjectType : Object
		{
			public Func<ObjectType, PropertyType> Method;

			public override VariableValue Get(Object obj)
			{
				var component = (ObjectType)obj;
				var value = Method(component);
				return VariableValue.CreateValue(value);
			}

			protected override void Setup(MethodInfo getMethod)
			{
				Method = (Func<ObjectType, PropertyType>)getMethod.CreateDelegate(typeof(Func<ObjectType, PropertyType>));
			}
		}
	}
}
