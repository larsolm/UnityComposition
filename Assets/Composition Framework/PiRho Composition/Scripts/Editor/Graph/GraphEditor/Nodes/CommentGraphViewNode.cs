using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class CommentGraphViewNode : GraphViewNode
	{
		public const string CommentUssClassName = UssClassName + "--comment";
		public const string CommentLabelUssClassName = CommentUssClassName + "__label";

		private readonly TextElement _comment;
		private readonly TextField _edit;

		private CommentNode _commentNode => Data.Node as CommentNode;

		public CommentGraphViewNode(GraphNode node) : base(node)
		{
			AddToClassList(CommentUssClassName);

			CreateDeleteButton();

			title = "Comment";

			_comment = new TextElement { tooltip = "Double click to edit this comment" };
			_comment.AddToClassList(CommentLabelUssClassName);
			_edit = CreateEditableLabel(_comment, nameof(_commentNode.Comment), true);

			extensionContainer.Add(_comment);
			extensionContainer.style.backgroundColor = Data.Node.NodeColor * 0.8f;

			RefreshExpandedState();
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Edit Comment", action => ShowEditableText(_edit));
			evt.menu.AppendSeparator();
		}

		public override void OnSelected()
		{
			base.OnSelected();

			Selection.activeObject = Data.Node;
		}

		public override void OnUnselected()
		{
			base.OnUnselected();

			HideEditableText(_edit);
		}
	}
}
