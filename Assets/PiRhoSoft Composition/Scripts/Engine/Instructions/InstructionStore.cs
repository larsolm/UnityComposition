﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum InstructionInputType
	{
		Reference,
		Value
	}

	public enum InstructionOutputType
	{
		Ignore,
		Reference
	}

	[Serializable]
	public class InstructionInput : ISerializationCallbackReceiver
	{
		public InstructionInputType Type;
		public VariableDefinition Definition;
		public VariableReference Reference = new VariableReference();
		[NonSerialized] public VariableValue Value;

		[SerializeField] private SerializedVariable _value;

		public void OnBeforeSerialize()
		{
			if (Type == InstructionInputType.Value)
			{
				_value = new SerializedVariable();
				_value.SetValue(Value);
			}
		}

		public void OnAfterDeserialize()
		{
			if (Type == InstructionInputType.Value)
			{
				Value = _value.GetValue();
				_value = null;
			}
		}
	}

	[Serializable]
	public class InstructionOutput
	{
		public InstructionOutputType Type;
		public VariableDefinition Definition;
		public VariableReference Reference = new VariableReference();
	}

	public class InstructionStore : IVariableStore
	{
		private const string _missingInputError = "(CISMI) failed to read input {0}: the variable '{1}' could not be found";
		private const string _missingOutputError = "(CISMO) failed to store output {0}: the variable '{1}' could not be found";
		private const string _readOnlyOutputError = "(CISROO) failed to store output {0}: the variable '{1}' is read only";
		private const string _invalidOutputError = "(CISIOT) failed to store output {0}: the variable '{1}' has an incompatible type";

		public const string ThisStoreName = "this";
		public const string SceneStoreName = "scene";
		public const string InputStoreName = "input";
		public const string OutputStoreName = "output";

		private static SceneVariableStore _sceneStore = new SceneVariableStore();

		public InstructionContext Context { get; private set; }
		public object This { get; private set; }
		public IVariableStore Locals { get; } = new VariableStore();

		private VariableStore _inputStore = new VariableStore();
		private VariableStore _outputStore = new VariableStore();
		private ContextStore _contextStore = new ContextStore();

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName.ToLowerInvariant() == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName.ToLowerInvariant() == OutputStoreName;

		public InstructionStore(InstructionContext context, object thisObject)
		{
			SetContext(context);
			ChangeThis(thisObject);
		}

		public void SetContext(InstructionContext context)
		{
			_contextStore.Clear();

			if (context != null)
			{
				foreach (var store in context.Stores)
					_contextStore.AddVariable(store.Key, VariableValue.Create(store.Value));
			}

			Context = context;
		}

		public void ChangeThis(object thisObject)
		{
			This = thisObject;
		}

		public void WriteInputs(List<InstructionInput> inputs)
		{
			foreach (var input in inputs)
			{
				if (input.Type == InstructionInputType.Reference)
				{
					var value = input.Reference.GetValue(this);

					if (value.Type != VariableType.Empty)
						_inputStore.SetVariable(input.Definition.Name, value);
					else
						Debug.LogWarningFormat(_missingInputError, input.Definition.Name, input.Reference);
				}
				else if (input.Type == InstructionInputType.Value)
				{
					_inputStore.SetVariable(input.Definition.Name, input.Value);
				}
			}
		}

		public void ReadOutputs(List<InstructionOutput> outputs)
		{
			foreach (var output in outputs)
			{
				if (output.Type == InstructionOutputType.Reference)
				{
					var value = _outputStore.GetVariable(output.Definition.Name);

					if (value.Type != VariableType.Empty)
					{
						var result = output.Reference.SetValue(this, value);

						switch (result)
						{
							case SetVariableResult.Success: break;
							case SetVariableResult.NotFound: Debug.LogWarningFormat(_missingOutputError, output.Definition.Name, output.Reference); break;
							case SetVariableResult.ReadOnly: Debug.LogWarningFormat(_readOnlyOutputError, output.Definition.Name, output.Reference); break;
							case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(_invalidOutputError, output.Definition.Name, output.Reference); break;
						}
					}
				}
			}
		}

		public VariableValue GetVariable(string name)
		{
			switch (name.ToLowerInvariant())
			{
				case ThisStoreName: return VariableValue.Create(This);
				case SceneStoreName: return VariableValue.Create(_sceneStore);
				case InputStoreName: return VariableValue.Create(_inputStore);
				case OutputStoreName: return VariableValue.Create(_outputStore);
				default:
				{
					var context = _contextStore.GetVariable(name);

					if (context.Type != VariableType.Empty)
						return context;
					else
						return Locals.GetVariable(name);
				}
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case ThisStoreName: return SetVariableResult.ReadOnly;
				case SceneStoreName: return SetVariableResult.ReadOnly;
				case InputStoreName: return SetVariableResult.ReadOnly;
				case OutputStoreName: return SetVariableResult.ReadOnly;
				default:
				{
					var context = _contextStore.SetVariable(name, value);

					if (context == SetVariableResult.ReadOnly)
						return context;
					else
						return Locals.SetVariable(name, value);
				}
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { InputStoreName, OutputStoreName, ThisStoreName, SceneStoreName }
				.Concat(_contextStore.GetVariableNames())
				.Concat(Locals.GetVariableNames());
		}

		private class ContextStore : VariableStore
		{
			public override SetVariableResult SetVariable(string name, VariableValue value)
			{
				if (Map.TryGetValue(name, out int index))
					return SetVariableResult.ReadOnly;
				else
					return SetVariableResult.NotFound;
			}
		}
	}
}
