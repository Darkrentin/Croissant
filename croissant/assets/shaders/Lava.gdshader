shader_type spatial;
render_mode shadows_disabled;

uniform vec4 dark_lava_color : source_color;
uniform vec4 light_lava_color : source_color;
uniform float speed : hint_range(0.0, 5.0);
uniform float scale : hint_range(0.1, 100.0) = 10;
uniform float sharpness : hint_range(1.0, 100.0);
uniform float emission_intensity : hint_range(0.0, 10.0);
uniform float normal_strength : hint_range(0.0, 20.0) = 5.0;
uniform float roughness_value : hint_range(0.0, 1.0) = 0.3;
uniform float roughness_variation : hint_range(0.0, 1.0) = 0.4;
uniform float displacement_strength : hint_range(0.0, 2.0) = 0.2; // Contrôle la hauteur physique

vec2 random2D(vec2 p) {
    return fract(sin(vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)))) * 43758.5453);
}

float voronoiIQ(vec2 _pos) {
    float _time = TIME * speed;
    _pos.y += _time;
    _pos.x += sin(_time);

    vec2 p = floor(_pos);
    vec2 f = fract(_pos);
    float res = 0.0;

    // Clamp the sharpness to avoid extreme behavior.
    float sharp = clamp(sharpness, 1.0, 50.0);

    // Sum contributions from neighboring cells.
    for (int j = -1; j <= 1; j++) {
        for (int i = -1; i <= 1; i++) {
            vec2 b = vec2(float(i), float(j));
            vec2 pnt = random2D(p + b);
            pnt = 0.5 + 0.5 * sin(_time + 6.2831 * pnt);
            vec2 r = vec2(b) - f + pnt;
            float d = dot(r, r);
            res += exp(-sharp * d);
        }
    }

    // Ensure we never take the log of zero.
    res = max(res, 1e-6);

    // Calculate the final value. Optionally, clamp the result.
    float result = -(1.0 / sharp) * log(res);
    return clamp(result, 0.0, 1.0);
}

vec3 calculateNormal(vec2 uv, float voronoi_value) {
    float eps = 0.01 / scale;
    float nx = voronoiIQ(uv + vec2(eps, 0.0)) - voronoiIQ(uv - vec2(eps, 0.0));
    float ny = voronoiIQ(uv + vec2(0.0, eps)) - voronoiIQ(uv - vec2(0.0, eps));

    vec3 normal = normalize(vec3(-nx * normal_strength, -ny * normal_strength, 1.0));
    return normal;
}

void vertex() {

    vec2 uv = UV * scale;
    float height = voronoiIQ(uv);

    VERTEX.y += height * displacement_strength;
}

void fragment() {
    vec2 uv = UV * scale;
    float voronoi_value = voronoiIQ(uv);

    // Calculer la couleur en fonction du motif voronoi
    vec4 final_color = mix(dark_lava_color, light_lava_color, voronoi_value);

    // Calculer la carte des normales pour donner du relief
    vec3 normal_map = calculateNormal(uv, voronoi_value);

    // Simuler des variations de hauteur
    float height = voronoi_value * 0.2;

    // Variation de rugosité basée sur le motif (les zones claires sont plus lisses)
    float material_roughness = roughness_value - roughness_variation * voronoi_value;

    // Appliquer les propriétés de surface
    ALBEDO = final_color.rgb;
    NORMAL = normal_map;
    ROUGHNESS = material_roughness;
    METALLIC = 0.0;
    EMISSION = final_color.rgb * emission_intensity * (1.0 + height);
    SPECULAR = 0.5 + voronoi_value * 0.3;
}
