using System;
using System.Collections.Generic;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class PropertyMap
	{
		public abstract int PropertyCount { get; }
		public abstract string GetPropertyName(int index);
		public abstract IVariableList CreateList(object owner);
	}

	public class PropertyMap<OwnerType> : PropertyMap where OwnerType : class
	{
		public List<Property<OwnerType>> Properties = new List<Property<OwnerType>>();

		public override int PropertyCount => Properties.Count;
		public override string GetPropertyName(int index) => index >= 0 && index < Properties.Count ? Properties[index].Name : "";
		public override IVariableList CreateList(object owner) => new PropertyList<OwnerType>(owner as OwnerType, this);

		public PropertyMap<OwnerType> Add(string name, Func<OwnerType, VariableValue> getter, Func<OwnerType, VariableValue, SetVariableResult> setter)
		{
			Properties.Add(new Property<OwnerType>
			{
				Name = name,
				Getter = getter,
				Setter = setter
			});

			return this;
		}
	}
}
