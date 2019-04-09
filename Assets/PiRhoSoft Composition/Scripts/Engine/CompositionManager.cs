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
			var store = new InstructionStore(instruction, context);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator, instruction));
		}

		public void RunInstruction(InstructionCaller caller, VariableValue context)
		{
			var enumerator = caller.Execute(context);
			StartCoroutine(new JoinEnumerator(enumerator, caller.Instruction));
		}

		#region Enumerator

		private class JoinEnumerator : IEnumerator
		{
			private const string _iterationLimitWarning = "(CJEIL) Cancelling JoinEnumerator after {0} unyielding iterations";

			public static int MaximumIterations = 5000;

			private IEnumerator _root;
			private Stack<IEnumerator> _enumerators = new Stack<IEnumerator>(10);
			private int _iterations = 0;

			public object Current
			{
				get { return _enumerators.Peek().Current; }
			}

			public JoinEnumerator(IEnumerator coroutine, Instruction instruction)
			{
				_root = coroutine;
				_enumerators.Push(coroutine);

				Start(instruction);
			}

			public bool MoveNext()
			{
				_iterations = 0;
				var result = MoveNext_();

				Continue(_iterations);
				if (!result) Finish();

				return result;
			}

			private bool MoveNext_()
			{
				while (_enumerators.Count > 0)
				{
					_iterations++;

					if (_iterations >= MaximumIterations)
					{
						// this is a protection against infinite loops that can crash or hang Unity

						Debug.LogWarningFormat(_iterationLimitWarning, MaximumIterations);
						_enumerators.Clear();
						return false;
					}

					var enumerator = _enumerators.Peek();
					var next = enumerator.MoveNext();

					// three scenarios
					//  - enumerator has no next: resume the parent (unless this is the root)
					//  - enumerator has a next and it is an IEnumerator: process that enumerator
					//  - enumerator has a next and it is something else: yield

					if (!next)
						_enumerators.Pop();
					else if (enumerator.Current is IEnumerator child)
						_enumerators.Push(child);
					else
						break;
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

			#region Debugging

#if UNITY_EDITOR

			private InstructionData _data;

			private void Start(Instruction instruction)
			{
				_data = new InstructionData();
				_data.Start(instruction);
			}

			private void Continue(int iterations)
			{
				_data.Continue(iterations);
			}

			private void Finish()
			{
				_data.Finish();
			}

#else

			private void Start(Instruction instruction) { }
			private void Continue(int iterations) { }
			private void Finish() { }
			
#endif

			#endregion
		}

		#endregion

		#region Debugging

#if UNITY_EDITOR

		public static bool LogInstructions = false;

		private const string _instructionStartFormat = "{0} started";
		private const string _instructionCompleteFormat = "{0} complete: ran {1} iterations in {2} frames and {3:F} seconds\n";

		public static Dictionary<Instruction, InstructionData> InstructionState { get; } = new Dictionary<Instruction, InstructionData>();

		public class InstructionData
		{
			public Instruction Instruction;
			public bool IsComplete;
			public int StartFrame;
			public float StartSeconds;
			public int EndFrame;
			public float EndSeconds;
			public int TotalIterations;

			public int TotalFrames => IsComplete ? EndFrame - StartFrame : Time.frameCount - StartFrame;
			public float TotalSeconds => IsComplete ? EndSeconds - StartSeconds : Time.realtimeSinceStartup - StartSeconds;

			public void Start(Instruction instruction)
			{
				InstructionState.Add(instruction, this);

				if (LogInstructions)
					Debug.LogFormat(_instructionStartFormat, instruction.name);

				Instruction = instruction;
				StartFrame = Time.frameCount;
				StartSeconds = Time.realtimeSinceStartup;
			}

			public void Continue(int iterations)
			{
				TotalIterations += iterations;
			}

			public void Finish()
			{
				IsComplete = true;
				EndFrame = Time.frameCount;
				EndSeconds = Time.realtimeSinceStartup;

				if (LogInstructions)
					Debug.LogFormat(_instructionCompleteFormat, Instruction.name, TotalIterations, TotalFrames, TotalSeconds);

				InstructionState.Remove(Instruction);
			}
		}

#endif

		#endregion
	}
}
