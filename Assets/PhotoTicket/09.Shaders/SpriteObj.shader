// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SpriteObj" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _RefNumber("Reference number", Int) = 1
    }
    SubShader {
        Tags {"ForceNoShadowCasting"="True" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
 
        Stencil {
            Ref [_RefNumber]
            Comp equal
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
             Pass {  
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fog
             
             #include "UnityCG.cginc"
 
             struct appdata_t {
                 float4 vertex : POSITION;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 half2 texcoord : TEXCOORD0;
                 UNITY_FOG_COORDS(1)
             };
 
             sampler2D _MainTex;
             float4 _MainTex_ST;
             
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }
             
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 col = tex2D(_MainTex, i.texcoord);
                 UNITY_APPLY_FOG(i.fogCoord, col);
                 return col;
             }
         ENDCG
     }
    }
    FallBack "Diffuse"
}


