using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ObjectPicker : BasePicker<Object>
	{
		private const string _invalidTypeWarning = "(PITPIT) Invalid type for TypePicker: the type '{0}' could not be found";

		private class Factory : UxmlFactory<TypePicker, Traits> { }

		public class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
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
				var type = Type.GetType(typeName);
				var showAbstract = _showAbstract.GetValueFromBag(bag, cc);

				if (type != null)
					typePicker.Setup(type, showAbstract, null);
				else
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
			}
		}

		public Type Type { get; private set; }

		public void Setup(Type type, Object initialValue)
		{
			Type = type;

			if (Type != null)
			{
				var assets = AssetHelper.GetAssetList(Type);
				CreateTree(assets.Type.Name, assets.Paths, assets.Assets, initialValue, asset =>
				{
					var icon = AssetPreview.GetMiniThumbnail(asset);
					return icon == null && asset ? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : icon;
				});
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, Type);
			}
		}
	}
}
