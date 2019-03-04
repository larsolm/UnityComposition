using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "composition-manager")]
	[AddComponentMenu("Composition/Composition Manager")]
	public class CompositionManager : SingletonBehaviour<CompositionManager>
	{
		public const string _processFailedError = "(CCMPF) Failed to process Instruction '{0}': the Instruction yielded a value other than null or IEnumerator";

		[Tooltip("The Composition asset to load when this CompositionManager is loaded")]
		[AssetPopup]
		public CommandSet Commands; // this exists to provide a place to assign a Composition asset so that it will be loaded by Unity

		public void RunInstruction(Instruction instruction, InstructionContext context, object thisObject)
		{
			var store = new InstructionStore(context, thisObject);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator));
		}

		public void RunInstruction(InstructionCaller caller, InstructionContext context, object thisObject)
		{
			var enumerator = caller.Execute(context, thisObject);
			StartCoroutine(new JoinEnumerator(enumerator));
		}
	}

	public class JoinEnumerator : IEnumerator
	{
		private IEnumerator _root;
		private Stack<IEnumerator> _enumerators = new Stack<IEnumerator>(10);

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
					MoveNext();
			}
			else if (enumerator.Current is IEnumerator child)
			{
				_enumerators.Push(child);
				MoveNext();
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
