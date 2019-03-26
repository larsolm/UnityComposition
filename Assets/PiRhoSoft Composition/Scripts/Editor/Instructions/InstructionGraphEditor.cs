using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(InstructionGraph), true)]
	public class InstructionGraphEditor : Editor
	{
		private static readonly IconButton _editButton = new IconButton(IconButton.Edit, "Edit this node");

		private SerializedProperty _nodesProperty;

		protected InstructionGraph _graph { get; private set; }

		public static void SelectNode(InstructionGraphNode node)
		{
			Selection.activeObject = node;
		}

		void OnEnable()
		{
			_graph = target as InstructionGraph;
			_nodesProperty = serializedObject.FindProperty("_nodes");

			_graph.RefreshInputs();
			_graph.RefreshOutputs();

			SyncNodes();
			SetupNodes(_nodesProperty);
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor"))
				InstructionGraphWindow.ShowWindowForGraph(_graph);

			DrawDefaultInspector();
			DrawNodes(_nodesProperty);
		}

		protected virtual void SetupNodes(SerializedProperty nodes)
		{
		}

		protected virtual void DrawNodes(SerializedProperty nodes)
		{
			EditorGUILayout.PropertyField(_nodesProperty);
		}

		private void SyncNodes()
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(_graph))
				.Select(asset => asset as InstructionGraphNode)
				.Where(node => node != null)
				.ToList();

			for (var i = 0; i < _graph.Nodes.Count; i++)
			{
				if (!assets.Contains(_graph.Nodes[i]))
				{
					Debug.LogWarningFormat(this, "Syncing nodes for InstructionGraph {0}: removed a node that was not in the asset list", _graph.name);
					_graph.Nodes.RemoveAt(i--);
					DestroyImmediate(_graph.Nodes[i], true);
					EditorUtility.SetDirty(_graph);
				}
			}

			foreach (var asset in assets)
			{
				if (asset is InstructionGraphNode node && !_graph.Nodes.Contains(node))
				{
					_graph.Nodes.Add(node);
					Debug.LogWarningFormat(this, "Syncing nodes for InstructionGraph {0}: added the node {1} that was an asset but was not contained in the list", _graph.name, node.Name);
					EditorUtility.SetDirty(_graph);
				}
			}
		}

		#region Graph Modification

		public static InstructionGraphNode CreateNode(InstructionGraph graph, Type type, string name, Vector2 position)
		{
			using (new UndoScope(graph, true))
			{
				var node = CreateInstance(type) as InstructionGraphNode;
				node.hideFlags = HideFlags.HideInHierarchy;
				node.name = name;
				node.Name = name;
				node.GraphPosition = position;

				graph.Nodes.Add(node);

				Undo.RegisterCreatedObjectUndo(node, "Create Node");
				AssetDatabase.AddObjectToAsset(node, graph);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
				return node;
			}
		}

		public static InstructionGraphNode CloneNode(InstructionGraphNode node)
		{
			var clone = Instantiate(node);
			clone.hideFlags = HideFlags.HideInHierarchy;
			return clone;
		}

		public static void AddClonedNodes(InstructionGraph graph, IList<InstructionGraphNode.NodeData> nodes, Vector2 position)
		{
			var bounds = nodes[0].Bounds;

			foreach (var node in nodes)
				bounds = RectHelper.Union(bounds, node.Bounds);

			var offset = new Vector2(position.x - bounds.xMin, position.y - bounds.yMin);

			using (new UndoScope(graph, true))
			{
				foreach (var node in nodes)
				{
					node.Position += offset;
					graph.Nodes.Add(node.Node);
					Undo.RegisterCreatedObjectUndo(node.Node, "Paste Node");
					AssetDatabase.AddObjectToAsset(node.Node, graph);
				}

				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
			}
		}

		public static void DestroyNode(InstructionGraph graph, InstructionGraphNode node, IList<InstructionGraphNode.ConnectionData> connections, InstructionGraphNode start)
		{
			foreach (var connection in connections)
				ChangeConnectionTarget(graph, connection, null, connection.From == start);

			using (new UndoScope(graph, true))
			{
				graph.Nodes.Remove(node);
				Undo.DestroyObjectImmediate(node);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
			}
		}

		public static void SetNodePosition(InstructionGraph graph, InstructionGraphNode.NodeData node, Vector2 position, bool isStart)
		{
			using (new UndoScope(isStart ? graph as ScriptableObject : node.Node, true))
			{
				node.Position = position;

				if (isStart)
					graph.StartPosition = position;
			}
		}

		public static void ChangeConnectionTarget(InstructionGraph graph, InstructionGraphNode.ConnectionData connection, InstructionGraphNode.NodeData target, bool isStart)
		{
			using (new UndoScope(isStart ? graph as ScriptableObject : connection.From, true)) // From is only node that changes in this method - the rest will be rebuild automatically
			{
				connection.ChangeTarget(target);
			}
		}

		#endregion
	}
}
