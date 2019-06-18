﻿using PiRhoSoft.PargonUtilities.Engine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(InlineAttribute))]
	class InlineDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var inline = attribute as InlineAttribute;
			var container = ElementHelper.CreatePropertyContainer(inline.ShowMemberLabels ? null : property.displayName);
			var end = property.GetEndProperty();

			property.NextVisible(true);

			while (!SerializedProperty.EqualContents(property, end))
			{
				var field = inline.ShowMemberLabels ? new PropertyField(property) : new PropertyField(property, null);

				container.Add(field);
				property.NextVisible(false);
			}

			return container;
		}
	}
}
