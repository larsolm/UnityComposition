using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable] public class GraphNodeList : SerializedList<GraphNode> { }
	[Serializable] public class GraphNodeDictionary : SerializedDictionary<string, GraphNode> { }

	public class CreateGraphNodeMenuAttribute : Attribute
	{
		public string MenuName { get; private set; }
		public int Order { get; private set; }

		public CreateGraphNodeMenuAttribute(string menuName, int order = 0)
		{
			MenuName = menuName;
			Order = order;
		}
	}

	public abstract class OutputCollectionNodeAttribute : Attribute
	{
		public string PropertyName { get; private set; }
		public bool Renameable { get; private set; }

		protected OutputCollectionNodeAttribute(string propertyName, bool renamable)
		{
			PropertyName = propertyName;
			Renameable = renamable;
		}
	}

	public class OutputListNodeAttribute : OutputCollectionNodeAttribute
	{
		public OutputListNodeAttribute(string propertyName) : base(propertyName, false)	{ }
	}

	public class OutputDictionaryNodeAttribute : OutputCollectionNodeAttribute
	{
		public OutputDictionaryNodeAttribute(string propertyName) : base(propertyName, true) { }
	}

	public abstract class GraphNode : ScriptableObject
	{
		public static class Colors
		{
			public static readonly Color Start = new Color(0.25f, 0.25f, 0.25f, 0.8f);
			public static readonly Color Default = new Color(0.35f, 0.35f, 0.35f, 0.8f);
			public static readonly Color ExecutionLight = new Color(0.45f, 0.45f, 0.0f, 0.8f);
			public static readonly Color ExecutionDark = new Color(0.25f, 0.25f, 0.0f, 0.8f);
			public static readonly Color Animation = new Color(0.35f, 0.0f, 0.35f, 0.8f);
			public static readonly Color Sequence = new Color(0.5f, 0.2f, 0.2f, 0.8f);
			public static readonly Color Loop = new Color(0.35f, 0.1f, 0.1f, 0.8f);
			public static readonly Color Branch = new Color(0.2f, 0.1f, 0.1f, 0.8f);
			public static readonly Color Break = new Color(0.1f, 0.05f, 0.05f, 0.8f);
			public static readonly Color Sequencing = new Color(0.0f, 0.35f, 0.0f, 0.8f);
			public static readonly Color SequencingLight = new Color(0.0f, 0.45f, 0.0f, 0.8f);
			public static readonly Color SequencingDark = new Color(0.0f, 0.25f, 0.0f, 0.8f);
			public static readonly Color Interface = new Color(0.0f, 0.0f, 0.35f);
			public static readonly Color InterfaceLight = new Color(0.0f, 0.0f, 0.45f, 0.8f);
			public static readonly Color InterfaceDark = new Color(0.0f, 0.0f, 0.25f, 0.8f);
			public static readonly Color InterfaceCyan = new Color(0.0f, 0.3f, 0.5f, 0.8f);
			public static readonly Color InterfaceTeal = new Color(0.0f, 0.5f, 0.3f, 0.8f);
		}

		public abstract IEnumerator Run(IGraphRunner graph, IVariableCollection variables);

		#region Inputs and Outputs

		public virtual void GetInputs(IList<VariableDefinition> inputs, string storeName)
		{
			var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.FieldType == typeof(VariableLookupReference))
				{
					var value = field.GetValue(this) as VariableLookupReference;
					var attribute = field.GetCustomAttribute<VariableReferenceAttribute>();

					if (value.UsesStore(storeName))
					{
						if (attribute != null)
							inputs.Add(attribute.GetDefinition(value.RootName));
						else
							inputs.Add(value.GetDefinition());
					}
				}
				else if (field.FieldType == typeof(Expression))
				{
					var value = field.GetValue(this) as Expression;
					value.GetInputs(inputs, storeName);
				}
				else if (field.FieldType == typeof(Message))
				{
					var value = field.GetValue(this) as Message;
					value.GetInputs(inputs, storeName);
				}
				else if (typeof(VariableSource).IsAssignableFrom(field.FieldType))
				{
					var value = field.GetValue(this) as VariableSource;
					var attribute = field.GetCustomAttribute<VariableReferenceAttribute>();

					if (value.UsesStore(storeName))
					{
						if (attribute != null)
							inputs.Add(attribute.GetDefinition(value.Reference.RootName));
						else
							inputs.Add(value.GetDefinition());
					}
				}
			}
		}

		public virtual void GetOutputs(IList<VariableDefinition> outputs, string storeName)
		{
			var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.FieldType == typeof(VariableAssignmentReference))
				{
					var value = field.GetValue(this) as VariableAssignmentReference;
					var attribute = field.GetCustomAttribute<VariableReferenceAttribute>();

					if (value.UsesStore(storeName))
					{
						if (attribute != null)
							outputs.Add(attribute.GetDefinition(value.RootName));
						else
							outputs.Add(value.GetDefinition());
					}
				}
				else if (field.FieldType == typeof(Expression))
				{
					var value = field.GetValue(this) as Expression;
					value.GetOutputs(outputs, storeName);
				}
			}
		}

		#endregion

		#region Editor Interface

		private const string _missingKeyError = "(CCNMK) Failed to set target: unable to find key {0} for graph node {1}";
		private const string _missingIndexError = "(CCNMI) Failed to set target: index {0} is out of range for graph node {1}";
		private const string _missingFieldError = "(CCNMF) Failed to set target: unable to find field {0} for graph node {1}";

		[HideInInspector] public Graph Graph;
		[HideInInspector] public Vector2 GraphPosition;
		[HideInInspector] public bool IsBreakpoint = false;

		public virtual Color NodeColor => Colors.Default;

