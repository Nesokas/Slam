Shader "Custom/LEDScreen" {
	Properties
	{
		_MainTex ("Main Texture", 2D) = "black" {}
		_PixelSize ("Pixel Size", float) = 0
		_Threshold ("Threshold", float) = 0
		_BillboardSize_x ("Billboard Size X", float) = 0
		_BillboardSize_y ("Billboard Size Y", float) = 0
		_Tolerance ("Tolerance", float) = 0
		_PixelRadius ("Pixel Radius", float) = 0
		_LuminanceSteps ("Luminace Steps", float) = 0
		_LuminanceBoost ("Luminance Boost", float) = 0
	}
	
	SubShader 
	{
		Tags { "Queue" = "Geometry" }
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			#define KERNEL_SIZE 9
			
			uniform sampler2D _MainTex;
			uniform float _PixelSize;
			uniform float _BillboardSize_x;
			uniform float _BillboardSize_y;
			uniform float _Tolerance;
			uniform float _PixelRadius;
			uniform float _LuminanceSteps;
			uniform float _LuminanceBoost;
			uniform float _Threshold;
			
			float2 texCoords0;
			float2 texCoords1;
			float2 texCoords2;
			float2 texCoords3;
			float2 texCoords4;
			float2 texCoords5;
			float2 texCoords6;
			float2 texCoords7;
			float2 texCoords8;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			
			struct fragmentInput
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			
			fragmentInput vert(vertexInput i)
			{
				fragmentInput o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.texcoord;
				
				return o;
			}
			
			half4 frag(fragmentInput i) : COLOR
			{
				float4 avgColor;
				float2 texCoordsStep = float2(1.0/(float(_BillboardSize_x)/float(_PixelSize)), 1.0/(float(_BillboardSize_y)/float(_PixelSize)));
				float2 pixelRegionCoords = frac(i.uv/texCoordsStep);
				float2 pixelBin = floor(i.uv/texCoordsStep);
				float2 inPixelStep = texCoordsStep/3.0;
				float2 inPixelHalfStep = inPixelStep/2.0;
				
				texCoords0 = float2(inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords1 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords2 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords3 = float2(inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords4 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords5 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords6 = float2(inPixelHalfStep.x, inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords7 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelHalfStep.y) + pixelBin * texCoordsStep;
				texCoords8 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelHalfStep.y) + pixelBin * texCoordsStep;
				
				avgColor = tex2D(_MainTex, texCoords0) + 
							tex2D(_MainTex, texCoords1) + 
							tex2D(_MainTex, texCoords2) + 
							tex2D(_MainTex, texCoords3) + 
							tex2D(_MainTex, texCoords4) + 
							tex2D(_MainTex, texCoords5) + 
							tex2D(_MainTex, texCoords6) + 
							tex2D(_MainTex, texCoords7) +
							tex2D(_MainTex, texCoords8);
						   
				avgColor = avgColor/float(9);

				if (avgColor.x < _Threshold)
					avgColor.x = 0;
				else
					avgColor.x = 1;
					
				if (avgColor.y < _Threshold)
					avgColor.y = 0;
				else
					avgColor.y = 1;
					
				if (avgColor.z < _Threshold)
					avgColor.z = 0;
				else
					avgColor.z = 1;
				
				float2 powers = pow(abs(pixelRegionCoords - 0.5), float2(2.0));
				float radiusSqrd = pow(_PixelRadius, 2.0);
				float gradient = smoothstep(radiusSqrd - _Tolerance, radiusSqrd + _Tolerance, powers.x + powers.y);

				return lerp(avgColor, float4(0, 0, 0, 1.0), gradient);
//				return avgColor;
			}
					
			ENDCG
		}
	}
	
	Fallback "Diffuse"
}
