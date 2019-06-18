using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class TypePicker : BasePicker<Type>
	{
		private const string _invalidTypeWarning = "(PITPIT) Invalid type for TypePicker: the type '{0}' could not be found";

		private class Factory : UxmlFactory<TypePicker, Traits> { }

		public class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };
			private UxmlBoolAttributeDescription _showAbstract = new UxmlBoolAttributeDescription { name = "show-abstract", defaultValue = true };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var typePicker = ve as TypePicker;
				var typeName = _type.GetValueFromBag(bag, cc);
				var valueName = _value.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName);
				var showAbstract = _showAbstract.GetValueFromBag(bag, cc);
				var value = Type.GetType(valueName);

				if (type != null)
					typePicker.Setup(type, showAbstract, value);
				else
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
			}
		}

		public Type Type { get; private set; }
		public bool ShowAbstract { get; private set; }

		public void Setup(Type type, bool showAbstract, Type initialType)
		{
			Type = type;
			ShowAbstract = showAbstract;

			if (Type != null)
			{
				var types = TypeHelper.GetTypeList(Type, ShowAbstract);
				CreateTree(types.BaseType.Name, types.Paths, types.Types, initialType, iconType => AssetPreview.GetMiniTypeThumbnail(iconType));
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, Type);
			}
		}
	}
}
