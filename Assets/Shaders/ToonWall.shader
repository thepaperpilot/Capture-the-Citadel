// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Roystan/ToonWall"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_Scale("Scale", Float) = 8
		// Ambient light is applied uniformly to all surfaces on the object.
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		// Controls the size of the specular reflection.
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		// Control how smoothly the rim blends when approaching unlit
		// parts of the surface.
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
		_XMult("X Multiplier", Float) = 0.57735
	}
	SubShader
	{
		Pass
		{
			// Setup our pass to use Forward rendering, and only receive
			// data on the main directional light and ambient light.
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Compile multiple versions of this shader depending on lighting settings.
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			// Files below include macros and functions to assist
			// with lighting and shadows.
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 viewDir : TEXCOORD1;	
				float3 localPos: TEXCOORD2;
				half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
				// Macro found in Autolight.cginc. Declares a vector4
				// into the TEXCOORD2 semantic with varying precision 
				// depending on platform target.
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			sampler2D _BumpMap;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.localPos = v.vertex.xyz;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

				o.viewDir = WorldSpaceViewDir(v.vertex);
				//o.uv = o.worldPos.xz;
				// Defined in Autolight.cginc. Assigns the above shadow coordinate
				// by transforming the vertex from world space to shadow-map space.
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 _AmbientColor;

			float4 _SpecularColor;
			float _Glossiness;		

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;	
			float _Scale;
			float _XMult;

			float4 frag (v2f i) : SV_Target
			{
				//float sideMult = 1;
				//if (i.localPos.x > 0 && i.localPos.z < 0) {
				//	sideMult = -1;
				//}
				float2 texCoord = float2(i.worldPos.z + i.worldPos.x * _XMult /** sideMult*/, i.worldPos.y);
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, texCoord / _Scale));
				half3 normal;
				normal.x = dot(i.tspace0, tnormal);
				normal.y = dot(i.tspace1, tnormal);
				normal.z = dot(i.tspace2, tnormal);

				//float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				// Lighting below is calculated using Blinn-Phong,
				// with values thresholded to creat the "toon" look.
				// https://en.wikipedia.org/wiki/Blinn-Phong_shading_model

				// Calculate illumination from directional light.
				// _WorldSpaceLightPos0 is a vector pointing the OPPOSITE
				// direction of the main directional light.
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				// Samples the shadow map, returning a value in the 0...1 range,
				// where 0 is in the shadow, and 1 is not.
				float shadow = SHADOW_ATTENUATION(i);
				// Partition the intensity into light and dark, smoothly interpolated
				// between the two to avoid a jagged break.
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);	
				// Multiply by the main directional light's intensity and color.
				float4 light = lightIntensity * _LightColor0;

				// Calculate specular reflection.
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				// Multiply _Glossiness by itself to allow artist to use smaller
				// glossiness values in the inspector.
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;				

				// Calculate rim lighting.
				float rimDot = 1 - dot(viewDir, normal);
				// We only want rim to appear on the lit side of the surface,
				// so multiply it by NdotL, raised to a power to smoothly blend it.
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, texCoord / _Scale);

				return (light + _AmbientColor + specular + rim) * _Color * sample;
			}
			ENDCG
		}

		// Shadow casting support.
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}