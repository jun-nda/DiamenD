Shader "Junnda/Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			CBUFFER_START(UnityPerMaterial)
			CBUFFER_END
			
			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				half4 color : COLOR;
			};

			struct v2f {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				float4 color0 : TEXCOORD1;
            };

			
            v2f vert(appdata v) {
				v2f vsout;
				vsout.pos = TransformObjectToHClip (v.vertex.xyz);
				vsout.uv0 = v.texcoord.xy;
				vsout.color0 = v.color;
                return vsout;
            }
			
			sampler2D _MainTex;
            CBUFFER_START(JUNNDA_UNLIT)
				float4 _Color;
            CBUFFER_END
            
            half4 frag(v2f psin) : SV_Target {
				half4 texcol = tex2D (_MainTex, psin.uv0);
            	half4 finalColor = texcol * _Color;
				finalColor.a = 1;
				return finalColor;
            }
            ENDHLSL
        }
    }
}
