shader_type canvas_item;

uniform sampler3D noise_tex : hint_default_white;
uniform vec3 anim_vel;
uniform vec4 top_val : source_color;
uniform vec4 mid_val : source_color;
uniform vec4 low_val : source_color;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec3 modded_uv;
	modded_uv.xy = mod(UV + anim_vel.xy * TIME, 1.0);
	modded_uv.z = mod(anim_vel.z * TIME, 1.0);
	//modded_uv = vec2(modded_uv., modded_uv.y % 1.0);
	
	
	vec3 noise_col = texture(noise_tex, modded_uv).rgb;
	
	float oldAlpha = COLOR.a;
	
	COLOR = 
	noise_col.r * top_val 
	+ noise_col.g * mid_val 
	+ noise_col.b * low_val;
	
	//fix alpha again
	COLOR.a *= oldAlpha; 
	
	//COLOR.rgb = noise_col;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
