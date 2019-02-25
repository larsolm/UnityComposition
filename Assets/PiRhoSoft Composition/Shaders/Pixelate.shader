Shader "PiRhoSoft/Composition/Shaders/Pixelate"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amount ("Amount", Int) = 5
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

				sampler2D _MainTex;
				int _Amount;

				fixed4 frag(v2f_img i) : COLOR
				{
					int2 Pixelate = int2(_Amount, _Amount);

					fixed4 color = float4(0,0,0,0);

						float2 PixelSize = 1 / float2(_ScreenParams.x, _ScreenParams.y);
						float2 BlockSize = PixelSize * Pixelate;
						float2 CurrentBlock = float2(
							floor(i.uv.x / BlockSize.x) * BlockSize.x,
							floor(i.uv.y / BlockSize.y) * BlockSize.y
						);

						color = tex2D(_MainTex, CurrentBlock + BlockSize / 2);
						color += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4,BlockSize.y / 4));
						color += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 2,BlockSize.y / 4));
						color += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3,BlockSize.y / 4));
						color += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4,BlockSize.y / 2));
						color += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3,BlockSize.y / 2));
						color += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 4,(BlockSize.y / 4) * 3));
						color += tex2D(_MainTex, CurrentBlock + float2(BlockSize.x / 2,(BlockSize.y / 4) * 3));
						color += tex2D(_MainTex, CurrentBlock + float2((BlockSize.x / 4) * 3,(BlockSize.y / 4) * 3));
						color /= 9;

					return color;
				}
			ENDCG
		}
	}
}
