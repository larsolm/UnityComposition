﻿using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[HelpURL(Composition.DocumentationUrl + "message-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Expression Binding")]
	public class ExpressionBinding : VariableBinding
	{
		[Tooltip("The expression to evaluate and display as text in this object")]
		public Expression Expression;

		public TextMeshProUGUI Text
		{
			get
			{
				if (!_text)
					_text = GetComponent<TextMeshProUGUI>();

				return _text;
			}
		}

		private TextMeshProUGUI _text;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			try
			{
				var result = Expression.Evaluate(variables);

				Text.enabled = result.Type != VariableType.Empty;
				Text.text = result.ToString();
			}
			catch
			{
				Text.enabled = false;
			}
		}
	}
}
