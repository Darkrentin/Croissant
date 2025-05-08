shader_type spatial;

uniform sampler2D world_noise_normal_texture : hint_normal;
uniform float noise_scale : hint_range(0.01, 10.0) = 1.0;
uniform float normal_strength : hint_range(0.0, 2.0) = 1.0;

varying vec3 v_world_position;
varying vec3 v_world_normal;

void vertex()
{
    v_world_position = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
    v_world_normal = normalize(MODEL_NORMAL_MATRIX * NORMAL);
}

void fragment() {
    vec3 blend_weights = abs(v_world_normal);
    float sum_weights = blend_weights.x + blend_weights.y + blend_weights.z;
    if (sum_weights > 0.0001)
    {
        blend_weights /= sum_weights;
    }
    else
    {
        blend_weights = vec3(1.0/3.0);
    }

    vec2 uv_x = v_world_position.yz * noise_scale;
    vec3 normal_x = texture(world_noise_normal_texture, uv_x).rgb;

    vec2 uv_y = v_world_position.xz * noise_scale;
    vec3 normal_y = texture(world_noise_normal_texture, uv_y).rgb;

    vec2 uv_z = v_world_position.xy * noise_scale;
    vec3 normal_z = texture(world_noise_normal_texture, uv_z).rgb;

    vec3 blended_normal_map_color = normal_x * blend_weights.x +normal_y * blend_weights.y + normal_z * blend_weights.z;

    ALBEDO = vec3(0.6, 0.55, 0.5);

    NORMAL_MAP = blended_normal_map_color;
    NORMAL_MAP_DEPTH = normal_strength;

    ROUGHNESS = 0.8;
}