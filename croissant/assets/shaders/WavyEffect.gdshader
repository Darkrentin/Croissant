shader_type canvas_item;

// Control parameters for the wave effect
uniform float speed : hint_range(0.1, 5.0) = 1.5;
uniform float strength : hint_range(0.001, 0.1) = 0.03; 
uniform float noise_scale : hint_range(1.0, 20.0) = 12.0;

// Returns a value between -1.0 and 1.0
float get_noise(vec3 p) {
    return fract(sin(dot(p, vec3(12.9898, 78.233, 45.164))) * 43758.5453) * 2.0 - 1.0;
}

void fragment() {
    // Original UV coordinates
    vec2 uv = UV;
    
    // Create time-dependent noise using custom noise function
    float time_factor = TIME * speed;
    
    float noise_x = get_noise(vec3(uv.x * noise_scale, uv.y * noise_scale, time_factor));
    float noise_y = get_noise(vec3(uv.x * noise_scale + 100.0, uv.y * noise_scale + 100.0, time_factor + 10.0));
    
    // Apply the noise displacement to UVs
    vec2 distorted_uv = uv;
    distorted_uv.x += strength * noise_x;
    distorted_uv.y += strength * noise_y;
    
    // Sample the texture with the distorted coordinates
    COLOR = texture(TEXTURE, distorted_uv);
}