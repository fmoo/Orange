// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/**
 * Modified Tiled/TextureTintSnap shader that supports receiving shadows
 * from directional light sources.
 *
 * TODO: Things that would be cool to add later
 * - Support for spot/other types of lighting
 */
Shader "Orange/Tiled/TextureTintSnap"
{
    Properties
    {
        [PerRendererData] _MainTex ("Tiled Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _AlphaColorKey ("Alpha Color Key", Color) = (0,0,0,0)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 1

    }

    SubShader
    {
        Tags
        { 
            //"Queue"="Transparent" 
            //"IgnoreProjector"="True" 
            "RenderType"="Opaque" 
            //"PreviewType"="Plane"
            //"CanUseSpriteAtlas"="True"
        }

        //Cull Off
        //Lighting On
        //Fog { Mode Off }
        //Blend SrcAlpha OneMinusSrcAlpha

        //  Shadow rendering pass

        Pass
        {
			Name "FORWARD"
			Tags {
				"LightMode" = "ForwardBase"
			}

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase DUMMY
			#pragma multi_compile _ PIXELSNAP_ON


			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
                LIGHTING_COORDS(1, 2)
            };


            fixed4 _Color;

            v2f vert(appdata_full IN)
            {
                v2f OUT;
                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.pos = UnityPixelSnap (OUT.pos);
                #endif

                // Supports animations through z-component of tile
                if (IN.vertex.z < 0)
                {
                    // "Hide" frames of a tile animation that are not active
                    OUT.pos.w = 0;
                }
                else
                {
                    OUT.pos.z = 0;
                }

                TRANSFER_VERTEX_TO_FRAGMENT(OUT);
                return OUT;
            }

            sampler2D _MainTex;
            float4 _AlphaColorKey;

            fixed4 frag(v2f IN) : COLOR
            {
                half4 texcol = tex2D(_MainTex, IN.texcoord);

                // The alpha color key is 'enabled' if it has solid alpha
                if (_AlphaColorKey.a == 1 &&
                    _AlphaColorKey.r == texcol.r &&
                    _AlphaColorKey.g == texcol.g &&
                    _AlphaColorKey.b == texcol.b)
                {
                    texcol.a = 0;
                }
                else
                {
                    texcol = texcol * IN.color;
                }

                fixed atten = LIGHT_ATTENUATION(IN);
                texcol.rgb *= atten;

                UNITY_OPAQUE_ALPHA(texcol.a);
                return texcol;
            }
        ENDCG
        }


    }

    //Fallback "Sprites/Default"
    //Fallback "Standard"
    //FallBack "VertexLit"
    FallBack "Mobile/VertexLit"
}
