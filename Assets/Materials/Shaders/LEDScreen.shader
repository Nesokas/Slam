// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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
		_DrawColor ("Draw Color", Color) = (0,0,0,1)
		_Invert ("Invert Colors", float) = 0
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
			uniform float4 _DrawColor;
			uniform float _Invert;
			
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
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = i.texcoord;
				
				return o;
			}
			
			float4 applyLuminanceStepping(float4 in_color)
			{
				float sum = in_color.x + in_color.y + in_color.z;
			 	float luminance = sum/3.0;
				float3 ratios = float3(in_color.x/luminance, in_color.y/luminance, in_color.z/luminance);

				float luminanceStep = 1.0/float(_LuminanceSteps);
			 	float luminanceBin = ceil(luminance/luminanceStep);
			 	float luminanceFactor = luminanceStep * luminanceBin + _LuminanceBoost;

			 	//use ratios * luminanceFactor as our new color so that original color hue is maintained
			 	return float4(ratios * luminanceFactor,1.0);
//			 	return in_color;
			}
			
			half4 frag(fragmentInput i) : COLOR
			{
				
				//will hold our averaged color from our sample points
				float4 avgColor;
				
				//width of "pixel region" in texture coords
				float2 texCoordsStep = float2(1.0/(float(_BillboardSize_x)/float(_PixelSize)), 1.0/(float(_BillboardSize_y)/float(_PixelSize)));
				
				float2 pixelRegionCoords = frac(i.uv/texCoordsStep);
				
				//"pixel region" number counting away from base case
				float2 pixelBin = floor(i.uv/texCoordsStep);
				
				//width of "pixel region" divided by 3 (for KERNEL_SIZE = 9, 3x3 square)
				float2 inPixelStep = texCoordsStep/3.0;
				float2 inPixelHalfStep = inPixelStep/2.0;
				
			    //use offset (pixelBin * texCoordsStep) from base case 
				// (the lower left corner of billboard) to compute texCoords
				float2 offset = pixelBin * texCoordsStep;
				
				texCoords0 = float2(inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + offset;
				texCoords1 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + offset;
				texCoords2 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelStep.y*2.0 + inPixelHalfStep.y) + offset;
				texCoords3 = float2(inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + offset;
				texCoords4 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + offset;
				texCoords5 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelStep.y + inPixelHalfStep.y) + offset;
				texCoords6 = float2(inPixelHalfStep.x, inPixelHalfStep.y) + offset;
				texCoords7 = float2(inPixelStep.x + inPixelHalfStep.x, inPixelHalfStep.y) + offset;
				texCoords8 = float2(inPixelStep.x*2.0 + inPixelHalfStep.x, inPixelHalfStep.y) + offset;
				
				//take average of 9 pixel samples
				avgColor = tex2D(_MainTex, texCoords0) + 
							tex2D(_MainTex, texCoords1) + 
							tex2D(_MainTex, texCoords2) + 
							tex2D(_MainTex, texCoords3) + 
							tex2D(_MainTex, texCoords4) + 
							tex2D(_MainTex, texCoords5) + 
							tex2D(_MainTex, texCoords6) + 
							tex2D(_MainTex, texCoords7) +
							tex2D(_MainTex, texCoords8);
							
				float4 prevAvgColor = avgColor;
				
				if (tex2D(_MainTex, texCoords4).x < _Threshold && tex2D(_MainTex, texCoords4).y < _Threshold && tex2D(_MainTex, texCoords4).z < _Threshold)
					avgColor = float4(0,0,0,1);
				else avgColor = _DrawColor * avgColor;
				
				avgColor = avgColor/float(KERNEL_SIZE);
				
				avgColor = applyLuminanceStepping(avgColor);
				
				float2 powers = pow(abs(pixelRegionCoords - 0.5), 2.0);
				float radiusSqrd = pow(_PixelRadius, 2.0);
				float gradient = smoothstep(radiusSqrd - _Tolerance, radiusSqrd + _Tolerance, powers.x + powers.y);
				
				float4 backColor = float4(0, 0, 0, 1.0);
				
				if(_Invert == 1) {
					if(prevAvgColor.x >= 0.8 && prevAvgColor.y >= 0.8 && prevAvgColor.z >= 0.8)
						avgColor = float4(0,0,0,1);
					else avgColor = _DrawColor;
				}
				

				return lerp(avgColor, backColor, gradient);
			}
					
			ENDCG
		}
	}
	
	Fallback "Diffuse"
}
