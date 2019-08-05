using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferencePopupControl : VisualElement
	{
		public const string UssClassName = VariableReferenceControl.SimpleUssClassName;
		public const string ItemUssClassName = UssClassName + "__item";
		public const string PopupUssClassName = ItemUssClassName + "__popup";
		public const string ComboBoxUssClassName = ItemUssClassName + "__combo-box";
		public const string ArrayIndexUssClassName = ItemUssClassName + "__array-index";
		public const string EmptyUssClassName = ItemUssClassName + "--empty";

		private const string _emptyText = "-";

		private static readonly char[] _separators = { VariableReference.Separator, VariableReference.LookupOpen, VariableReference.LookupClose };

		private VariableReferenceControl _control;

		public VariableReferencePopupControl(VariableReferenceControl control)
		{
			_control = control;

			Refresh();
		}

		public void Refresh()
		{
			Clear();

			var tokens = _control.Value.Tokens;
			var source = _control.Source;
			var index = 0;

			while (source != null)
			{
				var token = index < tokens.Count ? tokens[index] : null;
				var itemIndex = index++; // save this for capturing
				var item = source.GetItem(token?.Text);
				var items = source.Items.Select(value => value.Name);

				var container = new VisualElement();
				container.AddToClassList(ItemUssClassName);

				Add(container);

				if (source.SupportsCustom)
				{
					var comboBox = new ComboBoxControl(item?.Name ?? token?.Text ?? string.Empty, items.ToList());
					comboBox.AddToClassList(ComboBoxUssClassName);
					comboBox.RegisterCallback<ChangeEvent<string>>(evt => SelectItem(evt.newValue, itemIndex));
					comboBox.TextField.isDelayed = true;

					container.Add(comboBox);
				}
				else
				{
					if (item == null)
					{
						container.AddToClassList(EmptyUssClassName);

						if (!string.IsNullOrEmpty(token?.Text))
						{ } // Log that schema has changed
					}

					var popup = new PopupField<string>(items.Prepend(_emptyText).ToList(), item?.Name ?? _emptyText);
					popup.AddToClassList(PopupUssClassName);
					popup.RegisterValueChangedCallback(evt => SelectItem(evt.newValue, itemIndex));

					container.Add(popup);
				}

				if (item != null && item.IsArray && index < tokens.Count) // Index has already been incremented to the next token
				{
					var indexer = tokens[index++];
					var indexField = new IntegerField { value = int.TryParse(indexer.Text, out var number) ? number : 0, isDelayed = true };
					indexField.AddToClassList(ArrayIndexUssClassName);
					indexField.RegisterValueChangedCallback(evt => SelectIndex(evt.newValue, itemIndex + 1));

					container.Add(indexField);
				}

				source = item?.Source;
			}
		}

		private void SelectItem(string selectedItem, int itemIndex)
		{
			var source = _control.Source;
			var tokens = _control.Value.Tokens.GetRange(0, itemIndex).ToList();
			var selectedText = selectedItem == _emptyText ? string.Empty : selectedItem;

			if (selectedItem != _emptyText)
				tokens.Add(new VariableReference.VariableToken { Text = selectedItem, Type = VariableReference.VariableTokenType.Name });

			for (var i = 0; i < tokens.Count && source != null; i++)
			{
				var token = tokens[i];
				var item = source.GetItem(token.Text);

				if (i == tokens.Count - 1 && item != null && item.IsArray)
					tokens.Add(new VariableReference.VariableToken { Text = "0", Type = VariableReference.VariableTokenType.Number });

				source = item?.Source;
			}

			_control.Value.Tokens = tokens;

			this.SendChangeEvent(null, _control.Value);
		}

		private void SelectIndex(int selectedIndex, int itemIndex)
		{
			var tokens = _control.Value.Tokens;
			tokens[itemIndex].Text = selectedIndex.ToString();

			this.SendChangeEvent(null, _control.Value);
		}
	}
}