#if UNITY_EDITOR
		protected static string GetConnectionName(string node) => node;
		protected static string GetConnectionName(string node, int index) => $"{node} {index}";
		protected static string GetConnectionName(string node, string key) => $"{node} {key}";
#else
		protected static string GetConnectionName(string node) => string.Empty;
		protected static string GetConnectionName(string node, int index) => string.Empty;
		protected static string GetConnectionName(string node, string key) => string.Empty;
#endif

		public class NodeData
		{
			public GraphNode Node { get; private set; }
			public List<ConnectionData> Connections { get; private set; } = new List<ConnectionData>();

			public NodeData(GraphNode node)
			{
				Node = node;
				RefreshConnections();
			}

			public void RefreshConnections()
			{
				Connections.Clear();
				Node.GetConnections(this);
			}

			public void AddConnections(object obj)
			{
				var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

				foreach (var field in fields)
				{
					if (field.FieldType == typeof(GraphNode))
					{
						var value = field.GetValue(obj) as GraphNode;
						AddConnection(field.Name, value);
					}
					else if (field.FieldType == typeof(GraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as GraphNodeDictionary;

						foreach (var value in dictionary)
							AddConnection(field.Name, value.Key, value.Value);
					}
					else if (field.FieldType == typeof(GraphNodeList))
					{
						var list = field.GetValue(obj) as GraphNodeList;

						for (var i = 0; i < list.Count; i++)
							AddConnection(field.Name, i, list[i]);
					}
				}
			}

			public void AddConnection(string name, GraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, -1, this, to, Connections.Count));
			}

			public void AddConnection(string name, string key, GraphNode to)
			{
				Connections.Add(new ConnectionData(name, key, -1, this, to, Connections.Count));
			}

			public void AddConnection(string name, int index, GraphNode to)
			{
				Connections.Add(new ConnectionData(name, null, index, this, to, Connections.Count));
			}
		}

		public class ConnectionData
		{
			public string Field { get; private set; }
			public string FieldKey { get; private set; }
			public int FieldIndex { get; private set; }
			public int FromIndex { get; private set; }

			public GraphNode From { get; private set; }
			public GraphNode To { get; private set; }
			public NodeData Source { get; private set; }
			public NodeData Target { get; private set; }

			public string Name { get; private set; }

			public ConnectionData(string field, string key, int index, NodeData source, GraphNode to, int fromIndex)
			{
				Field = field;
				FieldKey = key;
				FieldIndex = index;
				Source = source;
				From = source.Node;
				FromIndex = fromIndex;
				To = to;

				if (index >= 0)
					Name = GetConnectionName(field, index);
				else if (!string.IsNullOrEmpty(key))
					Name = GetConnectionName(field, key);
				else
					Name = Name = GetConnectionName(field);
			}

			public void SetTarget(NodeData target)
			{
				Target = target;
				To = target?.Node;

				From.SetConnection(this, To);
			}

			public void ApplyConnection(object obj, GraphNode target)
			{
				var field = obj.GetType().GetField(Field, BindingFlags.Instance | BindingFlags.Public);

				if (field != null)
				{
					if (field.FieldType == typeof(GraphNode))
					{
						field.SetValue(obj, target);
					}
					else if (field.FieldType == typeof(GraphNodeDictionary))
					{
						var dictionary = field.GetValue(obj) as GraphNodeDictionary;

						if (dictionary.ContainsKey(FieldKey))
							dictionary[FieldKey] = target;
						else
							Debug.LogErrorFormat(_missingKeyError, Field, target.name);
					}
					else if (field.FieldType == typeof(GraphNodeList))
					{
						var list = field.GetValue(obj) as GraphNodeList;

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

		public virtual void SetConnection(ConnectionData connection, GraphNode target)
		{
			connection.ApplyConnection(this, target);
		}

#endregion
	}
}
