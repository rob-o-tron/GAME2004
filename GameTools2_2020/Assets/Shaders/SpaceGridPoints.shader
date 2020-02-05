Shader "PG/SpaceGridPoints"
{
	Properties{
		_Size("Size", Range(0, 10)) = 0.005
		_Color ("dotColor", Color) = (.9, .2, .7, 1.0)
		_Opacity ("opacity", Range(0, 1)) = 1.00
		_MaxDist("MaxDist", Range(0, 500)) = 5.0
	}
	SubShader
	{
		Tags { "Queue" = "Overlay" }
		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Off
		Cull Off
		ZWrite Off
		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma fragment frag
		#pragma geometry geom

		#include "UnityCG.cginc"

		uniform	float _Size;
		uniform float4 _Color;
		uniform	float _Opacity;
		float _MaxDist;

		struct vertexInput 
		{
            float4 vertex : POSITION;
			float2 uv	: TEXCOORD0;
        };

		struct gsin
		{
			float4 pos : SV_POSITION;
			float2 uv	: TEXCOORD0;
		};

		struct ps_input
		{
			float4 pos : SV_POSITION;
			float2 tex0	: TEXCOORD0;
			float2 uv1	: TEXCOORD1;
		};

		gsin vert(vertexInput input)
		{
			gsin o;
			o.pos = mul(unity_ObjectToWorld,float4(input.vertex.xyz,1));
			float dist=saturate(1.0-length(_WorldSpaceCameraPos-o.pos)/(_MaxDist*_Opacity));
			float alpha=abs(cos(-_Time.y*2.0+25.0f*dist));
			o.uv = float2(dist,alpha);
			return o;
		}

		[maxvertexcount(3)]
		void geom(point gsin p[1], inout TriangleStream<ps_input> triStream)
		{
			float3 up = float3(0, 1, 0);
			float3 look = _WorldSpaceCameraPos - p[0].pos;
			float dist= length(look);
			look = normalize(look);
			float3 right = cross(up, look);
			float halfS = 0.005f * _Size * dist;

			float4x4 vp = UNITY_MATRIX_VP;

			float4 v[4];
			v[0] = float4(p[0].pos + halfS * up, 1.0f);
			v[1] = float4(p[0].pos - halfS * right * 0.866f - 0.5f*halfS * up, 1.0f);
			v[2] = float4(p[0].pos + halfS * right * 0.866f - 0.5f*halfS * up, 1.0f);


			ps_input gout;
			gout.pos = mul(vp, v[0]);
			gout.tex0 = float2(0.0f, 1.0f);
			gout.uv1 = p[0].uv;
			triStream.Append(gout);

			gout.pos = mul(vp, v[1]);
			gout.tex0 = float2(0.866f, -0.5f);
			gout.uv1 = p[0].uv;
			triStream.Append(gout);

			gout.pos = mul(vp, v[2]);
			gout.tex0 = float2(-0.866f, -0.5f);
			gout.uv1 = p[0].uv;
			triStream.Append(gout);
		}

		//Pixel function returns a solid color for each point.
		float4 frag(ps_input i) : SV_Target
		{
			float falloff=i.uv1;
			float dist=3.0f*length(i.tex0);
			dist*=dist;
			float4 col;
			float a;
			a = clamp(1 -_Opacity*dist, 0, 1);
			
			col = _Color*_Opacity;
			col.a = _Color.a*a*_Opacity*falloff;
			return col;
		}

			ENDCG
		}
	}

		Fallback Off
}

