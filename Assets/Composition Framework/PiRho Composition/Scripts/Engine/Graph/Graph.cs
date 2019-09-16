using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public interface IGraphRunner
	{
		void Exit();
		void GoTo(GraphNode node, string source);
		IEnumerator Run(GraphNode node, IVariableCollection variables, string source);
	}

	[HelpURL(Configuration.DocumentationUrl + "graph")]
	[CreateAssetMenu(menuName = "PiRho Soft/Graph", fileName = nameof(Graph), order = 100)]
	public class Graph : ScriptableObject
	{
		// TODO: need to ensure a node isn't run by two different runners at the same time

		private const string _graphAlreadyRunningError = "(PCGGAR) Failed to run graph '{0}': the graph is already running";
		private const string _nodeAlreadyRunningError = "(PCGNAR) Failed to run GraphNode '{0}' on Graph '{1}': the node is already running";

		[Tooltip("The definition for the object that runs this graph")]
		public VariableDefinition Context = new VariableDefinition("context");

		[Tooltip("The definitions for input variables used in this graph")]
		[List(AllowAdd = ListAttribute.Never, AllowRemove = ListAttribute.Never, AllowReorder = ListAttribute.Never, EmptyLabel = "Input variables defined in nodes will appear here")]
		public VariableDefinitionList Inputs = new VariableDefinitionList();

		[Tooltip("The definitions for output variables used in this graph")]
		[List(AllowAdd = ListAttribute.Never, AllowRemove = ListAttribute.Never, AllowReorder = ListAttribute.Never, EmptyLabel = "Output variables defined in nodes will appear here")]
		public VariableDefinitionList Outputs = new VariableDefinitionList();

		public GraphNode StartNode = null;
		public List<GraphNode> Nodes = new List<GraphNode>();

		public bool IsRunning { get; private set; }
		public bool IsExiting { get; private set; }
		public IVariableCollection Variables { get; private set; }
		private List<GraphRunner> _runners = new List<GraphRunner>();

		#region Reset

		void OnEnable() => Reset();
		void OnDisable() => Reset();

		private void Reset()
		{
			// in case the editor exits play mode while the graph is running
			Variables = null;
			IsRunning = false;

			foreach (var runner in _runners)
				_graphRunnerPool.Release(runner as GraphRunner);

			_runners.Clear();
		}

		#endregion

		#region Input and Output Schemas

		public void GetInputs(IList<VariableDefinition> inputs, string storeName)
		{
			foreach (var node in Nodes)
			{
				if (node) // if a node type is deleted, the node will be null, and this might be called before the editor does a SyncNodes
					node.GetInputs(inputs, storeName);
			}
		}

		public void GetOutputs(IList<VariableDefinition> outputs, string storeName)
		{
			foreach (var node in Nodes)
			{
				if (node)
					node.GetOutputs(outputs, storeName);
			}
		}

		public void RefreshInputs()
		{
			var inputs = new VariableDefinitionList();
			GetInputs(inputs, GraphStore.InputStoreName);
			Inputs = RefreshDefinitions(inputs);
		}

		public void RefreshOutputs()
		{
			var outputs = new VariableDefinitionList();
			GetOutputs(outputs, GraphStore.OutputStoreName);
			Outputs = RefreshDefinitions(outputs);
		}

		private VariableDefinitionList RefreshDefinitions(VariableDefinitionList requests)
		{
			var results = new VariableDefinitionList();

			foreach (var request in requests)
				UpdateDefinition(results, request, true);

			foreach (var existing in Inputs)
				UpdateDefinition(results, existing, false);

			return results;
		}

		private void UpdateDefinition(VariableDefinitionList definitions, VariableDefinition definition, bool add)
		{
			// TODO: handle conflicting definitions with the same name (i.e log warning) and proper application of
			// existing definitions (i.e only update type/constraint if the generated one is empty/null)

			for (var i = 0; i < definitions.Count; i++)
			{
				if (definitions[i].Name == definition.Name)
				{
					definitions[i] = definition;
					return;
				}
			}

			if (add)
				definitions.Add(definition);
		}

		#endregion

		#region Playback

		public IEnumerator Execute(Variable context)
		{
			var store = GraphStore.Reserve(this, context);
			yield return Execute(store);
			GraphStore.Release(store);
		}

		public IEnumerator Execute(GraphStore variables)
		{
			if (IsRunning)
			{
				Debug.LogErrorFormat(this, _graphAlreadyRunningError, name);
			}
			else
			{
				Variables = variables;
				IsRunning = true;

				yield return CompositionManager.Track(this, Run(StartNode, Variables, nameof(NodeStarting)));

				IsRunning = false;
				Variables = null;
			}
		}

		private IEnumerator Run(GraphNode node, IVariableCollection variables, string source)
		{
			var runner = _graphRunnerPool.Reserve();
			_runners.Add(runner);

#if UNITY_EDITOR
			NodeStarting(source);
#endif

			yield return runner.Run(this, node, variables, source);

#if UNITY_EDITOR
			NodeFinished(source);
#endif

			_runners.Remove(runner);
			_graphRunnerPool.Release(runner);
		}

		#endregion

		#region Debugging

#if UNITY_EDITOR

		public enum PlaybackState
		{
			Running,
			Paused,
			Step,
			Stopped
		}

		public PlaybackState DebugState { get; private set; }

		public static bool IsDebugBreakEnabled = true;
		public static bool IsDebugLoggingEnabled = false;
		public static Action<Graph, GraphNode> OnBreakpointHit;

		public Action<GraphNode> OnProcessFrame;

		public bool CanDebugPlay => IsRunning && DebugState == PlaybackState.Paused;
		public bool CanDebugPause => IsRunning && DebugState == PlaybackState.Running;
		public bool CanDebugStep => IsRunning && DebugState == PlaybackState.Paused;
		public bool CanDebugStop => IsRunning && DebugState != PlaybackState.Stopped;

		public void DebugPlay()
		{
			if (CanDebugPlay)
				DebugState = PlaybackState.Running;
		}

		public void DebugPause()
		{
			if (CanDebugPause)
				DebugState = PlaybackState.Paused;
		}

		public void DebugStep()
		{
			if (CanDebugStep)
				DebugState = PlaybackState.Step;
		}

		public void DebugStop()
		{
			if (CanDebugStop)
				DebugState = PlaybackState.Stopped;
		}

		public bool IsInCallStack(GraphNode node)
		{
			foreach (var runner in _runners)
			{
				if (runner.IsInCallStack(node))
					return true;
			}

			return false;
		}

		public bool IsInCallStack(GraphNode.ConnectionData connection)
		{
			foreach (var runner in _runners)
			{
				if (runner.IsInCallStack(connection))
					return true;
			}

			return false;
		}

		private void NodeStarting(string source)
		{
			if (IsDebugLoggingEnabled)
				Debug.Log($"(Frame {Time.frameCount}) Graph {name}: running '{source}'", this);
		}

		private IEnumerator ProcessNode(IGraphRunner runner, GraphNode node, IVariableCollection variables, string source)
		{
			if (node.IsBreakpoint && IsDebugBreakEnabled)
			{
				DebugPause();
				OnBreakpointHit?.Invoke(this, node);
			}

			if (DebugState == PlaybackState.Paused && IsDebugLoggingEnabled)
				Debug.Log($"(Frame {Time.frameCount}) Graph {name}: pausing at node '{node.name}'");

			while (DebugState == PlaybackState.Paused)
				yield return null;

			if (DebugState == PlaybackState.Stopped)
				yield break;

			if (IsDebugLoggingEnabled)
				Debug.Log($"(Frame {Time.frameCount}) Graph {name}: following '{source}' to node '{node.name}'");

			OnProcessFrame?.Invoke(node);

			yield return node.Run(runner, variables);

			if (DebugState == PlaybackState.Step)
				DebugPause();
		}

		private void NodeFinished(string source)
		{
			if (IsDebugLoggingEnabled)
			{
				if (DebugState == PlaybackState.Stopped)
					Debug.Log($"(Frame {Time.frameCount}) Graph {name}: halting '{source}'", this);
				else
					Debug.Log($"(Frame {Time.frameCount}) Graph {name}: finished '{source}'", this);
			}
		}

#endif

		#endregion

		#region GraphRunner

		private class GraphRunnerPoolInfo : IPoolInfo { public int Size => 10; public int Growth => 5; }
		private static ClassPool<GraphRunner, GraphRunnerPoolInfo> _graphRunnerPool = new ClassPool<GraphRunner, GraphRunnerPoolInfo>();

		private class GraphRunner : IGraphRunner, IPoolable
		{
			private struct NodeFrame
			{
				public GraphNode Node;
				public string Source;
			}

			private Graph _graph;
			private NodeFrame _nextNode;

			public void GoTo(GraphNode node, string source)
			{
				if (!_graph.IsExiting)
				{
					_nextNode.Node = node;
					_nextNode.Source = source;
				}
			}

			public IEnumerator Run(GraphNode node, IVariableCollection variables, string source)
			{
				if (!_graph.IsExiting)
					yield return CompositionManager.Instance.GetEnumerator(_graph.Run(node, variables, source));
			}

			public IEnumerator Run(Graph graph, GraphNode root, IVariableCollection variables, string source)
			{
				_graph = graph;
				_graph.IsExiting = false;
				GoTo(root, source);

#if UNITY_EDITOR
				while (Iterate())
#else
				while(_nextNode.Node != null)
#endif
				{
					var node = _nextNode;
					_nextNode.Node = null;

					while (node.Node.IsRunning)
						yield return null;

					node.Node.IsRunning = true;

#if UNITY_EDITOR
					yield return _graph.ProcessNode(this, node.Node, variables, node.Source);
#else
					yield return node.Node.Run(this, variables);
#endif

					node.Node.IsRunning = false;
				}
			}

			public void Reset()
			{
				_graph = null;
				_nextNode = new NodeFrame();

#if UNITY_EDITOR
				_callstack.Clear();
#endif
			}

			public void Exit()
			{
				GoTo(null, string.Empty);
				_graph.IsExiting = true;
			}

#if UNITY_EDITOR
			private Stack<NodeFrame> _callstack = new Stack<NodeFrame>();

			private bool Iterate()
			{
				if (_graph.DebugState != PlaybackState.Stopped && _nextNode.Node != null)
				{
					_callstack.Push(_nextNode);
					return true;
				}

				return false;
			}

			public bool IsInCallStack(GraphNode node)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node)
						return true;
				}

				return false;
			}

			public bool IsInCallStack(GraphNode.ConnectionData connection)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == connection.To && frame.Source == connection.From.name)
						return true;
				}

				return false;
			}
#endif
		}

		#endregion
	}
}