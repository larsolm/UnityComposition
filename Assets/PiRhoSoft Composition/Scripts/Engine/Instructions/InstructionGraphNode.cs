using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PiRhoSoft.UtilityEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class InstructionGraphNodeList : SerializedList<InstructionGraphNode> { }
	[Serializable] public class InstructionGraphNodeDictionary : SerializedDictionary<string, InstructionGraphNode> { }

	public class CreateInstructionGraphNodeMenuAttribute : Attribute
	{
		public string MenuName { get; private set; }
		public int Order { get; private set; }

		public CreateInstructionGraphNodeMenuAttribute(string menuName, int order = 0)
		{
			MenuName = menuName;
			Order = order;
		}
	}

	public interface ISequenceNode
	{
	}

	public interface ILoopNode
	{
	}

	public abstract class InstructionGraphNode : ScriptableObject
	{
		public static class Colors
		{
			public static readonly Color Start = new Color(0.1f, 0.1f, 0.1f);
			public static readonly Color Default = new Color(0.35f, 0.35f, 0.35f);
			public static readonly Color ExecutionLight = new Color(0.45f, 0.45f, 0.0f);
			public static readonly Color ExecutionDark = new Color(0.25f, 0.25f, 0.0f);
			public static readonly Color Animation = new Color(0.35f, 0.0f, 0.35f);
			public static readonly Color Sequence = new Color(0.5f, 0.2f, 0.2f);
			public static readonly Color Loop = new Color(0.35f, 0.1f, 0.1f);
			public static readonly Color Branch = new Color(0.2f, 0.1f, 0.1f);
			public static readonly Color Break = new Color(0.1f, 0.05f, 0.05f);
			public static readonly Color Sequencing = new Color(0.0f, 0.35f, 0.0f);
			public static readonly Color SequencingLight = new Color(0.0f, 0.45f, 0.0f);
			public static readonly Color SequencingDark = new Color(0.0f, 0.25f, 0.0f);
			public static readonly Color Interface = new Color(0.0f, 0.0f, 0.35f);
			public static readonly Color InterfaceLight = new Color(0.0f, 0.0f, 0.45f);
			public static readonly Color InterfaceDark = new Color(0.0f, 0.0f, 0.25f);
			public static readonly Color InterfaceCyan = new Color(0.0f, 0.3f, 0.5f);
			public static readonly Color InterfaceTeal = new Color(0.0f, 0.5f, 0.3f);
		}

		private const string _missingVariableWarning = "(CIGNMV) failed to resolve variable '{0}' on node '{1}': the variable could not be found";
		private const string _invalidVariableWarning = "(CIGNIV) failed to resolve variable '{0}' on node '{1}': the variable has type {2} and should have type {3}";
		private const string _invalidEnumWarning = "(CIGNIV) failed to resolve variable '{0}' on node '{1}': the variable has enum type {2} and should have enum type {3}";
		private const string _invalidObjectWarning = "(CIGNIO) failed to resolve variable '{0}' on node '{1}': the object is a {2} and cannot be converted to a {3}";

		private const string _missingAssignmentWarning = "(CIGNMA) failed to assign to variable '{0}': the variable could not be found";
		private const string _readOnlyAssignmentWarning = "(CIGNROA) failed to assign to variable '{0}': the variable is read only";
		private const string _invalidAssignmentWarning = "(CIGNIA) failed to assign to variable '{0}': the variable has an incompatible type";

		[Tooltip("The name of the node")]
		[ChangeTrigger(nameof(UpdateName))]
		public string Name;

		public abstract IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration);

		#region Variable Lookup

		public bool Resolve(IVariableStore variables, VariableValueSource source, out VariableValue result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out VariableValue result)
		{
			result = reference.GetValue(variables);
			return true;
		}

		public bool Resolve(IVariableStore variables, BoolVariableSource source, out bool result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out bool result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBool(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Bool);
			return false;
		}

		public bool Resolve(IVariableStore variables, IntVariableSource source, out int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetInt(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int);
			return false;
		}

		public bool Resolve(IVariableStore variables, FloatVariableSource source, out float result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out float result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetFloat(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int);
			return false;
		}

		public bool Resolve(IVariableStore variables, Int2VariableSource source, out Vector2Int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Vector2Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetInt2(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int2);
			return false;
		}

		public bool Resolve(IVariableStore variables, Int3VariableSource source, out Vector3Int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Vector3Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetInt3(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int3);
			return false;
		}

		public bool Resolve(IVariableStore variables, IntRectVariableSource source, out RectInt result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out RectInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetIntRect(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.IntRect);
			return false;
		}

		public bool Resolve(IVariableStore variables, IntBoundsVariableSource source, out BoundsInt result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out BoundsInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetIntBounds(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.IntBounds);
			return false;
		}

		public bool Resolve(IVariableStore variables, Vector2VariableSource source, out Vector2 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Vector2 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector2(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector2);
			return false;
		}

		public bool Resolve(IVariableStore variables, Vector3VariableSource source, out Vector3 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Vector3 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector3(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector3);
			return false;
		}

		public bool Resolve(IVariableStore variables, Vector4VariableSource source, out Vector4 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Vector4 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector4(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector4);
			return false;
		}

		public bool Resolve(IVariableStore variables, QuaternionVariableSource source, out Quaternion result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Quaternion result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetQuaternion(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Quaternion);
			return false;
		}

		public bool Resolve(IVariableStore variables, RectVariableSource source, out Rect result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Rect result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetRect(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Rect);
			return false;
		}

		public bool Resolve(IVariableStore variables, BoundsVariableSource source, out Bounds result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Bounds result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBounds(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Bounds);
			return false;
		}

		public bool Resolve(IVariableStore variables, ColorVariableSource source, out Color result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out Color result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetColor(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Color);
			return false;
		}

		public bool Resolve(IVariableStore variables, StringVariableSource source, out string result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out string result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetString(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.String);
			return false;
		}

		public bool Resolve<EnumType>(IVariableStore variables, VariableSource<EnumType> source, out EnumType result) where EnumType : Enum
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve<EnumType>(IVariableStore variables, VariableReference reference, out EnumType result) where EnumType : Enum
		{
			var value = reference.GetValue(variables);

			if (value.TryGetEnum(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Enum, typeof(EnumType));
			return false;
		}

		public bool Resolve(IVariableStore variables, StoreVariableSource source, out IVariableStore result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out IVariableStore result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetStore(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Store);
			return false;
		}

		public bool Resolve(IVariableStore variables, ListVariableSource source, out IVariableList result)
		{
			if (source.Type == VariableSourceType.Reference)
				return Resolve(variables, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public bool Resolve(IVariableStore variables, VariableReference reference, out IVariableList result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetList(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.List);
			return false;
		}

		public bool ResolveObject<ObjectType>(IVariableStore variables, VariableSource<ObjectType> source, out ObjectType result) where ObjectType : Object
		{
			if (source.Type == VariableSourceType.Reference)
				return ResolveObject(variables, source.Reference, out result);

			result = source.Value;
			return result;
		}

		public bool ResolveObject<ObjectType>(IVariableStore variables, VariableReference reference, out ObjectType result) where ObjectType : Object
		{
			var value = reference.GetValue(variables);

			if (value.HasObject)
			{
				result = ComponentHelper.GetAsObject<ObjectType>(value.Object);

				if (result != null)
					return true;
			}

			result = null;
			LogResolveWarning(value, reference, VariableType.Object, typeof(ObjectType));
			return false;
		}

		public bool ResolveStore<StoreType>(IVariableStore variables, VariableReference reference, out StoreType result) where StoreType : class, IVariableStore
		{
			var value = reference.GetValue(variables);

			if (value.TryGetReference(out result))
				return true;
			
			LogResolveWarning(value, reference, VariableType.Store, typeof(StoreType));
			return false;
		}

		public bool ResolveList<ListType>(IVariableStore variables, VariableReference reference, out ListType result) where ListType : class, IVariableList
		{
			var value = reference.GetValue(variables);

			if (value.TryGetReference(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.List, typeof(ListType));
			return false;
		}

		public bool ResolveInterface<InterfaceType>(IVariableStore variables, VariableReference reference, out InterfaceType result) where InterfaceType : class
		{
			var value = reference.GetValue(variables);

			if (value.HasReference)
			{
				if (value.Reference is InterfaceType i)
				{
					result = i;
					return true;
				}
				else
				{
					result = null;
					LogResolveWarning(value, reference, VariableType.Object, typeof(InterfaceType));
					return false;
				}
			}

			result = null;
			LogResolveWarning(value, reference, VariableType.Object);
			return false;
		}

		public bool ResolveReference(IVariableStore variables, VariableReference reference, out object result)
		{
			var value = reference.GetValue(variables);

			if (value.HasReference)
			{
				result = value.Reference;
				return true;
			}

			result = null;
			LogResolveWarning(value, reference, VariableType.Object);
			return false;
		}

		private void LogResolveWarning(VariableValue value, VariableReference reference, VariableType expectedType, Type resolveType = null)
		{
			if (value.Type == VariableType.Empty)
				Debug.LogWarningFormat(this, _missingVariableWarning, reference, name);
			else if (value.Type == VariableType.Enum && resolveType != null)
				Debug.LogWarningFormat(this, _invalidEnumWarning, reference, name, value.EnumType.Name, resolveType.Name);
			else if (value.HasReference && resolveType != null)
				Debug.LogWarningFormat(this, _invalidObjectWarning, reference, name, value.ReferenceType.Name, resolveType.Name);
			else
				Debug.LogWarningFormat(this, _invalidVariableWarning, reference, name, value.Type, expectedType);
		}

		#endregion

		#region Variable Assignment

		public void Assign(IVariableStore variables, VariableReference reference, VariableValue value)
		{
			if (reference.IsAssigned)
			{
				var result = reference.SetValue(variables, value);

				switch (result)
				{
					case SetVariableResult.NotFound: Debug.LogWarningFormat(this, _missingAssignmentWarning, reference); break;
					case SetVariableResult.ReadOnly: Debug.LogWarningFormat(this, _readOnlyAssignmentWarning, reference); break;
					case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(this, _invalidAssignmentWarning, reference); break;
				}
			}
		}

		#endregion

		#region Inputs and Outputs

		public virtual void GetInputs(IList<VariableDefinition> inputs)
		{
			var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.FieldType == typeof(VariableReference))
				{
					var value = field.GetValue(this) as VariableReference;
					var constraint = field.GetCustomAttribute<VariableConstraintAttribute>();
					var definition = new VariableDefinition { Name = value.RootName, Definition = constraint == null ? ValueDefinition.Create(VariableType.Empty) : constraint.Definition };

					if (InstructionStore.IsInput(value))
						inputs.Add(definition);
				}
				else if (field.FieldType == typeof(Expression))
				{
					var value = field.GetValue(this) as Expression;
					value.GetInputs(inputs, InstructionStore.InputStoreName);
				}
				else if (field.FieldType == typeof(Message))
				{
					var value = field.GetValue(this) as Message;
					value.GetInputs(inputs);
				}
				else if (typeof(VariableSource).IsAssignableFrom(field.FieldType))
				{
					var value = field.GetValue(this) as VariableSource;

					var constraint = field.GetCustomAttribute<VariableConstraintAttribute>();
					if (constraint != null)
					{
						if (value.Type == VariableSourceType.Reference && InstructionStore.IsInput(value.Reference))
							inputs.Add(new VariableDefinition { Name = value.Reference.RootName, Definition = constraint.Definition });
					}
					else
					{
						value.GetInputs(inputs);
					}
				}
			}
		}

		public virtual void GetOutputs(IList<VariableDefinition> outputs)
		{
			var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.FieldType == typeof(VariableReference))
				{
					var value = field.GetValue(this) as VariableReference;
					if (InstructionStore.IsOutput(value))
						outputs.Add(new VariableDefinition { Name = value.RootName, Definition = ValueDefinition.Create(VariableType.Empty) });
				}
				else if (field.FieldType == typeof(Expression))
				{
					var value = field.GetValue(this) as Expression;
					value.GetOutputs(outputs, InstructionStore.OutputStoreName);
				}
			}
		}

		#endregion

		#region Editor Interface

		private const string _missingKeyError = "(CCNMK) failed to set target: unable to find key {0} for instruction graph node {1}";
		private const string _missingIndexError = "(CCNMI) failed to set target: index {0} is out of range for instruction graph node {1}";
		private const string _missingFieldError = "(CCNMF) failed to set target: unable to find field {0} for instruction graph node {1}";

		[HideInInspector] public Vector2 GraphPosition;
		[HideInInspector] public bool IsBreakpoint = false;

		public virtual Color NodeColor => Colors.Default;

		public void UpdateName()
		{
			name = Name;
		}

		public class NodeData
		{
			public const float Width = 256.0f;
			public const float HeaderHeight = 22.0f;
			public const float LineHeight = 18.0f;
			public const float FooterHeight = 2.0f;

			private float _innerHeight = 0.0f;

			public InstructionGraphNode Node { get; private set; }
			public Rect Bounds { get; private set; }
			public List<ConnectionData> Connections = new List<ConnectionData>();

			public Vector2 Position
			{
				get
				{
					return Node.GraphPosition;
				}
				set
				{
					Node.GraphPosition = value;
					UpdateBounds();
				}
			}

			public float InnerHeight
			{
				get
				{
					return _innerHeight;
				}
				set
				{
					_innerHeight = value;
					UpdateBounds();
				}
			}

			public NodeData(InstructionGraphNode node)
			{
				Node = node;
				UpdateBounds();
			}

			public void ClearConnections()
			{
				Connections.Clear();
				UpdateBounds();
			}

			public void AddConnections(object obj)
			{
				var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

				foreach (var field in fields)
				{
					if (field.FieldType == typeof(InstructionGraphNode))
					{
						var value = field.GetValue(obj) as InstructionGraphNode;
						AddConnection(field.Name, value);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as InstructionGraphNodeDictionary;

						foreach (var value in dictionary)
							AddConnection(field.Name, value.Key, value.Value);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeList))
					{
						var list = field.GetValue(obj) as InstructionGraphNodeList;

						for (var i = 0; i < list.Count; i++)
							AddConnection(field.Name, i, list[i]);
					}
				}
			}

			public void AddConnection(string name, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, -1, Node, to, Connections.Count));
				UpdateBounds();
			}

			public void AddConnection(string name, string key, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, key, -1, Node, to, Connections.Count));
				UpdateBounds();
			}

			public void AddConnection(string name, int index, InstructionGraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, index, Node, to, Connections.Count));
				UpdateBounds();
			}

			private void UpdateBounds()
			{
				Bounds = new Rect(Position, new Vector2(Width, Connections.Count * LineHeight + HeaderHeight + FooterHeight + _innerHeight));
			}
		}

		public class ConnectionData
		{
			public string Field { get; private set; }
			public string FieldKey { get; private set; }
			public int FieldIndex { get; private set; }

			public InstructionGraphNode From { get; private set; }
			public int FromIndex { get; private set; }
			public InstructionGraphNode To { get; private set; }
			public NodeData Target { get; private set; }

			public string Name { get; private set; }

			public static bool operator ==(ConnectionData left, ConnectionData right)
			{
				// need to override since connections are rebuilt for the selected node causing reference comparison
				// to return false

				if (ReferenceEquals(left, null))
					return ReferenceEquals(right, null);
				else if (ReferenceEquals(right, null))
					return false;
				else
					return left.From == right.From && left.FromIndex == right.FromIndex;
			}

			public static bool operator !=(ConnectionData left, ConnectionData right)
			{
				return !(left == right);
			}

			public override bool Equals(object obj)
			{
				if (obj is ConnectionData other)
					return this == other;

				return false;
			}

			public override int GetHashCode()
			{
				// not needed but Visual Studio warns without it

				var hashCode = -2083501448;
				hashCode = hashCode * -1521134295 + EqualityComparer<InstructionGraphNode>.Default.GetHashCode(From);
				hashCode = hashCode * -1521134295 + FromIndex.GetHashCode();
				return hashCode;
			}

			public ConnectionData(string field, string key, int index, InstructionGraphNode from, InstructionGraphNode to, int fromIndex)
			{
				Field = field;
				FieldKey = key;
				FieldIndex = index;
				From = from;
				FromIndex = fromIndex;
				To = to;

				if (index >= 0)
					Name = string.Format("{0} {1}", field, index);
				else if (!string.IsNullOrEmpty(key))
					Name = string.Format("{0} {1}", field, key);
				else
					Name = field;
			}

			public void SetTarget(NodeData target)
			{
				Target = target;
			}

			public void ChangeTarget(NodeData target)
			{
				To = target?.Node;
				Target = target;

				From.SetConnection(this, To);
			}

			public void ApplyConnection(object obj, InstructionGraphNode target)
			{
				var field = obj.GetType().GetField(Field, BindingFlags.Instance | BindingFlags.Public);

				if (field != null)
				{
					if (field.FieldType == typeof(InstructionGraphNode))
					{
						field.SetValue(obj, target);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as InstructionGraphNodeDictionary;

						if (dictionary.ContainsKey(FieldKey))
							dictionary[FieldKey] = target;
						else
							Debug.LogErrorFormat(_missingKeyError, Field, target.name);
					}
					else if (field.FieldType == typeof(InstructionGraphNodeList))
					{
						var list = field.GetValue(obj) as InstructionGraphNodeList;

						if (FieldIndex >= 0 && FieldIndex < list.Count)
							list[FieldIndex] = target;
						else
							Debug.LogErrorFormat(_missingIndexError, Field, Target.Node.name);
					}
					else
					{
						Debug.LogErrorFormat(_missingFieldError, Field, Target.Node.name);
					}
				}
				else
				{
					Debug.LogErrorFormat(_missingFieldError, Field, Target.Node.name);
				}
			}
		}

		public virtual void GetConnections(NodeData data)
		{
			data.AddConnections(this);
		}

		public virtual void SetConnection(ConnectionData connection, InstructionGraphNode target)
		{
			connection.ApplyConnection(this, target);
		}

		#endregion
	}
}
