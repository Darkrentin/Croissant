// NOTE: Shader automatically converted from Godot Engine 4.1.rc2's ParticleProcessMaterial.

shader_type particles;

render_mode disable_velocity;

uniform float inherit_emitter_velocity_percent = 0.0;
uniform vec3 direction = vec3(1.0,0.0,0.0);
uniform float spread = 15.0;
uniform float flatness= 0;
uniform float initial_linear_velocity_min;
uniform float initial_angle_min;
uniform float angular_velocity_min;
uniform float linear_accel_min;
uniform float radial_accel_min;
uniform float tangent_accel_min;
uniform float damping_min;
uniform float hue_variation_min;
uniform float anim_speed_min;
uniform float anim_offset_min;
uniform float initial_linear_velocity_max;
uniform float initial_angle_max;
uniform float angular_velocity_max;
uniform float linear_accel_max;
uniform float radial_accel_max;
uniform float tangent_accel_max;
uniform float damping_max;
uniform float hue_variation_max;
uniform float anim_speed_max;
uniform float anim_offset_max;
uniform float lifetime_randomness;
uniform vec4 color_value : source_color;
uniform float emission_value = 0.0;
uniform vec3 gravity;

// random axis
uniform bool random_initial_axis = false;

// scaling
uniform float scale_min = 1.0;
uniform float scale_max = 1.0;
uniform sampler2D scale_curve: repeat_disable;

// scale over velocity
uniform vec2 scale_over_velocity_range = vec2(0.0, 5.0);
uniform sampler2D scale_over_velocity_curve: repeat_disable;

// orient
uniform bool align_y = false;


// display textures
uniform sampler2D animation_speed_curve: repeat_disable;
uniform sampler2D animation_offset_curve: repeat_disable;
uniform sampler2D color_gradient: repeat_disable;
uniform sampler2D alpha_curve: repeat_disable;
uniform sampler2D emission_curve: repeat_disable;
uniform sampler2D color_initial_ramp: repeat_disable;
uniform sampler2D hue_rot_curve: repeat_disable;
uniform sampler2D damping_texture : repeat_disable;
uniform sampler2D angle_texture : repeat_disable;


uniform vec3 velocity_pivot = vec3(0.);
uniform float velocity_to_dir_tolerance = 0.02;

// absolute velocity control
uniform float directional_velocity_min = 0.0;
uniform float directional_velocity_max = 0.0;
uniform sampler2D directional_velocity_curve;
uniform int directional_velocity_mode = 0;

// limiter
uniform bool velocity_limit = false;
uniform float velocity_limit_multiplier= 1.0;
uniform sampler2D velocity_limit_curve: repeat_disable;

// orbit curves
uniform float orbit_velocity_min;
uniform float orbit_velocity_max;
uniform sampler2D orbit_velocity_curve: repeat_disable;


// new!
uniform float radial_velocity_min;
uniform float radial_velocity_max;
uniform sampler2D radial_velocity_curve: repeat_disable;
uniform bool radial_velocity_prevent_overshoot = false;
// velocity pivot random per particle? allow making particles move in a helix around their spawn point

// extra control
// TODO INSTANCE
uniform float interpolate_to_end : hint_range (0.0,1.0, 0.01) = 0;

// TODO MAKE INTO INSTANCE UNIFORMS!!!!
uniform int emission_shape_index = 0;
uniform vec3 emission_shape_offset = vec3(0.);
uniform vec3 emission_shape_scale = vec3(1.);

// Sphere params
uniform float emission_sphere_radius;

// Box params
uniform vec3 emission_box_extents;

// Points param
// probably to put behind ifdef?
uniform sampler2D emission_texture_points : hint_default_black;
uniform int emission_texture_point_count; // why is this needed?

// ring param
uniform vec3 emission_ring_axis;
uniform float emission_ring_height;
uniform float emission_ring_radius;
uniform float emission_ring_inner_radius;

// accel textures
uniform sampler2D linear_accel_texture : repeat_disable;
uniform sampler2D radial_accel_texture : repeat_disable;
uniform sampler2D tangent_accel_texture : repeat_disable;

// turbulence

uniform float turbulence_noise_strength;
uniform float turbulence_noise_scale;
uniform float turbulence_influence_min;
uniform float turbulence_influence_max;
uniform float turbulence_initial_displacement_min;
uniform float turbulence_initial_displacement_max;
uniform float turbulence_noise_speed_random;
uniform vec3 turbulence_noise_speed = vec3(1.0, 1.0, 1.0);
uniform sampler2D turbulence_influence_over_life: repeat_disable;

// collision
uniform float collision_friction;
uniform float collision_bounce;

// sub emission
uniform int sub_emitter_amount_at_end;
uniform bool sub_emitter_keep_velocity;

