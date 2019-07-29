using PiRhoSoft.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Sequencing/Transform Object", 200)]
	[HelpURL(Configuration.DocumentationUrl + "transform-node")]
	public class TransformNode : GraphNode
	{
		public enum AnimationType
		{
			None,
			Speed,
			Duration
		}

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Tranform to modify")]
		public VariableReference Transform = new VariableReference();

		[Tooltip("Whether to use a relative position from the original or an absolute position")]
		public bool UseRelativePosition = true;

		[Tooltip("Whether to use a relative rotation from the original or an absolute rotation")]
		public bool UseRelativeRotation = true;

		[Tooltip("Whether to use a relative scale from the original or an absolute scale")]
		public bool UseRelativeScale = true;

		[Tooltip("The target position to move to - offset from the original if UseRelativePosition is set")]
		[Inline]
		public Vector3VariableSource TargetPosition = new Vector3VariableSource();

		[Tooltip("The target rotation to change to - offset from the original if UseRelativeRotation is set")]
		[Inline]
		public Vector3VariableSource TargetRotation = new Vector3VariableSource();

		[Tooltip("The target scale to size to - multiplicative from the original if UseRelativeScale is set")]
		[Inline]
		public Vector3VariableSource TargetScale = new Vector3VariableSource(Vector3.one);

		[Tooltip("The method in which to animate toward the target transform")]
		public AnimationType AnimationMethod = AnimationType.None;

		[Tooltip("Whether to wait for the effect to finish before moving to Next")]
		[Conditional(nameof(AnimationMethod), (int)AnimationType.None, Test = ConditionalTest.Inequal)]
		public bool WaitForCompletion = true;

		[Tooltip("The amount of time it takes to move to the target transform")]
		[Conditional(nameof(AnimationMethod), (int)AnimationType.Duration)]
		[Inline]
		public FloatVariableSource Duration = new FloatVariableSource(1.0f);

		[Tooltip("The speed at which to move toward the target position (units per second)")]
		[Conditional(nameof(AnimationMethod), (int)AnimationType.Speed)]
		[Inline]
		public FloatVariableSource MoveSpeed = new FloatVariableSource(1.0f);

		[Tooltip("The speed at which to move toward the target rotation (degrees per second)")]
		[Conditional(nameof(AnimationMethod), (int)AnimationType.Speed)]
		[Inline]
		public FloatVariableSource RotationSpeed = new FloatVariableSource(1.0f);

		[Tooltip("The speed at which to scale toward the target scale (units per second)")]
		[Conditional(nameof(AnimationMethod), (int)AnimationType.Speed)]
		[Inline]
		public FloatVariableSource ScaleSpeed = new FloatVariableSource(1.0f);

		public override Color NodeColor => Colors.Sequencing;

		private static Dictionary<int, Coroutine> _currentlyRunning = new Dictionary<int, Coroutine>();

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (GraphStore.IsInput(Transform))
				inputs.Add(new VariableDefinition(Transform.RootName, new ObjectConstraint(typeof(Transform))));

			TargetPosition.GetInputs(inputs);
			TargetRotation.GetInputs(inputs);
			TargetScale.GetInputs(inputs);

			if (AnimationMethod == AnimationType.Duration)
			{
				Duration.GetInputs(inputs);
			}
			else if (AnimationMethod == AnimationType.Speed)
			{
				MoveSpeed.GetInputs(inputs);
				RotationSpeed.GetInputs(inputs);
				ScaleSpeed.GetInputs(inputs);
			}
		}

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (ResolveObject<Transform>(variables, Transform, out var transform))
				yield return Move(transform, variables);

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator Move(Transform transform, GraphStore variables)
		{
			var body2d = transform.GetComponent<Rigidbody2D>();
			var body3d = transform.GetComponent<Rigidbody>();

			Resolve(variables, TargetPosition, out var targetPosition);
			Resolve(variables, TargetRotation, out var targetAngles);
			Resolve(variables, TargetScale, out var targetScale);

			if (UseRelativePosition)
				targetPosition += transform.position;

			if (UseRelativeScale)
				targetScale.Scale(transform.localScale);

			var targetRotation = UseRelativeRotation ? transform.rotation * Quaternion.Euler(targetAngles) : Quaternion.Euler(targetAngles);

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
					Resolve(variables, Duration, out var duration);

					var step = duration > 0.0f ? Time.deltaTime / duration : 0.0f;
					var moveDistance = (targetPosition - transform.position).magnitude;
					var rotationDistance = Quaternion.Angle(targetRotation, transform.rotation);
					var scaleDifference = (targetScale - transform.localScale).magnitude;

					moveSpeed = moveDistance > 0.0f ? moveDistance * step : 0.0f;
					rotationSpeed = rotationDistance > 0.0f ? rotationDistance * step : 0.0f;
					scaleSpeed = scaleDifference > 0.0f ? scaleDifference * step : 0.0f;
				}
				else if (AnimationMethod == AnimationType.Speed)
				{
					Resolve(variables, MoveSpeed, out moveSpeed);
					Resolve(variables, RotationSpeed, out rotationSpeed);
					Resolve(variables, ScaleSpeed, out scaleSpeed);

					moveSpeed *= Time.deltaTime;
					rotationSpeed *= Time.deltaTime;
					scaleSpeed *= Time.deltaTime;
				}


				if (WaitForCompletion)
				{
					yield return DoMove(transform, body2d, body3d, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed);
				}
				else
				{
					if (_currentlyRunning.TryGetValue(transform.GetInstanceID(), out var old) && old != null)
						CompositionManager.Instance.StopCoroutine(old);

					var coroutine = CompositionManager.Instance.StartCoroutine(DoMove(transform, body2d, body3d, targetPosition, targetRotation, targetScale, moveSpeed, rotationSpeed, scaleSpeed));

					if (coroutine != null)
						_currentlyRunning[transform.GetInstanceID()] = coroutine;
				}
			}
		}

		private IEnumerator DoMove(Transform transform, Rigidbody2D body2d, Rigidbody body3d, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, float moveSpeed, float rotationSpeed, float scaleSpeed)
		{
			while (transform && (transform.position != targetPosition || transform.rotation != targetRotation || transform.localScale != targetScale))
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

			_currentlyRunning.Remove(transform.GetInstanceID());
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
