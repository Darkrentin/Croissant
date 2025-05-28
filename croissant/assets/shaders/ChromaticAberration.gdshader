shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform vec2 r_displacement = vec2(-2.0, -2.0);
uniform vec2 g_displacement = vec2(0.0, 0.0);
uniform vec2 b_displacement = vec2(2.0, 2.0);

void fragment()
{
    vec2 pixel_offset = SCREEN_PIXEL_SIZE;
    vec2 base_uv = SCREEN_UV;

    vec2 r_uv = base_uv + pixel_offset * r_displacement;
    vec2 g_uv = base_uv + pixel_offset * g_displacement;
    vec2 b_uv = base_uv + pixel_offset * b_displacement;

    float r = texture(SCREEN_TEXTURE, r_uv, 0.0).r;
    float g = texture(SCREEN_TEXTURE, g_uv, 0.0).g;
    float b = texture(SCREEN_TEXTURE, b_uv, 0.0).b;

    vec3 alpha_samples = vec3(
        texture(SCREEN_TEXTURE, r_uv, 0.0).a,
        texture(SCREEN_TEXTURE, g_uv, 0.0).a,
        texture(SCREEN_TEXTURE, b_uv, 0.0).a
    );

    vec3 weighted_alpha = alpha_samples * vec3(r, g, b);
    float final_alpha = max(weighted_alpha.r, max(weighted_alpha.g, weighted_alpha.b));

    COLOR = vec4(r, g, b, final_alpha);
}