//-------------------- TODO
// add minimum velocity tolerance to not move when velocity is only used for direction
// add lifetime interp, to push all particles from outside to interpolate to end of lifetime
// add y axis for color gradient as instance param
// add emitter shape as instance param
// add emitter scale + offset as instance param
// scale multiplier as instance param
// add domain boundaries, possibly as external stuff like attractors?
// Add max velocity curve/ limiting to imitate damping in a more controlled way
// size by speed range
// linear velocity vec3 is local/world
// velocity pivot in local space for both radial and orbit
// offset for velocity moving over life



vec4 grad(vec4 p) {
	p = fract(vec4(
		dot(p, vec4(0.143081, 0.001724, 0.280166, 0.262771)),
		dot(p, vec4(0.645401, -0.047791, -0.146698, 0.595016)),
		dot(p, vec4(-0.499665, -0.095734, 0.425674, -0.207367)),
		dot(p, vec4(-0.013596, -0.848588, 0.423736, 0.17044))));
	return fract((p.xyzw * p.yzwx) * 2365.952041) * 2.0 - 1.0;
}
float noise(vec4 coord) {
	// Domain rotation to improve the look of XYZ slices + animation patterns.
	coord = vec4(
		coord.xyz + dot(coord, vec4(vec3(-0.1666667), -0.5)),
		dot(coord, vec4(0.5)));

	vec4 base = floor(coord), delta = coord - base;

	vec4 grad_0000 = grad(base + vec4(0.0, 0.0, 0.0, 0.0)), grad_1000 = grad(base + vec4(1.0, 0.0, 0.0, 0.0));
	vec4 grad_0100 = grad(base + vec4(0.0, 1.0, 0.0, 0.0)), grad_1100 = grad(base + vec4(1.0, 1.0, 0.0, 0.0));
	vec4 grad_0010 = grad(base + vec4(0.0, 0.0, 1.0, 0.0)), grad_1010 = grad(base + vec4(1.0, 0.0, 1.0, 0.0));
	vec4 grad_0110 = grad(base + vec4(0.0, 1.0, 1.0, 0.0)), grad_1110 = grad(base + vec4(1.0, 1.0, 1.0, 0.0));
	vec4 grad_0001 = grad(base + vec4(0.0, 0.0, 0.0, 1.0)), grad_1001 = grad(base + vec4(1.0, 0.0, 0.0, 1.0));
	vec4 grad_0101 = grad(base + vec4(0.0, 1.0, 0.0, 1.0)), grad_1101 = grad(base + vec4(1.0, 1.0, 0.0, 1.0));
	vec4 grad_0011 = grad(base + vec4(0.0, 0.0, 1.0, 1.0)), grad_1011 = grad(base + vec4(1.0, 0.0, 1.0, 1.0));
	vec4 grad_0111 = grad(base + vec4(0.0, 1.0, 1.0, 1.0)), grad_1111 = grad(base + vec4(1.0, 1.0, 1.0, 1.0));

	vec4 result_0123 = vec4(
		dot(delta - vec4(0.0, 0.0, 0.0, 0.0), grad_0000), dot(delta - vec4(1.0, 0.0, 0.0, 0.0), grad_1000),
		dot(delta - vec4(0.0, 1.0, 0.0, 0.0), grad_0100), dot(delta - vec4(1.0, 1.0, 0.0, 0.0), grad_1100));
	vec4 result_4567 = vec4(
		dot(delta - vec4(0.0, 0.0, 1.0, 0.0), grad_0010), dot(delta - vec4(1.0, 0.0, 1.0, 0.0), grad_1010),
		dot(delta - vec4(0.0, 1.0, 1.0, 0.0), grad_0110), dot(delta - vec4(1.0, 1.0, 1.0, 0.0), grad_1110));
	vec4 result_89AB = vec4(
		dot(delta - vec4(0.0, 0.0, 0.0, 1.0), grad_0001), dot(delta - vec4(1.0, 0.0, 0.0, 1.0), grad_1001),
		dot(delta - vec4(0.0, 1.0, 0.0, 1.0), grad_0101), dot(delta - vec4(1.0, 1.0, 0.0, 1.0), grad_1101));
	vec4 result_CDEF = vec4(
		dot(delta - vec4(0.0, 0.0, 1.0, 1.0), grad_0011), dot(delta - vec4(1.0, 0.0, 1.0, 1.0), grad_1011),
		dot(delta - vec4(0.0, 1.0, 1.0, 1.0), grad_0111), dot(delta - vec4(1.0, 1.0, 1.0, 1.0), grad_1111));

	vec4 fade = delta * delta * delta * (10.0 + delta * (-15.0 + delta * 6.0));
	vec4 result_W0 = mix(result_0123, result_89AB, fade.w), result_W1 = mix(result_4567, result_CDEF, fade.w);
	vec4 result_WZ = mix(result_W0, result_W1, fade.z);
	vec2 result_WZY = mix(result_WZ.xy, result_WZ.zw, fade.y);
	return mix(result_WZY.x, result_WZY.y, fade.x);
}

