using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphViewWindow : EditorWindow
	{
		private const string _windowIconData = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC";

		private static readonly Icon _windowIcon = Icon.Base64(_windowIconData);

		private GraphViewEditor _editor;
		private Graph _graph;

		static GraphViewWindow()
		{
			// This is in a static constructor so that a new window opens if one isn't already open
			Graph.OnBreakpointHit += BreakpointHit;
		}

		[MenuItem("Window/PiRho Composition/Graph View")]
		public static GraphViewWindow ShowNewWindow()
		{
			var window = CreateWindow<GraphViewWindow>("Composition Graph");
			window.titleContent.image = _windowIcon.Texture;
			window.Show();
			return window;
		}

		public static GraphViewWindow FindWindowForGraph(Graph graph)
		{
			var windows = Resources.FindObjectsOfTypeAll<GraphViewWindow>();

			foreach (var window in windows)
			{
				if (window._editor == null)
					window.Close();
			}

			foreach (var window in windows)
			{
				if (window._editor != null && window._editor.CurrentGraph == graph)
					return window;
			}

			foreach (var window in windows)
			{
				if (window._editor != null && window._editor.CurrentGraph == null)
					return window;
			}

			foreach (var window in windows)
			{
				if (window._editor != null && !window._editor.IsLocked)
				{
					window._editor.SetGraph(graph);
					return window;
				}
			}

			return null;
		}

		public static GraphViewWindow ShowWindowForGraph(Graph graph)
		{
			var window = FindWindowForGraph(graph);

			if (window == null)
				window = ShowNewWindow();
			else
				window.Show();

			window.Focus();

			if (window._editor.CurrentGraph != graph)
				window._editor.SetGraph(graph);

			return window;
		}

		[OnOpenAsset]
		public static bool OpenGraph(int instanceID, int line)
		{
			var graph = EditorUtility.InstanceIDToObject(instanceID) as Graph;
			if (graph != null)
			{
				ShowWindowForGraph(graph);
				return true;
			}

			return false;
		}

		public static void BreakpointHit(Graph graph, GraphNode node)
		{
			var window = ShowWindowForGraph(graph);
			window._editor.GraphView.GoToNode(node);
		}

		private void OnEnable()
		{
			if (_editor == null)
			{
				_editor = new GraphViewEditor(this);
				rootVisualElement.Add(_editor);

				if (_graph != null)
					_editor.SetGraph(_graph);
			}
		}

		private void OnDisable()
		{
			_graph = _editor.CurrentGraph;
			_editor = null;
		}
	}
}
