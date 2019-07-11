using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomEditor(typeof(Graph), true)]
	public class GraphEditor : UnityEditor.Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var graph = target as Graph;
			graph.RefreshInputs();
			graph.RefreshOutputs();

			SyncNodes(graph);

			var container = new VisualElement();

			container.Add(new Button(() => GraphViewWindow.ShowWindowForGraph(graph)) { text = "Open Graph Window" });
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Graph.ContextName))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Graph.ContextDefinition))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Graph.Inputs))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Graph.Outputs))));

			return container;
		}

		#region Graph Modification

		public static void SyncNodes(Graph graph)
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(graph))
				.Select(asset => asset as GraphNode)
				.Where(node => node != null)
				.ToList();

			for (var i = 0; i < graph.Nodes.Count; i++)
			{
				if (!assets.Contains(graph.Nodes[i]))
				{
					Debug.LogWarningFormat(graph, "Syncing nodes for Graph {0}: removed a node that was not in the asset list", graph.name);
					DestroyImmediate(graph.Nodes[i], true);
					graph.Nodes.RemoveAt(i--);
					EditorUtility.SetDirty(graph);
				}
			}

			foreach (var asset in assets)
			{
				if (asset is GraphNode node && !graph.Nodes.Contains(node))
				{
					graph.Nodes.Add(node);
					Debug.LogWarningFormat(graph, "Syncing nodes for Graph {0}: added the node {1} that was an asset but was not contained in the list", graph.name, node.name);
					EditorUtility.SetDirty(graph);
				}
			}
		}

		public static void SelectNode(GraphNode node)
		{
			Selection.activeObject = node;
		}

		public static GraphNode CreateNode(Graph graph, Type type, string name, Vector2 position)
		{
			using (new ChangeScope(graph))
			{
				var node = CreateInstance(type) as GraphNode;
				node.hideFlags = HideFlags.HideInHierarchy;
				node.name = name;
				node.Graph = graph;
				node.GraphPosition = position;

				graph.Nodes.Add(node);

				Undo.RegisterCreatedObjectUndo(node, $"Create Node: {name}");
				AssetDatabase.AddObjectToAsset(node, graph);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
				return node;
			}
		}

		public static GraphNode CloneNode(GraphNode node)
		{
			var clone = Instantiate(node);
			clone.name = node.name;
			clone.hideFlags = HideFlags.HideInHierarchy;
			return clone;
		}

		public static void AddClonedNodes(Graph graph, List<GraphNode> nodes, Vector2 position)
		{
			var minimum = nodes.First().GraphPosition;

			foreach (var node in nodes)
				minimum = Vector2.Min(minimum, node.GraphPosition);

			var offset = position - minimum;

			using (new ChangeScope(graph))
			{
				foreach (var node in nodes)
				{
					node.GraphPosition += offset;
					node.Graph = graph;
					graph.Nodes.Add(node);
					Undo.RegisterCreatedObjectUndo(node, "Paste Node");
					AssetDatabase.AddObjectToAsset(node, graph);
				}

				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
			}
		}

		public static void DestroyNode(Graph graph, GraphNode node)
		{
			using (new ChangeScope(graph))
			{
				graph.Nodes.Remove(node);
				Undo.DestroyObjectImmediate(node);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
			}
		}

		public static void SetNodePosition(GraphNode node, Vector2 position)
		{
			using (new ChangeScope(node.Graph))
				node.GraphPosition = position;
		}

		public static void SetConnectionTarget(Graph graph, GraphNode.ConnectionData connection, GraphNode.NodeData target)
		{
			using (new ChangeScope(graph))
				connection.SetTarget(target);
		}

		#endregion
	}
}
