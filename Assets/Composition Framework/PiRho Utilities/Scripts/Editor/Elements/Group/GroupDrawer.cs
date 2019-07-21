using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(GroupAttribute))]
	class GroupDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			// TODO: style

			var group = attribute as GroupAttribute;
			var parent = property.GetParent();

			RolloutControl rollout = null;

			var sibling = parent.Copy();
			var next = sibling.NextVisible(true);

			while (next) // TODO: need end property when not at the root
			{
				var field = fieldInfo.DeclaringType.GetField(sibling.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (field != null && field.TryGetAttribute<GroupAttribute>(out var groupAttribute) && groupAttribute.Name == group.Name)
				{
					if (rollout != null)
					{
						var element = PropertyDrawerExtensions.CreateNextElement(field, groupAttribute, sibling);
						rollout.Content.Add(element);
					}
					else if (SerializedProperty.EqualContents(property, sibling))
					{
						// this property is first and is responsible for drawing
						rollout = new RolloutControl(true);
						rollout.Label.text = group.Name;

						var element = this.CreateNextElement(sibling);
						rollout.Content.Add(element);
					}
					else
					{
						// a different property was first and handled the drawing
						break;
					}
				}

				next = sibling.NextVisible(false);
			}

			return rollout ?? new VisualElement();
		}
	}
}