using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TransformMap : VariableMap<Transform>
	{
		public TransformMap()
		{
			AddProperty(nameof(Transform.eulerAngles), (transform) => transform.eulerAngles, (transform, value) => transform.eulerAngles = value);
			AddProperty(nameof(Transform.forward), (transform) => transform.forward, (transform, value) => transform.forward = value);
			AddProperty(nameof(Transform.localEulerAngles), (transform) => transform.localEulerAngles, (transform, value) => transform.localEulerAngles = value);
			AddProperty(nameof(Transform.localPosition), (transform) => transform.localPosition, (transform, value) => transform.localPosition = value);
			AddProperty(nameof(Transform.localRotation), (transform) => transform.localRotation, (transform, value) => transform.localRotation = value);
			AddProperty(nameof(Transform.localScale), (transform) => transform.localScale, (transform, value) => transform.localScale = value);
			AddProperty(nameof(Transform.lossyScale), (transform) => transform.lossyScale, null);
			AddProperty(nameof(Transform.parent), (transform) => transform.parent, (transform, value) => transform.parent = value);
			AddProperty(nameof(Transform.position), (transform) => transform.position, (transform, value) => transform.position = value);
			AddProperty(nameof(Transform.right), (transform) => transform.right, (transform, value) => transform.right = value);
			AddProperty(nameof(Transform.root), (transform) => transform.root, null);
			AddProperty(nameof(Transform.rotation), (transform) => transform.rotation, (transform, value) => transform.rotation = value);
			AddProperty(nameof(Transform.up), (transform) => transform.up, (transform, value) => transform.up = value);
		}
	}
}