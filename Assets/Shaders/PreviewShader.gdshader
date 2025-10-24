shader_type spatial;

uniform vec3 validRenderColor = vec3(0.0, 1.0, 0.0);
uniform float transparencyAmplitude = 0.25;
uniform float transparencyOffset = 0.75;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
render_mode unshaded;

void fragment() {
    // Use screen UVs instead of mesh UVs
    vec2 fragCoord = SCREEN_UV;
    vec2 center = vec2(0.5, 0.5);

    // Sample background color
    vec3 bgColor = texture(SCREEN_TEXTURE, fragCoord).rgb;

    // Distance from screen center [0..1]
    float distToCenter = length(fragCoord - center);

    // Smooth darkening effect based on distance
    float darkeningFactor = 1.0 - smoothstep(0.0, 0.5, distToCenter);

    // Alpha animation
    float alphaBase = (abs(sin(TIME)) + transparencyOffset) * transparencyAmplitude;
    ALPHA = clamp(alphaBase, 0.0, 1.0);

    // Final blended color
    vec3 currentColor = mix(bgColor, validRenderColor, darkeningFactor);
    ALBEDO = currentColor;
}
