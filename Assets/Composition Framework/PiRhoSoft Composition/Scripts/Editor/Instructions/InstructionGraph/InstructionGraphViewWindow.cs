using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class InstructionGraphViewWindow : EditorWindow
	{
		private InstructionGraphView _graphView;

		private void OnEnable()
		{
			_graphView = new InstructionGraphView();

			rootVisualElement.Add(_graphView);
		}
	}
}