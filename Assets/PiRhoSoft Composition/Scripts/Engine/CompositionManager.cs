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
		private class DefaultGlobalStore : IVariableStore
		{
			private static readonly string[] _names = new string[] { GlobalStoreName };

			public IList<string> GetVariableNames() => _names;
			public VariableValue GetVariable(string name) => name == GlobalStoreName ? VariableValue.Create(Instance.GlobalStore) : VariableValue.Empty;
			public SetVariableResult SetVariable(string name, VariableValue value) => name == GlobalStoreName ? SetVariableResult.ReadOnly : SetVariableResult.NotFound;
		}

		public const string GlobalStoreName = "global";
		public static string CommandFolder = "Commands";

		public IVariableStore DefaultStore { get; private set; } = new DefaultGlobalStore();
		public VariableStore GlobalStore { get; private set; } = new VariableStore();

		private List<VariableBinding> _bindings = new List<VariableBinding>();

		void Awake()
		{
			Resources.LoadAll(CommandFolder);
		}

		void Update()
		{
			UpdateBindings();
		}

		void LateUpdate()
		{
			InputHelper.LateUpdate();
		}

		#region Instructions

		public void RunInstruction(Instruction instruction, VariableValue context)
		{
			var store = new InstructionStore(instruction, context);
			var enumerator = instruction.Execute(store);

			StartCoroutine(new JoinEnumerator(enumerator, instruction));
		}

		public void RunInstruction(InstructionCaller caller, VariableValue context)
		{
			RunInstruction(caller, DefaultStore, context);
		}

		public void RunInstruction(InstructionCaller caller, IVariableStore store, VariableValue context)
		{
			var enumerator = caller.Execute(store ?? DefaultStore, context);
			StartCoroutine(new JoinEnumerator(enumerator, caller.Instruction));
		}

		#region Enumerator

		private class JoinEnumerator : IEnumerator
		{
			private const string _iterationLimitWarning = "(CJEIL) Cancelling JoinEnumerator after {0} unyielding iterations";

			public static int MaximumIterations = 10000;

			private IEnumerator _root;
			private Stack<IEnumerator> _enumerators = new Stack<IEnumerator>(10);

			public object Current
			{
				get { return _enumerators.Peek().Current; }
			}

			public JoinEnumerator(IEnumerator coroutine, Instruction instruction)
			{
				_root = coroutine;
				Push(coroutine);
			}

			private bool MoveNext_()
			{
				while (_enumerators.Count > 0 && Track())
				{
					var enumerator = _enumerators.Peek();
					var next = enumerator.MoveNext();

					// three scenarios
					//  - enumerator has no next: resume the parent (unless this is the root)
					//  - enumerator has a next and it is an IEnumerator: process that enumerator
					//  - enumerator has a next and it is something else: yield

					if (!next)
						Pop();
					else if (enumerator.Current is IEnumerator child)
						Push(child);
					else
						break;
				}

				return _enumerators.Count > 0;
			}

			private void Reset_()
			{
				while (_enumerators.Count > 0)
					_enumerators.Pop();

				_enumerators.Push(_root);
				_root.Reset();
			}

#if UNITY_EDITOR

			private int _iterations = 0;
			private Stack<TrackingEnumerator> _trackers = new Stack<TrackingEnumerator>(5);

			public bool MoveNext()
			{
				_iterations = 0;
				return MoveNext_();
			}

			public void Reset()
			{
				Reset_();
				_trackers.Clear();
				_iterations = 0;
			}

			private void Push(IEnumerator enumerator)
			{
				_enumerators.Push(enumerator);

				if (enumerator is TrackingEnumerator tracker)
					_trackers.Push(tracker);
			}

			private void Pop()
			{
				if (_trackers.Count > 0 && _enumerators.Peek() == _trackers.Peek())
				{
					_trackers.Peek().Finish();
					_trackers.Pop();
				}

				_enumerators.Pop();
			}

			private bool Track()
			{
				_iterations++;

				if (_iterations >= MaximumIterations)
				{
					// this is a protection against infinite loops that can crash or hang Unity

					Debug.LogWarningFormat(_iterationLimitWarning, MaximumIterations);
					_enumerators.Clear();
					_trackers.Clear();
					return false;
				}

				if (_trackers.Count > 0)
					_trackers.Peek().Continue();

				return true;
			}
#else
		
			public bool MoveNext() => MoveNext_();
			public void Reset() => Reset_();
			private void Push(IEnumerator enumerator) => _enumerators.Push(enumerator);
			private void Pop() => _enumerators.Pop();
			private bool Track() => true;

#endif
		}

		#endregion

		#region Tracking

#if UNITY_EDITOR

		public static bool LogTracking = false;

		private const string _trackingStartFormat = "{0} started";
		private const string _trackingCompleteFormat = "{0} complete: ran {1} iterations in {2} frames and {3:F3} seconds\n";

		public static Dictionary<Instruction, TrackingData> TrackingState { get; } = new Dictionary<Instruction, TrackingData>();

		public class TrackingEnumerator : IEnumerator
		{
			private IEnumerator _trackee;
			private TrackingData _data;

			public TrackingEnumerator(Instruction instruction, IEnumerator trackee)
			{
				_data = new TrackingData(instruction);
				_trackee = trackee;
			}

			public void Continue()
			{
				_data.TotalIterations++;
			}

			public void Finish()
			{
				_data.Finish();
			}

			public object Current => _trackee.Current;
			public bool MoveNext() => _trackee.MoveNext();
			public void Reset() => _trackee.Reset();
		}

		public class TrackingData
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

			public TrackingData(Instruction instruction)
			{
				TrackingState.Add(instruction, this);

				Instruction = instruction;
				StartFrame = Time.frameCount;
				StartSeconds = Time.realtimeSinceStartup;

				if (LogTracking)
					Debug.LogFormat(Instruction, _trackingStartFormat, Instruction);
			}

			public void Finish()
			{
				IsComplete = true;
				EndFrame = Time.frameCount;
				EndSeconds = Time.realtimeSinceStartup;

				if (LogTracking)
					Debug.LogFormat(Instruction, _trackingCompleteFormat, Instruction, TotalIterations, TotalFrames, TotalSeconds);

				TrackingState.Remove(Instruction);
			}
		}

		public static IEnumerator Track(Instruction instruction, IEnumerator enumerator) => new TrackingEnumerator(instruction, enumerator);

#else
		
		public static IEnumerator Track(Instruction instruction, IEnumerator enumerator) => enumerator;

#endif

		#endregion

		#endregion

		#region Bindings

		public void AddBinding(VariableBinding binding)
		{
			_bindings.Add(binding);
		}

		public void RemoveBinding(VariableBinding binding)
		{
			_bindings.Remove(binding);
		}

		private void UpdateBindings()
		{
			foreach (var binding in _bindings)
			{
				if (binding.enabled && binding.AutoUpdate)
					binding.UpdateBinding(string.Empty, null);
			}
		}

		#endregion
	}
}
