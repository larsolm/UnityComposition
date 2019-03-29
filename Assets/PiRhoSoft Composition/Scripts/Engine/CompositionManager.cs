using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "composition-manager")]
	public class CompositionManager : GlobalBehaviour<CompositionManager>
	{
		public const string GlobalStoreName = "global";
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

		public void RunInstruction(Instruction instruction, VariableValue context)
		{
			var store = new InstructionStore(instruction.ContextName, instruction.ContextDefinition, context);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator));
		}

		public void RunInstruction(InstructionCaller caller, VariableValue context)
		{
			var enumerator = caller.Execute(context);
			StartCoroutine(new JoinEnumerator(enumerator));
		}

		#region Debugging Support

#if UNITY_EDITOR

		public class InstructionData
		{
			public Instruction Instruction;
			public IVariableStore Variables;
			public bool IsComplete;
			public int StartFrame;
			public float StartSeconds;
			public int EndFrame;
			public float EndSeconds;
			// TODO: _totalIterations from JoinEnumerator

			public int TotalFrames => IsComplete ? EndFrame - StartFrame : Time.frameCount - StartFrame;
			public float TotalSeconds => IsComplete ? EndSeconds - StartSeconds : Time.realtimeSinceStartup - StartSeconds;

			public InstructionData(Instruction instruction, IVariableStore variables)
			{
				Instruction = instruction;
				Variables = variables;
				StartFrame = Time.frameCount;
				StartSeconds = Time.realtimeSinceStartup;
			}

			public void SetComplete()
			{
				IsComplete = true;
				EndFrame = Time.frameCount;
				EndSeconds = Time.realtimeSinceStartup;
			}
		}

		public class ExpressionData
		{
			public Operation Operation;
			public VariableValue Result;

			public ExpressionData(Operation operation, VariableValue result)
			{
				Operation = operation;
				Result = result;
			}
		}

		public Dictionary<Instruction, InstructionData> InstructionState { get; } = new Dictionary<Instruction, InstructionData>();
		public List<InstructionData> InstructionHistory { get; } = new List<InstructionData>();
		public List<ExpressionData> ExpressionHistory { get; } = new List<ExpressionData>();

		private int _instructionHistoryCount = 100;
		//private int _expressionHistoryCount = 100;

		internal void InstructionStarted(Instruction instruction, IVariableStore variables)
		{
			InstructionState.Add(instruction, new InstructionData(instruction, variables));
		}

		internal void InstructionComplete(Instruction instruction)
		{
			if (InstructionState.TryGetValue(instruction, out var data))
			{
				InstructionState.Remove(instruction);
				data.SetComplete();
				InstructionHistory.Add(data);

				if (InstructionHistory.Count > _instructionHistoryCount)
					InstructionHistory.RemoveAt(0);
			}
		}

		internal void OperationComplete(Operation operation, VariableValue result)
		{
			//ExpressionHistory.Add(new ExpressionData(operation, result));
			//
			//if (ExpressionHistory.Count > _expressionHistoryCount)
			//	ExpressionHistory.RemoveAt(0);
		}

		public void ClearHistory()
		{
			InstructionHistory.Clear();
			ExpressionHistory.Clear();
		}

#else
		
		internal void InstructionStarted(Instruction instruction, IVariableStore variables) { }
		internal void InstructionComplete(Instruction instruction) { }
		internal void OperationComplete(Operation operation, VariableValue result) { }

#endif

		#endregion
	}

	public class JoinEnumerator : IEnumerator
	{
		private const string _iterationLimitWarning = "(CJEIL) Cancelling JoinEnumerator after {0} unyielding iterations";

		public static int MaximumIterations = 1000;

		private IEnumerator _root;
		private Stack<IEnumerator> _enumerators = new Stack<IEnumerator>(10);
		private int _iterations = 0;
		private int _totalIterations = 0;

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
			var result = MoveNext_();
			_totalIterations += _iterations;
			return result;
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
			_totalIterations = 0;
		}
	}
}
