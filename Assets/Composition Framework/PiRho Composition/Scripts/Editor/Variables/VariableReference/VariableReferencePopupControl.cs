using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
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
			var source = _control.Autocomplete;
			var index = 0;

			while (source != null)
			{
				var itemIndex = index++; // save this for capturing
				var token = itemIndex < tokens.Count ? tokens[itemIndex] : null;
				var item = GetItem(source, token);
				var items = GetItems(source);

				var container = new VisualElement();
				container.AddToClassList(ItemUssClassName);

				Add(container);

				if (source.AllowsCustomFields)
				{
					var comboBox = new ComboBoxControl(item?.Name ?? token?.Text ?? string.Empty, items.ToList());
					comboBox.AddToClassList(ComboBoxUssClassName);
					comboBox.RegisterCallback<ChangeEvent<string>>(evt => SelectItem(evt.newValue, itemIndex));
					comboBox.TextField.isDelayed = true;

					container.Add(comboBox);
				}
				else
				{
					if (token == null)
						container.AddToClassList(EmptyUssClassName);

					var name = GetName(item, token);
					var popup = new PopupField<string>(items.Prepend(_emptyText).ToList(), name);
					popup.AddToClassList(PopupUssClassName);
					popup.RegisterValueChangedCallback(evt => SelectItem(evt.newValue, itemIndex));

					container.Add(popup);
				}

				if (token != null && token.Type == VariableReference.VariableTokenType.Type && index < tokens.Count)
				{
					token = tokens[index++];
					var types = source.GetTypes().Select(field => field.Name).Prepend(nameof(GameObject));
					var popup = new PopupField<string>(types.ToList(), token.Text);
					popup.AddToClassList(PopupUssClassName);
					popup.RegisterValueChangedCallback(evt => SelectItem(evt.newValue, itemIndex + 1));

					var itemContainer = new VisualElement();
					itemContainer.AddToClassList(ItemUssClassName);
					itemContainer.Add(popup);

					item = new ObjectAutocompleteItem(null);

					Add(itemContainer);
				}

				if (item != null && item.IsIndexable && index < tokens.Count)
				{
					token = tokens[index++];
					var indexField = new IntegerField { value = int.TryParse(token.Text, out var number) ? number : 0, isDelayed = true };
					indexField.AddToClassList(ArrayIndexUssClassName);
					indexField.RegisterValueChangedCallback(evt => SelectIndex(evt.newValue, itemIndex + 1));

					container.Add(indexField);
				}

				source = item;
			}
		}

		private IAutocompleteItem GetItem(IAutocompleteItem source, VariableReference.VariableToken token)
		{
			if (token != null && token.Type == VariableReference.VariableTokenType.Name)
				return source.IsIndexable ? source.GetIndexField() : source.GetField(token.Text);

			return null;
		}

		private IEnumerable<string> GetItems(IAutocompleteItem source)
		{
			var fields = source.GetFields()?.Select(field => field.Name);
			return source.IsCastable ? fields.Prepend(VariableReference.Cast) : fields;
		}

		private string GetName(IAutocompleteItem item, VariableReference.VariableToken token)
		{
			return token?.Type == VariableReference.VariableTokenType.Type ? VariableReference.Cast : item?.Name ?? _emptyText;
		}

		private void SelectItem(string selectedItem, int itemIndex)
		{
			var source = _control.Autocomplete;
			var tokens = _control.Value.Tokens.GetRange(0, itemIndex).ToList();
			var selectedText = selectedItem == _emptyText ? string.Empty : selectedItem;

			if (selectedItem == VariableReference.Cast)
			{
				tokens.Add(new VariableReference.VariableToken { Text = VariableReference.Cast, Type = VariableReference.VariableTokenType.Type });
				tokens.Add(new VariableReference.VariableToken { Text = nameof(GameObject), Type = VariableReference.VariableTokenType.Type });
			}
			else if (selectedItem != _emptyText)
			{
				tokens.Add(new VariableReference.VariableToken { Text = selectedItem, Type = VariableReference.VariableTokenType.Name });
			}

			for (var i = 0; i < tokens.Count && source != null; i++)
			{
				var token = tokens[i];
				var item = source.GetField(token.Text);

				if (i == tokens.Count - 1 && item != null && item.IsIndexable)
					tokens.Add(new VariableReference.VariableToken { Text = "0", Type = VariableReference.VariableTokenType.Number });

				source = item;
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
