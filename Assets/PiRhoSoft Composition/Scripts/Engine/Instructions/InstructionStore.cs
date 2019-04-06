using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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
		public string Name;
		public InstructionInputType Type;
		public VariableReference Reference = new VariableReference();
		[NonSerialized] public VariableValue Value;

		[SerializeField] private string _valueData;
		[SerializeField] private List<Object> _valueObjects;

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			if (Type == InstructionInputType.Value)
				VariableHandler.Save(Value, ref _valueData, ref _valueObjects);
		}

		public void OnAfterDeserialize()
		{
			if (Type == InstructionInputType.Value)
				Value = VariableHandler.Load(ref _valueData, ref _valueObjects);
		}

		#endregion
	}

	[Serializable]
	public class InstructionOutput
	{
		public string Name;
		public InstructionOutputType Type;
		public VariableReference Reference = new VariableReference();
	}

	public class InstructionStore : IVariableStore
	{
		private const string _invalidContextError = "(CISIC) failed to create context for {0}: the variable '{1}' does not satisfy the contraint";
		private const string _invalidInputError = "(CISII) failed to create input for {0}: the variable '{1}' does not satisfy the contraint";
		private const string _missingInputError = "(CISMI) failed to read input {0}: the variable '{1}' could not be found";
		private const string _missingOutputError = "(CISMO) failed to store output {0}: the variable '{1}' could not be found";
		private const string _readOnlyOutputError = "(CISROO) failed to store output {0}: the variable '{1}' is read only";
		private const string _invalidOutputError = "(CISIOT) failed to store output {0}: the variable '{1}' has an incompatible type";

		public const string SceneStoreName = "scene";
		public const string InputStoreName = "input";
		public const string OutputStoreName = "output";
		public const string LocalStoreName = "local";

		private static string[] _variableNames = new string[] { SceneStoreName, InputStoreName, OutputStoreName, LocalStoreName, CompositionManager.GlobalStoreName };
		private static SceneVariableStore _sceneStore = new SceneVariableStore();

		public string ContextName { get; private set; }
		public VariableValue Context { get; private set; }

		public VariableStore Input { get; } = new WritableStore();
		public VariableStore Output { get; } = new WritableStore();
		public VariableStore Local { get; } = new VariableStore();
		public VariableStore Global { get; } = CompositionManager.Instance.GlobalStore;

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName == OutputStoreName;
		public static bool IsInput(InstructionInput input) => input.Type == InstructionInputType.Reference && input.Reference.IsAssigned && input.Reference.StoreName == InputStoreName;
		public static bool IsOutput(InstructionOutput output) => output.Type == InstructionOutputType.Reference && output.Reference.IsAssigned && output.Reference.StoreName == OutputStoreName;

		public InstructionStore(Instruction instruction, VariableValue context)
		{
			ContextName = instruction.ContextName;
			Context = ResolveValue(instruction.ContextDefinition, context, instruction, _invalidContextError, ContextName);
		}

		public void WriteInputs(InstructionCaller instruction, IList<InstructionInput> inputs)
		{
			foreach (var input in inputs)
			{
				if (input.Type == InstructionInputType.Reference)
				{
					var value = input.Reference.GetValue(this);
					var definition = instruction.GetInputDefinition(input);

					value = ResolveValue(definition.Definition, value, instruction.Instruction, _invalidInputError, definition.Name);

					if (value.Type != VariableType.Empty)
						Input.AddVariable(input.Name, value);
					else
						Debug.LogWarningFormat(_missingInputError, input.Name, input.Reference);
				}
				else if (input.Type == InstructionInputType.Value)
				{
					Input.AddVariable(input.Name, input.Value);
				}
			}
		}

		public void WriteOutputs(IList<InstructionOutput> outputs)
		{
			foreach (var output in outputs)
				Output.AddVariable(output.Name, VariableValue.Empty);
		}

		public void ReadOutputs(IList<InstructionOutput> outputs)
		{
			foreach (var output in outputs)
			{
				if (output.Type == InstructionOutputType.Reference)
				{
					var value = Output.GetVariable(output.Name);

					if (value.Type != VariableType.Empty)
					{
						var result = output.Reference.SetValue(this, value);

						switch (result)
						{
							case SetVariableResult.Success: break;
							case SetVariableResult.NotFound: Debug.LogWarningFormat(_missingOutputError, output.Name, output.Reference); break;
							case SetVariableResult.ReadOnly: Debug.LogWarningFormat(_readOnlyOutputError, output.Name, output.Reference); break;
							case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(_invalidOutputError, output.Name, output.Reference); break;
						}
					}
				}
			}
		}

		public VariableValue GetVariable(string name)
		{
			if (ContextName == name)
				return Context;

			switch (name)
			{
				case SceneStoreName: return VariableValue.Create(_sceneStore);
				case InputStoreName: return VariableValue.Create(Input);
				case OutputStoreName: return VariableValue.Create(Output);
				case LocalStoreName: return VariableValue.Create(Local);
				case CompositionManager.GlobalStoreName: return VariableValue.Create(Global);
				default: return Local.GetVariable(name);
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (ContextName == name)
				return SetVariableResult.ReadOnly;

			switch (name)
			{
				case SceneStoreName: return SetVariableResult.ReadOnly;
				case InputStoreName: return SetVariableResult.ReadOnly;
				case OutputStoreName: return SetVariableResult.ReadOnly;
				case LocalStoreName: return SetVariableResult.ReadOnly;
				case CompositionManager.GlobalStoreName: return SetVariableResult.ReadOnly;
				default: return Local.SetVariable(name, value);
			}
		}

		public IEnumerable<string> GetVariableNames()
		{
			return _variableNames.Append(ContextName);
		}

		private VariableValue ResolveValue(ValueDefinition definition, VariableValue value, Object errorContext, string invalidError, string variableName)
		{
			if (definition.Type == VariableType.Object && definition.Constraint is ObjectVariableConstraint constraint && value.TryGetObject(out var obj))
			{
				var resolved = ComponentHelper.GetAsObject(constraint.Type, obj);
				value = VariableValue.Create(resolved);
			}

			if (definition.Type != VariableType.Empty && !definition.IsValid(value))
				Debug.LogWarningFormat(invalidError, variableName, value);

			return value;
		}
	}
}