// Curl 3D and three-noise function with friendly permission by Isaac Cohen.
// Modified to accept 4D noise.
vec3 noise_3x(vec4 p) {
	float s = noise(p);
	float s1 = noise(p + vec4(vec3(0.0), 1.7320508 * 2048.333333));
	float s2 = noise(p - vec4(vec3(0.0), 1.7320508 * 2048.333333));
	vec3 c = vec3(s, s1, s2);
	return c;
}
vec3 curl_3d(vec4 p, float c) {
	float epsilon = 0.001 + c;
	vec4 dx = vec4(epsilon, 0.0, 0.0, 0.0);
	vec4 dy = vec4(0.0, epsilon, 0.0, 0.0);
	vec4 dz = vec4(0.0, 0.0, epsilon, 0.0);
	vec3 x0 = noise_3x(p - dx).xyz;
	vec3 x1 = noise_3x(p + dx).xyz;
	vec3 y0 = noise_3x(p - dy).xyz;
	vec3 y1 = noise_3x(p + dy).xyz;
	vec3 z0 = noise_3x(p - dz).xyz;
	vec3 z1 = noise_3x(p + dz).xyz;
	float x = (y1.z - y0.z) - (z1.y - z0.y);
	float y = (z1.x - z0.x) - (x1.z - x0.z);
	float z = (x1.y - x0.y) - (y1.x - y0.x);
	return normalize(vec3(x, y, z));
}
vec3 get_noise_direction(vec3 pos) {
	float adj_contrast = max((turbulence_noise_strength - 1.0), 0.0) * 70.0;
	vec4 noise_time = TIME * vec4(turbulence_noise_speed, turbulence_noise_speed_random);
	vec4 noise_pos = vec4(pos * turbulence_noise_scale, 0.0);
	vec3 noise_direction = curl_3d(noise_pos + noise_time, adj_contrast);
	noise_direction = mix(0.9 * noise_direction, noise_direction, turbulence_noise_strength - 9.0);
	return noise_direction;
}

float rand_from_seed(inout uint seed) {
	int k;
	int s = int(seed);
	if (s == 0)
	s = 305420679;
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0)
		s += 2147483647;
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}

float rand_from_seed_m1_p1(inout uint seed) {
	return rand_from_seed(seed) * 2.0 - 1.0;
}

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}

struct DisplayParameters{
	vec3 scale;
	float hue_rotation;
	float animation_speed;
	float animation_offset;
	float lifetime;
	};

struct DynamicsParameters{
	float angle;
	float angular_velocity;
	float initial_velocity_multiplier;
	float directional_velocity;
	// ifdef for velocity control
	float radial_velocity;
	float orbit_velocity;
	// ifdef for turbulence
	float turb_influence;
};

// should be entirely ifdef'd
struct PhysicalParameters{
	// ifdef for acceleration
	float linear_accel;
	float radial_accel;
	float tangent_accel;
	float damping;
};

struct TurbulenceParameters{
	float turbulence_influence;
	float turbulence_initial_influence;
};

void calculate_initial_physical_params(inout PhysicalParameters params, inout uint alt_seed){
	// linear acceleration
	params.linear_accel = mix(linear_accel_min, linear_accel_min ,rand_from_seed(alt_seed));
	// radial acceleration
	params.radial_accel = mix(radial_accel_min, radial_accel_min,rand_from_seed(alt_seed));
	params.tangent_accel = mix(tangent_accel_min, tangent_accel_max,rand_from_seed(alt_seed));
	params.damping = mix(damping_min, damping_max,rand_from_seed(alt_seed));
}

void calculate_initial_dynamics_params(inout DynamicsParameters params,inout uint alt_seed){
	// -------------------- DO NOT REODER OPERATIONS, IT BREAKS VISUAL COMPATIBILITY
	// -------------------- ADD NEW OPERATIONS AT THE BOTTOM
	
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	// calculate angle
	params.angle = mix(initial_angle_min, initial_angle_max, rand_from_seed(alt_seed));
	params.angular_velocity = mix(angular_velocity_min, angular_velocity_max, rand_from_seed(alt_seed));

	// calculate initial velocity multiplier
	params.initial_velocity_multiplier = mix(initial_linear_velocity_min, initial_linear_velocity_max,rand_from_seed(alt_seed));
	params.directional_velocity = mix(directional_velocity_min, directional_velocity_max,rand_from_seed(alt_seed));
	// behind ifdef of velocity control or acceleration control
	params.radial_velocity = mix(radial_velocity_min, radial_velocity_max,rand_from_seed(alt_seed));
	params.orbit_velocity = mix(orbit_velocity_min, orbit_velocity_max,rand_from_seed(alt_seed));
}

