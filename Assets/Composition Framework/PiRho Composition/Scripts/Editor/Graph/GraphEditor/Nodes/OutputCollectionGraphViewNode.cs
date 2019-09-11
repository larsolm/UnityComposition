using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	public class OutputCollectionGraphViewNode : DefaultGraphViewNode
	{
		private const string _invalidListWarning = "(PCOCGVNIP) failed to bind output collection node to {0}: The property must be a GraphNodeList";
		private const string _invalidDictionaryWarning = "(PCOCGVNIP) failed to bind output collection node to {0}: The property must be a GraphNodeDictionary";

		public const string CollectionUssClassName = UssClassName + "--output-collection";

		private OutputCollectionNodeAttribute _attribute;

		public OutputCollectionGraphViewNode(GraphNode node, GraphViewConnector nodeConnector, OutputCollectionNodeAttribute attribute) : base(node, nodeConnector)
		{
			AddToClassList(CollectionUssClassName);

			_attribute = attribute;
		}

		public override void BindNode(SerializedObject serializedObject)
		{
			if (_attribute.Renameable)
			{
				var property = serializedObject.FindProperty($"{_attribute.PropertyName}._keys.Array.size");
				if (property != null)
				{
					Add(ChangeTriggerControl.Create(property, () => RefreshOutputs(property)));
					RefreshOutputs(property);
				}
				else
				{
					Debug.LogWarningFormat(Data.Node, _invalidListWarning, _attribute.PropertyName);
				}
			}
			else
			{
				var property = serializedObject.FindProperty($"{_attribute.PropertyName}._items.Array.size");
				if (property != null)
				{
					Add(ChangeTriggerControl.Create(property, () => RefreshOutputs(property)));
					RefreshOutputs(property);
				}
				else
				{
					Debug.LogWarningFormat(Data.Node, _invalidListWarning, _attribute.PropertyName);
				}
			}

			base.BindNode(serializedObject);
		}

		private void RefreshOutputs(SerializedProperty property)
		{
			ClearOutputs();
			CreateOutputs(Outputs, _nodeConnector, _attribute.Renameable ? property : null);
		}

		private void ClearOutputs()
		{
			outputContainer.Clear();
			Outputs.Clear();
		}
	}
}
