using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class SelectionNodeItem : SelectionItem
	{
		[Tooltip("The node to go to when this item is selected")]
		public InstructionGraphNode OnSelected;
	}

	[Serializable] public class SelectionNodeItemList : SerializedList<SelectionNodeItem> { }

	[HelpURL(Composition.DocumentationUrl + "selection-node")]
	[CreateInstructionGraphNodeMenu("Interface/Show Selection", 2)]
	public class SelectionNode : InstructionGraphNode
	{
		[Tooltip("The node to go to when no selection is made")]
		public InstructionGraphNode OnCanceled;

		[Tooltip("The SelectionControl to show")]
		[VariableConstraint(typeof(SelectionControl))]
		public VariableReference Control = new VariableReference();

		[Tooltip("The variable to store the selected item's variables in")]
		public VariableReference SelectedItem = new VariableReference { Variable = "selectedItem" };

		[Tooltip("The variable to store the selected item's index in")]
		public VariableReference SelectedIndex = new VariableReference { Variable = "selectedIndex" };

		[Tooltip("If set an item will always be selected (unless there are none)")]
		public bool IsSelectionRequired = false;

		[Tooltip("Specifies whether to automatically hide the selection control when a selection is made")]
		public bool AutoHide = true;

		[Tooltip("The items to show as part of the selection")]
		[ListDisplay(AllowCollapse = false, EmptyText = "Add items to create selection options")]
		[ClassDisplay(Type = ClassDisplayType.Foldout)]
		public SelectionNodeItemList Items = new SelectionNodeItemList();

		public override Color NodeColor => Colors.InterfaceTeal;

		public override void GetConnections(NodeData data)
		{
			foreach (var item in Items)
				data.AddConnection(nameof(Items), item.Id, item.OnSelected);

			base.GetConnections(data);
		}

		public override void SetConnection(ConnectionData connection, InstructionGraphNode target)
		{
			if (connection.Field == nameof(Items))
			{
				foreach (var item in Items)
				{
					if (item.Id == connection.FieldKey)
					{
						item.OnSelected = target;
						return;
					}
				}
			}
			else
			{
				base.SetConnection(connection, target);
			}
		}

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out SelectionControl control))
			{
				control.Show(variables, Items, IsSelectionRequired, iteration == 0);

				while (control.IsRunning)
					yield return null;

				Assign(variables, SelectedItem, control.SelectedValue);
				Assign(variables, SelectedIndex, VariableValue.Create(control.SelectedIndex));

				if (control.SelectedItem is SelectionNodeItem selectedItem)
					graph.GoTo(selectedItem.OnSelected, nameof(Items), selectedItem.Id);
				else
					graph.GoTo(OnCanceled, nameof(OnCanceled));
			}
			else
			{
				var builder = new StringBuilder();

				for (var i = 0; i < Items.Count; i++)
				{
					var item = Items[i];

					builder.Append(i == 0 ? ": " : ", ");
					builder.Append(item);
				}

				Debug.Log(builder);

				graph.GoTo(OnCanceled, nameof(OnCanceled));
			}
		}
	}
}
