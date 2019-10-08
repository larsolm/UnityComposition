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
		public const string ContainerUssClassName = UssClassName + "__container";
		public const string InvalidUssClassName = UssClassName + "--invalid";
		public const string ErrorMessageUssClassName = UssClassName + "__error";
		public const string ItemUssClassName = UssClassName + "__item";
		public const string PopupUssClassName = ItemUssClassName + "__popup";
		public const string ComboBoxUssClassName = ItemUssClassName + "__combo-box";
		public const string ArrayIndexUssClassName = ItemUssClassName + "__array-index";
		public const string EmptyUssClassName = ItemUssClassName + "--empty";
		public const string ItemInvalidUssClassName = ItemUssClassName + "--invalid";

		private const string _emptyText = "-Empty-";
		private const string _indexedText = "-Indexed-";

		private VariableReferenceControl _control;
		private MessageBox _errorMessage;

		public VariableReferencePopupControl(VariableReferenceControl control)
		{
			_control = control;

			_errorMessage = new MessageBox(MessageBoxType.Warning, "This Variable Reference cannot currently be edited with dropdowns. Either a variable definition has changed or invalid values were entered in text mode. Use text mode to fix");
			_errorMessage.AddToClassList(ErrorMessageUssClassName);
		}

		public void Refresh()
		{
			Clear();

			var baseContainer = new VisualElement();
			baseContainer.AddToClassList(ContainerUssClassName);

			var tokens = _control.Value.Tokens;
			var source = _control.Autocomplete;
			var index = 0;

			while (source != null)
			{
				var itemIndex = index++; // save this for capturing
				var token = itemIndex < tokens.Count ? tokens[itemIndex] : null;
				var item = GetItem(source, token);
				var items = GetItems(source); // This will include "as" and "Indexed" if it can be either

				var container = CreateItemContainer(baseContainer);

				if (source.AllowsCustomFields)
				{
					var name = GetName(item, token, string.Empty);
					var comboBox = new ComboBoxControl(name, items.ToList());
					comboBox.AddToClassList(ComboBoxUssClassName);
					comboBox.RegisterCallback<ChangeEvent<string>>(evt => SelectItem(evt.newValue, itemIndex));
					comboBox.TextField.isDelayed = true;

					container.Add(comboBox);
				}
				else if (items.Count() > 0) // Only draw this if there are any items that this could possibly be
				{
					if (token == null)
						container.AddToClassList(EmptyUssClassName);
					else if (item == null)
						items = items.Prepend(token.Text);

					var invalid = token != null && item == null;

					var name = GetName(item, token, _emptyText);
					var popup = new PopupField<string>(items.Prepend(_emptyText).ToList(), name);
					popup.AddToClassList(PopupUssClassName);
					popup.RegisterValueChangedCallback(evt => SelectItem(evt.newValue, itemIndex));
					popup.SetEnabled(!invalid);

					EnableInClassList(InvalidUssClassName, invalid);

					container.Add(popup);
				}

				if (token != null && token.Type == VariableReference.VariableTokenType.Type)
				{
					// TODO: Make this a Picker (requires a custom picker and provider)
					var types = source.GetTypes().Select(field => field.Name).Prepend(nameof(GameObject));
					var valid = types.Contains(token.Text);
					var popup = new PopupField<string>(valid ? types.ToList() : types.Prepend(token.Text).ToList(), token.Text);
					popup.AddToClassList(PopupUssClassName);
					popup.RegisterValueChangedCallback(evt => SelectItem(evt.newValue, itemIndex + 1));
					popup.SetEnabled(valid);

					EnableInClassList(InvalidUssClassName, !valid);

					CreateItemContainer(baseContainer).Add(popup);

					item = valid ? item?.GetTypeField(token.Text) : null;
				}

				// Only draw this 
				if (item != null && item.IsIndexable && !item.IsCastable && !item.AllowsCustomFields && index < tokens.Count)
				{
					token = tokens[index];

					if (token.Type == VariableReference.VariableTokenType.Number)
					{
						var indexField = new IntegerField { value = int.TryParse(token.Text, out var number) ? number : 0, isDelayed = true };
						indexField.AddToClassList(ArrayIndexUssClassName);
						indexField.RegisterValueChangedCallback(evt => SelectIndex(evt.newValue, itemIndex + 1));
						index++;

						container.Add(indexField);
					}
				}

				source = item;
			}

			while (index < tokens.Count)// There are excess tokens that are not defined by the AutocompleteItem so the control doesn't match
			{
				var token = tokens[index++];
				var container = CreateItemContainer(baseContainer);
				var label = new TextField { value = token.Text };
				label.AddToClassList(ItemInvalidUssClassName);
				label.SetEnabled(false);

				container.Add(label);

				AddToClassList(InvalidUssClassName);
			}

			Add(baseContainer);
			Add(_errorMessage);
		}

		private VisualElement CreateItemContainer(VisualElement parent)
		{
			var container = new VisualElement();
			container.AddToClassList(ItemUssClassName);
			parent.Add(container);
			return container;
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

			if (source.IsCastable)
				fields = fields.Prepend(VariableReference.Cast);

			if (source.IsIndexable)
				fields = fields.Prepend(_indexedText);

			return fields;
		}

		private string GetName(IAutocompleteItem item, VariableReference.VariableToken token, string emptyText)
		{
			if (token == null)
				return emptyText;

			return token.Type == VariableReference.VariableTokenType.Type ? VariableReference.Cast : item?.Name ?? token.Text;
		}

		private void SelectItem(string selectedItem, int itemIndex)
		{
			var source = _control.Autocomplete;
			var tokens = _control.Value.Tokens.GetRange(0, itemIndex).ToList();
			var selectedText = selectedItem == _emptyText ? string.Empty : selectedItem;

			if (selectedText == VariableReference.Cast)
				tokens.Add(new VariableReference.VariableToken { Text = nameof(GameObject), Type = VariableReference.VariableTokenType.Type });
			else if (int.TryParse(selectedText, out var number) || selectedText == _indexedText)
				tokens.Add(new VariableReference.VariableToken { Text = number.ToString(), Type = VariableReference.VariableTokenType.Number });
			else if (!string.IsNullOrEmpty(selectedText))
				tokens.Add(new VariableReference.VariableToken { Text = selectedText, Type = VariableReference.VariableTokenType.Name });

			for (var i = 0; i < tokens.Count && source != null; i++)
			{
				var token = tokens[i];
				var item = source.GetField(token.Text);

				if (i == tokens.Count - 1 && item != null && item.IsIndexable && !item.IsCastable && !item.AllowsCustomFields)
					tokens.Add(new VariableReference.VariableToken { Text = "0", Type = VariableReference.VariableTokenType.Number });

				source = item;
			}

			var value = VariableReference.Format(tokens);
			this.SendChangeEvent(_control.Value.Variable, value);
		}

		private void SelectIndex(int selectedIndex, int itemIndex)
		{
			var tokens = _control.Value.Tokens;
			tokens[itemIndex].Text = selectedIndex.ToString();

			this.SendChangeEvent(null, _control.Value);
		}
	}
}
