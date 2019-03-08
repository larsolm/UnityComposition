﻿using PiRhoSoft.UtilityEngine;
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
				yield return Move(component.transform);
			else if (variables.This is GameObject gameObject)
				yield return Move(gameObject.transform);
			else
				Debug.LogWarningFormat(this, _invalidObjectWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator Move(Transform transform)
		{
			var body2d = transform.GetComponent<Rigidbody2D>();
			var body3d = transform.GetComponent<Rigidbody>();

			var targetPosition = UseRelativePosition ? transform.position + TargetPosition : TargetPosition;
			var targetRotation = UseRelativeRotation ? transform.rotation * TargetRotation : TargetRotation;
			var targetScale = TargetScale;

			if (UseRelativeScale)
				targetScale.Scale(transform.localScale);

			if (AnimationMethod == AnimationType.None)
			{
				// Don't use body's because we want to directly warp
				DoTransform(transform, targetPosition, targetRotation, targetScale);
			}
			else
			{
				var moveSpeed = 0.0f;
				var rotationSpeed = 0.0f;
				var scaleSpeed = 0.0f;

				if (AnimationMethod == AnimationType.Duration)
				{
					var step = Duration > 0.0f ? Time.deltaTime / Duration : 0.0f;
					var moveDistance = (targetPosition - transform.position).magnitude;
					var rotationDistance = Quaternion.Angle(targetRotation, transform.rotation);
					var scaleDifference = (targetScale - transform.localScale).magnitude;

					moveSpeed = moveDistance > 0.0f ? moveDistance * step : 0.0f;
					rotationSpeed = rotationDistance > 0.0f ? rotationDistance * step : 0.0f;
					scaleSpeed = scaleDifference > 0.0f ? scaleDifference * step : 0.0f;
				}
				else if (AnimationMethod == AnimationType.Speed)
				{
					moveSpeed = MoveSpeed * Time.deltaTime;
					rotationSpeed = RotationSpeed * Time.deltaTime;
					scaleSpeed = ScaleSpeed * Time.deltaTime;
				}

				if (WaitForCompletion)
					yield return DoMove(transform, body2d, body3d, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed);
				else
					InstructionManager.Instance.StartCoroutine(DoMove(transform, body2d, body3d, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed));
			}
		}

		private IEnumerator DoMove(Transform transform, Rigidbody2D body2d, Rigidbody body3d, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, float moveSpeed, float rotationSpeed, float scaleSpeed)
		{
			while (transform.position != targetPosition || transform.rotation != targetRotation || transform.localScale != targetScale)
			{
				var position = moveSpeed > 0.0f ? Vector3.MoveTowards(transform.position, targetPosition, moveSpeed) : targetPosition;
				var rotation = rotationSpeed > 0.0f ? Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed) : targetRotation;
				var scale = scaleSpeed > 0.0f ? Vector3.MoveTowards(transform.localScale, targetScale, scaleSpeed) : targetScale;

				if (body2d)
					DoRigidBody2D(body2d, position, rotation, scale);
				else if (body3d)
					DoRigidBody3D(body3d, position, rotation, scale);
				else
					DoTransform(transform, position, rotation, scale);

				yield return null;
			}
		}

		private void DoRigidBody2D(Rigidbody2D body, Vector2 position, Quaternion rotation, Vector3 scale)
		{
			body.MovePosition(position);
			body.MoveRotation(rotation.eulerAngles.z);
			body.transform.localScale = scale;
		}

		private void DoRigidBody3D(Rigidbody body, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			body.MovePosition(position);
			body.MoveRotation(rotation);
			body.transform.localScale = scale;
		}

		private void DoTransform(Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			transform.position = position;
			transform.rotation = rotation;
			transform.localScale = scale;
		}
	}
}
