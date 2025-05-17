shader_type canvas_item;

// Paramètres pour le terminal de suppression
uniform vec4 background_color : source_color = vec4(0.05, 0.05, 0.06, 0.9); // Fond très sombre
uniform vec4 text_color : source_color = vec4(0.9, 0.2, 0.2, 1.0); // Rouge danger
uniform vec4 highlight_color : source_color = vec4(1.0, 0.4, 0.0, 1.0); // Orange pour alertes
uniform float time_scale : hint_range(0.1, 3.0) = 1.0;
uniform float deletion_progress : hint_range(0.0, 1.0) = 0.7; // Avancement visuel du processus
uniform float darkness : hint_range(0.0, 1.0) = 0.8; // Contrôle l'obscurité globale
uniform float transparency : hint_range(0.5, 1.0) = 0.92; // Transparence globale

// Fonction aléatoire
float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

// Crée un effet de texte de terminal
float terminal_line(vec2 uv, float line_number, float seed) {
    float line_height = 0.023;
    float y_pos = line_number * line_height;
    
    // Si on est sur cette ligne
    if (uv.y >= y_pos && uv.y < y_pos + line_height * 0.8) {
        // Blinking cursor at line end
        float cursor_x = 0.1 + mod(floor(seed * 30.0), 50.0) * 0.015;
        float cursor = step(cursor_x - 0.015, uv.x) * step(uv.x, cursor_x);
        
        // Text simulation
        float chars = 0.0;
        for (float i = 0.0; i < 15.0; i++) {
            float char_x = 0.1 + i * 0.015;
            if (char_x > cursor_x) break;
            
            float char_width = 0.01;
            float is_char = step(0.4, random(vec2(i, line_number + seed * 31.37)));
            
            chars += is_char * step(abs(uv.x - char_x), char_width * 0.5);
        }
        
        // Pulsating effect for active line
        float active_line = step(abs(seed * 20.0 - line_number), 1.0);
        float pulse = (sin(seed * 20.0) * 0.5 + 0.5) * active_line;
        
        return max(chars, cursor * pulse);
    }
    return 0.0;
}

// Crée une barre de progression
float progress_bar(vec2 uv, float progress, float y_position) {
    // Position et taille de la barre
    float bar_height = 0.025;
    float bar_width = 0.5;
    
    // Si on est au niveau de la barre
    if (uv.y > y_position && uv.y < y_position + bar_height &&
        uv.x > 0.25 && uv.x < 0.25 + bar_width) {
        
        // Bordure
        float border = 0.002;
        if (uv.y < y_position + border || uv.y > y_position + bar_height - border ||
            uv.x < 0.25 + border || uv.x > 0.25 + bar_width - border) {
            return 0.5; // Bordure un peu transparente
        }
        
        // Remplissage de la barre selon le progrès
        if (uv.x < 0.25 + bar_width * progress) {
            // Animation de pulsation pour la partie remplie
            float pulse = sin(uv.x * 20.0 + TIME * 5.0) * 0.1 + 0.9;
            return pulse;
        } else {
            // Motif pour la partie vide
            float pattern = step(0.5, fract((uv.x - uv.y) * 50.0));
            return pattern * 0.2;
        }
    }
    
    return 0.0;
}

// Génère un message de danger
float warning_message(vec2 uv, float time) {
    // Position et taille du message
    float y_position = 0.4;
    float height = 0.1;
    
    if (uv.y > y_position && uv.y < y_position + height) {
        // Effet clignotant rapide
        float blink = step(0.5, fract(time * 2.0));
        
        // Pattern du message d'alerte
        float warning_text = 0.0;
        
        // Texte "DELETION" simplifié
        if (abs(uv.y - y_position - height * 0.5) < height * 0.3) {
            float xoffset = 0.0;
            
            // D
            xoffset = 0.3;
            if (uv.x > xoffset && uv.x < xoffset + 0.02) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.3) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.7) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            if (abs(uv.x - xoffset - 0.04) < 0.01 && uv.y > y_position + height * 0.3 && uv.y < y_position + height * 0.7) warning_text = 1.0;
            
            // E
            xoffset = 0.35;
            if (uv.x > xoffset && uv.x < xoffset + 0.02) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.3) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.5) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.03) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.7) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            
            // L
            xoffset = 0.4;
            if (uv.x > xoffset && uv.x < xoffset + 0.02) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.7) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            
            // E
            xoffset = 0.45;
            if (uv.x > xoffset && uv.x < xoffset + 0.02) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.3) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.5) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.03) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.7) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            
            // T
            xoffset = 0.5;
            if (abs(uv.x - xoffset - 0.02) < 0.01) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.3) < 0.01 && uv.x > xoffset - 0.02 && uv.x < xoffset + 0.06) warning_text = 1.0;
            
            // I
            xoffset = 0.57;
            if (abs(uv.x - xoffset - 0.01) < 0.01) warning_text = 1.0;
            
            // O
            xoffset = 0.62;
            if (abs(uv.x - xoffset) < 0.02 && abs(uv.y - y_position - height * 0.5) > height * 0.1) warning_text = 1.0;
            if (abs(uv.x - xoffset - 0.04) < 0.01 && abs(uv.y - y_position - height * 0.5) > height * 0.1) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.3) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            if (abs(uv.y - y_position - height * 0.7) < 0.01 && uv.x > xoffset && uv.x < xoffset + 0.04) warning_text = 1.0;
            
            // N
            xoffset = 0.67;
            if (abs(uv.x - xoffset) < 0.01) warning_text = 1.0;
            if (abs(uv.x - xoffset - 0.04) < 0.01) warning_text = 1.0;
            if (abs(uv.x - uv.y * 0.5 - xoffset + 0.2) < 0.01 && uv.y > y_position + height * 0.3 && uv.y < y_position + height * 0.7) warning_text = 1.0;
        }
        
        return warning_text * blink;
    }
    
    return 0.0;
}

