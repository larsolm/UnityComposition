using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class TransformMap : VariableMap<Transform>
	{
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
#else
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
		static void Register() => Add(new TransformMap());

		public TransformMap()
		{
			AddValue(nameof(Transform.eulerAngles), transform => transform.eulerAngles, (transform, value) => transform.eulerAngles = value);
			AddValue(nameof(Transform.forward), transform => transform.forward, (transform, value) => transform.forward = value);
			AddValue(nameof(Transform.localEulerAngles), transform => transform.localEulerAngles, (transform, value) => transform.localEulerAngles = value);
			AddValue(nameof(Transform.localPosition), transform => transform.localPosition, (transform, value) => transform.localPosition = value);
			AddValue(nameof(Transform.localRotation), transform => transform.localRotation, (transform, value) => transform.localRotation = value);
			AddValue(nameof(Transform.localScale), transform => transform.localScale, (transform, value) => transform.localScale = value);
			AddValue(nameof(Transform.lossyScale), transform => transform.lossyScale, null);
			AddValue(nameof(Transform.parent), transform => transform.parent, (transform, value) => transform.parent = value);
			AddValue(nameof(Transform.position), transform => transform.position, (transform, value) => transform.position = value);
			AddValue(nameof(Transform.right), transform => transform.right, (transform, value) => transform.right = value);
			AddValue(nameof(Transform.root), transform => transform.root, null);
			AddValue(nameof(Transform.rotation), transform => transform.rotation, (transform, value) => transform.rotation = value);
			AddValue(nameof(Transform.up), transform => transform.up, (transform, value) => transform.up = value);
		}
	}
}