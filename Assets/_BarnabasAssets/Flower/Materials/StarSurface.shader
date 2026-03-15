Shader "Custom/BlobStarSurface_VR_Fixed"
{
    Properties
    {
        [Header(Fresnel Base)]
        _FresnelColor("Fresnel Color", Color) = (0, 0.8, 1, 1)
        _FresnelPower("Fresnel Power", Range(0.5, 5.0)) = 2.0

        [Header(Sphere Settings)]
        _Density("Density", Range(1, 50)) = 10
        _MinSize("Min Size", Range(0, 0.5)) = 0.1
        _MaxSize("Max Size", Range(0, 0.5)) = 0.4
        _Smoothness("Sphere Softness", Range(0.01, 0.5)) = 0.2
        _Distortion("Blob Distortion", Range(0, 1)) = 0.2
        _FadeSpeed("Fade Speed", Range(0.1, 5)) = 1.0

        [Header(Targeting System)]
        [Toggle] _UseTarget("Concentrate at Target", Float) = 0
        _TargetRadius("Target Area Radius", Range(0.1, 20)) = 5.0
        _TargetPos("Target Position", Vector) = (0,0,0,0)

        [Header(Colors)]
        _Col1("Color 1", Color) = (1, 0.5, 0.5, 1)
        _Col2("Color 2", Color) = (0.5, 1, 0.5, 1)
        _Col3("Color 3", Color) = (0.5, 0.5, 1, 1)
        _Col4("Color 4", Color) = (1, 1, 0.5, 1)

        [Header(Primary Texture)]
        _MainTex("Primary Texture (Alpha)", 2D) = "white" {}
        _TexOpacity("Texture Opacity", Range(0, 1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing // Vital for VR
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID // VR Fix 1
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                    float3 worldNormal : TEXCOORD1;
                    float3 viewDir : TEXCOORD4;
                    float2 uv : TEXCOORD3;
                    UNITY_VERTEX_OUTPUT_STEREO // VR Fix 2
                };

                sampler2D _MainTex;
                float4 _FresnelColor, _Col1, _Col2, _Col3, _Col4;
                float _FresnelPower, _Density, _MinSize, _MaxSize, _Smoothness, _FadeSpeed, _TexOpacity;
                float _Distortion, _UseTarget, _TargetRadius;
                float3 _TargetPos;

                float hash(float n) { return frac(sin(n) * 43758.5453123); }

                v2f vert(appdata v) {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v); // VR Fix 3
                    UNITY_INITIALIZE_OUTPUT(v2f, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // VR Fix 4

                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);

                    // Use the Unity macro for view direction to ensure eye-parity in VR
                    o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); // VR Fix 5

                    // 1. Fresnel
                    float fresnel = pow(1.0 - saturate(dot(normalize(i.worldNormal), i.viewDir)), _FresnelPower);
                    float4 base = _FresnelColor;
                    base.a *= fresnel;

                    // 2. Blobby UV Distortion
                    float2 noiseUV = i.uv * 5.0;
                    float2 distort = float2(
                        sin(noiseUV.y + _Time.y * _FadeSpeed),
                        cos(noiseUV.x + _Time.y * _FadeSpeed)
                    ) * _Distortion;

                    float2 gUV = (i.uv + distort) * _Density;
                    float2 gID = floor(gUV);
                    float2 fUV = frac(gUV);

                    // 3. Targeting Mask
                    float targetMask = 1.0;
                    if (_UseTarget > 0.5) {
                        float d = distance(i.worldPos, _TargetPos);
                        targetMask = saturate(1.0 - (d / _TargetRadius));
                    }

                    float4 sphereLayer = float4(0,0,0,0);

                    for (int y = -1; y <= 1; y++) {
                        for (int x = -1; x <= 1; x++) {
                            float2 offset = float2(x, y);
                            float2 id = gID + offset;
                            float randID = hash(id.x * 12.9898 + id.y * 78.233);

                            float2 pos = 0.5 + 0.3 * sin(_Time.y * 0.5 + randID * 6.28);
                            float dist = distance(fUV, offset + pos);

                            float pulse = (sin(_Time.y * _FadeSpeed + randID * 10.0) * 0.5 + 0.5);
                            pulse *= targetMask;

                            float size = lerp(_MinSize, _MaxSize, hash(randID));
                            float mask = smoothstep(size + _Smoothness, size - _Smoothness, dist) * pulse;

                            float4 col = _Col1;
                            if (randID > 0.75) col = _Col4;
                            else if (randID > 0.5) col = _Col3;
                            else if (randID > 0.25) col = _Col2;

                            sphereLayer += col * mask;
                        }
                    }

                    float4 finalCol = base + sphereLayer;
                    float4 tex = tex2D(_MainTex, i.uv);
                    float finalTexAlpha = tex.a * _TexOpacity;
                    finalCol = lerp(finalCol, tex, finalTexAlpha);

                    return saturate(finalCol);
                }
                ENDCG
            }
        }
}