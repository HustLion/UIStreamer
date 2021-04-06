Shader "Hidden/UI/Default (StreamerWithRamp)"
{
    Properties
    {
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        _Color("Vertex Color from Graphics component", Color) = (1, 1, 1, 1)
		
		_StreamerColor("Streamer Color", Color) = (1, 0, 0, 1)
		_MoveSpeed("Speed", Float) = 1
		_Power("Power", Float) = 1
//		[HideInInspector]_Progress("Progress", Float) = 0
		_Progress("Progress", Range(0.0, 1.0)) = 0
		[HideInInspector]_FromPosition("From Position", Vector) = (0, 0, 0, 0)
		[HideInInspector]_ToPosition("To Position", Vector) = (0, 0, 0, 0)
        [HideInInspector]_ScreenHeight("Screen Height", Float) = 0 // Platform specific due to DX/OpenGL differences
		_StreamerTexture("Streamer Texture", 2D) = "White" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                o.color = v.color;
                
                #ifdef UNITY_HALF_TEXEL_OFFSET
                o.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
                #endif
                
                o.worldPosition = v.vertex;
                
                return o;
            }
            
            float _Power;
            float _MoveSpeed;
            float _Progress;
            float _ScreenHeight;
            fixed4 _StreamerColor;
            fixed4 _FromPosition;
            fixed4 _ToPosition;
            sampler2D _StreamerTexture;
            
            float4 _ClipRect;
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 color = texColor * i.color; // apply vertex color
                #ifdef _CANVAS_GROUP_COMPATIBLE
                color.rgb *= i.color.a;
                #endif
                
                color *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                float2 dir = _ToPosition.xy - _FromPosition.xy;
                float2 screenPos = i.vertex.xy;
                #ifdef UNITY_UV_STARTS_AT_TOP
                if (_ScreenHeight > 0.0) { // screenHeight set only for Overlay Canvas
                    screenPos.y = _ScreenHeight - screenPos.y;
                }
                #endif
                float d = clamp(dot(dir, screenPos.xy - _FromPosition.xy) / pow(length(dir), 2.0), 0.0, 1.0);
                float t = frac(_Progress * _MoveSpeed);
                d += t;
                if (d > 1.0) d -= 1.0;
                if (d < 0.0) d += 1.0;
                
                fixed4 ramp = tex2D(_StreamerTexture, float2(d, 0.5));
                fixed4 cAdd = _StreamerColor * ramp.r * _Power;
                color.rgb += cAdd.rgb * cAdd;
                
                // this causes compile error when building towards WebGL (gles, gles3)
                // #ifdef UNITY_UI_ALPHACLIP
                // clip(color.a - 0.001)
                // #endif
                
                return color;
            }
            ENDCG
        }
    }
}