void calculate_initial_display_params(inout DisplayParameters params,inout uint alt_seed){
	// -------------------- DO NOT REODER OPERATIONS, IT BREAKS VISUAL COMPATIBILITY
	// -------------------- ADD NEW OPERATIONS AT THE BOTTOM
	
	// NOTE: this can, one day, be extended to support emitter lifetime.
	//velocity, color, etc. can also be sampled with an extra step with curves over emitter life.
	// this would add an extra sample step, because we'd want to sample at the moment where the particle
	// was emitted. dynamic control of that is not allowed (we'd have to use tons of userdata to support it)
	
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	
	// calculate scale
	params.scale = vec3(mix(scale_min, scale_max, rand_from_seed(alt_seed)));
	params.scale = sign(params.scale) * max(abs(params.scale), 0.001);
	
	// calculate hue variation
	params.hue_rotation =  pi * 2.0 * mix(hue_variation_min, hue_variation_max, rand_from_seed(alt_seed));
	
	// particle animation
	params.animation_speed = mix(anim_speed_min, anim_speed_max, rand_from_seed(alt_seed));
	params.animation_offset = mix(anim_offset_min, anim_offset_max, rand_from_seed(alt_seed)) ;

	// lifetime
	params.lifetime = (1.0 - lifetime_randomness * rand_from_seed(alt_seed));
}

vec3 calculate_initial_position(inout uint alt_seed) {
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	vec3 pos = vec3(0.);
	if (emission_shape_index == 0){
		// do nothing
	}
	if (emission_shape_index == 1){
		float s = rand_from_seed(alt_seed) * 2.0 - 1.0;
		float t = rand_from_seed(alt_seed) * 2.0 * pi;
		float p = rand_from_seed(alt_seed);
		float radius = emission_sphere_radius * sqrt(1.0 - s * s);
		pos = mix(vec3(0.0, 0.0, 0.0), vec3(radius * cos(t), radius * sin(t), emission_sphere_radius * s), p);
	}
	if (emission_shape_index == 2){
		float s = rand_from_seed(alt_seed) * 2.0 - 1.0;
		float t = rand_from_seed(alt_seed) * 2.0 * pi;
		float radius = emission_sphere_radius * sqrt(1.0 - s * s);
		pos = vec3(radius * cos(t), radius * sin(t), emission_sphere_radius * s);
	}
	if (emission_shape_index == 3){
		pos = vec3(rand_from_seed(alt_seed) * 2.0 - 1.0, rand_from_seed(alt_seed) * 2.0 - 1.0, rand_from_seed(alt_seed) * 2.0 - 1.0) * emission_box_extents;
	}
	if (emission_shape_index == 4 || emission_shape_index == 5){
		int point = min(emission_texture_point_count - 1, int(rand_from_seed(alt_seed) * float(emission_texture_point_count)));
		ivec2 emission_tex_size = textureSize(emission_texture_points, 0);
		ivec2 emission_tex_ofs = ivec2(point % emission_tex_size.x, point / emission_tex_size.x);
		pos = texelFetch(emission_texture_points, emission_tex_ofs, 0).xyz;
	}
	if (emission_shape_index == 6){
		
		float ring_spawn_angle = rand_from_seed(alt_seed) * 2.0 * pi;
		float ring_random_radius = rand_from_seed(alt_seed) * (emission_ring_radius - emission_ring_inner_radius) + emission_ring_inner_radius;
		vec3 axis = normalize(emission_ring_axis);
		vec3 ortho_axis = vec3(0.0);
		if (axis == vec3(1.0, 0.0, 0.0)) {
			ortho_axis = cross(axis, vec3(0.0, 1.0, 0.0));
		} else {
 			ortho_axis = cross(axis, vec3(1.0, 0.0, 0.0));
		}
		ortho_axis = normalize(ortho_axis);
		float s = sin(ring_spawn_angle);
		float c = cos(ring_spawn_angle);
		float oc = 1.0 - c;
		ortho_axis = mat3(
			vec3(c + axis.x * axis.x * oc, axis.x * axis.y * oc - axis.z * s, axis.x * axis.z *oc + axis.y * s),
			vec3(axis.x * axis.y * oc + s * axis.z, c + axis.y * axis.y * oc, axis.y * axis.z * oc - axis.x * s),
			vec3(axis.z * axis.x * oc - axis.y * s, axis.z * axis.y * oc + axis.x * s, c + axis.z * axis.z * oc)
			) * ortho_axis;
		ortho_axis = normalize(ortho_axis);
		pos = ortho_axis * ring_random_radius + (rand_from_seed(alt_seed) * emission_ring_height - emission_ring_height / 2.0) * axis;
	}
	return pos * emission_shape_scale + emission_shape_offset;
}

void process_display_param(inout DisplayParameters parameters, float lifetime){
	// compile-time add textures
	parameters.scale *= texture(scale_curve, vec2(lifetime)).rgb;
	parameters.hue_rotation *= texture(hue_rot_curve, vec2(lifetime)).r;
	parameters.animation_offset += texture(animation_offset_curve, vec2(lifetime)).r;
	parameters.animation_speed *= texture(animation_speed_curve, vec2(lifetime)).r;
//	parameters.scale *= 1.;
	//etc.
}

