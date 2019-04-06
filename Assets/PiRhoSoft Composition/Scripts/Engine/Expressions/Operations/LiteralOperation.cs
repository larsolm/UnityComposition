using System.Globalization;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class LiteralOperation : Operation
	{
		private const string _invalidLiteralException = "unable to parse '{0}' as a literal {1}";

		private VariableValue _value;

		public override void Parse(ExpressionParser parser, ExpressionToken token)
		{
			var text = parser.GetText(token);

			switch (token.Type)
			{
				case ExpressionTokenType.Int:
				{
					if (int.TryParse(text, out var i))
					{
						_value = VariableValue.Create(i);
						return;
					}

					break;
				}
				case ExpressionTokenType.Float:
				{
					if (float.TryParse(text, out var f))
					{
						_value = VariableValue.Create(f);
						return;
					}

					break;
				}
				case ExpressionTokenType.String:
				{
					_value = VariableValue.Create(text);
					return;
				}
				case ExpressionTokenType.Color:
				{
					if (TryParseColor(text, out var c))
					{
						_value = VariableValue.Create(c);
						return;
					}

					break;
				}
			}

			throw new ExpressionParseException(token, _invalidLiteralException, text, token.Type);
		}

		public override VariableValue Evaluate(IVariableStore variables)
		{
			return _value;
		}

		public override void ToString(StringBuilder builder)
		{
			VariableHandler.ToString(_value, builder);
		}

		private bool TryParseColor(string text, out Color color)
		{
			if (uint.TryParse(text, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var number))
			{
				var hasAlpha = text.Length == 8;
				var rMask = hasAlpha ? 0xFF000000 : 0xFF0000;
				var gMask = hasAlpha ? 0xFF0000 : 0xFF00;
				var bMask = hasAlpha ? 0xFF00 : 0xFF;
				var aMask = hasAlpha ? 0xFF : 0x00;

				var r = number & rMask;
				var g = number & gMask;
				var b = number & bMask;
				var a = hasAlpha ? number & aMask : 255;
				var divisor = 1 / 255.0f;

				color = new Color(r * divisor, g * divisor, b * divisor, a * divisor);
				return true;
			}
			else
			{
				color = Color.black;
				return false;
			}
		}
	}
}
