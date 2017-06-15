// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MRCaptureTestB"
{
	Properties
	{
		_Color("", Color) = (1,1,1,1)
		_MainTex("Color", 2D) = "white" {}
		_DepthFeedTex("Depth", 2D) = "white" {}
		_OffsetScale("Offset Scale",Range(0,10)) = 1
		_Smoothing("Smoothing",Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Cull Off

		CGPROGRAM

#pragma surface surf Standard vertex:vert nolightmap  addshadow alpha:fade 
#pragma target 3.0

//#include "Common.cginc"

	struct Input { 
		float2 uv_MainTex : TEXCOORD0;
		float dummy; 
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	sampler2D _MainTex;
	sampler2D _DepthFeedTex;

	float3 _CameraPos;
	float3 _DistortionAxis;
	float _OffsetScale = 1;
	float _Smoothing = 0.9;


	float getDisplacement(float2 uv) {
		return tex2Dlod(_DepthFeedTex, float4(uv, 0, 0)).r;
		/*float depthVal = tex2Dlod(_DepthFeedTex, float4(uv, 0, 0)).r;
		float colVal = tex2Dlod(_MainTex, float4(uv, 0, 0)).b;
		colVal = saturate(1-colVal * 10);
		return depthVal * colVal *_OffsetScale;*/
	}

	void vert(inout appdata_full v)
	{	
		float3 wsVertex = mul(unity_ObjectToWorld, v.vertex);
		
		//_DistortionAxis = _CameraPos.xyz - wsVertex; //UnityObjectToWorldPos(v.vertex);

		_DistortionAxis = float3(0, 1, 0);
		//float4 ssVert = UnityObjectToClipPos(v.vertex);

		float v1 = getDisplacement(v.texcoord.xy);// v.vertex.xyz;

		float2 offset = float2(0.01, 0);
		float3 v2 = float3(offset.x,0,offset.y) + getDisplacement(v.texcoord.xy +offset)*_DistortionAxis;
		float3 v3 = float3(offset.y, 0, offset.x) + getDisplacement(v.texcoord.xy +offset.yx)*_DistortionAxis;;
		//float4 d = tex2Dlod(_DepthFeedTex, float4(v.texcoord.xy,0,0));
		//v1.y += (1-d.r);
		//float3 v2 = displace(v.texcoord.xyz);
		//float3 v3 = displace(v.texcoord1.xyz);

		//v2.y -= (v2.y - v1.y)*_Smoothing;
		//v3.y -= (v3.y - v1.y)*_Smoothing;

		//ssVert.x += v1;
		//v.vertex = ssVert;
		v.vertex.xyz += v1*_DistortionAxis;
		
		//v.normal = normalize(cross(v2 - v1, v3 - v1));
	}

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;//_Color.rgb;
		o.Metallic = 0;// _Metallic;
		o.Smoothness = 0;// _Glossiness;
		float depthVal = saturate(tex2D(_DepthFeedTex, IN.uv_MainTex).r*10);
		float colVal = saturate((1 - tex2D(_MainTex, IN.uv_MainTex).b) * 10); //make blue alpha .. but wrong blue for now
		o.Alpha = depthVal*colVal;
	}

	ENDCG
	}
		FallBack "Diffuse"
}
