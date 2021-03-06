﻿Shader "CL/Stencil/Unlit/Color"
{
	// A version of Unity's Unlit Color that supports stencil comparrision.
	// Fog support has been disabled, can be re-enabled by removing comments, though unsure what the effect will be.

	// Unlit shader. Simplest possible colored shader.
	// no lighting, no lightmap support, no texture

	Properties
	{
		_Color 		("Main Color", 	Color) 	= (1,1,1,1)
		_StencilID 	("Stencil ID", 	Float) 	= 0
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass 
		{  
			Stencil
			{			
				Ref [_StencilID]	// Compare with the stencil value.
				Comp equal			// Only pass pixel if the stencil equals the ref value.
			}
			
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			//	#pragma multi_compile_fog
				
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				//	UNITY_FOG_COORDS(0)
				};

				fixed4 _Color;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//	UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = _Color;
				//	UNITY_APPLY_FOG(i.fogCoord, col);
					UNITY_OPAQUE_ALPHA(col.a);
					return col;
				}
			ENDCG
		}
	}

}
