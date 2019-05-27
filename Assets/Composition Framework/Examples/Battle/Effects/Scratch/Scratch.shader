Shader "PiRhoSoft/Examples/Scratch"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[PerRendererData] _Color("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		[PerRendererData] _Progress("Progress", Range(0.0, 1.0)) = 0.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata_t input)
				{
					v2f output;
					output.vertex = UnityObjectToClipPos(input.vertex);
					output.texcoord = input.texcoord;
					output.color = input.color * _Color;

					return output;
				}

				sampler2D _MainTex;
				float _Progress;

				fixed4 frag(v2f input) : SV_Target
				{
					fixed4 color = tex2D(_MainTex, input.texcoord);
					float startRange = 0.2 * (1 - _Progress);
					float endRange = 0.2 * _Progress;
					color.a *= 1 - smoothstep(_Progress - startRange, _Progress + endRange, input.texcoord.x);
					return color * input.color;
				}

			ENDCG
		}
	}
}
