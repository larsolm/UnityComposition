using PiRhoSoft.Composition.Engine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class CommentGraphViewNode : GraphViewNode
	{
		private readonly Label _comment;
		private readonly TextField _edit;

		private CommentNode _commentNode => Data.Node as CommentNode;

		public CommentGraphViewNode(GraphNode node) : base(node, false)
		{
			title = "Comment";

			CreateDeleteButton();

			_comment = new Label(_commentNode.Comment) { tooltip = "Double click to edit this comment" };
			_edit = CreateEditableLabel(_comment, () => _commentNode.Comment, CommentChanged);

			extensionContainer.Add(_comment);

			RefreshExpandedState();
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Edit Comment", action => ShowEditableText(_edit, _commentNode.Comment));
			evt.StopPropagation();
		}

		private void CommentChanged(string comment)
		{
			_comment.text = comment;
			_commentNode.Comment = comment;

			RefreshExpandedState();
		}

		public override void OnUnselected()
		{
			base.OnUnselected();

			HideEditableText(_edit);
		}
	}
}
