using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TransformMap : ClassMap<Transform>
	{
		private static List<string> _names = new List<string>
		{
			nameof(Transform.eulerAngles),
			nameof(Transform.forward),
			nameof(Transform.localEulerAngles),
			nameof(Transform.localPosition),
			nameof(Transform.localRotation),
			nameof(Transform.localScale),
			nameof(Transform.lossyScale),
			nameof(Transform.parent),
			nameof(Transform.position),
			nameof(Transform.right),
			nameof(Transform.root),
			nameof(Transform.rotation),
			nameof(Transform.up)
		};

		public override IList<string> GetVariableNames()
		{
			return _names;
		}

		public override Variable GetVariable(Transform obj, string name)
		{
			switch (name)
			{
				case nameof(Transform.eulerAngles): return Variable.Vector3(obj.eulerAngles);
				case nameof(Transform.forward): return Variable.Vector3(obj.forward);
				case nameof(Transform.localEulerAngles): return Variable.Vector3(obj.localEulerAngles);
				case nameof(Transform.localPosition): return Variable.Vector3(obj.localPosition);
				case nameof(Transform.localRotation): return Variable.Quaternion(obj.localRotation);
				case nameof(Transform.localScale): return Variable.Vector3(obj.localScale);
				case nameof(Transform.lossyScale): return Variable.Vector3(obj.lossyScale);
				case nameof(Transform.parent): return Variable.Object(obj.parent);
				case nameof(Transform.position): return Variable.Vector3(obj.position);
				case nameof(Transform.right): return Variable.Vector3(obj.right);
				case nameof(Transform.root): return Variable.Object(obj.root);
				case nameof(Transform.rotation): return Variable.Quaternion(obj.rotation);
				case nameof(Transform.up): return Variable.Vector3(obj.up);
			}

			return Variable.Empty;
		}

		public override SetVariableResult SetVariable(Transform obj, string name, Variable value)
		{
			switch (name)
			{
				case nameof(Transform.eulerAngles): if (value.TryGetVector3(out var eulerAngles)) { obj.eulerAngles = eulerAngles; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.forward): if (value.TryGetVector3(out var forward)) { obj.forward = forward; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.localEulerAngles): if (value.TryGetVector3(out var localEulerAngles)) { obj.localEulerAngles = localEulerAngles; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.localPosition): if (value.TryGetVector3(out var localPosition)) { obj.localPosition = localPosition; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.localRotation): if (value.TryGetQuaternion(out var localRotation)) { obj.localRotation = localRotation; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.localScale): if (value.TryGetVector3(out var localScale)) { obj.localScale = localScale; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.lossyScale): return SetVariableResult.ReadOnly;
				case nameof(Transform.parent): if (value.TryGetObject<Transform>(out var parent)) { obj.parent = parent; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.position): if (value.TryGetVector3(out var position)) { obj.position = position; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.right): if (value.TryGetVector3(out var right)) { obj.right = right; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.root): return SetVariableResult.ReadOnly;
				case nameof(Transform.rotation): if (value.TryGetQuaternion(out var rotation)) { obj.rotation = rotation; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Transform.up): if (value.TryGetVector3(out var up)) { obj.up = up; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
			}

			return SetVariableResult.NotFound;
		}
	}
}
