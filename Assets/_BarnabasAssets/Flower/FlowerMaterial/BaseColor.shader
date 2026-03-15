Shader "Custom/ColorLerpTransparent_VR"
{
    Properties
    {
        _ColorA("Color A", Color) = (1, 0, 0, 0.5)
        _ColorB("Color B", Color) = (0, 0, 1, 0.5)
        _LerpFactor("Lerp Factor", Range(0, 1)) = 0.5
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 1. Tell Unity to generate variants for VR instancing
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                // 2. Add Instance ID to input
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                // 3. Add Stereo Output to vertex-to-fragment struct
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _ColorA;
            fixed4 _ColorB;
            float _LerpFactor;

            v2f vert(appdata v)
            {
                v2f o;
                // 4. Initialize Stereo IDs
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 5. Tell the fragment shader which eye index we are currently on
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                return lerp(_ColorA, _ColorB, _LerpFactor);
            }
            ENDCG
        }
    }
        FallBack "Transparent/Diffuse"
}