shader_type canvas_item;

uniform vec4 color: source_color = vec4(1.0);
uniform float direction: hint_range(-1.0, 1.0, 1.0) = -1.0;
uniform float frequency: hint_range(0.1, 30.0, 0.1) = 15.0;
uniform float speed: hint_range(0.5, 20, 0.5) = 3.0;
uniform float radius: hint_range(0.01, 1.0, 0.01) = 1.0;
uniform float line_thickness: hint_range(0.0, 0.5, 0.01) = 0.03;

void fragment() {
  vec2 pos = UV - 0.5;
  float dist = length(pos);
  float circle = step(dist, radius);

  float angle = atan(pos.y, pos.x);
  float wave = sin(direction * frequency * angle + TIME * speed) * 0.5 + 0.5;

  circle *= step(0.5, wave);

  float border_thickness = 0.5 - line_thickness;
  vec2 abs_pos = abs(pos);
  float rect_alpha = 1.0 - step(abs_pos.x, border_thickness) * step(abs_pos.y, border_thickness);

  COLOR = vec4(color.rgb, circle * color.a * rect_alpha);
}
