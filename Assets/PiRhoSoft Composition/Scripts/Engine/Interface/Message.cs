using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class Message
	{
		private const string _missingVariableWarning = "(CIMMV) Unable to set text on message '{0}': the variable '{1}' could not be found";
		private static VariableReference _temporaryReference = new VariableReference();
		private static StringBuilder _temporaryBuilder = new StringBuilder();

		[TextArea(3, 8)]
		public string Text = string.Empty;
		public bool HasText => !string.IsNullOrEmpty(Text);

		public void GetInputs(IList<VariableDefinition> inputs)
		{
			var start = 0;

			while (start < Text.Length)
			{
				var open = Text.IndexOf('{', start);
				var close = Text.IndexOf('}', open + 1);

				if (open > start && Text[open - 1] == '\\')
				{
					start = open + 1;
				}
				else if (open < 0 || close < 0)
				{
					break;
				}
				else
				{
					_temporaryReference.Variable = Text.Substring(open + 1, close - open - 1);

					if (InstructionStore.IsInput(_temporaryReference))
						inputs.Add(new VariableDefinition { Name = _temporaryReference.RootName, Definition = ValueDefinition.Create(VariableType.Empty) });

					start = close + 1;
				}
			}
		}

		public string GetText(IVariableStore variables, bool suppressErrors)
		{
			_temporaryBuilder.Clear();
			Append(variables, Text, _temporaryBuilder, suppressErrors);
			return _temporaryBuilder.ToString();
		}

		private void Append(IVariableStore variables, string input, StringBuilder output, bool suppressErrors)
		{
			var start = 0;

			while (start < input.Length)
			{
				var open = input.IndexOf('{', start);
				var close = input.IndexOf('}', open + 1);

				if (open > start && input[open - 1] == '\\')
				{
					output.Append(input, start, open - start - 1);
					output.Append('{');
					start = open + 1;
				}
				else if (open < 0 || close < 0)
				{
					output.Append(input, start, input.Length - start);
					break;
				}
				else
				{
					output.Append(input, start, open - start);

					_temporaryReference.Variable = input.Substring(open + 1, close - open - 1);

					var value = _temporaryReference.GetValue(variables);

					if (!suppressErrors && value.IsEmpty)
						Debug.LogWarningFormat(_missingVariableWarning, input, _temporaryReference.Variable);

					Append(variables, value.ToString(), output, suppressErrors);
					start = close + 1;
				}
			}
		}
	}
}
