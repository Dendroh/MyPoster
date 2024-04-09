Shader "Custom/StencilWrite" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        LOD 200
 
        ZWrite On
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }
        ColorMask 0
     
        Pass{}
    }
}