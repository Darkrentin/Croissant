shader_type canvas_item;

uniform vec4 color1: source_color = vec4(1.0, 0.0, 0.0, 1.0);  // First color 
uniform vec4 color2: source_color = vec4(0.0, 0.0, 1.0, 1.0);  // Second color
uniform bool animate = true;  // Whether to animate the outline
uniform float direction: hint_range(-1.0, 1.0, 1.0) = -1.0;
uniform float frequency: hint_range(0.1, 30.0, 0.1) = 15.0;
uniform float speed: hint_range(0.5, 20, 0.5) = 3.0;
uniform float border_width_x = 0.01; // Border width as proportion (0.01 = 1%)

void fragment() {
    // Get actual size in pixels
    vec2 size_pixels = 1.0 / TEXTURE_PIXEL_SIZE;
    
    // Calculate aspect ratio
    float aspect_ratio = size_pixels.x / size_pixels.y;
    
    // Calculate border width for x and y, adjusting y based on aspect ratio
    float border_x_pixels = size_pixels.x * border_width_x;
    float border_y_pixels = size_pixels.y * border_width_x * aspect_ratio;
    
    // Get pixel position
    vec2 pixel_position = UV * size_pixels;
    
    // Calculate distance from each edge in pixels
    float distance_from_left = pixel_position.x;
    float distance_from_right = size_pixels.x - pixel_position.x;
    float distance_from_top = pixel_position.y;
    float distance_from_bottom = size_pixels.y - pixel_position.y;
    
    // Check if we're in the border area, using the adjusted pixel widths
    bool is_border = 
        distance_from_left < border_x_pixels ||
        distance_from_right < border_x_pixels ||
        distance_from_top < border_y_pixels ||
        distance_from_bottom < border_y_pixels;
    
    // Calculate angle for color pattern
    vec2 center = size_pixels * 0.5;
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