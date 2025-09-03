shader_type spatial;

uniform vec3 validRenderColor = vec3(0.0, 1.0, 0.0);
uniform vec3 invalidRenderColor = vec3(1.0, 0.0, 0.0);
uniform bool isValid = true;
uniform float transparencyAmplitude = 0.25;
uniform float transparencyOffset = 0.125;
render_mode unshaded;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;

void vertex() {

}

void fragment() {
	// Calculate the distance from the center of the screen
    vec2 center = vec2(0.5, 0.5); // Center of the screen in normalized space
    vec2 fragCoord = UV;
	vec3 bgColor = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;

	float distToCenter = length(fragCoord - center);
	float darkeningFactor = max(0.0, 1.0 - distToCenter * sin(TIME));

	ROUGHNESS = 1.0;
	ALPHA = (abs(sin(TIME)) + transparencyOffset) * transparencyAmplitude;
	vec3 currentColor;
	if(isValid){
		currentColor = validRenderColor;
	}
	else{
		currentColor = invalidRenderColor;
	}
	ALBEDO = currentColor - bgColor;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
