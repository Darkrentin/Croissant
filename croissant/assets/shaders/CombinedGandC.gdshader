shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform vec2 r_displacement = vec2(-2.0, -2.0);
uniform vec2 g_displacement = vec2(0.0, 0.0);
uniform vec2 b_displacement = vec2(2.0, 2.0);

uniform float shake_power = 0; //0.03
uniform float shake_rate : hint_range(0.0, 1.0) = 0; //0.2
uniform float shake_speed = 0; //5.0
uniform float shake_block_size = 30.5; //30.5
uniform float shake_color_rate : hint_range(0.0, 1.0) = 0.01; //0.01

float random(float seed)
{
    return fract(543.2543 * sin(dot(vec2(seed, seed), vec2(3525.46, -54.3415))));
}

void fragment()
{
    float enable_shift = float(random(trunc(TIME * shake_speed)) < shake_rate);

    vec2 base_uv = SCREEN_UV;
    base_uv.x += (random((trunc(SCREEN_UV.y * shake_block_size) / shake_block_size) + TIME) - 0.5) * shake_power * enable_shift;

    float r = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * r_displacement), 0.0).r;
    float g = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * g_displacement), 0.0).g;
    float b = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * b_displacement), 0.0).b;

    float a_r = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * r_displacement), 0.0).a;
    float a_g = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * g_displacement), 0.0).a;
    float a_b = texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * b_displacement), 0.0).a;

    float final_alpha = max(max(a_r * r, a_g * g), a_b * b);

    if (enable_shift > 0.5) {
        r = mix(
            r,
            texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * r_displacement) + vec2(shake_color_rate, 0.0), 0.0).r,
            enable_shift
        );

        b = mix(b, texture(SCREEN_TEXTURE, base_uv + vec2(SCREEN_PIXEL_SIZE * b_displacement) + vec2(-shake_color_rate, 0.0), 0.0).b,enable_shift);
    }

    COLOR = vec4(r, g, b, final_alpha);
}