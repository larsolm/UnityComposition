using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(InstructionGraph), true)]
	public class InstructionGraphEditor : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var graph = target as InstructionGraph;
			graph.RefreshInputs();
			graph.RefreshOutputs();

			SyncNodes(graph);

			var container = new VisualElement();

			container.Add(new Button(() => InstructionGraphWindow.ShowWindowForGraph(graph)) { text = "Open Graph Window" });
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Instruction.ContextName))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Instruction.ContextDefinition))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Instruction.Inputs))));
			container.Add(new PropertyField(serializedObject.FindProperty(nameof(Instruction.Outputs))));
			container.Add(new PropertyField(serializedObject.FindProperty("_nodes")));

			return container;
		}

		#region Graph Modification

		public static void SyncNodes(InstructionGraph graph)
		{
			var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(graph))
				.Select(asset => asset as InstructionGraphNode)
				.Where(node => node != null)
				.ToList();

			foreach (var asset in assets)
			{
				if (asset is InstructionGraphNode node && !graph.Nodes.Contains(node))
				{
					graph.Nodes.Add(node);
					Debug.LogWarningFormat(graph, "Syncing nodes for InstructionGraph {0}: added the node {1} that was an asset but was not contained in the list", graph.name, node.Name);
					EditorUtility.SetDirty(graph);
				}
			}

			for (var i = 0; i < graph.Nodes.Count; i++)
			{
				if (!assets.Contains(graph.Nodes[i]))
				{
					Debug.LogWarningFormat(graph, "Syncing nodes for InstructionGraph {0}: removed a node that was not in the asset list", graph.name);
					DestroyImmediate(graph.Nodes[i], true);
					graph.Nodes.RemoveAt(i--);
					EditorUtility.SetDirty(graph);
				}
				else
				{
					// temporary so all old nodes have Graph assigned
					graph.Nodes[i].Graph = graph;
				}
			}
		}

		public static void SelectNode(InstructionGraphNode node)
		{
			Selection.activeObject = node;
		}

		public static InstructionGraphNode CreateNode(InstructionGraph graph, Type type, string name, Vector2 position)
		{
			using (new ChangeScope(graph))
			{
				var node = CreateInstance(type) as InstructionGraphNode;
				node.hideFlags = HideFlags.HideInHierarchy;
				node.name = name;
				node.Name = name;
				node.Graph = graph;
				node.GraphPosition = position;

				graph.Nodes.Add(node);

				Undo.RegisterCreatedObjectUndo(node, $"Create Node: {name}");
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
			var minimum = position;

			foreach (var node in nodes)
				minimum = Vector2.Min(minimum, node.Node.GraphPosition);

			var offset = position - minimum;

			using (new ChangeScope(graph))
			{
				foreach (var node in nodes)
				{
<<<<<<< HEAD
<<<<<<< HEAD
					node.Node.GraphPosition += offset;
=======
=======
>>>>>>> fe33a8dc09542dc84c0dfc6eda1b8d82a1efaf31
					node.Node.Graph = graph;
					node.Position += offset;
>>>>>>> fe33a8d... autocomplete progress
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

			using (new ChangeScope(graph))
			{
				graph.Nodes.Remove(node);
				Undo.DestroyObjectImmediate(node);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
			}
		}

		public static void SetNodePosition(InstructionGraphNode node, Vector2 position)
		{
			using (new ChangeScope(node))
				node.GraphPosition = position;
		}

		public static void ChangeConnectionTarget(InstructionGraph graph, InstructionGraphNode.ConnectionData connection, InstructionGraphNode.NodeData target, bool isStart)
		{
			using (new ChangeScope(isStart ? graph as ScriptableObject : connection.From)) // From is only node that changes in this method - the rest will be rebuild automatically
				connection.ChangeTarget(target);
		}

		#endregion
	}
}
