using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionGraphViewWindow : EditorWindow
	{
		private const string _windowIconData = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAABJ0AAASdAHeZh94AAAAB3RJTUUH4wEOBR4Wp+XVagAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMS41ZEdYUgAAATdJREFUOE+lkUtuglAYhWnnfQwITUdEQF6CPFRimBqcdKCzdl3GCZg0cS8O3IeJ2NA90HNSBsqlsYkkPx+H++VcbpDqur5pfm+SdJem6UOSJI/Xhh79i4LRaPQ0Ho8PmNNkMqnAsqGQMQf6FwVBEDzj5ck0zfcwDD/7/f4badv2x3nmOj36QkEcx5XneVs878FNQyHToy8URFFUYsdsOByuDcOYksjzVs7odRZg+AU5xJ3v+ysSuWjlnF5nAYQSZ1wOBoMCnDUUMr3OAuxSOY6zhbgHNw2FTE8oUFWVBWWv18sgrZGnJM4+P89cp0dfKMD5+AW567o7cNWwaOWcXmcBFktd1xeWZRWaps1I5GUrL+j9VXDEb/oCv8FTw658FApw3SuK8iLL8uu1oUf/ouCW6Xz5/6mlH0LCqCZdcm2YAAAAAElFTkSuQmCC";

		private static readonly Icon _windowIcon = Icon.Base64(_windowIconData);

		private InstructionGraphViewEditor _editor;
		private bool _isLocked = false;

		[UnityEditor.MenuItem("Window/PiRho Soft/Instruction Graph")]
		public static InstructionGraphViewWindow ShowNewWindow()
		{
			var window = CreateWindow<InstructionGraphViewWindow>("Instruction Graph");
			window.titleContent.image = _windowIcon.Content;
			window._editor = new InstructionGraphViewEditor(window);
			window.rootVisualElement.Add(window._editor);
			window.Show();
			return window;
		}

		public static InstructionGraphViewWindow FindWindowForGraph(InstructionGraph graph)
		{
			var windows = Resources.FindObjectsOfTypeAll<InstructionGraphViewWindow>();

			foreach (var window in windows)
			{
				if (window._editor.CurrentGraph == graph)
					return window;
			}

			foreach (var window in windows)
			{
				if (window._editor.CurrentGraph == null)
					return window;
			}

			foreach (var window in windows)
			{
				if (!window._isLocked)
				{
					window._editor.SetGraph(graph);
					return window;
				}
			}

			return null;
		}

		public static InstructionGraphViewWindow ShowWindowForGraph(InstructionGraph graph)
		{
			var window = FindWindowForGraph(graph);

			if (window == null)
				window = ShowNewWindow();
			else
				window.Focus();

			if (window._editor.CurrentGraph != graph)
				window._editor.SetGraph(graph);

			return window;
		}

		[OnOpenAsset]
		static bool OpenAsset(int instanceID, int line)
		{
			var graph = EditorUtility.InstanceIDToObject(instanceID) as InstructionGraph;
			if (graph != null)
			{
				ShowWindowForGraph(graph);
				return true;
			}

			return false;
		}
	}
}