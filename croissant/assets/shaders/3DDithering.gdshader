// Credit to Sam Bigos for the shader inspired by Lucas Pope’s Return of the Obra Dinn
/*
This shader is under MIT license. Feel free to use, improve and
change this shader according to your needs and consider sharing
the modified result to godotshaders.com.
*/
shader_type canvas_item;

// Samplers
uniform sampler2D u_dither_tex : repeat_enable, filter_nearest;
uniform sampler2D u_color_tex : filter_nearest;
// NOTE: TEXTURE is a built-in sampler for canvas_item shaders.
// It refers to the texture of the node this shader is applied to.
// Do NOT declare 'uniform sampler2D TEXTURE' here.

// If you intend to use the screen's backbuffer (for a full-screen post-process effect),
// you would declare and use SCREEN_TEXTURE like this:
// uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;
// And then replace all uses of 'TEXTURE' below with 'SCREEN_TEXTURE'.
// You would also need to enable "Screen Texture" in the Material properties in the Godot editor.

// Parameters
uniform int u_bit_depth = 8;
uniform float u_contrast = 1.0;
uniform float u_offset = 0.0;
uniform int u_dither_size = 1; // How much to "pixelate" the source texture before dithering
uniform float u_exclusion_tolerance : hint_range(0.0, 0.5, 0.01) = 0.05;

void fragment()
{
	// --- 1. SAMPLE ORIGINAL PIXEL (from the node's own texture/appearance) ---
	// TEXTURE here refers to the built-in texture of the CanvasItem.
	vec2 source_texture_true_size = vec2(textureSize(TEXTURE, 0));
	
	// Ensure u_dither_size is at least 1 to avoid division by zero or unexpected behavior.
	float dither_size_clamped = float(max(1, u_dither_size));
	
	// Calculate the number of "pixel blocks" across the source texture.
	vec2 pixelated_block_count = source_texture_true_size / dither_size_clamped;
	
	// UV for sampling the input TEXTURE at the "pixelated" resolution.
	// This effectively "pixelates" the source texture based on u_dither_size.
	vec2 screen_sample_uv = floor(UV * pixelated_block_count) / pixelated_block_count;
	
	vec3 screen_col = texture(TEXTURE, screen_sample_uv).rgb;
	float original_alpha = texture(TEXTURE, screen_sample_uv).a;

	vec3 processed_color; // This will hold the color after dithering or fallback

	// --- 2. CHECK IF DITHERING CAN BE PERFORMED (dependencies are valid) ---
	ivec2 palette_tex_dims = textureSize(u_color_tex, 0);
	ivec2 dither_tex_dims = textureSize(u_dither_tex, 0);

	// Conditions for performing dithering:
	// - Palette texture must have dimensions > 0
	// - Dither texture must have dimensions > 0
	bool can_dither = palette_tex_dims.x > 0 && palette_tex_dims.y > 0 &&
	                  dither_tex_dims.x > 0 && dither_tex_dims.y > 0;

	if (can_dither)
	{
		// --- 3. DITHERING LOGIC (applied to screen_col from node's TEXTURE) ---
		
		// Calculate pixel luminosity
		float lum = (screen_col.r * 0.299) + (screen_col.g * 0.587) + (screen_col.b * 0.114);

		// Adjust with contrast and offset parameters
		lum = (lum - 0.5 + u_offset) * u_contrast + 0.5;
		lum = clamp(lum, 0.0, 1.0);

		// Reduce luminosity bit depth
		float bits = float(max(1, u_bit_depth)); // Ensure bits is at least 1
		lum = floor(lum * bits) / bits;

		// Palette properties
		ivec2 col_size_normalized_h = palette_tex_dims;
		col_size_normalized_h.x = max(1, col_size_normalized_h.x); // Ensure at least 1xY
		col_size_normalized_h.y = max(1, col_size_normalized_h.y); // Ensure at least Xx1
		col_size_normalized_h /= col_size_normalized_h.y; // Conceptually make palette a 1px high strip

		float col_x = float(col_size_normalized_h.x) - 1.0; // Number of color boundaries
        if (col_x < 0.0) col_x = 0.0; // Handle single color palettes (col_x becomes 0)

		float col_texel_size = 0.0; // Size of one color band in UV space for palette
		if (col_x > 0.0) { // If more than one color in palette
			col_texel_size = 1.0 / col_x;
		}

		lum = max(lum - 0.00001, 0.0); // Nudge lum slightly for floor() behavior at 1.0
		
		// lum_scaled: How far lum is into its current quantized band [0,1)
		float lum_scaled = lum * col_x - floor(lum * col_x);
		// If col_x is 0 (single color palette), lum_scaled becomes 0.

		// lum_lower, lum_upper: UV coordinates for sampling the color palette
		float lum_lower = floor(lum * col_x) * col_texel_size;
		float lum_upper = (floor(lum * col_x) + 1.0) * col_texel_size;
        lum_upper = clamp(lum_upper, 0.0, 1.0); // Ensure upper bound is valid (e.g., not > 1.0)

		// Dither texture mapping
		vec2 inv_dither_tex_size = vec2(1.0); // Default if dither_tex_dims are invalid
        if (dither_tex_dims.x > 0 && dither_tex_dims.y > 0) {
            inv_dither_tex_size = vec2(1.0 / float(dither_tex_dims.x), 1.0 / float(dither_tex_dims.y));
        }
        // Map dither pattern based on the node's UVs, scaled by pixelation factor
		vec2 dither_uv = UV * inv_dither_tex_size * pixelated_block_count;

		float threshold = texture(u_dither_tex, dither_uv).r;
		threshold = threshold * 0.99 + 0.005; // Adjust threshold range

		// Pick lower or upper color from palette based on threshold
		float ramp_val = lum_scaled < threshold ? 0.0f : 1.0f;
		float col_sample_x = mix(lum_lower, lum_upper, ramp_val); // X-coordinate for palette lookup
        col_sample_x = clamp(col_sample_x, 0.0, 1.0); // Clamp final palette sample UV

		processed_color = texture(u_color_tex, vec2(col_sample_x, 0.5)).rgb;
	}
	else // Not all conditions for dithering met (e.g., invalid textures)
	{
		processed_color = screen_col; // Fallback to the original (pixelated) screen color
	}

	// --- 4. EXCLUSION LOGIC ---
	// Decide whether to use the original color or the processed (dithered/fallback) color.
	bool exclude_pixel = false;
	float high_threshold = 1.0 - u_exclusion_tolerance;
	float low_threshold = u_exclusion_tolerance;

	// Check for predominantly Red, Green, or Blue based on the (pixelated) original color
	if (screen_col.r >= high_threshold && screen_col.g <= low_threshold && screen_col.b <= low_threshold) {
		exclude_pixel = true;
	}
	else if (screen_col.g >= high_threshold && screen_col.r <= low_threshold && screen_col.b <= low_threshold) {
		exclude_pixel = true;
	}
	else if (screen_col.b >= high_threshold && screen_col.r <= low_threshold && screen_col.g <= low_threshold) {
		exclude_pixel = true;
	}

	if (exclude_pixel) {
		COLOR.rgb = screen_col; // Use original (pixelated) color
	} else {
		COLOR.rgb = processed_color; // Use dithered color (or fallback if dithering failed)
	}
	
	// --- 5. APPLY ORIGINAL ALPHA (from the node's own texture/appearance) ---
	COLOR.a = original_alpha;
}