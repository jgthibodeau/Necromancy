//Shader "Simple/Lit Vertex Color_Alpha" {
//	Properties {
//		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
//	    _MainTex ("Base (RGB)", 2D) = "white" {}
//		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
//	}
//
//	SubShader {
//		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
//	//	Lighting Off ZWrite Off Fog { Mode Off }
////		Lighting On ZWrite Off Fog { Mode Off }
//		Lighting On
//		//Blend SrcAlpha OneMinusSrcAlpha
//
//		BindChannels {
//			Bind "Color", color
//			Bind "Vertex", vertex
//			Bind "texcoord", texcoord
//		}
//	   
//		Pass {
//			ColorMaterial AmbientAndDiffuse
//			SetTexture [_MainTex] {
//	        	Combine texture * primary
//	        }
//	    }

Shader "Simple/Lit Vertex Color_Alpha" {
	Properties {
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR;
		};

		fixed4 _TintColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * (IN.color * _TintColor);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}