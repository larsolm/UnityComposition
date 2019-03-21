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
		public FieldInfo Field { get; set; }
		public PropertyInfo Property { get; set; }

		public override Color NodeColor => Colors.ExecutionDark;

		private Setter _setter;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Resolve<Component>(variables, Target, out var target))
			{
				if (target.GetType() == TargetType)
				{
					ResolveOther(variables, Value, out var value);

					if (_setter != null)
						_setter.Set(target, value);
					else if (Field != null)
						Set(target, value);
				}
				else
				{
					Debug.LogWarningFormat(this, _invalidComponentTypeWarning, Name, Target, TargetTypeName);
				}
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void Set(Component target, VariableValue value)
		{
			switch (value.Type)
			{
				case VariableType.Bool: Field.SetValue(target, value.Bool); break;
				case VariableType.Int: Field.SetValue(target, value.Int); break;
				case VariableType.Float: Field.SetValue(target, value.Float); break;
				case VariableType.Int2: Field.SetValue(target, value.Int2); break;
				case VariableType.Int3: Field.SetValue(target, value.Int3); break;
				case VariableType.IntRect: Field.SetValue(target, value.IntRect); break;
				case VariableType.IntBounds: Field.SetValue(target, value.IntBounds); break;
				case VariableType.Vector2: Field.SetValue(target, value.Vector2); break;
				case VariableType.Vector3: Field.SetValue(target, value.Vector3); break;
				case VariableType.Vector4: Field.SetValue(target, value.Vector4); break;
				case VariableType.Quaternion: Field.SetValue(target, value.Quaternion); break;
				case VariableType.Rect: Field.SetValue(target, value.Rect); break;
				case VariableType.Bounds: Field.SetValue(target, value.Bounds); break;
				case VariableType.Color: Field.SetValue(target, value.Color); break;
				case VariableType.String: Field.SetValue(target, value.String); break;
				case VariableType.Object: Field.SetValue(target, value.Reference); break;
			}
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnAfterDeserialize()
		{
			TargetType = Type.GetType(TargetTypeName);
			Field = TargetType?.GetField(PropertyName);
			Property = TargetType?.GetProperty(PropertyName);

			_setter = Setter.Create(TargetType, Property);
		}

		public void OnBeforeSerialize()
		{
			TargetTypeName = TargetType?.AssemblyQualifiedName;
			PropertyName = (Field?.Name) ?? (Property?.Name);
		}

		#endregion

		private abstract class Setter
		{
			public static Setter Create(Type componentType, PropertyInfo property)
			{
				if (componentType != null || property == null)
					return null;

				var setter = Create(componentType, property.PropertyType);
				var setMethod = property.GetSetMethod();

				if (property.PropertyType == typeof(bool)) setter.SetupAsBool(setMethod);
				else if (property.PropertyType == typeof(int)) setter.SetupAsInt(setMethod);
				else if (property.PropertyType == typeof(float)) setter.SetupAsFloat(setMethod);
				else if (property.PropertyType == typeof(Vector2Int)) setter.SetupAsInt2(setMethod);
				else if (property.PropertyType == typeof(Vector3Int)) setter.SetupAsInt3(setMethod);
				else if (property.PropertyType == typeof(RectInt)) setter.SetupAsIntRect(setMethod);
				else if (property.PropertyType == typeof(BoundsInt)) setter.SetupAsIntBounds(setMethod);
				else if (property.PropertyType == typeof(Vector2)) setter.SetupAsVector2(setMethod);
				else if (property.PropertyType == typeof(Vector3)) setter.SetupAsVector3(setMethod);
				else if (property.PropertyType == typeof(Vector4)) setter.SetupAsVector4(setMethod);
				else if (property.PropertyType == typeof(Quaternion)) setter.SetupAsQuaternion(setMethod);
				else if (property.PropertyType == typeof(Rect)) setter.SetupAsRect(setMethod);
				else if (property.PropertyType == typeof(Bounds)) setter.SetupAsBounds(setMethod);
				else if (property.PropertyType == typeof(Color)) setter.SetupAsColor(setMethod);
				else if (property.PropertyType == typeof(string)) setter.SetupAsString(setMethod);
				else setter.SetupAsObject(setMethod);

				return setter;
			}

			private static Setter Create(Type componentType, Type propertyType)
			{
				var open = typeof(Setter<,>);
				var closed = open.MakeGenericType(componentType, propertyType);

				return Activator.CreateInstance(closed) as Setter;
			}

			public abstract void Set(Component obj, VariableValue value);

			protected abstract void SetupAsBool(MethodInfo setMethod);
			protected abstract void SetupAsInt(MethodInfo setMethod);
			protected abstract void SetupAsFloat(MethodInfo setMethod);
			protected abstract void SetupAsInt2(MethodInfo setMethod);
			protected abstract void SetupAsInt3(MethodInfo setMethod);
			protected abstract void SetupAsIntRect(MethodInfo setMethod);
			protected abstract void SetupAsIntBounds(MethodInfo setMethod);
			protected abstract void SetupAsVector2(MethodInfo setMethod);
			protected abstract void SetupAsVector3(MethodInfo setMethod);
			protected abstract void SetupAsVector4(MethodInfo setMethod);
			protected abstract void SetupAsQuaternion(MethodInfo setMethod);
			protected abstract void SetupAsRect(MethodInfo setMethod);
			protected abstract void SetupAsBounds(MethodInfo setMethod);
			protected abstract void SetupAsColor(MethodInfo setMethod);
			protected abstract void SetupAsString(MethodInfo setMethod);
			protected abstract void SetupAsObject(MethodInfo setMethod);
		}

		private class Setter<ComponentType, PropertyType> : Setter where ComponentType : Component
		{
			public Action<ComponentType, VariableValue> Method;

			public override void Set(Component obj, VariableValue value)
			{
				var component = (ComponentType)obj;
				Method(component, value);
			}

			protected override void SetupAsBool(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, bool>)setMethod.CreateDelegate(typeof(Action<ComponentType, bool>));
				Method = (component, value) => SetBool(component, value, set);
			}

			protected override void SetupAsInt(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, int>)setMethod.CreateDelegate(typeof(Action<ComponentType, int>));
				Method = (component, value) => SetInt(component, value, set);
			}

			protected override void SetupAsFloat(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, float>)setMethod.CreateDelegate(typeof(Action<ComponentType, float>));
				Method = (component, value) => SetFloat(component, value, set);
			}

			protected override void SetupAsInt2(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Vector2Int>)setMethod.CreateDelegate(typeof(Action<ComponentType, Vector2Int>));
				Method = (component, value) => SetInt2(component, value, set);
			}

			protected override void SetupAsInt3(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Vector3Int>)setMethod.CreateDelegate(typeof(Action<ComponentType, Vector3Int>));
				Method = (component, value) => SetInt3(component, value, set);
			}

			protected override void SetupAsIntRect(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, RectInt>)setMethod.CreateDelegate(typeof(Action<ComponentType, RectInt>));
				Method = (component, value) => SetIntRect(component, value, set);
			}

			protected override void SetupAsIntBounds(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, BoundsInt>)setMethod.CreateDelegate(typeof(Action<ComponentType, BoundsInt>));
				Method = (component, value) => SetIntBounds(component, value, set);
			}

			protected override void SetupAsVector2(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Vector2>)setMethod.CreateDelegate(typeof(Action<ComponentType, Vector2>));
				Method = (component, value) => SetVector2(component, value, set);
			}

			protected override void SetupAsVector3(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Vector3>)setMethod.CreateDelegate(typeof(Action<ComponentType, Vector3>));
				Method = (component, value) => SetVector3(component, value, set);
			}

			protected override void SetupAsVector4(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Vector4>)setMethod.CreateDelegate(typeof(Action<ComponentType, Vector4>));
				Method = (component, value) => SetVector4(component, value, set);
			}

			protected override void SetupAsQuaternion(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Quaternion>)setMethod.CreateDelegate(typeof(Action<ComponentType, Quaternion>));
				Method = (component, value) => SetQuaternion(component, value, set);
			}

			protected override void SetupAsRect(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Rect>)setMethod.CreateDelegate(typeof(Action<ComponentType, Rect>));
				Method = (component, value) => SetRect(component, value, set);
			}

			protected override void SetupAsBounds(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Bounds>)setMethod.CreateDelegate(typeof(Action<ComponentType, Bounds>));
				Method = (component, value) => SetBounds(component, value, set);
			}

			protected override void SetupAsColor(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, Color>)setMethod.CreateDelegate(typeof(Action<ComponentType, Color>));
				Method = (component, value) => SetColor(component, value, set);
			}

			protected override void SetupAsString(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, string>)setMethod.CreateDelegate(typeof(Action<ComponentType, string>));
				Method = (component, value) => SetString(component, value, set);
			}

			protected override void SetupAsObject(MethodInfo setMethod)
			{
				var set = (Action<ComponentType, PropertyType>)setMethod.CreateDelegate(typeof(Action<ComponentType, PropertyType>));
				Method = (component, value) => SetObject(component, value, set);
			}

			private static void SetBool(ComponentType component, VariableValue value, Action<ComponentType, bool> setter) => setter(component, value.Bool);
			private static void SetInt(ComponentType component, VariableValue value, Action<ComponentType, int> setter) => setter(component, value.Int);
			private static void SetFloat(ComponentType component, VariableValue value, Action<ComponentType, float> setter) => setter(component, value.Number);
			private static void SetInt2(ComponentType component, VariableValue value, Action<ComponentType, Vector2Int> setter) => setter(component, value.Int2);
			private static void SetInt3(ComponentType component, VariableValue value, Action<ComponentType, Vector3Int> setter) => setter(component, value.Int3);
			private static void SetIntRect(ComponentType component, VariableValue value, Action<ComponentType, RectInt> setter) => setter(component, value.IntRect);
			private static void SetIntBounds(ComponentType component, VariableValue value, Action<ComponentType, BoundsInt> setter) => setter(component, value.IntBounds);
			private static void SetVector2(ComponentType component, VariableValue value, Action<ComponentType, Vector2> setter) => setter(component, value.Vector2);
			private static void SetVector3(ComponentType component, VariableValue value, Action<ComponentType, Vector3> setter) => setter(component, value.Vector3);
			private static void SetVector4(ComponentType component, VariableValue value, Action<ComponentType, Vector4> setter) => setter(component, value.Vector4);
			private static void SetQuaternion(ComponentType component, VariableValue value, Action<ComponentType, Quaternion> setter) => setter(component, value.Quaternion);
			private static void SetRect(ComponentType component, VariableValue value, Action<ComponentType, Rect> setter) => setter(component, value.Rect);
			private static void SetBounds(ComponentType component, VariableValue value, Action<ComponentType, Bounds> setter) => setter(component, value.Bounds);
			private static void SetColor(ComponentType component, VariableValue value, Action<ComponentType, Color> setter) => setter(component, value.Color);
			private static void SetString(ComponentType component, VariableValue value, Action<ComponentType, string> setter) => setter(component, value.String);
			private static void SetObject(ComponentType component, VariableValue value, Action<ComponentType, PropertyType> setter) => setter(component, (PropertyType)value.Reference);
		}
	}
}
