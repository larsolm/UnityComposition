﻿using System.Collections.Generic;

namespace PiRhoSoft.Composition.Editor
{
	[Autocomplete(typeof(Graph))]
	public class GraphAutocompleteItem : AutocompleteItem<Graph>
	{
		private Graph _graph;

		private InputAutocompleteItem _input = new InputAutocompleteItem { Name = GraphStore.InputStoreName };
		private OutputAutocompleteItem _output = new OutputAutocompleteItem { Name = GraphStore.OutputStoreName };
		private LocalAutocompleteItem _local = new LocalAutocompleteItem { Name = GraphStore.LocalStoreName };
		private GlobalAutocompleteItem _global = new GlobalAutocompleteItem { Name = CompositionManager.GlobalStoreName };
		private SceneAutocompleteItem _scene = new SceneAutocompleteItem { Name = CompositionManager.SceneStoreName };
		private IAutocompleteItem _context = null;

		protected override void Setup(Graph graph)
		{
			_graph = graph;

			AllowsCustomFields = false;
			IsCastable = false;
			IsIndexable = false;
			Fields = new List<IAutocompleteItem> { _context, _input, _output, _local, _global, _scene };
			Types = null;

			Reset();
			GraphEditor.AutocompleteChanged.Subscribe(this, item => item.Reset());
		}

		private void Reset()
		{
			if (GraphEditor.AutocompleteContext != null)
				_context = new ObjectAutocompleteItem(GraphEditor.AutocompleteContext) { Name = _graph.Context.Name };
			else
				_context = new DefinitionAutocompleteItem(_graph.Context) { Name = _graph.Context.Name };

			Fields[0] = _context;

			_input.Setup(_graph);
			_output.Setup(_graph);
			_local.Setup(_graph);
			//_global.Setup(null);
			//_scene.Setup(null);
		}

		public class InputAutocompleteItem : AutocompleteItem
		{
			public InputAutocompleteItem()
			{
				AllowsCustomFields = true;
				IsCastable = false;
				IsIndexable = false;
				Fields = new List<IAutocompleteItem>();
				Types = null;
			}

			public override void Setup(object obj)
			{
				Fields.Clear();

				var graph = obj as Graph;
				var inputs = new VariableDefinitionList();

				graph.GetInputs(inputs, GraphStore.InputStoreName);

				foreach (var input in inputs)
					Fields.Add(new DefinitionAutocompleteItem(input)); // if GraphEditor.AutocompleteCaller is assigned, see if there is a value assigned and use that instead
			}
		}

		public class OutputAutocompleteItem : AutocompleteItem
		{
			public OutputAutocompleteItem()
			{
				AllowsCustomFields = true;
				IsCastable = false;
				IsIndexable = false;
				Fields = new List<IAutocompleteItem>();
				Types = null;
			}

			public override void Setup(object obj)
			{
				Fields.Clear();

				var graph = obj as Graph;
				var outputs = new VariableDefinitionList();

				graph.GetOutputs(outputs, GraphStore.OutputStoreName);

				foreach (var output in outputs)
					Fields.Add(new DefinitionAutocompleteItem(output));
			}
		}

		public class LocalAutocompleteItem : AutocompleteItem
		{
			public LocalAutocompleteItem()
			{
				AllowsCustomFields = true;
				IsCastable = false;
				IsIndexable = false;
				Fields = new List<IAutocompleteItem>();
				Types = null;
			}

			public override void Setup(object obj)
			{
				Fields.Clear();

				var graph = obj as Graph;
				var locals = new VariableDefinitionList();

				graph.GetInputs(locals, GraphStore.LocalStoreName);
				graph.GetOutputs(locals, GraphStore.LocalStoreName);

				foreach (var local in locals)
					Fields.Add(new DefinitionAutocompleteItem(local));
			}
		}
	}

	[Autocomplete(typeof(GraphNode))]
	public class GraphNodeAutocompleteItem : GraphAutocompleteItem
	{
		public override void Setup(object obj)
		{
			base.Setup((obj as GraphNode).Graph);
		}
	}
}