using PiRhoSoft.CompositionExample;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(TestClass))]
	public class TestClassDrawer : PropertyDrawer
	{
		private class DummyEvent : EventBase<DummyEvent>
		{
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = property.CreateField();
			var trigger = new ChangeTriggerControl<TestClass>(property, (oldValue, newValue) => Debug.Log("changed"));
			var wrapper = new VisualElement();
			//wrapper.RegisterCallback<DummyEvent>(evt => { });

			wrapper.Add(element);
			wrapper.Add(trigger);

			return wrapper;
		}

		public class WrapperElement : VisualElement
		{
			public override void HandleEvent(EventBase evt)
			{
				if (evt is IChangeEvent change)
				{
					Debug.Log("Changed");
				}

				base.HandleEvent(evt);
			}
		}
	}
}