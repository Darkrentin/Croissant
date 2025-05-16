shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float scale : hint_range(1.0, 8.0, 1.0) = 1.0;
uniform vec4 col : source_color;
uniform float mult = 1.0;

const float bayer[16] = float[]
(
    0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
   12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
    3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
   15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
);

void fragment()
{
    ivec2 coords = ivec2(floor(FRAGCOORD.xy / scale));
    int index = (coords.x % 4) + (coords.y % 4) * 4;
    float threshold = bayer[index];

    float value = dot(col.rgb, vec3(0.3333));

    if (value < threshold*mult) {discard;}

    COLOR = col;
}
