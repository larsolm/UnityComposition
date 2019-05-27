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
		private static readonly Label _editButton = new Label(Icon.BuiltIn(Icon.Edit), "", "Edit this node");
		private static readonly Icon _previewIcon = Icon.Base64("iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC");

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

			SyncNodes(_graph);
			SetupNodes(_nodesProperty);
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor"))
				InstructionGraphWindow.ShowWindowForGraph(_graph);

			using (new UndoScope(serializedObject))
			{
				DrawPropertiesExcluding(serializedObject, "m_Script", "_nodes");
				DrawNodes(_nodesProperty);
			}
		}

		protected virtual void SetupNodes(SerializedProperty nodes)
		{
		}

		protected virtual void DrawNodes(SerializedProperty nodes)
		{
			EditorGUILayout.PropertyField(_nodesProperty);
		}

		public static void SyncNodes(InstructionGraph graph)
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(graph))
				.Select(asset => asset as InstructionGraphNode)
				.Where(node => node != null)
				.ToList();

			for (var i = 0; i < graph.Nodes.Count; i++)
			{
				if (!assets.Contains(graph.Nodes[i]))
				{
					Debug.LogWarningFormat(graph, "Syncing nodes for InstructionGraph {0}: removed a node that was not in the asset list", graph.name);
					DestroyImmediate(graph.Nodes[i], true);
					graph.Nodes.RemoveAt(i--);
					EditorUtility.SetDirty(graph);
				}
			}

			foreach (var asset in assets)
			{
				if (asset is InstructionGraphNode node && !graph.Nodes.Contains(node))
				{
					graph.Nodes.Add(node);
					Debug.LogWarningFormat(graph, "Syncing nodes for InstructionGraph {0}: added the node {1} that was an asset but was not contained in the list", graph.name, node.Name);
					EditorUtility.SetDirty(graph);
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
			clone.name = node.name;
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
