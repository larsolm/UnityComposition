using System.Collections.Generic;

namespace PiRhoSoft.Composition.Editor
{
	public class DefinitionAutocompleteItem : AutocompleteItem<VariableDefinition>
	{
		public DefinitionAutocompleteItem(VariableDefinition definition) => Setup(definition);

		protected override void Setup(VariableDefinition definition)
		{
			switch (definition.Type)
			{
				case VariableType.Empty: SetupEmpty(); break;
				case VariableType.Bool: SetupValueType(); break;
				case VariableType.Int: SetupValueType(); break;
				case VariableType.Float: SetupValueType(); break;
				case VariableType.Vector2Int: SetupValueType(); break;
				case VariableType.Vector3Int: SetupValueType(); break;
				case VariableType.RectInt: SetupValueType(); break;
				case VariableType.BoundsInt: SetupValueType(); break;
				case VariableType.Vector2: SetupValueType(); break;
				case VariableType.Vector3: SetupValueType(); break;
				case VariableType.Vector4: SetupValueType(); break;
				case VariableType.Quaternion: SetupValueType(); break;
				case VariableType.Rect: SetupValueType(); break;
				case VariableType.Bounds: SetupValueType(); break;
				case VariableType.Color: SetupValueType(); break;
				case VariableType.Enum: SetupEnum(definition.Constraint as EnumConstraint); break;
				case VariableType.String: SetupValueType(); break;
				case VariableType.List: SetupList(definition.Constraint as ListConstraint); break;
				case VariableType.Dictionary: SetupDictionary(definition.Constraint as DictionaryConstraint); break;
				case VariableType.Asset: SetupAsset(definition.Constraint as AssetConstraint); break;
				case VariableType.Object: SetupObject(definition.Constraint as ObjectConstraint); break;
			}
		}

		private void SetupEmpty()
		{
			AllowsCustomFields = true;
			IsCastable = false;
			IsIndexable = false;
			Fields = new List<IAutocompleteItem>();
			Types = null;
		}

		private void SetupValueType()
		{
			AllowsCustomFields = false;
			IsCastable = false;
			IsIndexable = false;
			Fields = new List<IAutocompleteItem>();
			Types = null;
		}

		private void SetupEnum(EnumConstraint constraint)
		{
			AllowsCustomFields = false;
			IsCastable = false;
			IsIndexable = false;
			Fields = new List<IAutocompleteItem>();
			Types = null;

			foreach (var name in constraint.EnumType.GetEnumNames())
				Fields.Add(new LeafAutocompleteItem(name));
		}

		private void SetupList(ListConstraint constraint)
		{
			AllowsCustomFields = false;
			IsCastable = false;
			IsIndexable = true;
			Fields = null;
			Types = null;

			var definition = constraint.ItemConstraint != null
				? new VariableDefinition(string.Empty, constraint.ItemConstraint)
				: new VariableDefinition(string.Empty, constraint.ItemType);

			IndexField = new DefinitionAutocompleteItem(definition);
		}

		private void SetupDictionary(DictionaryConstraint constraint)
		{
			AllowsCustomFields = constraint.Schema == null;
			IsCastable = false;
			IsIndexable = true;
			Fields = null;
			Types = null;

			if (constraint.Schema != null)
			{
				Fields = new List<IAutocompleteItem>();

				for (var i = 0; i < constraint.Schema.EntryCount; i++)
				{
					var entry = constraint.Schema.GetEntry(i);
					Fields.Add(new DefinitionAutocompleteItem(entry.Definition));
				}
			}
		}

		private void SetupAsset(AssetConstraint constraint)
		{
		}

		private void SetupObject(ObjectConstraint constraint)
		{
		}
	}
}
