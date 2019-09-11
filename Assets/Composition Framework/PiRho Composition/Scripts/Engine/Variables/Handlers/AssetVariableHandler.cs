using PiRhoSoft.Utilities;
using System.IO;
using UnityEngine.AddressableAssets;

namespace PiRhoSoft.Composition
{
	internal class AssetVariableHandler : VariableHandler
	{
		protected internal override string ToString_(Variable value)
		{
			return value.AsAsset.ToString();
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			var asset = value.AsAsset;
			var set = asset.RuntimeKeyIsValid();

			writer.Write(set);

			if (set)
				writer.Write(asset.RuntimeKey.ToString());
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var set = reader.ReadBoolean();

			if (set)
			{
				var guid = reader.ReadString();
				return Variable.Asset(new AssetReference(guid));
			}

			return Variable.Asset(null);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.IsEmpty || right.IsNullObject)
				return !left.AsAsset.RuntimeKeyIsValid();
			else if (right.TryGetAsset(out var asset))
				return left.AsAsset.RuntimeKey.Equals(asset.RuntimeKey);
			else
				return null;
		}
	}
}
