float4 blend_overlay(float4 base, float4 blend)
{
    return (base.a < 0.5) ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend));
}