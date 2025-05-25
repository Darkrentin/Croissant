shader_type canvas_item;

uniform vec4 border_color: source_color = vec4(1.0, 1.0, 1.0, 1.0);  // Border color
uniform float border_width = 10.0; // Border width in pixels
uniform vec2 window_size = vec2(800.0, 600.0); // Window size in pixels

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

    // Apply border color or transparent
    COLOR = is_border ? border_color : vec4(0.0, 0.0, 0.0, 0.0);
}
