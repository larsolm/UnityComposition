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
		[Tooltip("The node to go to when this item is selected")] public InstructionGraphNode OnSelected;
	}

	[Serializable] public class SelectionNodeItemList : SerializedList<SelectionNodeItem> { }

	[HelpURL(Composition.DocumentationUrl + "selection-node")]
	[CreateInstructionGraphNodeMenu("Interface/Selection", 2)]
	public class SelectionNode : InstructionGraphNode
	{
		[Tooltip("The node to go to when no selection is made")]
		public InstructionGraphNode OnCanceled;

		[Tooltip("The SelectionControl to show")]
		[VariableConstraint(typeof(SelectionControl))]
		public VariableReference Control = new VariableReference();

		[Tooltip("If set an item will always be selected (unless there are none)")]
		public bool IsSelectionRequired = false;

		[Tooltip("The items to show as part of the selection")]
		[ListDisplay(ItemDisplay = ListItemDisplayType.Foldout, EmptyText = "Add items to create selection options")]
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
			SelectionNodeItem selectedItem = null;
			IVariableStore selectedVariables = null;

			if (Resolve<SelectionControl>(variables, Control, out var control))
			{
				yield return control.MakeSelection(variables, Items, IsSelectionRequired);

				selectedItem = control.SelectedItem as SelectionNodeItem;
				selectedVariables = control.SelectedVariables;
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
			}

			if (selectedItem != null)
			{
				graph.ChangeRoot(selectedVariables);
				graph.GoTo(selectedItem.OnSelected, nameof(Items), selectedItem.Id);
			}
			else
			{
				graph.GoTo(OnCanceled, nameof(OnCanceled));
			}
		}
	}
}
