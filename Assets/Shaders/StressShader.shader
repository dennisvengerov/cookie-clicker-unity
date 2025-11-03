Shader "Stress/ShaderComplexity"
{
    Properties
    {
        _MainTex ("Main Tex (small PNG)", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,0.15)
        _operation_count ("Operation Count", Int) = 100
        _texture_sample_count ("Texture Sample Count", Int) = 0
        _branch_factor ("Branch Factor", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _TintColor;
            int _operation_count;
            int _texture_sample_count;
            float _branch_factor;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float3 acc = 0;

                float3 tex_acc = 0;
                if (_texture_sample_count > 0)
                {
                    float2 base_uv = uv;
                    [loop]
                    for (int t = 0; t < _texture_sample_count; t++)
                    {
                        float angle = (t + 1) * 0.37;
                        float2 offs = float2(cos(angle), sin(angle)) * (0.002 * (t + 1));
                        tex_acc += tex2D(_MainTex, base_uv + offs).rgb;
                    }
                    tex_acc /= max(1, _texture_sample_count);
                }

                float3 v = float3(uv, 1.0);
                [loop]
                for (int k = 0; k < _operation_count; k++)
                {
                    v = normalize(v + float3(0.0003 * k, 0.0001 * k, 0.0002 * k));
                    float s = sin(dot(v.xy, float2(12.9898,78.233)) * 43758.5453);
                    float c = cos(dot(v.yz, float2(93.9898,12.233)) * 12345.6789);
                    v += float3(s, c, s * c);
                    v = frac(v * 1.1234);
                }
                acc = frac(v + tex_acc);

                if (_branch_factor > 0.0)
                {
                    float r = hash21(uv * 1024.0);
                    if (r > 0.5)
                    {
                        [unroll(4)]
                        for (int j = 0; j < 4; j++)
                        {
                            acc = frac(acc + float3(sin(r + j), cos(r - j), sin(r * j)));
                        }
                    }
                    else
                    {
                        float2 joff = float2(frac(r), frac(r * 1.7)) * 0.01;
                        acc += tex2D(_MainTex, uv + joff).rgb * 0.25;
                    }
                }

                return float4(acc, _TintColor.a) * float4(_TintColor.rgb, 1.0);
            }
            ENDCG
        }
    }
}