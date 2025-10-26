shader_type spatial;
render_mode unshaded, cull_back, blend_mix;

uniform vec3 outline_color = vec3(0.0, 1.0, 1.0);
uniform float outline_thickness = 0.005;
uniform float fill_alpha = 0.5;

void vertex() {
    // store expanded vertex for outline in a separate varying
    VERTEX += NORMAL * outline_thickness;
}

void fragment() {
    // faint transparent green fill
    ALBEDO = outline_color;
    ALPHA = fill_alpha;

    // optional pulsing alpha for “phantom” effect
    ALPHA *= 0.75 + 0.25 * sin(TIME * 2.0);
}
