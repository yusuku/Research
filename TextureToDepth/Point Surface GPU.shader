Shader "Graph/Point Surface GPU" {

	Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_Size ("Size",Range(0.01,10))=1.0
	}
	
	SubShader {
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5


		
		struct Input {
			float3 worldPos;
		};

		float _Smoothness;
		float _Size;
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Positions;
			StructuredBuffer<float4> _Colors;
		#endif
		void ConfigureProcedural () {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
				float3 position = _Positions[unity_InstanceID];
				float distance = length(position);

				// 10メートル以上なら非表示（無効な位置に移動）
				if (distance > 10) {
					position = float3(0, 0, 0); // 例えば原点に移動（非表示用）
					_Size = 0; // サイズをゼロにして見えなくする
				}

				unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
				unity_ObjectToWorld._m00_m11_m22 = _Size;
			#endif
		}
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
				surface.Albedo = _Colors[unity_InstanceID];
			#else
				surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
			#endif
			
			surface.Smoothness = _Smoothness;
		}
		ENDCG
	}
						
	FallBack "Diffuse"
}