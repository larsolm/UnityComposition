using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class Message
	{
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
					var variable = Text.Substring(open + 1, close - open - 1);
					_temporaryReference.Update(variable);

					if (InstructionStore.IsInput(_temporaryReference))
						inputs.Add(new VariableDefinition { Name = _temporaryReference.RootName, Definition = ValueDefinition.Create(VariableType.Empty) });

					start = close + 1;
				}
			}
		}

		public string GetText(IVariableStore variables)
		{
			_temporaryBuilder.Clear();
			Append(variables, Text, _temporaryBuilder);
			return _temporaryBuilder.ToString();
		}

		private void Append(IVariableStore variables, string input, StringBuilder output)
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

					var variable = input.Substring(open + 1, close - open - 1);
					_temporaryReference.Update(variable);

					var value = _temporaryReference.GetValue(variables);
					Append(variables, value.ToString(), output);
					start = close + 1;
				}
			}
		}
	}
}
