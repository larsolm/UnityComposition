using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class InputNodeButton
	{
		public enum ButtonType
		{
			Axis,
			Button,
			Key
		}

		[Tooltip("The type of input")]
		public ButtonType Type = ButtonType.Axis;

		[Tooltip("The input axis that is triggers this branch")]
		[Conditional(nameof(Type), (int)ButtonType.Key, Test = ConditionalTest.Inequal)]
		public string Name;

		[Tooltip("The value needed for the axis in order to be triggered")]
		[Conditional(nameof(Type), (int)ButtonType.Axis)]
		public float Value;

		[Tooltip("The key tha was pressed")]
		[Conditional(nameof(Type), (int)ButtonType.Key)]
		public KeyCode Key;

		[Tooltip("The node to go to when this item is selected")]
		public GraphNode OnSelected;
	}

	[Serializable] public class InputNodeButtonList : SerializedList<InputNodeButton> { }

	[HelpURL(Configuration.DocumentationUrl + "input-node")]
	[CreateGraphNodeMenu("Interface/Input", 3)]
	public class InputNode : GraphNode
	{
		[Tooltip("The buttons that can trigger selections")]
		[List(EmptyLabel = "Add items to create input options")]
		[Inline]
		public InputNodeButtonList Buttons = new InputNodeButtonList();

		public override Color NodeColor => Colors.InterfaceTeal;

		public override void GetConnections(NodeData data)
		{
			foreach (var button in Buttons)
				data.AddConnection(nameof(Buttons), button.Type == InputNodeButton.ButtonType.Key ? button.Key.ToString() : button.Name, button.OnSelected);

			base.GetConnections(data);
		}

		public override void SetConnection(ConnectionData connection, GraphNode target)
		{
			if (connection.Field == nameof(Buttons))
			{
				foreach (var button in Buttons)
				{
					if ((button.Type == InputNodeButton.ButtonType.Key && button.Key.ToString() == connection.FieldKey) || button.Name == connection.FieldKey)
					{
						button.OnSelected = target;
						return;
					}
				}
			}
			else
			{
				base.SetConnection(connection, target);
			}
		}

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			while (true)
			{
				yield return null;

				foreach (var button in Buttons)
				{
					switch (button.Type)
					{
						case InputNodeButton.ButtonType.Axis:
						{
							if (InputHelper.GetWasAxisPressed(button.Name, button.Value))
							{
								graph.GoTo(button.OnSelected, nameof(Buttons), button.Name);
								yield break;
							}

							break;
						}
						case InputNodeButton.ButtonType.Button:
						{
							if (InputHelper.GetWasButtonPressed(button.Name))
							{
								graph.GoTo(button.OnSelected, nameof(Buttons), button.Name);
								yield break;
							}

							break;
						}
						case InputNodeButton.ButtonType.Key:
						{
							if (InputHelper.GetWasKeyPressed(button.Key))
							{
								graph.GoTo(button.OnSelected, nameof(Buttons), button.Key.ToString());
								yield break;
							}

							break;
						}
					}
				}
			}
		}
	}
}
