shader_type spatial;

// Glitch intensity
uniform float shake_power = 0.5;

// Probability
uniform float shake_rate : hint_range(0.0, 1.0) = 0.5;

uniform float shake_speed = 5.0;

// Hard to describe, change it and monitor result
uniform float shake_block_size = 30.5;

uniform float shake_color_rate : hint_range(0.0, 1.0) = 0.5;

uniform sampler2D main_tex : filter_nearest, source_color;

varying float enable_shift;

float random(float seed) {
    return fract(sin(seed * 12345.678) * 43758.5453);
}

void vertex() {
	float adjusted_time = mod(TIME, 5.0);

    enable_shift = float(random(trunc(adjusted_time * shake_speed)) < shake_rate);

    float offset_x = (random((trunc(VERTEX.y * shake_block_size) / shake_block_size) + adjusted_time) - 0.5) * shake_power * enable_shift;
    VERTEX.x += offset_x;
}

void fragment() {
	float adjusted_time = mod(TIME, 5.0);
    vec2 fixed_uv = UV;
    fixed_uv.x += (
        random((trunc(UV.y * shake_block_size) / shake_block_size) + adjusted_time) - 0.5
    ) * shake_power * enable_shift;

    vec4 pixel_color = texture(main_tex, fixed_uv);

    pixel_color.r = mix(
        pixel_color.r,
        texture(main_tex, fixed_uv + vec2(shake_color_rate, 0.0)).r,
        enable_shift
    );
    pixel_color.b = mix(
        pixel_color.b,
        texture(main_tex, fixed_uv + vec2(-shake_color_rate, 0.0)).b,
        enable_shift
    );

    ALBEDO = pixel_color.rgb;
}
