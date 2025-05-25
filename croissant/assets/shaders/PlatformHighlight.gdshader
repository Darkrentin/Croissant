shader_type canvas_item;

uniform vec4 color1: source_color = vec4(1.0, 0.0, 0.0, 1.0);  // First color
uniform vec4 color2: source_color = vec4(0.0, 0.0, 1.0, 1.0);  // Second color
uniform bool animate = true;  // Whether to animate the outline
uniform float direction: hint_range(-1.0, 1.0, 1.0) = -1.0;
uniform float frequency: hint_range(0.1, 100.0, 0.1) = 15.0;
uniform float speed: hint_range(0.1, 10, 0.1) = 3.0;
uniform float border_width = 10.0; // Border width in pixels
uniform vec2 window_size = vec2(800.0, 600.0); // Window size in pixels
uniform float Time = 0.0;

void fragment() {
    // Get UV-based position in pixels
    vec2 pixel_position = UV * window_size;

    // Calculate distance from each edge in pixels
    float distance_from_left = pixel_position.x;
    float distance_from_right = window_size.x - pixel_position.x;
    float distance_from_top = pixel_position.y;
    float distance_from_bottom = window_size.y - pixel_position.y;

    // Check if we're in the border area
    bool is_border =
        distance_from_left < border_width ||
        distance_from_right < border_width ||
        distance_from_top < border_width ||
        distance_from_bottom < border_width;

    // Calculate parameterized position along the perimeter (0.0 to 1.0)
    float perimeter = 2.0 * (window_size.x + window_size.y);
    float position_on_perimeter = 0.0;

    if (distance_from_top < border_width) {
        // Top border
        position_on_perimeter = pixel_position.x / perimeter;
    } else if (distance_from_right < border_width) {
        // Right border
        position_on_perimeter = (window_size.x + pixel_position.y) / perimeter;
    } else if (distance_from_bottom < border_width) {
        // Bottom border
        position_on_perimeter = (window_size.x + window_size.y + (window_size.x - pixel_position.x)) / perimeter;
    } else if (distance_from_left < border_width) {
        // Left border
        position_on_perimeter = (2.0 * window_size.x + window_size.y + (window_size.y - pixel_position.y)) / perimeter;
    }

    // Use Time directly - the speed is integrated in Platform.cs
    float time_offset = Time;

    // Create sharp transitions between colors using position along perimeter
    float pattern = step(0.5, fract((position_on_perimeter * frequency + time_offset * direction)));

    // Select color based on pattern
    vec4 outline_color = mix(color1, color2, pattern);

    // Apply color only to border
    COLOR = is_border ? outline_color : vec4(0.0, 0.0, 0.0, 0.0);
}
