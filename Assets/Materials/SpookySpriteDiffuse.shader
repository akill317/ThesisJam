Shader "Custom/SpookySpriteDiffuse" {
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_NoiseTex("NoiseTexture", 2D) = "white" {}
		_DistortionSpreader("_DistortionSpreader", Float) = 100
		_DistortionDamper("_DistortionDamper", Float) = 0.1
		_VertexOffsetAmount("VertexOffsetAmount", FLoat) = 5
	}

		SubShader
		{
			Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
	#pragma surface surf Lambert vertex:vert nofog keepalpha
	#pragma multi_compile _ PIXELSNAP_ON
	#pragma shader_feature ETC1_EXTERNAL_ALPHA

			sampler2D _MainTex;
		fixed4 _Color;
		sampler2D _AlphaTex;
		sampler2D _NoiseTex;
		float _DistortionSpreader;
		float _DistortionDamper;
		float _VertexOffsetAmount;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
			float4 worldPosition;
		};

		void vert(inout appdata_full v, out Input o)
		{
	#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap(v.vertex);
	#endif	
			float4 offset = float4(
				tex2Dlod(_NoiseTex, float4(v.vertex.y / _DistortionSpreader + _Time[1]/ _VertexOffsetAmount, 0, 0, 0)).r - 0.5,
				tex2Dlod(_NoiseTex, float4(0, v.vertex.x / _DistortionSpreader + _Time[1] / _VertexOffsetAmount, 0, 0)).r - 0.5,
				0,
				0
				);
			v.vertex += offset * 10;
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
			o.worldPosition = v.vertex;
		}

		fixed4 SampleSpriteTexture(float2 uv)
		{
			fixed4 color = tex2D(_MainTex, uv);

	#if ETC1_EXTERNAL_ALPHA
			color.a = tex2D(_AlphaTex, uv).r;
	#endif //ETC1_EXTERNAL_ALPHA

			return color;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float2 offset = -0.5 + float2(
				tex2D(_NoiseTex, float2(IN.worldPosition.y / _DistortionSpreader + _Time[1], 0)).r ,
				tex2D(_NoiseTex, float2(0, IN.worldPosition.x / _DistortionSpreader + _Time[1])).r);
			fixed4 c = SampleSpriteTexture(IN.uv_MainTex + offset * _DistortionDamper) * IN.color;
			o.Albedo = c.rgb * c.a;
			o.Alpha = c.a;
		}
		ENDCG
		}

			Fallback "Transparent/VertexLit"
}
