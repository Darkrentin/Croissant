shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float amplitude : hint_range(0.0, 0.1) = 0.02;
uniform float frequency : hint_range(0.0, 20.0) = 10.0; 
uniform float speed : hint_range(0.0, 5.0) = 1.0;
uniform float time_offset : hint_range(0.0, 10.0) = 0.0;
uniform float random_strength : hint_range(0.0, 1.0) = 0.3;
uniform vec2 random_seed = vec2(12.9898, 78.233);

float random(vec2 uv)
{
    return fract(sin(dot(uv, random_seed)) * 43758.5453123);
}

void fragment()
{
    vec2 uv = SCREEN_UV;
    float rand_offset_x = random(uv + vec2(0.1, 0.3)) * random_strength;
    float rand_offset_y = random(uv + vec2(0.7, 0.9)) * random_strength;
    float wave_x = sin((uv.y * frequency + TIME * speed + time_offset + rand_offset_x)) * amplitude;
    float wave_y = cos((uv.x * frequency + TIME * speed + time_offset + rand_offset_y)) * amplitude;
    vec2 distorted_uv = uv + vec2(wave_x, wave_y);
    COLOR = texture(SCREEN_TEXTURE, distorted_uv);
}