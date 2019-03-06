using System;
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
		public const string LocalStoreName = "local";
		public const string GlobalStoreName = "global";

		private static SceneVariableStore _sceneStore = new SceneVariableStore();

		public object This { get; private set; }
		public InstructionContext Context { get; private set; }

		public VariableStore Input { get; } = new WritableStore();
		public VariableStore Output { get; } = new WritableStore();
		public VariableStore Local { get; } = new VariableStore();
		public ReadOnlyStore Global { get; } = new ReadOnlyStore();

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName == OutputStoreName;
		public static bool IsInput(InstructionInput input) => input.Type == InstructionInputType.Reference && input.Reference.IsAssigned && input.Reference.StoreName == InputStoreName;
		public static bool IsOutput(InstructionOutput output) => output.Type == InstructionOutputType.Reference && output.Reference.IsAssigned && output.Reference.StoreName == OutputStoreName;

		public InstructionStore(InstructionContext context, object thisObject)
		{
			SetContext(context);
			ChangeThis(thisObject);
		}

		public void SetContext(InstructionContext context)
		{
			Global.Clear();

			if (context != null)
			{
				foreach (var store in context.Stores)
					Global.AddVariable(store.Key, VariableValue.Create(store.Value));
			}

			Context = context;
		}

		public void ChangeThis(object thisObject)
		{
			This = thisObject;
		}

		public void WriteInputs(IList<InstructionInput> inputs)
		{
			foreach (var input in inputs)
			{
				if (input.Type == InstructionInputType.Reference)
				{
					var value = input.Reference.GetValue(this);

					if (value.Type != VariableType.Empty)
						Input.AddVariable(input.Definition.Name, value);
					else
						Debug.LogWarningFormat(_missingInputError, input.Definition.Name, input.Reference);
				}
				else if (input.Type == InstructionInputType.Value)
				{
					Input.AddVariable(input.Definition.Name, input.Value);
				}
			}
		}

		public void WriteOutputs(IList<InstructionOutput> outputs)
		{
			foreach (var output in outputs)
				Output.AddVariable(output.Definition.Name, VariableValue.Create(output.Definition.Type));
		}

		public void ReadOutputs(IList<InstructionOutput> outputs)
		{
			foreach (var output in outputs)
			{
				if (output.Type == InstructionOutputType.Reference)
				{
					var value = Output.GetVariable(output.Definition.Name);

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
			switch (name)
			{
				case ThisStoreName: return VariableValue.Create(This);
				case SceneStoreName: return VariableValue.Create(_sceneStore);
				case InputStoreName: return VariableValue.Create(Input);
				case OutputStoreName: return VariableValue.Create(Output);
				case LocalStoreName: return VariableValue.Create(Local);
				case GlobalStoreName: return VariableValue.Create(Global);
				default: return Global.GetVariable(name);
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
				case LocalStoreName: return SetVariableResult.ReadOnly;
				case GlobalStoreName: return SetVariableResult.ReadOnly;
				default: return Global.SetVariable(name, value);
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return new List<string> { ThisStoreName, SceneStoreName, InputStoreName, OutputStoreName, LocalStoreName }
				.Concat(Global.GetVariableNames());
		}
	}
}
