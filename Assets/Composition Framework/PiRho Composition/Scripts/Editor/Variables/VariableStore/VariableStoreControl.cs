using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableStoreControl : VisualElement
	{
		public IVariableStore Value { get; private set; }

		public bool ShouldClose { get; private set; }

		private ValuesProxy _proxy;
		private ListControl _list;

		public VariableStoreControl(string label, IVariableStore value, bool isStatic, bool isClosable)
		{
			Value = value;

			_proxy = isStatic ? (ValuesProxy)new StaticValuesProxy(value) : new DynamicValuesProxy(value);
			_proxy.Label = label;
			_proxy.Tooltip = "The variables contained in this variable store";
			_list = new ListControl(_proxy);
			
			if (value is Object obj)
				_list.AddHeaderButton(Icon.Inspect.Texture, "View this object in the inspector", null, () => Selection.activeObject = obj);
			
			if (isClosable)
				_list.AddHeaderButton(Icon.Close.Texture, "Close this store", null, RemoveFromHierarchy);
			
			Add(_list);
		}

		#region Proxy

		private abstract class ValuesProxy : ListProxy
		{
			protected readonly IVariableStore _variables;

			public override bool AllowAdd => false;
			public override bool AllowRemove => false;
			public override bool AllowReorder => false;

			public abstract string GetName(int index);

			public ValuesProxy(IVariableStore variables)
			{
				_variables = variables;
			}

			public override VisualElement CreateElement(int index)
			{
				var container = new VisualElement();

				var name = GetName(index);

				container.Add(new Label(name));

				var variable = _variables.GetVariable(name);

				if (variable.IsEmpty)
				{
					container.Add(new Label("(empty)"));
				}
				else
				{
					//if (variable.HasStore)
					//{
					//	var view = new IconButton(Icon.View.Texture, "View the contents of the store", () =>
					//	{
					//		using (var evt = WatchWindow.WatchEvent.GetPooled(_variables, name))
					//		{
					//			evt.target = container;
					//			container.SendEvent(evt);
					//		}
					//	});
					//
					//	container.Add(view);
					//}
					//
					//var control = new VariableValueControl(_variables.GetVariable(name), ValueDefinition.Empty);
					//control.RegisterCallback<ChangeEvent<VariableValue>>(evt =>
					//{
					//	_variables.SetVariable(name, evt.newValue);
					//});
					//
					//container.Add(control);
				}

				return container;
			}

			public override bool NeedsUpdate(VisualElement item, int index) { return true; }
			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void ReorderItem(int from, int to) { }
		}

		private class StaticValuesProxy : ValuesProxy
		{
			private readonly IList<string> _names;

			public override int ItemCount => _names.Count;
			public override string GetName(int index) => _names[index];

			public StaticValuesProxy(IVariableStore variables) : base(variables)
			{
				_names = variables.GetVariableNames();
			}
		}

		private class DynamicValuesProxy : ValuesProxy
		{
			public override int ItemCount => _variables.GetVariableNames().Count;
			public override string GetName(int index) => _variables.GetVariableNames()[index];

			public DynamicValuesProxy(IVariableStore variables) : base(variables)
			{
			}
		}

		#endregion
	}
}
