Shader "CityGrid/WallShield"
{
	Properties{
	  _Color("Color", Color) = (1,1,1,1)
	  _ShieldColor("ShieldColor", Color) = (1,1,1,1)
	}
		SubShader{
		  Tags { "RenderType" = "Opaque" }
		  Cull Off
		  CGPROGRAM
		  #pragma surface surf Lambert
		  struct Input {
			  float3 worldPos;
		  };
		  sampler2D _MainTex;
		  float4 _Color;
		  float4 _ShieldColor;
		  void surf(Input IN, inout SurfaceOutput o) {
			  float shield = sin(2.5f*IN.worldPos.y - 3.0f*_Time.y)*0.5+0.5;
			  o.Albedo = _Color;
			  o.Emission = shield * _ShieldColor;
			  o.Alpha = 0.1f;
		  }
		  ENDCG
	}
		Fallback "Diffuse"
}