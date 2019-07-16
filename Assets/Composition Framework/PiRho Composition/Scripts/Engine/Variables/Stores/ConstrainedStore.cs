namespace PiRhoSoft.Composition
{
	public class ConstrainedStore : WritableStore, ISchemaOwner
	{
		public VariableSchema Schema { get; private set; }

		public ConstrainedStore(VariableSchema schema)
		{
			Schema = schema;
			SetupSchema();
		}

		public void SetupSchema()
		{
			for (var i = 0; i < Schema.Count; i++)
			{
				var definition = Schema[i];
				var value = definition.Definition.Generate(null);

				AddVariable(definition.Name, value);
			}
		}
	}
}
