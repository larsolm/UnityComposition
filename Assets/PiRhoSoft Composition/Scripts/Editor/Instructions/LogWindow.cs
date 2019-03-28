using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class LogWindow : EditorWindow
	{
		private const string _noLogText = "No instructions or expressions have been executed";
		private const string _instructionFormat = "{0}: ran for {1} frames and {2:F} seconds\n";
		private const string _expressionFormat = "{0}: {1}\n";

		private Vector2 _scrollPosition;

		private int _lastInstruction = 0;
		private int _lastExpression = 0;
		private string _log = string.Empty;

		[MenuItem("Window/PiRho Soft/Log Window")]
		public static void ShowWindow()
		{
			GetWindow<LogWindow>("Log Window").Show();
		}

		void OnInspectorUpdate()
		{
			if (CompositionManager.Exists && (_lastInstruction < CompositionManager.Instance.InstructionHistory.Count || _lastExpression < CompositionManager.Instance.ExpressionHistory.Count))
			{
				UpdateLog();
				Repaint();
			}
		}

		void OnEnable()
		{
			UpdateLog();
		}

		void OnDisable()
		{
			ResetLog();
		}

		void OnGUI()
		{
			DrawToolbar();

			using (var scroller = new EditorGUILayout.ScrollViewScope(_scrollPosition))
			{
				DrawLog();
				_scrollPosition = scroller.scrollPosition;
			}

			DrawPrompt();
		}

		private void DrawToolbar()
		{
			// clear button
			// toggle clear on play
			// toggle expression capture
			// toggle binding capture
		}

		private void UpdateLog()
		{
			if (CompositionManager.Exists)
			{
				for (var i = _lastInstruction; i < CompositionManager.Instance.InstructionHistory.Count; i++)
				{
					var data = CompositionManager.Instance.InstructionHistory[i];
					_log += string.Format(_instructionFormat, data.Instruction.name, data.TotalFrames, data.TotalSeconds);
				}

				// expression stuff shouldn't come from or be stored on CompositionManager (unless it's global) - should be accessible in editor

				for (var i = _lastExpression; i < CompositionManager.Instance.ExpressionHistory.Count; i++)
				{
					var data = CompositionManager.Instance.ExpressionHistory[i];
					_log += string.Format(_expressionFormat, data.Operation.ToString(), data.Result.ToString());
				}

				ResetLog();
			}
		}

		private void DrawLog()
		{
			var text = string.IsNullOrEmpty(_log) ? _noLogText : _log;

			EditorGUILayout.TextArea(text);
		}

		private void ResetLog()
		{
			if (CompositionManager.Exists)
				CompositionManager.Instance.ClearHistory();

			_lastInstruction = 0;
			_lastExpression = 0;
		}

		private void ClearLog()
		{
			ResetLog();
			_log = string.Empty;
		}

		private void DrawPrompt()
		{
			using (new EditorGUI.DisabledScope(!CompositionManager.Exists))
			{
			}
		}

		private void ExecutePrompt()
		{
			if (CompositionManager.Exists)
			{
			}
		}
	}
}
