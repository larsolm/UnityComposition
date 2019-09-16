using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class Message : ISerializationCallbackReceiver
	{
		private const string _missingVariableWarning = "(CIMMV) Unable to get text from Message '{0}': the Variable '{1}' could not be found";
		private static StringBuilder _temporaryBuilder = new StringBuilder();

		[TextArea(3, 8)]
		public string Text = string.Empty;
		public bool HasText => !string.IsNullOrEmpty(Text);

		private List<MessageToken> _tokens = new List<MessageToken>();
		private string _result = string.Empty;
		private bool _dirty = true;

		public void GetInputs(IList<VariableDefinition> inputs, string storeName)
		{
#if UNITY_EDITOR
			// the editor calls this method, and may also edit the text after serialization. The simplest way to make sure it
			// is correct is to just re Parse.
			Parse(Text);
#endif

			foreach (var token in _tokens)
			{
				if (token.Reference != null && token.Reference.UsesStore(storeName))
					inputs.Add(new VariableDefinition(token.Reference.RootName));
			}
		}

		public string GetText(IVariableCollection variables, bool suppressErrors)
		{
			var changed = _dirty;

			foreach (var token in _tokens)
			{
				if (token.Reference != null)
				{
					var value = token.Reference.GetValue(variables);
					var equal = VariableHandler.IsEqual(value, token.Value);

					if (value.IsEmpty)
					{
						if (!suppressErrors)
							Debug.LogWarningFormat(_missingVariableWarning, Text, token.Reference.Variable);
					}
					else if (!equal.HasValue || !equal.Value)
					{
						token.Value = value;
						token.Text = value.ToString();
						changed = true;
					}
				}
			}

			if (changed)
			{
				_temporaryBuilder.Clear();

				foreach (var token in _tokens)
					_temporaryBuilder.Append(token.Text);

				_result = _temporaryBuilder.ToString();
				_dirty = false;
			}

			return _result;
		}

		#region Parsing

		private class MessageToken
		{
			public VariableLookupReference Reference;
			public Variable Value;
			public string Text;
			//public List<MessageToken> Tokens; // this will eventually be used to support VariableReferences that themselves contain a string with format parameters
		}

		private void Parse(string input)
		{
			_tokens.Clear();
			_result = string.Empty;
			_dirty = true;

			var start = 0;

			while (start < input.Length)
			{
				var open = input.IndexOf('{', start);
				var close = input.IndexOf('}', open + 1);

				if (open > start && input[open - 1] == '\\')
				{
					var text = input.Substring(start, open - start - 1) + '{';
					_tokens.Add(new MessageToken { Text = text });
					start = open + 1;
				}
				else if (open < 0 || close < 0)
				{
					var text = input.Substring(start, input.Length - start);
					_tokens.Add(new MessageToken { Text = text });
					start = input.Length;
				}
				else
				{
					var text = input.Substring(start, open - start);
					_tokens.Add(new MessageToken { Text = text });

					var token = new MessageToken { Reference = new VariableLookupReference() };
					token.Reference.Variable = input.Substring(open + 1, close - open - 1);
					_tokens.Add(token);

					start = close + 1;
				}
			}
		}

		#endregion

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			Parse(Text);
		}

		#endregion
	}
}
