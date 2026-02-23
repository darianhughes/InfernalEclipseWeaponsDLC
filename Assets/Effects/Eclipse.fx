sampler uImage0 : register(s0); // Contents of the screen.
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime; // Should be set to Main.GlobalTime or a fraction of incrementing time for shine effect.
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float2 uPixel;

float4 Eclipse(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float2 center = (0.5f, 0.5f);
    float dist = (distance(center, coords));
    
    if (color.a != 0)
        color.rgb /= color.a;
    
    if (dist < 0.3f)
        return float4(0, 0, 0, 1.0f);
    
    if (dist < 0.35f)
        color.rgb *= (dist - 0.35f) * 20;
    
    if (dist < 0.5f)
        color = float4(1, 0.27f, 0, (0.5f - dist) / 5) * 5;

    color.rgb *= color.a;
    color.a = 0;
    
    return color;
}

technique Technique1
{
    pass Eclipse
    {
        PixelShader = compile ps_2_0 Eclipse();
    }
}