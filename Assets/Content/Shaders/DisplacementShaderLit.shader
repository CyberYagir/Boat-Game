Shader "Custom/DisplacementShaderLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,0,0,1)
        _Power ("Power", Float) = 1
        _Offset ("Offset", Float) = 1
    }
    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True" 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }


            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcord : TEXCOORD0;
            }; 

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            // v2f vert (appdata v)
            // {
            //     v2f o;
            //     o.vertex = TransformObjectToHClip(v.vertex);
            //     o.worldPos = TransformObjectToWorld(v.vertex);
            //     o.normal = v.normal;
            //     return o;
            // }

            float4 _Color;
            float _Power;
            float _Offset;
            
            sampler2D _MainTex;

            float hash( float n )
			{
			    return frac(sin(n)*43758.5453);
			}

			float noise( float3 x )
			{
			    // The noise function returns a value in the range -1.0f -> 1.0f

			    float3 p = floor(x);
			    float3 f = frac(x);

			    f       = f*f*(3.0-2.0*f);
			    float n = p.x + p.y*57.0 + 113.0*p.z;

			    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
			                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
			               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
			                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
			}

            
            float3 Lambert(float3 lightColor, float3 lightDir, float3 normal)
            {
                float NdotL = saturate(dot(normal, lightDir));
                return lightColor * NdotL;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = _Color;

                float3 lightPos = _MainLightPosition.xyz;
                float3 lightCol = Lambert(_MainLightColor * unity_LightData.z, lightPos, i.normal);

                uint lightsCount = GetAdditionalLightsCount();
                for (int j = 0; j < lightsCount; j++)
                {
                    Light light = GetAdditionalLight(j, i.worldPos);
                    lightCol += Lambert(light.color * (light.distanceAttenuation * light.shadowAttenuation), light.direction, i.normal);
                }

                color.rgb += lightCol;
                return color * _Color;
            }

             void vert (inout appdata v) {

                float3 worldPos = TransformObjectToWorld(v.vertex);
                worldPos.y += (noise(worldPos) * _Power) + _Offset;
                float3 objectPos = TransformWorldToObject(worldPos);
                
                v.vertex = TransformObjectToHClip(objectPos);
                v.normal = float3(0,worldPos.y,0);
             }
            
            ENDHLSL
        }
    }
}
