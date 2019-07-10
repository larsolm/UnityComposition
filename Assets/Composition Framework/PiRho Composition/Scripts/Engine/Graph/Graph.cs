using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "graph")]
	[CreateAssetMenu(menuName = "PiRho Soft/Graph", fileName = nameof(Graph), order = 100)]
	public class Graph : ScriptableObject
	{
		private const string _alreadyRunningError = "(CIAR) Failed to run Graph '{0}': the Graph is already running";

		[Tooltip("The name used to access the context object from nodes in this graph")]
		public string ContextName = "context";

		[Tooltip("The variable definition for the context object that runs this graph")]
		public ValueDefinition ContextDefinition = ValueDefinition.Empty;

		[Tooltip("The variables definitions used as inputs for this graph")]
		[List(AllowAdd = false, AllowRemove = false, AllowReorder = false, EmptyLabel = "Inputs defined in nodes will appear here")]
		public VariableDefinitionList Inputs = new VariableDefinitionList();

		[Tooltip("The variables definitions used as outputs for this graph")]
		[List(AllowAdd = false, AllowRemove = false, AllowReorder = false, EmptyLabel = "Outputs defined in nodes will appear here")]
		public VariableDefinitionList Outputs = new VariableDefinitionList();

		public GraphNode Process = null;

		public List<GraphNode> Nodes = new List<GraphNode>();

		private Stack<NodeFrame> _callstack = new Stack<NodeFrame>();
		private GraphStore _rootStore;
		private NodeFrame _nextNode;
		private bool _shouldBreak = false;
		private bool _shouldExit = false;

		public IVariableStore Variables { get; private set; }
		public bool IsRunning { get; private set; }

		void OnEnable()
		{
			// in case the editor exits play mode while the graph is running
			Variables = null;
			IsRunning = false;
		}

		void OnDisable()
		{
			// not really necessary but might as well
			Variables = null;
			IsRunning = false;
		}

		#region Input and Output Schemas

		private void GetInputs(IList<VariableDefinition> inputs)
		{
			foreach (var node in Nodes)
			{
				if (node) // if a node type is deleted, the node will be null, and this might be called before the editor does a SyncNodes
					node.GetInputs(inputs);
			}
		}

		private void GetOutputs(IList<VariableDefinition> outputs)
		{
			foreach (var node in Nodes)
			{
				if (node)
					node.GetOutputs(outputs);
			}
		}

		public void RefreshInputs()
		{
			var inputs = new VariableDefinitionList();
			GetInputs(inputs);
			Inputs = RefreshDefinitions(inputs);
		}

		public void RefreshOutputs()
		{
			var outputs = new VariableDefinitionList();
			GetOutputs(outputs);
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
			for (var i = 0; i < definitions.Count; i++)
			{
				if (definitions[i].Name == definition.Name)
				{
					if (!definitions[i].Definition.IsTypeLocked || (definitions[i].Definition.Type == definition.Definition.Type && !definitions[i].Definition.IsConstraintLocked))
					{
						definitions[i] = definition;
						return;
					}
				}
			}

			if (add)
				definitions.Add(definition);
		}

		#endregion


		#region Traversal

		public void GoTo(GraphNode node, string name)
		{
			switch (node)
			{
				case ILoopNode loop: _nextNode.Type = NodeType.Loop; break;
				case ISequenceNode sequence: _nextNode.Type = NodeType.Sequence; break;
				default: _nextNode.Type = NodeType.Normal; break;
			}

			_nextNode.Node = node;
			_nextNode.Source = name;
		}

		public void GoTo(GraphNode node, string name, int index)
		{
			var source = string.Format("{0} {1}", name, index);
			GoTo(node, source);
		}

		public void GoTo(GraphNode node, string name, string key)
		{
			var source = string.Format("{0} {1}", name, key);
			GoTo(node, source);
		}

		public void Break()
		{
			_shouldBreak = true;
		}

		public void BreakAll()
		{
			_shouldExit = true;
		}

		#endregion

		#region Playback

		public IEnumerator Execute(VariableValue context)
		{
			var store = GraphStore.Reserve(this, context);
			yield return Execute(store);
			GraphStore.Release(store);
		}

		public IEnumerator Execute(GraphStore variables)
		{
			if (IsRunning)
			{
				Debug.LogErrorFormat(this, _alreadyRunningError, name);
			}
			else
			{
				Variables = variables;
				IsRunning = true;
				yield return CompositionManager.Track(this, Run(variables, Process, nameof(Process)));
				IsRunning = false;
				Variables = null;
			}
		}

		private IEnumerator Run(GraphStore variables, GraphNode root, string source)
		{
			_rootStore = variables;

			StartRunning(root, source);
			GoTo(root, source);

			while (ShouldContinue())
			{
				var frame = SetupFrame(_nextNode);
				_nextNode.Reset();

				if (frame.Node != null)
				{
					_callstack.Push(frame);

					yield return ProcessFrame(frame);
				}

				if (_shouldBreak)
				{
					HandleBreak();
					_shouldBreak = false;
				}

				if (_shouldExit)
				{
					_shouldExit = false;
					break;
				}
			}

			_callstack.Clear();
			_nextNode.Reset();
		}

		private enum NodeType
		{
			Normal,
			Sequence,
			Loop
		}

		private struct NodeFrame
		{
			public NodeType Type;
			public int Iteration;
			public GraphNode Node;
			public string Source;

			public NodeFrame Increment()
			{
				var frame = this;
				frame.Iteration++;
				return frame;
			}

			public NodeFrame Break()
			{
				var frame = this;
				frame.Type = NodeType.Normal;
				return frame;
			}

			public void Reset()
			{
				Iteration = 0;
				Node = null;
			}
		}

#if UNITY_EDITOR

		private string _currentBranch;

		private void StartRunning(GraphNode root, string source)
		{
			_currentBranch = source;

			DebugState = PlaybackState.Running;

			if (IsDebugLoggingEnabled)
				Debug.LogFormat(this, "(Frame {0}) Graph {1}: running branch '{2}'", Time.frameCount, name, source);
		}

		private bool ShouldContinue()
		{
			if (IsDebugLoggingEnabled)
			{
				if (DebugState == PlaybackState.Stopped)
					Debug.LogFormat(this, "(Frame {0}) Graph {1}: halting branch '{2}'", Time.frameCount, name, _currentBranch);
				else if (_callstack.Count == 0 && _nextNode.Node == null)
					Debug.LogFormat(this, "(Frame {0}) Graph {1}: finished running branch '{2}'", Time.frameCount, name, _currentBranch);
			}

			return DebugState != PlaybackState.Stopped && (_callstack.Count > 0 || _nextNode.Node != null);
		}

		private IEnumerator ProcessFrame(NodeFrame frame)
		{
			if (frame.Node.IsBreakpoint && IsDebugBreakEnabled)
			{
				DebugState = PlaybackState.Paused;
				OnBreakpointHit?.Invoke(this, frame.Node);
			}

			if (DebugState == PlaybackState.Paused && IsDebugLoggingEnabled)
				Debug.LogFormat(this, "(Frame {0}) Graph {1}: pausing at node '{2}'", Time.frameCount, name, frame.Node.name);

			while (DebugState == PlaybackState.Paused)
				yield return null;

			if (DebugState == PlaybackState.Stopped)
				yield break;

			if (IsDebugLoggingEnabled)
			{
				if (frame.Iteration > 0)
					Debug.LogFormat(this, "(Frame {0}) Graph {1}: running iteration {2} of node '{3}' ", Time.frameCount, name, frame.Iteration + 1, frame.Node.name);
				else
					Debug.LogFormat(this, "(Frame {0}) Graph {1}: following '{2}' to node '{3}'", Time.frameCount, name, frame.Source, frame.Node.name);
			}

			OnProcessFrame?.Invoke(frame.Node, frame.Iteration);

			yield return frame.Node.Run(this, _rootStore, frame.Iteration);

			if (DebugState == PlaybackState.Step)
				DebugState = PlaybackState.Paused;
		}

#else

		private void StartRunning(GraphNode root, string source)
		{
		}

		private bool ShouldContinue()
		{
			return _callstack.Count > 0 || _nextNode.Node != null;
		}

		private IEnumerator ProcessFrame(NodeFrame frame)
		{
			yield return frame.Node.Run(this, _rootStore, frame.Iteration);
		}

#endif

		private NodeFrame SetupFrame(NodeFrame node)
		{
			if (node.Node == null)
			{
				// the current frame should never continue if there is no next

				if (_callstack.Count > 0)
					_callstack.Pop();

				// check if there is a sequence or loop node in the call stack to iterate

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Type != NodeType.Normal)
						return frame.Increment();
				}
			}
			else if (IsNodeInStack(node.Node))
			{
				// the node is already in the call stack, so retreat to the existing entry rather than adding a new one
				// and re-use its original variables - any loop or sequence nodes on the way are bypassed which is
				// probably the intended behavior

				while (_callstack.Count > 0)
				{
					var frame = _callstack.Pop();

					if (frame.Node == node.Node)
						return frame.Increment();
				}
			}

			return node;
		}

		private bool IsNodeInStack(GraphNode node)
		{
			foreach (var frame in _callstack)
			{
				if (frame.Node == node)
					return true;
			}

			return false;
		}

		private void HandleBreak()
		{
			while (_callstack.Count > 0)
			{
				var frame = _callstack.Pop();

				if (frame.Type == NodeType.Loop)
				{
					_callstack.Push(frame.Break());
					GoTo(null, string.Empty);
					break;
				}
			}
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

		public Action<GraphNode, int> OnProcessFrame;

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
			if (_callstack.Count > 0)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node)
						return true;
				}
			}

			return false;
		}

		public bool IsInCallStack(GraphNode node, string source)
		{
			if (_callstack.Count > 0)
			{
				foreach (var frame in _callstack)
				{
					if (frame.Node == node && frame.Source == source)
						return true;
				}
			}

			return false;
		}

#endif

		#endregion

	}
}
