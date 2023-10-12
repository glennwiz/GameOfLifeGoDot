shader_type canvas_item;

uniform float pop_out_amount : hint_range(0, 10) = 1.0;

void fragment() {
	 float tolerance = 0.1; // Adjust as needed to capture the range of 'yellow' you want
    if (abs(FRAGCOLOR.r - 1.0) < tolerance && 
        abs(FRAGCOLOR.g - 1.0) < tolerance && 
        abs(FRAGCOLOR.b - 0.0) < tolerance) {
        
        // Emboss effect
        vec2 offset = normalize(vec2(0.5, 0.5) - FRAGCOORD.xy) * pop_out_amount;
        FRAGCOORD.xy += offset;
    }
}
