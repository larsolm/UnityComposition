using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(InstructionGraphNode))]
	public class InstructionGraphNodeDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var node = property.objectReferenceValue as InstructionGraphNode;
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			
			var label = new Label(node == null ? "Unconnected" : node.Name);
			label.style.flexGrow = new StyleFloat(1);

			var icon = ElementHelper.CreateIconButton(Icon.Inspect.Content, "Select and edit this node", () => InstructionGraphEditor.SelectNode(node));

			container.Add(label);
			container.Add(icon);

			return container;
		}
	}
}
