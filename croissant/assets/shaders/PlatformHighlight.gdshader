shader_type canvas_item;

uniform vec4 color1: source_color = vec4(1.0, 0.0, 0.0, 1.0);  // Highlight color
uniform float frequency: hint_range(0.1, 100.0, 0.1) = 15.0;
uniform vec2 window_size = vec2(800.0, 600.0); // Window size in pixels

const float BORDER_WIDTH = 4.0;
const float DIRECTION = -1.0;

void fragment() {
    // Get UV-based position in pixels
    vec2 pixel_position = UV * window_size;

    // Calculate minimum distance from any edge
    vec2 distance_from_edges = min(pixel_position, window_size - pixel_position);
    float min_distance = min(distance_from_edges.x, distance_from_edges.y);
    
    // Check if we're in the border area
    if (min_distance < BORDER_WIDTH) {
        // Calculate position along perimeter (0.0 to 1.0)
        float perimeter = 2.0 * (window_size.x + window_size.y);
        float position_on_perimeter;

        if (pixel_position.y <= BORDER_WIDTH) {
            // Top border
            position_on_perimeter = pixel_position.x / perimeter;
        } else if (pixel_position.x >= window_size.x - BORDER_WIDTH) {
            // Right border
            position_on_perimeter = (window_size.x + pixel_position.y) / perimeter;
        } else if (pixel_position.y >= window_size.y - BORDER_WIDTH) {
            // Bottom border
            position_on_perimeter = (window_size.x + window_size.y + (window_size.x - pixel_position.x)) / perimeter;
        } else {
            // Left border
            position_on_perimeter = (2.0 * window_size.x + window_size.y + (window_size.y - pixel_position.y)) / perimeter;
        }

        // Create pattern with color1 and transparent alternating
        float pattern = step(0.5, fract(position_on_perimeter * frequency + TIME * DIRECTION));
        
        // Apply color1 or transparent based on pattern
        COLOR = pattern > 0.5 ? color1 : vec4(0.0, 0.0, 0.0, 0.0);
    } else {
        // Outside border area - transparent
        COLOR = vec4(0.0, 0.0, 0.0, 0.0);
    }
}