vec3 process_orbit_displacement(DynamicsParameters param, float lifetime, inout uint alt_seed, mat4 transform, mat4 emission_transform,float delta, float total_lifetime){
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	vec3 displacement = vec3(0.);
	vec3 orbit_velocities = texture(orbit_velocity_curve, vec2(lifetime)).rgb;
	orbit_velocities *= degree_to_rad;
	orbit_velocities *= delta/total_lifetime; // we wanna process those by the delta angle
	//vec3 local_velocity_pivot = ((emission_transform) * vec4(velocity_pivot,1.0)).xyz;
	// X axis
	vec3 local_pos = (inverse(emission_transform) * transform[3]).xyz;
	local_pos -= velocity_pivot;
	local_pos.x = 0.;
	mat3 x_rotation_mat = mat3(
		vec3(1.0,0.0,0.0),
		vec3(0.0, cos(orbit_velocities.x), sin(orbit_velocities.x)),
		vec3(0.0, -sin(orbit_velocities.x), cos(orbit_velocities.x))
	);
	vec3 new_pos = x_rotation_mat * local_pos;
	displacement = new_pos - local_pos;

	// Y axis
	local_pos = (inverse(emission_transform) * transform[3]).xyz;
	local_pos -= velocity_pivot;
	local_pos.y = 0.;
	mat3 y_rotation_mat = mat3(
		vec3(cos(orbit_velocities.y), 0.0, -sin(orbit_velocities.y)),
		vec3(0.0, 1.0,0.0),
		vec3(sin(orbit_velocities.y), 0.0, cos(orbit_velocities.y))
	);
	new_pos = y_rotation_mat * local_pos;
	displacement += new_pos - local_pos;
	// z axis

	local_pos = (inverse(emission_transform) * transform[3]).xyz;
	local_pos -= velocity_pivot;
	local_pos.z = 0.;
	mat3 z_rotation_mat = mat3(
		vec3(cos(orbit_velocities.z),-sin(orbit_velocities.z),0.0),
		vec3(-sin(orbit_velocities.z),cos(orbit_velocities.z), 0.0),
		vec3(0.0,0.0,1.0)
	);
	new_pos = z_rotation_mat * local_pos;
	displacement += new_pos - local_pos;

	return (emission_transform * vec4(displacement, 0.0)).xyz * param.orbit_velocity;
}

vec3 process_radial_displacement(DynamicsParameters param, float lifetime, inout uint alt_seed, mat4 transform, mat4 emission_transform, float delta){
	vec3 radial_displacement;
	float radial_displacement_multiplier = texture(radial_velocity_curve, vec2(lifetime)).r;
	vec3 global_velocity_pivot = (emission_transform * vec4(velocity_pivot, 1.0)).xyz;
	if(length(transform[3].xyz - global_velocity_pivot)> 0.01){
		radial_displacement = normalize(transform[3].xyz - global_velocity_pivot) * radial_displacement_multiplier * param.radial_velocity;
	}
	if (radial_velocity_prevent_overshoot && radial_displacement_multiplier * param.radial_velocity < 0.0){
		if (length(radial_displacement)> 0.01){
		radial_displacement = normalize(radial_displacement) * min(length(radial_displacement), length(global_velocity_pivot - transform[3].xyz));
		}
	}
	
	return radial_displacement;
}

vec3 process_linear_velocity(DynamicsParameters param, inout uint alt_seed){
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	vec3 velocity = vec3(0.);
	float spread_rad = spread * degree_to_rad;
	float angle1_rad = rand_from_seed_m1_p1(alt_seed) * spread_rad;
	float angle2_rad = rand_from_seed_m1_p1(alt_seed) * spread_rad * (1.0 - flatness);
	vec3 direction_xz = vec3(sin(angle1_rad), 0.0, cos(angle1_rad));
	vec3 direction_yz = vec3(0.0, sin(angle2_rad), cos(angle2_rad));
	direction_yz.z = direction_yz.z / max(0.0001,sqrt(abs(direction_yz.z))); // better uniform distribution
	vec3 spread_direction = vec3(direction_xz.x * direction_yz.z, direction_yz.y, direction_xz.z * direction_yz.z);
	vec3 direction_nrm = length(direction) > 0.0 ? normalize(direction) : vec3(0.0, 0.0, 1.0);
	// rotate spread to direction
	vec3 binormal = cross(vec3(0.0, 1.0, 0.0), direction_nrm);
	if (length(binormal) < 0.0001) {
		// direction is parallel to Y. Choose Z as the binormal.
		binormal = vec3(0.0, 0.0, 1.0);
	}
	binormal = normalize(binormal);
	vec3 normal = cross(binormal, direction_nrm);
	spread_direction = binormal * spread_direction.x + normal * spread_direction.y + direction_nrm * spread_direction.z;
	velocity = spread_direction * param.initial_velocity_multiplier;
//	return vec3(100.0,0.0,0.0);
	return velocity;
}