// Création de bordure de fenêtre
float window_border(vec2 uv) {
    float border_width = 0.02;
    float titlebar_height = 0.05;
    
    // Titre et bordures
    if (uv.x < border_width || uv.x > 1.0 - border_width || 
        uv.y < titlebar_height || uv.y > 1.0 - border_width) {
        
        // Texture nervurée pour la barre de titre
        if (uv.y < titlebar_height) {
            float pattern = step(0.5, fract(uv.x * 30.0));
            return 0.3 + pattern * 0.1;
        }
        
        return 0.4; // Bordure standard
    }
    
    return 0.0;
}

// Fragment shader principal
void fragment() {
    float time = TIME * time_scale;
    
    // Application d'un léger tremblement/glitch à la texture
    vec2 uv = SCREEN_UV;
    
    // Effet de glitch aléatoire sur certaines lignes
    float line_glitch = floor(uv.y * 40.0);
    if (random(vec2(floor(time * 5.0), line_glitch)) < 0.1) {
        float glitch_amount = (random(vec2(time, line_glitch)) - 0.5) * 0.03;
        uv.x += glitch_amount;
    }
    
    // Glitch intense périodique
    if (fract(time * 0.5) < 0.05) {
        float strong_glitch = (random(vec2(floor(uv.y * 20.0), floor(time * 20.0))) - 0.5) * 0.2;
        uv.x += strong_glitch;
    }
    
    // Couleur de base (sombre)
    vec4 color = background_color;
    color.a *= transparency;
    
    // Bordures de fenêtre
    float border = window_border(uv);
    if (border > 0.0) {
        color = mix(color, vec4(0.15, 0.15, 0.15, 0.95), border);
    }
    
    // Simulation de lignes de terminal
    float text = 0.0;
    for (float i = 0.0; i < 15.0; i++) {
        text += terminal_line(uv, i, time + i * 0.37);
    }
    
    // Message d'alerte clignotant
    float warning = warning_message(uv, time);
    
    // Barre de progression
    float progress = deletion_progress * (0.8 + 0.2 * sin(time * 0.5)); // Animation légère
    float bar = progress_bar(uv, progress, 0.6);
    
    // Motif hexagonal de fond (suggestion de fragmentation de données)
    vec2 hex_uv = uv * vec2(10.0, 17.32);
    hex_uv.x += mod(floor(hex_uv.y), 2.0) * 0.5;
    vec2 hex_id = floor(hex_uv);
    vec2 hex_center = fract(hex_uv) - 0.5;
    float hex_dist = length(hex_center) * 2.0;
    float hex_pattern = smoothstep(0.9, 1.0, hex_dist) * 0.15;
    
    // Animation de suppression/fragmentation
    float deletion_effect = 0.0;
    if (random(vec2(hex_id)) < deletion_progress) {
        float flicker = sin(time * 10.0 + random(hex_id) * 6.28) * 0.5 + 0.5;
        deletion_effect = flicker * 0.5;
    }
    
    // Application du pattern hexagonal et effet de suppression
    color = mix(color, highlight_color * 0.2, hex_pattern + deletion_effect);
    
    // Superposition du texte terminal
    color = mix(color, text_color, text * 0.8);
    
    // Superposition du message d'alerte
    color = mix(color, highlight_color, warning);
    
    // Superposition de la barre de progression
    color = mix(color, highlight_color, bar);
    
    // Effet de fragmentation apériodique 
    if (random(vec2(floor(uv.x * 30.0), floor(uv.y * 30.0)) + floor(time)) < deletion_progress * 0.3) {
        color.rgb = mix(color.rgb, vec3(0.0), 0.7); // Blocs noirs pour simulation de corruption
    }
    
    // Lignes de scan occasionnelles
    float scan_line = step(0.98, fract(uv.y * 20.0 - time * 2.0)) * 0.5;
    color.rgb += vec3(scan_line);
    
    // Vibration globale d'écran quand la suppression est presque terminée
    if (deletion_progress > 0.8) {
        float shake = sin(time * 20.0) * 0.01;
        if (uv.y + shake > 0.0 && uv.y + shake < 1.0) {
            uv.y += shake;
        }
    }
    
    // Assombrissement global basé sur le paramètre darkness
    color.rgb *= (1.0 - darkness * 0.5);
    
    COLOR = color;
}
