using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class CustomCommand
	{
		public string Name;
		public Expression Expression;
		internal bool IsRegistered = false;
	}

	[Serializable]
	public class CustomCommandList : SerializedList<CustomCommand> { }

	[CreateAssetMenu(fileName = nameof(CommandSet), menuName = "Composition/Command Set", order = 100)]
	[HelpURL(Composition.DocumentationUrl + "command-set")]
	public class CommandSet : ScriptableObject
	{
		private const string _missingExpressionWarning = "(CCME) The Command '{0}' could not be registered because the Expression has not been set";
		private const string _invalidExpressionWarning = "(CCIE) The Command '{0}' could not be added because the Expression '{1}' failed to compile";

		[Tooltip("The Commands to make available to Expressions")]
		public CustomCommandList CustomCommands = new CustomCommandList();

		void OnEnable()
		{
			RegisterCommands();
		}

		void OnDisable()
		{
			UnregisterCommands();
		}

		#region Registration

		protected virtual void RegisterCommands()
		{
			foreach (var command in CustomCommands)
			{
				if (ValidateCommand(command))
					RegisterCommand(command);
			}
		}

		protected virtual void UnregisterCommands()
		{
			foreach (var command in CustomCommands)
				UnregisterCommand(command);
		}

		private bool ValidateCommand(CustomCommand command)
		{
			if (ApplicationHelper.IsPlaying)
			{
				if (command.Expression.HasError)
					Debug.LogWarningFormat(this, _invalidExpressionWarning, command.Name, command.Expression.Statement);
				else if (!command.Expression.IsValid)
					Debug.LogWarningFormat(this, _missingExpressionWarning, command.Name);
			}

			return command.Expression.IsValid;
		}

		private void RegisterCommand(CustomCommand command)
		{
			if (command.Expression.IsValid)
			{
				ExpressionParser.AddCommand(command.Name, new ExpressionCommand(command.Expression));
				command.IsRegistered = true;
			}
		}

		private void UnregisterCommand(CustomCommand command)
		{
			if (command.IsRegistered)
			{
				ExpressionParser.RemoveCommand(command.Name);
				command.IsRegistered = false;
			}
		}

		#endregion

		#region Editor Interface

		public void AddExpression(string name)
		{
			CustomCommands.Add(new CustomCommand { Name = name, Expression = new Expression() });
		}
		
		public void RemoveCommand(int index)
		{
			UnregisterCommand(CustomCommands[index]);
			CustomCommands.RemoveAt(index);
		}

		public void SetExpression(int index, Expression expression)
		{
			var command = CustomCommands[index];
			command.Expression = expression;
			UpdateCommand(command);
		}

		private void UpdateCommand(CustomCommand command)
		{
			UnregisterCommand(command);
			RegisterCommand(command);
		}

		public bool IsNameAvailable(string name)
		{
			return ExpressionParser.GetCommand(name) == null && GetCommand(name) < 0;
		}

		private int GetCommand(string name)
		{
			for (var i = 0; i < CustomCommands.Count; i++)
			{
				if (CustomCommands[i].Name == name)
					return i;
			}

			return -1;
		}

		#endregion
	}
}