vec3 process_directional_displacement(DynamicsParameters param, float lifetime_percent,mat4 transform, mat4 emission_transform, float delta){
	vec3 displacement = vec3(0.);
	if (directional_velocity_mode == 0){
		displacement = texture(directional_velocity_curve, vec2(lifetime_percent)).xyz * param.directional_velocity;
	}
	if(directional_velocity_mode == 1){
		displacement = texture(directional_velocity_curve, vec2(lifetime_percent)).xyz * param.directional_velocity;
		displacement = (emission_transform * vec4(displacement, 0.0)).xyz;
	}
	if(directional_velocity_mode == 2){
		displacement = texture(directional_velocity_curve, vec2(lifetime_percent)).xyz * param.directional_velocity;
		displacement = normalize(transform[0].xyz) * displacement.x + normalize(transform[1].xyz) * displacement.y + normalize(transform[2].xyz) * displacement.z;
	}
	return displacement * delta;
}

vec4 rotate_color(vec4 current_color, float hue_rot_angle){
	float hue_rot_c = cos(hue_rot_angle);
	float hue_rot_s = sin(hue_rot_angle);
	mat4 hue_rot_mat = mat4(vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.299, 0.587, 0.114, 0.0),
			vec4(0.000, 0.000, 0.000, 1.0)) +
		mat4(vec4(0.701, -0.587, -0.114, 0.0),
			vec4(-0.299, 0.413, -0.114, 0.0),
			vec4(-0.300, -0.588, 0.886, 0.0),
			vec4(0.000, 0.000, 0.000, 0.0)) * hue_rot_c +
		mat4(vec4(0.168, 0.330, -0.497, 0.0),
			vec4(-0.328, 0.035,  0.292, 0.0),
			vec4(1.250, -1.050, -0.203, 0.0),
			vec4(0.000, 0.000, 0.000, 0.0)) * hue_rot_s;
	return hue_rot_mat * current_color;
}

vec4 process_color(DisplayParameters param, float lifetime_percent){
	vec4 color = texture(color_gradient, vec2(lifetime_percent)) * color_value;
	color.a *= texture(alpha_curve, vec2(lifetime_percent)).r;
	color = rotate_color(color, param.hue_rotation);
	color.rgb *= 1.0 + texture(emission_curve, vec2(lifetime_percent)).r * emission_value;
	return color;
}

void process_physical_parameters(inout PhysicalParameters params, float lifetime_percent){
	params.linear_accel *= texture(linear_accel_texture, vec2(lifetime_percent)).r;
	params.radial_accel *= texture(radial_accel_texture, vec2(lifetime_percent)).r;
	params.tangent_accel *= texture(tangent_accel_texture, vec2(lifetime_percent)).r;
	params.damping *= texture(damping_texture, vec2(lifetime_percent)).r;
	
}

void start() {
	uint base_number = NUMBER;
	uint alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	DisplayParameters params;
	calculate_initial_display_params(params, alt_seed);
	// reset alt seed?
	// alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	DynamicsParameters dynamic_params;
	calculate_initial_dynamics_params(dynamic_params, alt_seed);
	PhysicalParameters physics_params;
	calculate_initial_physical_params(physics_params, alt_seed);
	
	float initial_turbulence_displacement = mix(turbulence_initial_displacement_min, turbulence_initial_displacement_max, rand_from_seed(alt_seed));
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;
	
	if (RESTART_CUSTOM){
		CUSTOM = vec4(0.);
		CUSTOM.w = params.lifetime;
		CUSTOM.x = dynamic_params.angle;
	}
	if (RESTART_COLOR){
		COLOR = process_color(params, 0.0);
	}
	if (RESTART_ROT_SCALE) {
		TRANSFORM[0].xyz = vec3(1.0, 0.0, 0.0);
		TRANSFORM[1].xyz = vec3(0.0, 1.0, 0.0);
		TRANSFORM[2].xyz = vec3(0.0, 0.0, 1.0);
	}

	if (RESTART_POSITION) {
		TRANSFORM[3].xyz = calculate_initial_position(alt_seed);
		if(turbulence_initial_displacement_max > 0.){
			vec3 noise_direction = get_noise_direction(TRANSFORM[3].xyz);
			TRANSFORM[3].xyz += noise_direction * initial_turbulence_displacement;
		}
		if (random_initial_axis){
			float spread_rad = spread * degree_to_rad;
			float angle1_rad = rand_from_seed_m1_p1(alt_seed) * spread_rad;
			float angle2_rad = rand_from_seed_m1_p1(alt_seed) * spread_rad * (1.0 - flatness);
			vec3 direction_xz = vec3(sin(angle1_rad), 0.0, cos(angle1_rad));
			vec3 direction_yz = vec3(0.0, sin(angle2_rad), cos(angle2_rad));
			direction_yz.z = direction_yz.z / max(0.0001,sqrt(abs(direction_yz.z))); // better uniform distribution
			vec3 spread_direction = vec3(direction_xz.x * direction_yz.z, direction_yz.y, direction_xz.z * direction_yz.z);
			vec3 direction_nrm = length(direction) > 0.0 ? normalize(direction) : vec3(0.0, 0.0, 1.0);
			// rotate spread to direction
			vec3 binormal = cross(vec3(0.0, 1.0, 0.0), direction_nrm);
			if (length(binormal) < 0.0001) {
				// direction is parallel to Y. Choose Z as the binormal.
				binormal = vec3(0.0, 0.0, 1.0);
			}
			binormal = normalize(binormal);
			vec3 normal = cross(binormal, direction_nrm);
			spread_direction = binormal * spread_direction.x + normal * spread_direction.y + direction_nrm * spread_direction.z;
			TRANSFORM[0] = vec4(binormal, 0.);
			TRANSFORM[1] = vec4(normal, 0.);
			TRANSFORM[2] = vec4(direction_nrm, 0.);
		}
		TRANSFORM = EMISSION_TRANSFORM * TRANSFORM;
		}
	if (RESTART_VELOCITY) {
		VELOCITY = process_linear_velocity(dynamic_params, alt_seed);
		}
	
	process_display_param(params, 0.);
//	process_dynamic_parameters(dynamic_params, 0., alt_seed, TRANSFORM, EMISSION_TRANSFORM, DELTA);
	VELOCITY = (EMISSION_TRANSFORM * vec4(VELOCITY, 0.0)).xyz;
}

