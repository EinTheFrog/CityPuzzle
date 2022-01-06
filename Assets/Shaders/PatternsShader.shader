Shader "Custom/PatternsShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) =  (0, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _ColorBack ("Background color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.14159265359;

            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _ColorBack;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const fixed scale = 8;
                const fixed w = 0.1;
                fixed t = _Time;
                fixed t1 = (clamp(cos(t * 20), 0, 1) - 0.2) * w;
                fixed pi = PI;
                fixed t2 = (clamp(cos(t * 20 + pi), 0, 1) - 0.2) * w;
                fixed t3 = cos(t * 20) * w;
                fixed t4 = cos(t * 20 + pi) * w;
                
                fixed x = frac(i.uv.x * scale);
                fixed y = frac(i.uv.y * scale);
                
                fixed f1 = abs(y - 0.5) + w;
                fixed f2 = -f1 + 1;
                
                bool line1_1 = abs(x - f1) < 0.5 * w && y > 0.35 * w - t4/1.5 && y < 9.65 * w + t4/1.5;
                bool line1_2 = abs(x - f2) < 0.5 * w && y > 0.35 * w - t4/1.5 && y < 9.65 * w + t4/1.5;
                bool line1 = line1_1 || line1_2;

                bool line2_1 = abs(x - f1) > 1.5 * w;
                bool line2_2 = abs(x - f2) > 1.5 * w;
                bool line2 = line2_1 && line2_2;

                bool line3_1 = abs(x - f1) < 2.5 * w && x < 4.8 * w + t1;
                bool line3_2 = abs(x - f2) < 2.5 * w && x > 5.2 * w - t1;
                bool line3 = line3_1 || line3_2;

                bool line4_1 = abs(x - f1) < 2.5 * w && (y < 4.8 * w + t2 || y > 5.2 * w - t2);
                bool line4_2 = abs(x - f2) < 2.5 * w && (y < 4.8 * w + t2 || y > 5.2 * w - t2);
                bool line4 = line4_1 || line4_2;

                bool line5_1 = (abs(x - f1) > 3.5 * w);
                bool line5_2 = (abs(x - f2) > 3.5 * w);
                bool line5 = line5_1 && line5_2;

                bool line6_1 = abs(x - f1) < 4.5 * w && x > 0.5 * w - t3 && x < 9.5 * w + t3;
                bool line6_2 = abs(x - f2) < 4.5 * w && x > 0.5 * w - t3 && x < 9.5 * w + t3;
                bool line6 = line6_1 || line6_2;

                fixed4 color = fixed4(0, 0, 0, 1);
                if (line1) color = _Color1;
                else if (line2 && line3 && line4) color = _Color2;
                else if (line5 && line6) color = _Color1;
                else color = _ColorBack;
                return color;
            }
            ENDCG
        }
    }
}
