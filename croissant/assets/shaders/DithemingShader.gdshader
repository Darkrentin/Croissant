shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform vec4 col;
// 4x4 Bayer matrix for dithering
const float bayer[16] = float[](
    0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
   12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
    3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
   15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
);

void fragment() {

    // Get pixel position in screen space
    ivec2 pixel = ivec2(FRAGCOORD.xy);

    // 4x4 Bayer index
    int index = (pixel.x % 4) + (pixel.y % 4) * 4;
    float threshold = bayer[index];

    // Use the average of RGB as the dither reference (or use alpha if you want to dither transparency)
    float value = dot(col.rgb, vec3(0.3333));

    // Dither: keep color, but discard pixel if below threshold
    if (value < threshold) {
        discard;
    }

    COLOR = col;
}