void process() {
	uint base_number = NUMBER;
	uint alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	DisplayParameters params;
	calculate_initial_display_params(params, alt_seed);
	// reset alt seed?
	// alt_seed = hash(base_number + uint(1) + RANDOM_SEED);
	DynamicsParameters dynamic_params;
	calculate_initial_dynamics_params(dynamic_params, alt_seed);
	PhysicalParameters physics_params;
	calculate_initial_physical_params(physics_params, alt_seed);
	float pi = 3.14159;
	float degree_to_rad = pi / 180.0;

	CUSTOM.y += DELTA / LIFETIME;
	CUSTOM.y = mix(CUSTOM.y, 1.0, interpolate_to_end);
	float lifetime_percent = CUSTOM.y/ params.lifetime;
	if (CUSTOM.y > CUSTOM.w) {
		ACTIVE = false;
	}
	
	
	// will use this later to calculate final displacement and orient the particle.
	vec3 starting_position = TRANSFORM[3].xyz;
	vec3 controlled_displacement;
	// FIXME use instance uniform!!!!!
	controlled_displacement += (EMISSION_TRANSFORM[3].xyz - USERDATA1.xyz) * inherit_emitter_velocity_percent;
	
//	VELOCITY += process_physics_parameters(dynamic_params, lifetime_percent, alt_seed, TRANSFORM, EMISSION_TRANSFORM, DELTA);
	
	controlled_displacement += process_orbit_displacement(dynamic_params, lifetime_percent, alt_seed, TRANSFORM, EMISSION_TRANSFORM, DELTA, params.lifetime * LIFETIME);
	// calculate all velocity
	
	controlled_displacement += process_radial_displacement(dynamic_params, lifetime_percent, alt_seed, TRANSFORM, EMISSION_TRANSFORM, DELTA);
	
	controlled_displacement += process_directional_displacement(dynamic_params, lifetime_percent, TRANSFORM, EMISSION_TRANSFORM, DELTA);
	
	process_physical_parameters(physics_params, lifetime_percent);
	vec3 force;
	{
		// copied from previous version
		vec3 pos = TRANSFORM[3].xyz;
		force = gravity;
		// apply linear acceleration
		force += length(VELOCITY) > 0.0 ? normalize(VELOCITY) * physics_params.linear_accel : vec3(0.0);
		// apply radial acceleration
		vec3 org = EMISSION_TRANSFORM[3].xyz;
		vec3 diff = pos - org;
		force += length(diff) > 0.0 ? normalize(diff) * physics_params.radial_accel : vec3(0.0);
		// apply tangential acceleration;
		float tangent_accel_val = physics_params.tangent_accel;
		vec3 crossDiff = cross(normalize(diff), normalize(gravity));
		force += length(crossDiff) > 0.0 ? normalize(crossDiff) * tangent_accel_val : vec3(0.0);
		force += ATTRACTOR_FORCE;

		// apply attractor forces
		VELOCITY += force * DELTA;
//
	}
	{
		// copied from previous version
		if (physics_params.damping > 0.0) {
			float v = length(VELOCITY);
			v -= physics_params.damping * DELTA;
			if (v < 0.0) {
				VELOCITY = vec3(0.0);
			} else {
				VELOCITY = normalize(VELOCITY) * v;
			}
		}
		
	}
	
	vec3 final_velocity = controlled_displacement + VELOCITY;
	
	// turbulence before limiting
	if (turbulence_influence_max > 0.0){
		float turbulence_influence = textureLod(turbulence_influence_over_life, vec2(lifetime_randomness, 0.0), 0.0).r;
		
		vec3 noise_direction = get_noise_direction(TRANSFORM[3].xyz);
		if (!COLLIDED) {
			
			float vel_mag = length(final_velocity);
			float vel_infl = clamp(mix(turbulence_influence_min, turbulence_influence_max, rand_from_seed(alt_seed)) * turbulence_influence, 0.0, 1.0);
			final_velocity = mix(final_velocity, normalize(noise_direction) * vel_mag * (1.0 + (1.0 - vel_infl) * 0.2), vel_infl);
		}
	}
	
	// limit velocity
	if (length(final_velocity)> 0.001 && velocity_limit){
		final_velocity = normalize(final_velocity) * min(abs(length(final_velocity)), abs(velocity_limit_multiplier * texture(velocity_limit_curve, vec2(lifetime_percent)).r));
	}
	
	TRANSFORM[3].xyz += final_velocity * DELTA;
	
	if (align_y){
		if (length(final_velocity) > 0.0) {
			TRANSFORM[1].xyz = normalize(final_velocity);
		} else {
			TRANSFORM[1].xyz = normalize(TRANSFORM[1].xyz);
		}
		if (TRANSFORM[1].xyz == normalize(TRANSFORM[0].xyz)) {
			TRANSFORM[0].xyz = normalize(cross(normalize(TRANSFORM[1].xyz), normalize(TRANSFORM[2].xyz)));
			TRANSFORM[2].xyz = normalize(cross(normalize(TRANSFORM[0].xyz), normalize(TRANSFORM[1].xyz)));
		} else {
			TRANSFORM[2].xyz = normalize(cross(normalize(TRANSFORM[0].xyz), normalize(TRANSFORM[1].xyz)));
			TRANSFORM[0].xyz = normalize(cross(normalize(TRANSFORM[1].xyz), normalize(TRANSFORM[2].xyz)));
		}
	}
	
	process_display_param(params, lifetime_percent);
	
	// if scale over velocity
	if(length(final_velocity)> 0.001){
		params.scale *= texture(scale_over_velocity_curve, vec2(clamp(length(final_velocity)/(scale_over_velocity_range.y - scale_over_velocity_range.x), 0.0,1.0), 0.0)).rgb;
	}
//	params.scale *= length(final_velocity)/100.0;

	TRANSFORM[0].xyz = normalize(TRANSFORM[0].xyz);
	TRANSFORM[1].xyz = normalize(TRANSFORM[1].xyz);
	TRANSFORM[2].xyz = normalize(TRANSFORM[2].xyz);
	TRANSFORM[0].xyz *= sign(params.scale.x) * max(abs(params.scale.x), 0.001);
	TRANSFORM[1].xyz *= sign(params.scale.y) * max(abs(params.scale.y), 0.001);
	TRANSFORM[2].xyz *= sign(params.scale.z) * max(abs(params.scale.z), 0.001);
	
	// angle
	float base_angle = dynamic_params.angle;
	base_angle += CUSTOM.y * LIFETIME * dynamic_params.angular_velocity;
	CUSTOM.x = base_angle * degree_to_rad;
	
	// 
	CUSTOM.z = params.animation_offset + lifetime_percent * params.animation_speed;
	
	COLOR = process_color(params, lifetime_percent);
	
	if (COLLIDED) {
		if (length(VELOCITY) > 3.0) {
			TRANSFORM[3].xyz += COLLISION_NORMAL * COLLISION_DEPTH;
			VELOCITY -= COLLISION_NORMAL * dot(COLLISION_NORMAL, VELOCITY) * (1.0 + collision_bounce);
			VELOCITY = mix(VELOCITY,vec3(0.0),clamp(collision_friction, 0.0, 1.0));
		} else {
			VELOCITY = vec3(0.0);
		}
	}
	int emit_count = 0;
	float unit_delta = DELTA/LIFETIME;
	float end_time = CUSTOM.w * 0.95;
	if (CUSTOM.y < end_time && (CUSTOM.y + unit_delta) >= end_time) emit_count = sub_emitter_amount_at_end;
	for(int i=0;i<emit_count;i++) {
		uint flags = FLAG_EMIT_POSITION|FLAG_EMIT_ROT_SCALE;
		if (sub_emitter_keep_velocity) flags|=FLAG_EMIT_VELOCITY;
		emit_subparticle(TRANSFORM,VELOCITY,vec4(0.0),vec4(0.0),flags);
	}	if (CUSTOM.y > CUSTOM.w) {
		ACTIVE = false;
	}
	USERDATA1.xyz = EMISSION_TRANSFORM[3].xyz;
}