shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float scale : hint_range(1.0, 8.0, 1.0) = 1.0;
uniform vec4 col : source_color;
uniform float mult = 1.0;

const float bayer[16] = float[]
(
    0.0, 0.5, 0.125, 0.625,
    0.75, 0.25, 0.875, 0.375,
    0.1875, 0.6875, 0.0625, 0.5625,
    0.9375, 0.4375, 0.8125, 0.3125
);

void fragment()
{
    vec2 scaled_coords = FRAGCOORD.xy / scale;
    ivec2 coords = ivec2(scaled_coords);
    int index = (coords.x & 3) + ((coords.y & 3) << 2);
    float threshold = bayer[index] * mult;

    float value = dot(col.rgb, vec3(0.333333));

    if (value < threshold) {discard;}

    COLOR = col;
}
