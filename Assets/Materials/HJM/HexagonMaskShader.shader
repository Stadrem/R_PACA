Shader "Custom/HexagonMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 중심 이동 및 크기 조정
                float2 uv = (i.uv - 0.5) * 2.0;

                // 30도 회전을 적용하여 육각형의 방향을 바꿈
                float angle = radians(60.0); // 60도 회전 (각진 부분을 위로)
                float2 rotatedUV = float2(
                    uv.x * cos(angle) - uv.y * sin(angle),
                    uv.x * sin(angle) + uv.y * cos(angle)
                );

                // 육각형 마스크 계산 (크기 조정)
                float2 absUV = abs(rotatedUV);
                float mask = max(absUV.x * 0.5 + absUV.y * 0.8660254, absUV.x); // 0.8660254 = sqrt(3)/2

                // 경계값 조정 (1.0은 육각형 크기, 크기를 더 조정하려면 이 값을 수정)
                if (mask > 1.0) discard;

                // 텍스처 샘플링 및 색상 적용
                fixed4 col = tex2D(_MainTex, i.uv) * _Color; 
                return col;
            }
            ENDCG
        }
    }
}
