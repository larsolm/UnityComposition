﻿using PiRhoSoft.UtilityEngine;
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
				_valueData = VariableHandler.SaveValue(Value, ref _valueObjects);
		}

		public void OnAfterDeserialize()
		{
			if (Type == InstructionInputType.Value)
				Value = VariableHandler.LoadValue(ref _valueData, ref _valueObjects);
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

	public class InstructionStore : IVariableStore, IPoolable
	{
		private const string _invalidContextError = "(CISIC) Failed to create context '{0}' for instruction '{1}': the value '{2}' does not satisfy the constraint";
		private const string _invalidInputError = "(CISII) Failed to create input '{0}' for instruction '{1}': the value '{2}' does not satisfy the constraint";
		private const string _missingInputError = "(CISMI) Failed to read input '{0}' for instruction '{1}': the variable '{2}' could not be found";
		private const string _missingOutputError = "(CISMO) Failed to store output '{0}' for instruction '{1}': the variable '{2}' could not be found";
		private const string _readOnlyOutputError = "(CISROO) Failed to store output '{0}' for instruction '{1}': the variable '{2}' is read only";
		private const string _invalidOutputError = "(CISIOT) Failed to store output '{0}' for instruction '{1}': the variable '{2}' has an incompatible type";

		public const string InputStoreName = "input";
		public const string OutputStoreName = "output";
		public const string LocalStoreName = "local";

		public string ContextName { get; private set; }
		public VariableValue Context { get; private set; }

		public VariableStore Input { get; } = new WritableStore();
		public VariableStore Output { get; } = new WritableStore();
		public VariableStore Local { get; } = new VariableStore();

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName == OutputStoreName;
		public static bool IsInput(InstructionInput input) => input.Type == InstructionInputType.Reference && input.Reference.IsAssigned && input.Reference.StoreName == InputStoreName;
		public static bool IsOutput(InstructionOutput output) => output.Type == InstructionOutputType.Reference && output.Reference.IsAssigned && output.Reference.StoreName == OutputStoreName;

		private readonly string[] _variableNames = new string[] { InputStoreName, OutputStoreName, LocalStoreName, CompositionManager.GlobalStoreName, CompositionManager.SceneStoreName, string.Empty };

		#region Pooling

		private class PoolInfo : IPoolInfo
		{
			public int Size => 10;
			public int Growth => 5;
		}

		private static ClassPool<InstructionStore, PoolInfo> _pool = new ClassPool<InstructionStore, PoolInfo>();

		internal static InstructionStore Reserve(Instruction instruction, VariableValue context)
		{
			var store = _pool.Reserve();

			store.ContextName = instruction.ContextName;
			store.Context = ResolveValue(instruction.ContextDefinition, context, instruction, _invalidContextError, store.ContextName);
			store._variableNames[store._variableNames.Length - 1] = store.ContextName;

			return store;
		}

		internal static void Release(InstructionStore store)
		{
			_pool.Release(store);
		}

		#endregion

		#region Inputs and Outputs

		public void WriteInputs(InstructionCaller instruction, IList<InstructionInput> inputs, IVariableStore caller)
		{
			foreach (var input in inputs)
			{
				if (input.Type == InstructionInputType.Reference)
				{
					var value = input.Reference.GetValue(caller);
					var definition = instruction.GetInputDefinition(input);

					value = ResolveValue(definition.Definition, value, instruction.Instruction, _invalidInputError, definition.Name);

					if (value.Type != VariableType.Empty)
						Input.AddVariable(input.Name, value);
					else
						Debug.LogWarningFormat(_missingInputError, input.Name, instruction.Instruction, input.Reference);
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

		public void ReadOutputs(InstructionCaller instruction, IList<InstructionOutput> outputs, IVariableStore caller)
		{
			foreach (var output in outputs)
			{
				if (output.Type == InstructionOutputType.Reference)
				{
					var value = Output.GetVariable(output.Name);

					if (value.Type != VariableType.Empty)
					{
						var result = output.Reference.SetValue(caller, value);

						switch (result)
						{
							case SetVariableResult.Success: break;
							case SetVariableResult.NotFound: Debug.LogWarningFormat(_missingOutputError, output.Name, instruction.Instruction, output.Reference); break;
							case SetVariableResult.ReadOnly: Debug.LogWarningFormat(_readOnlyOutputError, output.Name, instruction.Instruction, output.Reference); break;
							case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(_invalidOutputError, output.Name, instruction.Instruction, output.Reference); break;
						}
					}
				}
			}
		}

		private static VariableValue ResolveValue(ValueDefinition definition, VariableValue value, Object errorContext, string invalidError, string variableName)
		{
			if (definition.Type == VariableType.Object && definition.Constraint is ObjectVariableConstraint constraint && value.TryGetObject(out var obj))
			{
				var resolved = ComponentHelper.GetAsObject(constraint.Type, obj);
				value = VariableValue.Create(resolved);
			}

			if (definition.Type != VariableType.Empty && !definition.IsValid(value))
				Debug.LogWarningFormat(invalidError, variableName, errorContext, value);

			return value;
		}

		#endregion

		#region IVariableStore Implementation

		public IList<string> GetVariableNames()
		{
			return _variableNames;
		}

		public VariableValue GetVariable(string name)
		{
			if (ContextName == name)
				return Context;

			switch (name)
			{
				case InputStoreName: return VariableValue.Create(Input);
				case OutputStoreName: return VariableValue.Create(Output);
				case LocalStoreName: return VariableValue.Create(Local);
				case CompositionManager.GlobalStoreName: return VariableValue.Create(CompositionManager.Instance.GlobalStore);
				case CompositionManager.SceneStoreName: return VariableValue.Create(CompositionManager.Instance.SceneStore);
				default: return Local.GetVariable(name);
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (ContextName == name)
				return SetVariableResult.ReadOnly;

			switch (name)
			{
				case InputStoreName: return SetVariableResult.ReadOnly;
				case OutputStoreName: return SetVariableResult.ReadOnly;
				case LocalStoreName: return SetVariableResult.ReadOnly;
				case CompositionManager.GlobalStoreName: return SetVariableResult.ReadOnly;
				case CompositionManager.SceneStoreName: return SetVariableResult.ReadOnly;
				default: return Local.SetVariable(name, value);
			}
		}

		#endregion

		#region IPoolable Implementation

		public void Reset()
		{
			Input.Clear();
			Output.Clear();
			Local.Clear();
		}

		#endregion
	}
}
