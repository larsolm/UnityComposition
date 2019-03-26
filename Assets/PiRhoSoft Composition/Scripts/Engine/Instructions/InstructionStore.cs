using System;
using System.Collections.Generic;
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
				VariableValue.Save(Value, ref _valueData, ref _valueObjects);
		}

		public void OnAfterDeserialize()
		{
			if (Type == InstructionInputType.Value)
				VariableValue.Load(ref Value, ref _valueData, ref _valueObjects);
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
		private const string _missingInputError = "(CISMI) failed to read input {0}: the variable '{1}' could not be found";
		private const string _missingOutputError = "(CISMO) failed to store output {0}: the variable '{1}' could not be found";
		private const string _readOnlyOutputError = "(CISROO) failed to store output {0}: the variable '{1}' is read only";
		private const string _invalidOutputError = "(CISIOT) failed to store output {0}: the variable '{1}' has an incompatible type";

		public const string RootStoreName = "root";
		public const string SceneStoreName = "scene";
		public const string InputStoreName = "input";
		public const string OutputStoreName = "output";
		public const string LocalStoreName = "local";
		public const string GlobalStoreName = "global";

		private static string[] _variableNames = new string[] { RootStoreName, SceneStoreName, InputStoreName, OutputStoreName, LocalStoreName, GlobalStoreName };
		private static SceneVariableStore _sceneStore = new SceneVariableStore();

		public object Root { get; private set; }

		public VariableStore Input { get; } = new WritableStore();
		public VariableStore Output { get; } = new WritableStore();
		public VariableStore Local { get; } = new VariableStore();
		public VariableStore Global { get; } = CompositionManager.Instance.GlobalStore;

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName == OutputStoreName;
		public static bool IsInput(InstructionInput input) => input.Type == InstructionInputType.Reference && input.Reference.IsAssigned && input.Reference.StoreName == InputStoreName;
		public static bool IsOutput(InstructionOutput output) => output.Type == InstructionOutputType.Reference && output.Reference.IsAssigned && output.Reference.StoreName == OutputStoreName;

		public InstructionStore(object root)
		{
			ChangeRoot(root);
		}

		public void ChangeRoot(object root)
		{
			Root = root;
		}

		public void WriteInputs(IList<InstructionInput> inputs)
		{
			foreach (var input in inputs)
			{
				if (input.Type == InstructionInputType.Reference)
				{
					var value = input.Reference.GetValue(this);

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
			switch (name)
			{
				case RootStoreName: return VariableValue.CreateReference(Root);
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
				case RootStoreName: return SetVariableResult.ReadOnly;
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
			return _variableNames;
		}
	}
}
