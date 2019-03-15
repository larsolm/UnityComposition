using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "composition-manager")]
	[AddComponentMenu("PiRho Soft/Composition/Composition Manager")]
	public class CompositionManager : GlobalBehaviour<CompositionManager>
	{
		public static string CommandFolder = "Commands";

		public VariableStore GlobalStore = new VariableStore();

		void Awake()
		{
			Resources.LoadAll(CommandFolder);
		}

		void LateUpdate()
		{
			InputHelper.LateUpdate();
		}

		public void RunInstruction(Instruction instruction, object root)
		{
			var store = new InstructionStore(root);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator));
		}

		public void RunInstruction(InstructionCaller caller, object root)
		{
			var enumerator = caller.Execute(root);
			StartCoroutine(new JoinEnumerator(enumerator));
		}
	}

	public class JoinEnumerator : IEnumerator
	{
		private const string _iterationLimitWarning = "(CJEIL) Cancelling JoinEnumerator after {0} unyielding iterations";

		public static int MaximumIterations = 1000;

		private IEnumerator _root;
		private Stack<IEnumerator> _enumerators = new Stack<IEnumerator>(10);
		private int _iterations = 0;

		public object Current
		{
			get { return _enumerators.Peek().Current; }
		}

		public JoinEnumerator(IEnumerator coroutine)
		{
			_enumerators.Push(coroutine);
			_root = coroutine;
		}

		public bool MoveNext()
		{
			_iterations = 0;
			return MoveNext_();
		}

		private bool MoveNext_()
		{
			// TODO: this should probably be iterative instead of recursive since the stack can get pretty deep

			_iterations++;

			if (_iterations >= MaximumIterations)
			{
				// This is a protection against infinite loops that require a Unity restart. As it stands, a stack
				// overflow will happen if the iterator continues for too long, but that will change if this becomes
				// iterative instead of recursive. Also, the iterations warning is a little nicer.

				Debug.LogWarningFormat(_iterationLimitWarning, MaximumIterations);
				_enumerators.Clear();
				return false;
			}

			var enumerator = _enumerators.Peek();
			var next = enumerator.MoveNext();

			// three scenarios
			//  - enumerator has a next and it is an IEnumerator: process that enumerator
			//  - enumerator has a next and it is something else: stop so that something else is retrievable from Current
			//  - enumerator has no next: continue running the parent, unless enumerator is the root, in which case this enumerator is finished

			if (!next)
			{
				_enumerators.Pop();

				if (_enumerators.Count > 0)
					MoveNext_();
			}
			else if (enumerator.Current is IEnumerator child)
			{
				_enumerators.Push(child);
				MoveNext_();
			}

			return _enumerators.Count > 0;
		}

		public void Reset()
		{
			while (_enumerators.Count > 0)
				_enumerators.Pop();

			_enumerators.Push(_root);
			_root.Reset();
		}
	}
}
