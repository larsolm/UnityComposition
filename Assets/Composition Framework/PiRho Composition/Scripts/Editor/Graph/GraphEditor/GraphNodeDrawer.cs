using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(GraphNode))]
	public class GraphNodeDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var node = property.objectReferenceValue as GraphNode;
			var container = new FieldContainer(property.displayName, this.GetTooltip());
			
			var label = new TextElement { text = node == null ? "Unconnected" : node.name };
			label.style.flexGrow = 1;
			label.style.flexShrink = 1;
			label.style.overflow = Overflow.Hidden;

			var icon = new IconButton(Icon.Inspect.Texture, "Select and edit this node", () => GraphEditor.SelectNode(node));
			icon.style.width = 16;
			icon.style.height = 14;
			icon.style.alignSelf = Align.Center;

			container.Add(label);
			container.Add(icon);

			return container;
		}
	}
}
