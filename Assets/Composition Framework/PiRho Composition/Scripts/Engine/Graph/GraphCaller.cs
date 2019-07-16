using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class GraphCaller
	{
		[Tooltip("The Graph to execute when this is called")] [SerializeField] private Graph _graph;
		[Tooltip("The input values to set for the Graph")] [SerializeField] private List<GraphInput> _inputs = new List<GraphInput>();
		[Tooltip("The output values from the Graph to store")] [SerializeField] private List<GraphOutput> _outputs = new List<GraphOutput>();

		public Graph Graph
		{
			get { return _graph; }
			set { _graph = value; UpdateVariables(); }
		}

		public IList<GraphInput> Inputs => _inputs;
		public IList<GraphOutput> Outputs => _outputs;
		public bool IsRunning => Graph != null && Graph.IsRunning;

		public IEnumerator Execute(IVariableStore store, VariableValue context)
		{
			if (Graph)
			{
				var localStore = GraphStore.Reserve(Graph, context);

				localStore.WriteInputs(this, _inputs, store);
				localStore.WriteOutputs(_outputs);
				yield return Graph.Execute(localStore);
				localStore.ReadOutputs(this, _outputs, store);

				GraphStore.Release(localStore);
			}
		}

		#region Inputs and Outputs

		public void UpdateVariables()
		{
			UpdateInputs();
			UpdateOutputs();
		}

		public VariableDefinition GetInputDefinition(GraphInput input)
		{
			if (Graph)
			{
				foreach (var definition in Graph.Inputs)
				{
					if (input.Name == definition.Name)
						return definition;
				}
			}

			return new VariableDefinition { Name = input.Name, Definition = ValueDefinition.Create(VariableType.Empty) };
		}

		public VariableDefinition GetOutputDefinition(GraphOutput output)
		{
			if (Graph)
			{
				foreach (var definition in Graph.Outputs)
				{
					if (output.Name == definition.Name)
						return definition;
				}
			}

			return new VariableDefinition { Name = output.Name, Definition = ValueDefinition.Create(VariableType.Empty) };
		}

		private void UpdateInputs()
		{
			var inputs = new List<GraphInput>();

			if (_graph != null)
			{
				_graph.RefreshInputs();

				foreach (var definition in _graph.Inputs)
				{
					var existing = _inputs.Where(input => input.Name == definition.Name).FirstOrDefault();

					if (existing != null && (existing.Type == GraphInputType.Reference || definition.Definition.IsValid(existing.Value)))
					{
						if (!inputs.Any(input => input.Name == definition.Name))
							inputs.Add(existing);
					}
					else
					{
						inputs.Add(new GraphInput
						{
							Name = definition.Name,
							Type = GraphInputType.Value,
							Value = definition.Definition.Generate(null)
						});
					}
				}
			}

			_inputs = inputs;
		}

		private void UpdateOutputs()
		{
			var outputs = new List<GraphOutput>();

			if (_graph != null)
			{
				_graph.RefreshOutputs();

				foreach (var definition in _graph.Outputs)
				{
					var existing = _outputs.Where(output => output.Name == definition.Name).FirstOrDefault();

					if (existing != null)
					{
						if (!outputs.Any(output => output.Name == definition.Name))
							outputs.Add(existing);
					}
					else
					{
						outputs.Add(new GraphOutput
						{
							Name = definition.Name,
							Type = GraphOutputType.Ignore
						});
					}
				}
			}

			_outputs = outputs;
		}

		#endregion
	}
}
