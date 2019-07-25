using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class SelectionNodeItem : MenuItemTemplate
	{
		[Tooltip("The node to go to when this item is selected")]
		public GraphNode OnSelected;
	}

	[Serializable] public class SelectionNodeItemList : SerializedList<SelectionNodeItem> { }

	[HelpURL(Composition.DocumentationUrl + "selection-node")]
	[CreateGraphNodeMenu("Interface/Show Selection", 2)]
	public class SelectionNode : GraphNode
	{
		[Tooltip("The node to go to when no selection is made")]
		public GraphNode OnCanceled;

		[Tooltip("The SelectionControl to show")]
		[VariableReference(typeof(SelectionControl))]
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
		[List(EmptyLabel = "Add items to create selection options")]
		[Inline]
		public SelectionNodeItemList Items = new SelectionNodeItemList();

		public override Color NodeColor => Colors.InterfaceTeal;

		public override void GetConnections(NodeData data)
		{
			foreach (var item in Items)
				data.AddConnection(nameof(Items), item.Id, item.OnSelected);

			base.GetConnections(data);
		}

		public override void SetConnection(ConnectionData connection, GraphNode target)
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

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject(variables, Control, out SelectionControl control))
			{
				control.Show(variables, Items, IsSelectionRequired, iteration == 0);

				while (control.IsRunning)
					yield return null;

				Assign(variables, SelectedItem, control.SelectedValue);
				Assign(variables, SelectedIndex, Variable.Int(control.SelectedIndex));

				if (control.SelectedItem?.Template is SelectionNodeItem selectedItem)
					graph.GoTo(selectedItem.OnSelected, nameof(Items), selectedItem.Id);
				else
					graph.GoTo(OnCanceled, nameof(OnCanceled));

				if (AutoHide)
					control.Deactivate();
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
