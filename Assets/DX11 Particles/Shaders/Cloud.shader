Shader "DX11/Cloud" {
Properties {
	_MainTex ("", 2D) = "white" {}
	_Sprite ("", 2D) = "white" {}
	_RotationSpeed ("Rotation Speed", Float) = 2.0
}
SubShader {
//
//Pass
//{
//	ZWrite Off ZTest Always Cull Off Fog { Mode Off }
//
//	CGPROGRAM
//	#pragma vertex vert
//	#pragma fragment frag
//	#pragma target 5.0
//	#include "UnityCG.cginc"
//
//	struct appdata {
//		float4 vertex : POSITION;
//		float2 texcoord : TEXCOORD0;
//	};
//
//	struct v2f {
//		float4 pos : SV_POSITION;
//		float2 uv : TEXCOORD0;
//	};
//
//	v2f vert (appdata v)
//	{
//		v2f o;
//		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//		o.uv = v.texcoord;
//		return o;
//	}
//
//	sampler2D _MainTex;
//	AppendStructuredBuffer<float2> pointBufferOutput : register(u1);
//
//
//	fixed4 frag (v2f i) : COLOR0
//	{
//		fixed4 c = tex2D (_MainTex, i.uv);
//		fixed4 c1 = tex2D (_MainTex, i.uv + 2.0/_ScreenParams.xy);
//		float lumc = Luminance (c);
//		float lumc1 = Luminance (c1);
//		[branch]
//		if (lumc > 0.98 && lumc > lumc1)
//		{
//			pointBufferOutput.Append (i.uv);
//		}
//		return c * 0.6;
//	}
//	ENDCG
//}


Pass {
Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Offset -1, -1
	ColorMask RGB
	Lighting Off ZWrite Off
	CGPROGRAM
	#pragma target 5.0

	#pragma vertex vert
	#pragma geometry geom
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct PerParticleData
	{
		float3 Originalposition;
		float3 position;
		float3 velocity;
		float3 scale;
		float4 colour;
		float life;
	};
	
	StructuredBuffer<PerParticleData> pointBuffer;

	struct vs_out 
	{
		float4 pos : SV_POSITION;
		float3	normal	: NORMAL;
		float2 size		: TEXCOORD0;
		float4 color	: COLOR;
	};

	vs_out vert (appdata_base v, uint id : SV_VertexID)
	{
		vs_out o;
		o.pos = mul(_Object2World, float4(pointBuffer[id].position, 1));
		o.normal = v.normal;
		o.size = float2(pointBuffer[id].scale.x, 0);
		o.color = pointBuffer[id].colour;
		return o;
	}

	struct gs_out {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 color : COLOR;
	};

	float _Size;
	float _RotationSpeed;

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
		
		float s = sin ( _RotationSpeed * _Time );
        float c = cos ( _RotationSpeed * _Time );
        
        float2x2 rotationMatrix = float2x2( c, -s, s, c);
		rotationMatrix *= 0.42;
		rotationMatrix += 0.5;
		rotationMatrix = rotationMatrix * 2-1;
		
		float4x4 vp = mul(UNITY_MATRIX_MVP, _Object2World);
		
		gs_out output;
		output.pos = mul(vp, v[0]); output.uv= mul(float2(-0.5,-0.5), rotationMatrix) +0.5; output.color = input[0].color; outStream.Append (output);
		output.pos = mul(vp, v[1]); output.uv= mul(float2(0.5,-0.5), rotationMatrix) +0.5; output.color = input[0].color; outStream.Append (output);
		output.pos = mul(vp, v[2]); output.uv= mul(float2(-0.5,0.5), rotationMatrix) +0.5; output.color = input[0].color; outStream.Append (output);
		output.pos = mul(vp, v[3]); output.uv= mul(float2(0.5,0.5), rotationMatrix) +0.5; output.color = input[0].color; outStream.Append (output);
		outStream.RestartStrip();
	}

	sampler2D _Sprite;
	float4 _Color;

	fixed4 frag (gs_out i) : COLOR0
	{
	 	float3 tint = _Color.rgb;
		float4 col = tex2D (_Sprite, i.uv);
		col = col * _Color;
		return col;
	}

	ENDCG

}

}

Fallback Off
}
