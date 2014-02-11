Shader "Custom/ImageEffects" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SaturationAmount ("Saturation Amount", Range(0.0, 1.0)) = 1.0
		_BrightnessAmount ("Brightness Amount", Range(0.0, 1.0)) = 1.0
		_ContrastAmount ("Contrast Amount", Range(0.0,1.0)) = 1.0
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
    		#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float _SaturationAmount;
			uniform float _BrightnessAmount;
			uniform float _ContrastAmount;
			uniform float4 _MainTex_ST;
			
			float3 ContrastSaturationBrightness( float3 color, float brt, float sat, float con)
			{
				//RGB Color Channels
				float AvgLumR = 0.5;
				float AvgLumG = 0.5;
				float AvgLumB = 0.5;
				
				//Luminace Coefficients for brightness of image
				float3 LuminaceCoeff = float3(0.2125,0.7154,0.0721);
				
				//Brigntess calculations
				float3 AvgLumin = float3(AvgLumR,AvgLumG,AvgLumB);
				float3 brtColor = color * brt;
				float intensityf = dot(brtColor, LuminaceCoeff);
				float3 intensity = float3(intensityf, intensityf, intensityf);
				
				//Saturation calculation
				float3 satColor = lerp(intensity, brtColor, sat);
				
				//Contrast calculations
				float3 conColor = lerp(AvgLumin, satColor, con);
				
				return conColor;
				
			}
			
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
		    
		    fragmentInput vert( vertexInput i )
		    {
		        fragmentInput o;
		        o.pos = mul( UNITY_MATRIX_MVP, i.vertex );
		 
		//This is a standard defined function in Unity, 
		//Does exactly the same as the next line of code
		        //o.uv = TRANSFORM_TEX( i.texcoord, _MainTex );
		 
		//LOOK! _MainTex_ST.xy is tiling and _MainTex_ST.zw is offset
		        o.uv =  i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
		 
		        return o;
		    }
			
			
			half4 frag( fragmentInput i ) : COLOR
			{
				half4 renderTex = half4( tex2D( _MainTex, i.uv ).rgb, 1.0);
				
				renderTex.rgb = ContrastSaturationBrightness(renderTex.rgb, _BrightnessAmount, _SaturationAmount, _ContrastAmount); 
				
				return renderTex;
			
			}
			
			ENDCG
		}
		
	}
}
