using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Transform Object", 200)]
	[HelpURL(Composition.DocumentationUrl + "transform-node")]
	public class TransformNode : InstructionGraphNode
	{
		public enum AnimationType
		{
			None,
			Speed,
			Duration
		}
		private const string _invalidObjectWarning = "(CSTNIO) Unable to transform object for {0}: the given variables must be a GameObject or a MonoBehaviour";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target position to move to - offset from the original if UseRelativePosition is set")]
		public Vector3 TargetPosition = Vector3.zero;

		[Tooltip("The target rotation to change to - offset from the original if UseRelativeRotation is set")]
		[EulerAngles]
		public Quaternion TargetRotation = Quaternion.identity;

		[Tooltip("The target scale to size to - multiplicative from the original if UseRelativeScale is set")]
		public Vector3 TargetScale = Vector3.one;

		[Tooltip("Whether to use a relative position from the original or an absolute position")]
		public bool UseRelativePosition = true;

		[Tooltip("Whether to use a relative rotation from the original or an absolute rotation")]
		public bool UseRelativeRotation = true;

		[Tooltip("Whether to use a relative scale from the original or an absolute scale")]
		public bool UseRelativeScale = true;

		[Tooltip("The method in which to animate toward the target transform")]
		public AnimationType AnimationMethod = AnimationType.None;

		[Tooltip("Whether to wait for the effect to finish before moving to Next")]
		[ConditionalDisplaySelf(nameof(AnimationMethod), EnumValue = (int)AnimationType.None, Invert = true)]
		public bool WaitForCompletion = true;

		[Tooltip("The amount of time it takes to move to the target transform")]
		[ConditionalDisplaySelf(nameof(AnimationMethod), EnumValue = (int)AnimationType.Duration)]
		[Minimum(0.0f)]
		public float Duration = 1.0f;

		[Tooltip("The speed at which to move toward the target position (units per second)")]
		[ConditionalDisplaySelf(nameof(AnimationMethod), EnumValue = (int)AnimationType.Speed)]
		[Minimum(0.0f)]
		public float MoveSpeed = 1.0f;

		[Tooltip("The speed at which to move toward the target rotation (degrees per second)")]
		[ConditionalDisplaySelf(nameof(AnimationMethod), EnumValue = (int)AnimationType.Speed)]
		[Minimum(0.0f)]
		public float RotationSpeed = 1.0f;

		[Tooltip("The speed at which to scale toward the target scale (units per second)")]
		[ConditionalDisplaySelf(nameof(AnimationMethod), EnumValue = (int)AnimationType.Speed)]
		[Minimum(0.0f)]
		public float ScaleSpeed = 1.0f;

		public override Color NodeColor => Colors.Sequencing;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Component component)
				yield return Transform(component.transform);
			else if (variables.This is GameObject gameObject)
				yield return Transform(gameObject.transform);
			else
				Debug.LogWarningFormat(this, _invalidObjectWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator Transform(Transform transform)
		{
			var targetPosition = UseRelativePosition ? transform.position + TargetPosition : TargetPosition;
			var targetRotation = UseRelativeRotation ? transform.rotation * TargetRotation : TargetRotation;
			var targetScale = TargetScale;

			if (UseRelativeScale)
				targetScale.Scale(transform.localScale);

			if (AnimationMethod == AnimationType.None)
			{
				transform.position = targetPosition;
				transform.rotation = targetRotation;
				transform.localScale = targetScale;
			}
			else
			{
				var moveSpeed = 0.0f;
				var rotationSpeed = 0.0f;
				var scaleSpeed = 0.0f;

				if (AnimationMethod == AnimationType.Duration)
				{
					var step = Duration * Time.deltaTime;
					var moveDistance = (targetPosition - transform.position).magnitude;
					var rotationDistance = Quaternion.Angle(targetRotation, transform.rotation);
					var scaleDifference = (targetScale - transform.localScale).magnitude;

					moveSpeed = moveDistance > 0.0f ? step / moveDistance : 0.0f;
					rotationSpeed = rotationDistance > 0.0f ? step / rotationDistance : 0.0f;
					scaleSpeed = scaleDifference > 0.0f ? step / scaleDifference : 0.0f;
				}
				else if (AnimationMethod == AnimationType.Speed)
				{
					moveSpeed = MoveSpeed * Time.deltaTime;
					rotationSpeed = RotationSpeed * Time.deltaTime;
					scaleSpeed = ScaleSpeed * Time.deltaTime;
				}

				if (WaitForCompletion)
					yield return DoTransform(transform, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed);
				else
					InstructionManager.Instance.StartCoroutine(DoTransform(transform, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed));
			}
		}

		private IEnumerator DoTransform(Transform transform, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, float moveSpeed, float rotationSpeed, float scaleSpeed)
		{
			while (transform.position != targetPosition || transform.rotation != targetRotation || transform.localScale != targetScale)
			{
				transform.position = moveSpeed > 0.0f ? Vector3.MoveTowards(transform.position, targetPosition, moveSpeed) : targetPosition;
				transform.rotation = rotationSpeed > 0.0f ? Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed) : targetRotation;
				transform.localScale = scaleSpeed > 0.0f ? Vector3.MoveTowards(transform.localScale, targetScale, scaleSpeed) : targetScale;

				yield return null;
			}
		}
	}
}
