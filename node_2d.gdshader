shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    vec2 offset = vec2(-0.0125, 0.0167); // bottom-right by 10px in UV space
    vec3 future_color = textureLod(screen_texture, SCREEN_UV + offset, 0.0).rgb;
    vec3 current_color = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;
    
    vec3 final_color = current_color;
    
    // Check if the future pixel is yellow or close to yellow
    if (abs(future_color.r - 1.0) < 0.1 && abs(future_color.g - 1.0) < 0.1 && abs(future_color.b - 0.0) < 0.1) {
        final_color = future_color;
    }
    
    // Check for edges and apply bevel effect
    vec2 small_offset = vec2(0.0025, 0.0033); // small offset for edge detection
    vec3 bottom_right = textureLod(screen_texture, SCREEN_UV + small_offset, 0.0).rgb;
    vec3 top_left = textureLod(screen_texture, SCREEN_UV - small_offset, 0.0).rgb;

    if (abs(final_color.r - 1.0) < 0.1 && abs(final_color.g - 1.0) < 0.1 && abs(final_color.b - 0.0) < 0.1) {
        if (abs(bottom_right.r - 1.0) > 0.1 || abs(bottom_right.g - 1.0) > 0.1 || abs(bottom_right.b - 0.0) > 0.1) {
            final_color *= 0.8; // Darken
        }
        if (abs(top_left.r - 1.0) > 0.1 || abs(top_left.g - 1.0) > 0.1 || abs(top_left.b - 0.0) > 0.1) {
            final_color *= 1.2; // Lighten
        }
    }
    
    COLOR.rgb = final_color;
}