shader_type canvas_item;

uniform vec4 color1: source_color = vec4(1.0, 0.0, 0.0, 1.0);  // First color 
uniform vec4 color2: source_color = vec4(0.0, 0.0, 1.0, 1.0);  // Second color
uniform bool animate = true;  // Whether to animate the outline
uniform float direction: hint_range(-1.0, 1.0, 1.0) = -1.0;
uniform float frequency: hint_range(0.1, 30.0, 0.1) = 15.0;
uniform float speed: hint_range(0.5, 20, 0.5) = 3.0;
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
    
    // Calculate angle for color pattern
    vec2 center = window_size * 0.5;
    vec2 to_center = pixel_position - center;
    float angle = atan(to_center.y, to_center.x);
    
    // Apply animation
    float time_offset = animate ? TIME * speed : 0.0;
    
    // Create sharp transitions between colors
    float pattern = step(0.5, fract((angle + time_offset * direction) * frequency / (PI * 2.0)));
    
    // Select color based on pattern
    vec4 outline_color = mix(color1, color2, pattern);
    
    // Apply color only to border
    COLOR = is_border ? outline_color : vec4(0.0, 0.0, 0.0, 0.0);
}