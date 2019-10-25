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
		public const string GraphField = nameof(_graph);
		public const string InputsField = nameof(_inputs);
		public const string OutputsField = nameof(_outputs);

		[Tooltip("The Graph to run")]
		[SerializeField] private Graph _graph;
		[SerializeField] private List<GraphInput> _inputs = new List<GraphInput>();
		[SerializeField] private List<GraphOutput> _outputs = new List<GraphOutput>();

		public Graph Graph
		{
			get { return _graph; }
			set { _graph = value; UpdateVariables(); }
		}

		public IList<GraphInput> Inputs => _inputs;
		public IList<GraphOutput> Outputs => _outputs;
		public bool IsRunning => Graph && Graph.IsRunning;
		public override string ToString() => Graph ? Graph.name : base.ToString();

		public IEnumerator Execute(IVariableMap store, Variable context)
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

			return new VariableDefinition(input.Name);
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

			return new VariableDefinition(output.Name);
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

					if (existing != null && (existing.Type == GraphInputType.Reference || definition.IsValid(existing.Value.Variable)))
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
							Value = new SerializedVariable { Variable = definition.Generate() }
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
