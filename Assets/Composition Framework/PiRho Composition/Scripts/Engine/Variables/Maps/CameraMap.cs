using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.Composition
{
	internal class CameraMap : ClassMap<Camera>
	{
		private static List<string> _names = new List<string>
		{
			nameof(Camera.activeTexture),
			nameof(Camera.actualRenderingPath),
			nameof(Camera.allowDynamicResolution),
			nameof(Camera.allowHDR),
			nameof(Camera.allowMSAA),
			nameof(Camera.areVRStereoViewMatricesWithinSingleCullTolerance),
			nameof(Camera.aspect),
			nameof(Camera.backgroundColor),
			nameof(Camera.cameraType),
			nameof(Camera.clearFlags),
			nameof(Camera.clearStencilAfterLightingPass),
			nameof(Camera.commandBufferCount),
			nameof(Camera.cullingMask),
			nameof(Camera.depth),
			nameof(Camera.depthTextureMode),
			nameof(Camera.eventMask),
			nameof(Camera.farClipPlane),
			nameof(Camera.fieldOfView),
			nameof(Camera.focalLength),
			nameof(Camera.forceIntoRenderTexture),
			nameof(Camera.gateFit),
			nameof(Camera.lensShift),
			nameof(Camera.nearClipPlane),
			nameof(Camera.opaqueSortMode),
			nameof(Camera.orthographic),
			nameof(Camera.orthographicSize),
			nameof(Camera.overrideSceneCullingMask),
			nameof(Camera.pixelHeight),
			nameof(Camera.pixelRect),
			nameof(Camera.pixelWidth),
			nameof(Camera.rect),
			nameof(Camera.renderingPath),
			nameof(Camera.scaledPixelHeight),
			nameof(Camera.scaledPixelWidth),
			nameof(Camera.sensorSize),
			nameof(Camera.stereoActiveEye),
			nameof(Camera.stereoConvergence),
			nameof(Camera.stereoEnabled),
			nameof(Camera.stereoSeparation),
			nameof(Camera.stereoTargetEye),
			nameof(Camera.targetDisplay),
			nameof(Camera.targetTexture),
			nameof(Camera.transparencySortAxis),
			nameof(Camera.transparencySortMode),
			nameof(Camera.useJitteredProjectionMatrixForTransparentRendering),
			nameof(Camera.useOcclusionCulling),
			nameof(Camera.usePhysicalProperties),
			nameof(Camera.velocity)
		};

		public override IList<string> GetVariableNames()
		{
			return _names;
		}

		public override VariableValue GetVariable(Camera obj, string name)
		{
			switch (name)
			{
				case nameof(Camera.activeTexture): return VariableValue.Create(obj.activeTexture);
				case nameof(Camera.actualRenderingPath): return VariableValue.Create(obj.actualRenderingPath);
				case nameof(Camera.allowDynamicResolution): return VariableValue.Create(obj.allowDynamicResolution);
				case nameof(Camera.allowHDR): return VariableValue.Create(obj.allowHDR);
				case nameof(Camera.allowMSAA): return VariableValue.Create(obj.allowMSAA);
				case nameof(Camera.areVRStereoViewMatricesWithinSingleCullTolerance): return VariableValue.Create(obj.areVRStereoViewMatricesWithinSingleCullTolerance);
				case nameof(Camera.aspect): return VariableValue.Create(obj.aspect);
				case nameof(Camera.backgroundColor): return VariableValue.Create(obj.backgroundColor);
				case nameof(Camera.cameraType): return VariableValue.Create(obj.cameraType);
				case nameof(Camera.clearFlags): return VariableValue.Create(obj.clearFlags);
				case nameof(Camera.clearStencilAfterLightingPass): return VariableValue.Create(obj.clearStencilAfterLightingPass);
				case nameof(Camera.commandBufferCount): return VariableValue.Create(obj.commandBufferCount);
				case nameof(Camera.cullingMask): return VariableValue.Create(obj.cullingMask);
				case nameof(Camera.depth): return VariableValue.Create(obj.depth);
				case nameof(Camera.depthTextureMode): return VariableValue.Create(obj.depthTextureMode);
				case nameof(Camera.eventMask): return VariableValue.Create(obj.eventMask);
				case nameof(Camera.farClipPlane): return VariableValue.Create(obj.farClipPlane);
				case nameof(Camera.fieldOfView): return VariableValue.Create(obj.fieldOfView);
				case nameof(Camera.focalLength): return VariableValue.Create(obj.focalLength);
				case nameof(Camera.forceIntoRenderTexture): return VariableValue.Create(obj.forceIntoRenderTexture);
				case nameof(Camera.gateFit): return VariableValue.Create(obj.gateFit);
				case nameof(Camera.lensShift): return VariableValue.Create(obj.lensShift);
				case nameof(Camera.nearClipPlane): return VariableValue.Create(obj.nearClipPlane);
				case nameof(Camera.opaqueSortMode): return VariableValue.Create(obj.opaqueSortMode);
				case nameof(Camera.orthographic): return VariableValue.Create(obj.orthographic);
				case nameof(Camera.orthographicSize): return VariableValue.Create(obj.orthographicSize);
				case nameof(Camera.overrideSceneCullingMask): return VariableValue.Create(obj.overrideSceneCullingMask);
				case nameof(Camera.pixelHeight): return VariableValue.Create(obj.pixelHeight);
				case nameof(Camera.pixelRect): return VariableValue.Create(obj.pixelRect);
				case nameof(Camera.pixelWidth): return VariableValue.Create(obj.pixelWidth);
				case nameof(Camera.rect): return VariableValue.Create(obj.rect);
				case nameof(Camera.renderingPath): return VariableValue.Create(obj.renderingPath);
				case nameof(Camera.scaledPixelHeight): return VariableValue.Create(obj.scaledPixelHeight);
				case nameof(Camera.scaledPixelWidth): return VariableValue.Create(obj.scaledPixelWidth);
				case nameof(Camera.sensorSize): return VariableValue.Create(obj.sensorSize);
				case nameof(Camera.stereoActiveEye): return VariableValue.Create(obj.stereoActiveEye);
				case nameof(Camera.stereoConvergence): return VariableValue.Create(obj.stereoConvergence);
				case nameof(Camera.stereoEnabled): return VariableValue.Create(obj.stereoEnabled);
				case nameof(Camera.stereoSeparation): return VariableValue.Create(obj.stereoSeparation);
				case nameof(Camera.stereoTargetEye): return VariableValue.Create(obj.stereoTargetEye);
				case nameof(Camera.targetDisplay): return VariableValue.Create(obj.targetDisplay);
				case nameof(Camera.targetTexture): return VariableValue.Create(obj.targetTexture);
				case nameof(Camera.transparencySortAxis): return VariableValue.Create(obj.transparencySortAxis);
				case nameof(Camera.transparencySortMode): return VariableValue.Create(obj.transparencySortMode);
				case nameof(Camera.useJitteredProjectionMatrixForTransparentRendering): return VariableValue.Create(obj.useJitteredProjectionMatrixForTransparentRendering);
				case nameof(Camera.useOcclusionCulling): return VariableValue.Create(obj.useOcclusionCulling);
				case nameof(Camera.usePhysicalProperties): return VariableValue.Create(obj.usePhysicalProperties);
				case nameof(Camera.velocity): return VariableValue.Create(obj.velocity);
			}

			return VariableValue.Empty;
		}

		public override SetVariableResult SetVariable(Camera obj, string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(Camera.activeTexture): return SetVariableResult.ReadOnly;
				case nameof(Camera.actualRenderingPath): return SetVariableResult.ReadOnly;
				case nameof(Camera.allowDynamicResolution): if (value.TryGetBool(out var allowDynamicResolution)) { obj.allowDynamicResolution = allowDynamicResolution; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.allowHDR): if (value.TryGetBool(out var allowHDR)) { obj.allowHDR = allowHDR; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.allowMSAA): if (value.TryGetBool(out var allowMSAA)) { obj.allowMSAA = allowMSAA; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.areVRStereoViewMatricesWithinSingleCullTolerance): return SetVariableResult.ReadOnly;
				case nameof(Camera.aspect): if (value.TryGetFloat(out var aspect)){ obj.aspect = aspect; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.backgroundColor): if (value.TryGetColor(out var backgroundColor)) { obj.backgroundColor = backgroundColor; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.cameraType): if (value.TryGetEnum<CameraType>(out var cameraType)) { obj.cameraType = cameraType; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.clearFlags): if (value.TryGetEnum<CameraClearFlags>(out var clearFlags)) { obj.clearFlags = clearFlags; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.clearStencilAfterLightingPass): if (value.TryGetBool(out var clearStencilAfterLightingPass)) { obj.clearStencilAfterLightingPass = clearStencilAfterLightingPass; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.commandBufferCount): return SetVariableResult.ReadOnly;
				case nameof(Camera.cullingMask): if (value.TryGetInt(out var cullingMask)) { obj.cullingMask = cullingMask; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.depth): if (value.TryGetFloat(out var depth)) { obj.depth = depth; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.depthTextureMode): if (value.TryGetEnum<DepthTextureMode>(out var depthTextureMode)) { obj.depthTextureMode = depthTextureMode; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.eventMask): if (value.TryGetInt(out var eventMask)) { obj.eventMask = eventMask; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.farClipPlane): if (value.TryGetFloat(out var farClipPlane)) { obj.farClipPlane = farClipPlane; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.fieldOfView): if (value.TryGetFloat(out var fieldOfView)) { obj.fieldOfView = fieldOfView; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.focalLength): if (value.TryGetFloat(out var focalLength)) { obj.focalLength = focalLength; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.forceIntoRenderTexture): if (value.TryGetBool(out var forceIntoRenderTexture)) { obj.forceIntoRenderTexture = forceIntoRenderTexture; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.gateFit): if (value.TryGetEnum<Camera.GateFitMode>(out var gateFit)) { obj.gateFit = gateFit; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.lensShift): if (value.TryGetVector2(out var lensShift)) { obj.lensShift = lensShift; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.nearClipPlane): if (value.TryGetFloat(out var nearClipPlane)) { obj.nearClipPlane = nearClipPlane; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.opaqueSortMode): if (value.TryGetEnum<OpaqueSortMode>(out var opaqueSortMode)) { obj.opaqueSortMode = opaqueSortMode; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.orthographic): if (value.TryGetBool(out var orthographic)) { obj.orthographic = orthographic; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.orthographicSize): if (value.TryGetFloat(out var orthographicSize)) { obj.orthographicSize = orthographicSize; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.overrideSceneCullingMask): if (value.TryGetInt(out var overrideSceneCullingMask)) { obj.overrideSceneCullingMask = (ulong)overrideSceneCullingMask; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.pixelHeight): return SetVariableResult.ReadOnly;
				case nameof(Camera.pixelRect): if (value.TryGetRect(out var pixelRect)) { obj.pixelRect = pixelRect; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.pixelWidth): return SetVariableResult.ReadOnly;
				case nameof(Camera.rect): if (value.TryGetRect(out var rect)) { obj.rect = rect; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.renderingPath): if (value.TryGetEnum<RenderingPath>(out var renderingPath)) { obj.renderingPath = renderingPath; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.scaledPixelHeight): return SetVariableResult.ReadOnly;
				case nameof(Camera.scaledPixelWidth): return SetVariableResult.ReadOnly;
				case nameof(Camera.sensorSize): if (value.TryGetVector3(out var sensorSize)) { obj.sensorSize = sensorSize; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.stereoActiveEye): return SetVariableResult.ReadOnly;
				case nameof(Camera.stereoConvergence): if (value.TryGetFloat(out var stereoConvergence)) { obj.stereoConvergence = stereoConvergence; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.stereoEnabled): return SetVariableResult.ReadOnly;
				case nameof(Camera.stereoSeparation): if (value.TryGetFloat(out var stereoSeparation)) { obj.stereoSeparation = stereoSeparation; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.stereoTargetEye): if (value.TryGetEnum<StereoTargetEyeMask>(out var stereoTargetEye)) { obj.stereoTargetEye = stereoTargetEye; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.targetDisplay): if (value.TryGetInt(out var targetDisplay)) { obj.targetDisplay = targetDisplay; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.targetTexture): if (value.TryGetReference<RenderTexture>(out var targetTexture)) { obj.targetTexture = targetTexture; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.transparencySortAxis): if (value.TryGetVector3(out var transparencySortAxis)) { obj.transparencySortAxis = transparencySortAxis; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.transparencySortMode): if (value.TryGetEnum<TransparencySortMode>(out var transparencySortMode)) { obj.transparencySortMode = transparencySortMode; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.useJitteredProjectionMatrixForTransparentRendering): if (value.TryGetBool(out var useJitteredProjectionMatrixForTransparentRendering)) { obj.useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.useOcclusionCulling): if (value.TryGetBool(out var useOcclusionCulling)) { obj.useOcclusionCulling = useOcclusionCulling; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.usePhysicalProperties): if (value.TryGetBool(out var usePhysicalProperties)) { obj.usePhysicalProperties = usePhysicalProperties; return SetVariableResult.Success; } return SetVariableResult.TypeMismatch;
				case nameof(Camera.velocity): return SetVariableResult.ReadOnly;
			}

			return SetVariableResult.NotFound;
		}
	}
}
