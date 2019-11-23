Shader "Custom/BallTrailShader"
{
    Category{
        Tags { "RenderType"="Tranparent" "RenderType" = "Tranparent"}
        Blend SrcAlpha DstAlpha
        ZWrite Off
        ZTest Less

        SubShader{
            Pass{
                
            }
        }
    }
}
