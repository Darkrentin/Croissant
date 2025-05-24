shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0; // Transition progress (0 = start, 1 = end)
uniform vec4 color : source_color = vec4(0.0, 0.0, 0.0, 1.0); // ColorRect color
uniform vec2 center = vec2(0.5, 0.5); // Circle center (default at screen center)

void fragment() {
    vec2 uv = SCREEN_UV;
    vec2 centered_uv = uv - center;
    centered_uv.x *= SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
    float dist = length(centered_uv);
    float max_radius = length(vec2(0.5, 0.5 * SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x));
    float current_radius = progress * max_radius * 1.5;

    if (dist < current_radius)
    {
        COLOR = vec4(0.0, 0.0, 0.0, 0.0);
    }
    else
    {
        COLOR = color;
    }
}
