Shader "Custom/Cloud" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_SpriteTex ("Base (RGB)", 2D) = "white" {}
		_GradientTex ("Gradient (RGB)", 2D) = "white" {}
		_Size ("Size", Range(0, 64)) = 0.5
		 _RotationSpeed ("Rotation Speed", Float) = 2.0
	}

	#warning Upgrade NOTE: SubShader commented out; uses Unity 2.x per-pixel lighting. You should rewrite shader into a Surface Shader.
SubShader 
	{
		Pass
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
			ColorMask RGB
			Lighting Off ZWrite Off
			
			CGPROGRAM
			#pragma debug
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"

				// **************************************************************
				// Data structures												*
				// **************************************************************
				struct GS_INPUT
				{
					float4	pos		: POSITION;
					float3	normal	: NORMAL;
					float2  tex0	: TEXCOORD0;
					float2 scale	: TEXCOORD1;
					float4 myColor	: COLOR;
				};

				struct FS_INPUT
				{
					float4	pos		: SV_POSITION;
					float2  tex0	: TEXCOORD0;
					float4 color	: COLOR;
				};

				struct ParticleData
				{
					float3 position;
					float3 velocity;
				};
				
				struct ColorData
				{
					float4 colorData;
				};
				
				struct ScaleData
				{
					float scaleData;
				};
				
				// **************************************************************
				// Vars															*
				// **************************************************************

				float _Size;
				float4x4 _VP;
				Texture2D _SpriteTex;
				StructuredBuffer<ParticleData> pointBuffer;
				StructuredBuffer<ColorData> buf_Colors;
				StructuredBuffer<ScaleData> buf_Scales;
				SamplerState sampler_SpriteTex;

				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(appdata_base v, uint id : SV_VertexID, uint inst : SV_InstanceID)
				{
					GS_INPUT output = (GS_INPUT)0;

					output.pos =  mul(_Object2World, pointBuffer[id].position);
					output.normal = v.normal;
					output.tex0 = float2(0, 0);
					output.myColor = float4(1,0,1,0.5);
					//output.myColor = buf_Colors[id].colorData;
					output.scale = float2(1,1);
					//output.scale.x = buf_Scales[id].scaleData;
					return output;
				}
				
				float _RotationSpeed;

				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(4)]
				void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
				{
					float3 up =  UNITY_MATRIX_IT_MV[1].xyz;
					float3 look = _WorldSpaceCameraPos - p[0].pos;
					look = normalize(look);
					float3 right = cross(up, look);
					
					float halfS = 0.5f * (_Size * p[0].scale.x);
							
					float4 v[4];
					v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
					v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
					v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
					v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

					float4x4 vp = mul(UNITY_MATRIX_MVP, _Object2World);
					
					
					float sinX = sin ( _RotationSpeed * _Time );

		            float cosX = cos ( _RotationSpeed * _Time );
		
		            float sinY = sin ( _RotationSpeed * _Time );
		
		            float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
            
					FS_INPUT pIn;
					pIn.pos = mul(vp, v[0]);
					pIn.tex0 = mul(float2(1.0f, 0.0f), rotationMatrix);
					pIn.color = p[0].myColor;
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[1]);
					pIn.tex0 = mul(float2(1.0f, 1.0f), rotationMatrix);
					pIn.color = p[0].myColor;
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[2]);
					pIn.tex0 = mul(float2(0.0f, 0.0f), rotationMatrix);
					pIn.color = p[0].myColor;
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[3]);
					pIn.tex0 = mul(float2(0.0f, 1.0f), rotationMatrix);
					pIn.color = p[0].myColor;
					triStream.Append(pIn);
				}

				fixed4 _TintColor;
				Texture2D _GradientTex;
				SamplerState Sampler_GradientTex;
			
				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : COLOR
				{
					float4 c = input.color;
					
					float3 col = _TintColor* (_SpriteTex.Sample(sampler_SpriteTex, input.tex0)) ;//* (_GradientTex.Sample(Sampler_GradientTex, input.tex0));
					
					float4 _Color = float4(col.r, col.g, col.b, _TintColor.a * (_SpriteTex.Sample(sampler_SpriteTex, input.tex0)).a) ;
					return _Color;
				}

			ENDCG
		}
		
	} 
	
	
}
