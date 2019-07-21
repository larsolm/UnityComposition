using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class VariableStoreElement : VisualElement
	{
		public IVariableStore Store { get; private set; }

		public bool ShouldClose { get; private set; }

		public VariableStoreElement(Object owner, string label, IVariableStore variables, bool isStatic, bool isClosable)
		{
			Store = variables;

			var proxy = isStatic ? (ValuesProxy)new StaticValuesProxy(owner, variables) : new DynamicValuesProxy(owner, variables);

			//var list = new ListElement(proxy, label, "The variables contained in this variable store", false , false, false);
			//
			//if (variables is Object obj)
			//	list.AddHeaderButton(Icon.Inspect.Content, "View this object in the inspector", () => Selection.activeObject = obj);
			//
			//if (isClosable)
			//	list.AddHeaderButton(Icon.Close.Content, "Close this store", RemoveFromHierarchy);
			//
			//Add(list);
		}

		#region Proxy

		private abstract class ValuesProxy : ListProxy
		{
			private readonly Object _owner;
			protected readonly IVariableStore _variables;

			public abstract string GetName(int index);

			public ValuesProxy(Object owner, IVariableStore variables)
			{
				_owner = owner;
				_variables = variables;
			}

			public override VisualElement CreateField(int index)
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
					if (variable.HasStore)
					{
						container.Add(ElementHelper.CreateIconButton(Icon.View.Content, "View the contents of the store", () =>
						{
							using (var evt = WatchWindow.WatchEvent.GetPooled(_owner, _variables, name))
							{
								evt.target = container;
								container.SendEvent(evt);
							}
						}));
					}

					container.Add(new VariableValueElement(_owner, () => _variables.GetVariable(name), value => _variables.SetVariable(name, value), () => ValueDefinition.Create(VariableType.Empty)));
				}

				return container;
			}

			public override bool NeedsUpdate(VisualElement item, int index)
			{
				return true;
			}

			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void ReorderItem(int from, int to) { }
		}

		private class StaticValuesProxy : ValuesProxy
		{
			private readonly IList<string> _names;

			public override int ItemCount => _names.Count;
			public override string GetName(int index) => _names[index];

			public StaticValuesProxy(Object owner, IVariableStore variables) : base(owner, variables)
			{
				_names = variables.GetVariableNames();
			}
		}

		private class DynamicValuesProxy : ValuesProxy
		{
			public override int ItemCount => _variables.GetVariableNames().Count;
			public override string GetName(int index) => _variables.GetVariableNames()[index];

			public DynamicValuesProxy(Object owner, IVariableStore variables) : base(owner, variables)
			{
			}
		}

		#endregion
	}
}
