// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/CubeBlend"
{
    Properties
    {
        [NoScaleOffset] _TexA("Cubemap", Cube) = "grey" {}
        [NoScaleOffset] _TexB("Cubemap", Cube) = "grey" {}
        [NoScaleOffset] _TexC("Cubemap", Cube) = "grey" {}
        _value("Value", Range(0, 2)) = 0.5
        _Rotation("Rotation", Range(0, 360)) = 0
    }

        CGINCLUDE

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

        half4 _TexA_HDR;
        half4 _TexB_HDR;
        half4 _TexC_HDR;

        UNITY_DECLARE_TEXCUBE(_TexA);
        UNITY_DECLARE_TEXCUBE(_TexB);
        UNITY_DECLARE_TEXCUBE(_TexC);

        float _Level;
        float _value;
        float _Rotation;

        float3 RotateAroundYInDegrees(float3 vertex, float degrees)
        {
            float alpha = degrees * UNITY_PI / 180.0;
            float sina, cosa;
            sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, -sina, sina, cosa);
            return float3(mul(m, vertex.xz), vertex.y).xzy;
        }

        struct appdata_t {
            float4 vertex : POSITION;
            float3 texcoord : TEXCOORD0;            
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            //o.vertex = UnityObjectToClipPos(v.vertex);
            //o.texcoord = v.texcoord;
            float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
            o.vertex = UnityObjectToClipPos(rotated);
            o.texcoord = v.vertex.xyz;
            return o;
        }

        half4 frag(v2f i) : SV_Target
        {
            half3 texA = DecodeHDR(UNITY_SAMPLE_TEXCUBE_LOD(_TexA, i.texcoord, _Level), _TexA_HDR);
            half3 texB = DecodeHDR(UNITY_SAMPLE_TEXCUBE_LOD(_TexB, i.texcoord, _Level), _TexB_HDR);
            half3 texC = DecodeHDR(UNITY_SAMPLE_TEXCUBE_LOD(_TexC, i.texcoord, _Level), _TexC_HDR);

            half3 res;
            if (_value<=1){
                res = lerp(texA, texB, _value);
            }else{
                res = lerp(texB, texC, _value-1);
            }
            return half4(res, 1.0);
        }
            ENDCG

            SubShader {
            Tags{ "Queue" = "Background" "RenderType" = "Background" }                
                Pass
            {
                CGPROGRAM
                #pragma target 3.0
                ENDCG
            }
        }

        SubShader {
            Tags{ "Queue" = "Background" "RenderType" = "Background" }                

                Pass
            {
                CGPROGRAM
                #pragma target 2.0
                ENDCG
            }
        }

        Fallback Off
}