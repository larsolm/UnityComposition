﻿using PiRhoSoft.Utilities.Engine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "message-node")]
	[CreateGraphNodeMenu("Interface/Show Message", 1)]
	public class MessageNode : GraphNode
	{
		[Tooltip("The node to follow when the message is dismissed")]
		public GraphNode Next = null;

		[Tooltip("The MessageControl to show")]
		[VariableConstraint(typeof(MessageControl))]
		public VariableReference Control = new VariableReference();

		[Tooltip("Specifies whether to wait for the message to complete before moving on to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Specifies whether to automatically hide the message when it is complete")]
		public bool AutoHide = false;

		[Tooltip("How long to wait after message is complete before hiding")]
		[Conditional(nameof(AutoHide), true)]
		public float WaitTime = 0.0f;

		[Tooltip("The message to display when this node is traversed")]
		public Message Message = new Message();

		public override Color NodeColor => Colors.InterfaceCyan;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			var text = Message.GetText(variables, false);

			if (ResolveObject(variables, Control, out MessageControl message))
				yield return Show(message, text);

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator Show(MessageControl message, string text)
		{
			message.Show(text);

			if (WaitForCompletion)
			{
				while (message.IsRunning)
					yield return null;
			}

			if (AutoHide)
			{
				if (WaitForCompletion)
					yield return Hide(message);
				else
					message.StartCoroutine(Hide(message));
			}
		}

		private IEnumerator Hide(MessageControl message)
		{
			if (WaitTime > 0.0f)
				yield return new WaitForSeconds(WaitTime);
	
			message.Deactivate();
		}
	}
}
