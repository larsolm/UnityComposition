Shader "PiRhoSoft/Composition/Shaders/Cutoff"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTexture("Transition Texture", 2D) = "white" {}
		_Color("Screen Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Cutoff("Cutoff", Range(0.0, 1.0)) = 0.0
		[MaterialToggle] _Distort("Distort", Float) = 0.0
		_Fade("Fade", Range(0.0, 1.0)) = 0.0
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _TransitionTexture;
				int _Distort;
				float _Fade;

				sampler2D _MainTex;
				float _Cutoff;
				fixed4 _Color;

				fixed4 frag(v2f_img input) : SV_Target
				{
					fixed4 transition = tex2D(_TransitionTexture, input.uv);
					fixed2 direction = _Distort ? normalize(float2((transition.r - 0.5) * 2, (transition.g - 0.5) * 2)) : float2(0.0, 0.0);
					fixed4 color = tex2D(_MainTex, input.uv + _Cutoff * direction);

					return transition.b < _Cutoff ? lerp(color, _Color, _Fade) : color;
				}
			ENDCG
		}
	}
}
