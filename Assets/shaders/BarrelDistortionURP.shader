Shader "Hidden/BarrelDistortionURP"
{
    Properties
    {
        _Strength ("Distortion Strength", Float) = 0.25
        _Zoom ("Zoom", Float) = 1.05
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _K1 ("K1", Float) = 1.0
        _K2 ("K2", Float) = 0.0
        _K3 ("K3", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "BarrelDistortionPass"
            Tags { "LightMode" = "UniversalRenderPipeline" }
            ZTest Always ZWrite Off Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _Strength;
            float _Zoom;
            float2 _Center;
            float _K1, _K2, _K3;
            float4 _BlitTexture_TexelSize;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_Position;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float2 DistortUV(float2 uv, float aspect)
            {
                float2 p = uv - _Center;
                p.x *= aspect;

                float r2 = dot(p, p);
                float r4 = r2 * r2;
                float r6 = r4 * r2;

                float radial = 1.0 + _Strength * (_K1 * r2 + _K2 * r4 + _K3 * r6);
                float2 distorted = p * radial;

                distorted.x /= aspect;
                distorted *= _Zoom;

                return distorted + _Center;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float aspect = _BlitTexture_TexelSize.z / _BlitTexture_TexelSize.w;
                float2 duv = DistortUV(IN.uv, aspect);
                duv = clamp(duv, float2(0.001, 0.001), float2(0.999, 0.999));
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, duv);
            }
            ENDHLSL
        }
    }
}
