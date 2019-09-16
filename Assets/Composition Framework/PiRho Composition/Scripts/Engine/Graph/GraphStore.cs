using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public enum GraphInputType
	{
		Reference,
		Value
	}

	public enum GraphOutputType
	{
		Ignore,
		Reference
	}

	[Serializable]
	public class GraphInput
	{
		public string Name;
		public GraphInputType Type;
		public VariableReference Reference = new VariableReference();
		public VariableValue Value = new VariableValue();
	}

	[Serializable]
	public class GraphOutput
	{
		public string Name;
		public GraphOutputType Type;
		public VariableReference Reference = new VariableReference();
	}

	public class GraphStore : IVariableCollection, IPoolable
	{
		private const string _invalidContextError = "(CISIC) Failed to create context '{0}' for graph '{1}': the value '{2}' does not satisfy the constraint";
		private const string _invalidInputError = "(CISII) Failed to create input '{0}' for graph '{1}': the value '{2}' does not satisfy the constraint";
		private const string _missingInputError = "(CISMI) Failed to read input '{0}' for graph '{1}': the variable '{2}' could not be found";
		private const string _missingOutputError = "(CISMO) Failed to store output '{0}' for graph '{1}': the variable '{2}' could not be found";
		private const string _readOnlyOutputError = "(CISROO) Failed to store output '{0}' for graph '{1}': the variable '{2}' is read only";
		private const string _invalidOutputError = "(CISIOT) Failed to store output '{0}' for graph '{1}': the variable '{2}' has an incompatible type";

		public const string InputStoreName = "input";
		public const string OutputStoreName = "output";
		public const string LocalStoreName = "local";

		public string ContextName { get; private set; }
		public Variable Context { get; private set; }

		public VariableStore Input { get; } = new VariableStore();
		public VariableStore Output { get; } = new VariableStore();
		public VariableStore Local { get; } = new VariableStore();

		public static bool IsInput(VariableReference variable) => variable.IsAssigned && variable.StoreName == InputStoreName;
		public static bool IsOutput(VariableReference variable) => variable.IsAssigned && variable.StoreName == OutputStoreName;
		public static bool IsInput(GraphInput input) => input.Type == GraphInputType.Reference && input.Reference.IsAssigned && input.Reference.StoreName == InputStoreName;
		public static bool IsOutput(GraphOutput output) => output.Type == GraphOutputType.Reference && output.Reference.IsAssigned && output.Reference.StoreName == OutputStoreName;

		private readonly string[] _variableNames = new string[] { InputStoreName, OutputStoreName, LocalStoreName, CompositionManager.GlobalStoreName, CompositionManager.SceneStoreName, string.Empty };

		#region Pooling

		private class PoolInfo : IPoolInfo
		{
			public int Size => 10;
			public int Growth => 5;
		}

		private static ClassPool<GraphStore, PoolInfo> _pool = new ClassPool<GraphStore, PoolInfo>();

		internal static GraphStore Reserve(Graph graph, Variable context)
		{
			var store = _pool.Reserve();

			store.ContextName = graph.Context.Name;
			store.Context = ResolveValue(graph.Context, context, graph, _invalidContextError, store.ContextName);
			store._variableNames[store._variableNames.Length - 1] = store.ContextName;

			return store;
		}

		internal static void Release(GraphStore store)
		{
			_pool.Release(store);
		}

		#endregion

		#region Inputs and Outputs

		public void WriteInputs(GraphCaller graph, IList<GraphInput> inputs, IVariableCollection caller)
		{
			Input.Locked = false;

			foreach (var input in inputs)
			{
				if (input.Type == GraphInputType.Reference)
				{
					var value = input.Reference.GetValue(caller);
					var definition = graph.GetInputDefinition(input);

					value = ResolveValue(definition, value, graph.Graph, _invalidInputError, definition.Name);

					if (value.Type != VariableType.Empty)
						Input.SetVariable(input.Name, value);
					else
						Debug.LogWarningFormat(_missingInputError, input.Name, graph.Graph, input.Reference);
				}
				else if (input.Type == GraphInputType.Value)
				{
					Input.SetVariable(input.Name, input.Value.Variable);
				}
			}

			Input.Locked = true;
		}

		public void WriteOutputs(IList<GraphOutput> outputs)
		{
			Output.Locked = false;

			foreach (var output in outputs)
				Output.SetVariable(output.Name, Variable.Empty);

			Output.Locked = true;
		}

		public void ReadOutputs(GraphCaller graph, IList<GraphOutput> outputs, IVariableCollection caller)
		{
			foreach (var output in outputs)
			{
				if (output.Type == GraphOutputType.Reference)
				{
					var value = Output.GetVariable(output.Name);

					if (value.Type != VariableType.Empty)
					{
						var result = output.Reference.SetValue(caller, value);

						switch (result)
						{
							case SetVariableResult.Success: break;
							case SetVariableResult.NotFound: Debug.LogWarningFormat(_missingOutputError, output.Name, graph.Graph, output.Reference); break;
							case SetVariableResult.ReadOnly: Debug.LogWarningFormat(_readOnlyOutputError, output.Name, graph.Graph, output.Reference); break;
							case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(_invalidOutputError, output.Name, graph.Graph, output.Reference); break;
						}
					}
				}
			}
		}

		private static Variable ResolveValue(VariableDefinition definition, Variable value, Object errorContext, string invalidError, string variableName)
		{
			if (definition.Type == VariableType.Object && definition.Constraint is ObjectConstraint constraint && value.TryGetObject<Object>(out var obj))
			{
				var resolved = obj.GetAsObject(constraint.ObjectType);
				value = Variable.Object(resolved);
			}

			if (definition.Type != VariableType.Empty && !definition.IsValid(value))
				Debug.LogWarningFormat(invalidError, variableName, errorContext, value);

			return value;
		}

		#endregion

		#region IVariableStore Implementation

		public IReadOnlyList<string> VariableNames
		{
			get => _variableNames;
		}

		public Variable GetVariable(string name)
		{
			if (ContextName == name)
				return Context;

			switch (name)
			{
				case InputStoreName: return Variable.Object(Input);
				case OutputStoreName: return Variable.Object(Output);
				case LocalStoreName: return Variable.Object(Local);
				case CompositionManager.GlobalStoreName: return Variable.Object(CompositionManager.Instance.GlobalStore);
				case CompositionManager.SceneStoreName: return Variable.Object(CompositionManager.Instance.SceneStore);
				default: return Variable.Empty;
			}
		}

		public SetVariableResult SetVariable(string name, Variable value)
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
				default: return SetVariableResult.NotFound;
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
