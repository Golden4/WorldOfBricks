// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GradientShader"
{
Properties
 {
     _TopColor ("Top Color", Color) = (1, 1, 1, 1)
     _BottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
     _Color("Color", Color)= (1, 1, 1, 1)
     _FillAlpha ("FillAlpha", Range(0, 1)) = 0
     [PerRendererData] _MainTex ("Base (RGB)", 2D) = "white" {}
 }
 
 SubShader
 {

     Pass
     {
          Tags { "IgnoreProjector"="True" "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100

        Blend One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

         Blend SrcAlpha OneMinusSrcAlpha
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag

         struct vertexIn {
             float4 pos : POSITION;
             float2 uv : TEXCOORD0;
             float4 vertexColor : COLOR;
         };
         
         struct v2f {
             float4 pos : SV_POSITION;
             float2 uv : TEXCOORD0;
             float4 vertexColor : COLOR;
         };
         

         fixed4 _TopColor, _BottomColor, _Color;
         float _FillAlpha;
         sampler2D _MainTex;

         v2f vert(vertexIn input)
         {
             v2f output;
             output.pos = UnityObjectToClipPos(input.pos);
             output.uv = input.uv;
             output.vertexColor = input.vertexColor;
             return output;
         }

         fixed4 frag(v2f i) : COLOR
         {
                float4 gradColor = lerp(_BottomColor, _TopColor, abs((i.uv.x + 1 - i.uv.y)/2));
                float4 rawColor = tex2D(_MainTex,i.uv);
                float finalAlpha = (rawColor.a * i.vertexColor.a * _Color.a);
                float3 finalColor = lerp((_Color.rgb * rawColor.rgb * finalAlpha), (gradColor.rgb * finalAlpha), _FillAlpha);
	            return fixed4(finalColor, finalAlpha);
         }
         ENDCG
     }
     
 }
 
    FallBack "Diffuse"
}