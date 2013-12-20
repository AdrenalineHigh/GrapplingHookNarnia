Shader "DX11/DrawIndirect" {
Properties {
	_MainTex ("", 2D) = "white" {}
	_Sprite ("", 2D) = "white" {}
}
SubShader {


Pass {

	ZWrite Off Cull Off Fog { Mode Off }
	Blend SrcAlpha One

	CGPROGRAM
	#pragma target 5.0

	#pragma vertex vert
	#pragma geometry geom
	#pragma fragment frag

	#include "UnityCG.cginc"
	
	struct PerParticleData
	{
		float3 Originalposition;
		float3 position;
		float3 velocity;
		float3 scale;
		float4 colour;
		float life;
		float startingLife;
	};
	
	StructuredBuffer<PerParticleData> pointBuffer;

	struct vs_out 
	{
		float4 pos : SV_POSITION;
		float3	normal	: NORMAL;
		float2 size		: TEXCOORD0;
		float4 colour	: TEXCOORD1;
	};

	vs_out vert (appdata_base v, uint id : SV_VertexID)
	{
		vs_out o;
		o.pos = mul(_Object2World, float4(pointBuffer[id].position, 1));
		o.normal = v.normal;
		o.size = float2(pointBuffer[id].scale.x, 0);
		o.colour = pointBuffer[id].colour;
		return o;
	}

	struct gs_out {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 colour : TEXCOORD1;
	};

	float _Size;

	[maxvertexcount(4)]
	void geom (point vs_out input[1], inout TriangleStream<gs_out> outStream)
	{
		float3 up =  UNITY_MATRIX_IT_MV[1].xyz;
		float3 look = _WorldSpaceCameraPos - input[0].pos;
		look = normalize(look);
		float3 right = cross(up, look);
		
		float halfS = 0.5f * (_Size * input[0].size.x);
				
		float4 v[4];
		v[0] = float4(input[0].pos + halfS * right - halfS * up, 1.0f);
		v[1] = float4(input[0].pos + halfS * right + halfS * up, 1.0f);
		v[2] = float4(input[0].pos - halfS * right - halfS * up, 1.0f);
		v[3] = float4(input[0].pos - halfS * right + halfS * up, 1.0f);
		
		float4x4 vp = mul(UNITY_MATRIX_MVP, _Object2World);
		
		gs_out output;
		output.pos = mul(vp, v[0]); output.uv=float2(0,0); output.colour = input[0].colour; outStream.Append (output);
		output.pos = mul(vp, v[1]); output.uv=float2(1,0); output.colour = input[0].colour; outStream.Append (output);
		output.pos = mul(vp, v[2]); output.uv=float2(0,1); output.colour = input[0].colour; outStream.Append (output);
		output.pos = mul(vp, v[3]); output.uv=float2(1,1); output.colour = input[0].colour; outStream.Append (output);
		outStream.RestartStrip();
	}

	sampler2D _Sprite;
	fixed4 _Color;

	fixed4 frag (gs_out i) : COLOR0
	{
		fixed4 col = tex2D (_Sprite, i.uv);
		//fixed4 finalColour = fixed4(_Color.rgb * col.rgb * i.colour.rgb, _Color.a * col.a);
		fixed4 finalColour = fixed4(_Color.rgb * col.rgb, 1);
		return finalColour;
	}

	ENDCG

}

}

Fallback Off
}
