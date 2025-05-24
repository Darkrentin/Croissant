shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform vec2 r_displacement = vec2(-3.0, -3.0);
uniform vec2 g_displacement = vec2(0.0, 0.0);
uniform vec2 b_displacement = vec2(3.0, 3.0);

uniform float shake_power = 0; //0.03
uniform float shake_rate : hint_range(0.0, 1.0) = 0; //0.2
uniform float shake_speed = 0; //5.0
uniform float shake_block_size = 30.5; //30.5
uniform float shake_color_rate : hint_range(0.0, 1.0) = 0.01; //0.01

float random(float seed)
{
    return fract(543.2543 * sin(dot(vec2(seed), vec2(3525.46, -54.3415))));
}

void fragment()
{
    float time_seed = trunc(TIME * shake_speed);
    float enable_shift = float(random(time_seed) < shake_rate);
    
    vec2 pixel_offset = SCREEN_PIXEL_SIZE;
    vec2 base_uv = SCREEN_UV;
    base_uv.x += (random((trunc(base_uv.y * shake_block_size) / shake_block_size) + TIME) - 0.5) * shake_power * enable_shift;

    vec2 r_uv = base_uv + pixel_offset * r_displacement;
    vec2 g_uv = base_uv + pixel_offset * g_displacement;
    vec2 b_uv = base_uv + pixel_offset * b_displacement;
    
    vec4 r_sample = texture(SCREEN_TEXTURE, r_uv);
    vec4 g_sample = texture(SCREEN_TEXTURE, g_uv);
    vec4 b_sample = texture(SCREEN_TEXTURE, b_uv);
    
    vec3 rgb = vec3(r_sample.r, g_sample.g, b_sample.b);
    vec3 alpha_weights = vec3(r_sample.a * rgb.r, g_sample.a * rgb.g, b_sample.a * rgb.b);
    float final_alpha = max(alpha_weights.r, max(alpha_weights.g, alpha_weights.b));

    if (enable_shift > 0.5)
    {
        vec2 shake_offset = vec2(shake_color_rate, 0.0);
        rgb.r = mix(rgb.r, texture(SCREEN_TEXTURE, r_uv + shake_offset).r, enable_shift);
        rgb.b = mix(rgb.b, texture(SCREEN_TEXTURE, b_uv - shake_offset).b, enable_shift);
    }

    COLOR = vec4(rgb, final_alpha);
}
