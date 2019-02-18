using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class InterfaceReferenceControl : ObjectControl<InterfaceReference>
	{
		private InterfaceReference _lookup;

		public static float GetHeight(InterfaceReference lookup)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public static void Draw(Rect position, InterfaceReference lookup, GUIContent label)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				var content = string.Format("{0}.{1}", lookup.InterfaceName, lookup.ControlName);
				var output = EditorGUI.TextField(position, label, content);

				if (changes.changed)
				{
					var separator = output.IndexOf('.');
					if (separator < 0)
					{
						lookup.InterfaceName = "";
						lookup.ControlName = output;
					}
					else
					{
						lookup.InterfaceName = output.Substring(0, separator);
						lookup.ControlName = output.Substring(separator + 1);
					}
				}
			}
		}

		public override void Setup(InterfaceReference target, SerializedProperty property, FieldInfo fieldInfo)
		{
			_lookup = target;
		}

		public override float GetHeight(GUIContent label)
		{
			return GetHeight(_lookup);
		}

		public override void Draw(Rect position, GUIContent label)
		{
			Draw(position, _lookup, label);
		}
	}

	[CustomPropertyDrawer(typeof(InterfaceReference))]
	public class InterfaceReferenceDrawer : ControlDrawer<InterfaceReferenceControl>
	{
	}
}
