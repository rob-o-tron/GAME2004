// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PG/SpaceGridLines"
{
	Properties
	{
	    _Color ("Color", Color) = (.05, .2, .5, 0.5)
		_Color2("Color2", Color) = (.05, .2, .5, 0.5)
		_Opacity("Opacity", Range(0, 1)) = 1.0
		_MaxDist("MaxDist", Range(0, 500)) = 5.0
		_Frequency("Scanline Frequency", Range(0.0001, 50)) = 0.2
		_Speed("Scanline Speed", Range(-2, 2)) = 0.5
		_Pulse("Scanline Pulse", Range(0, 50)) = 1.0
	}
	SubShader
	{
		Tags { "Queue"="Overlay"}
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Off
			Cull Off
			ZWrite Off
			ZTest On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 objpos : TEXCOORD1;
			};

			float4 _Color;
			float4 _Color2;
			float _Opacity;
			float _MaxDist;
			float _Speed;
			float _Pulse;
			float _Frequency;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex =UnityObjectToClipPos(v.vertex);
				o.objpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float dist=length(i.objpos);
				float alpha=0.4f+0.6*abs((0.8+0.2*sin(_Time.y*_Pulse+ dist))*sin((i.objpos.x+i.objpos.y+i.objpos.z+_Time.y*_Speed)*_Frequency));
				//alpha*=alpha;
				float4 col=lerp(_Color,_Color2, alpha);
				col.a*=alpha*_Opacity*saturate(1.0-dist/(_MaxDist*_Opacity));
				return col;
			}
			ENDCG
		}
	}
}
