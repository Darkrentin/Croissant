shader_type canvas_item;

// Paramètres de la transition
uniform float progress : hint_range(0.0, 1.0) = 0.0; // Progression de la transition (0 = début, 1 = fin)
uniform vec4 color : source_color = vec4(0.0, 0.0, 0.0, 1.0); // Couleur du ColorRect
uniform vec2 center = vec2(0.5, 0.5); // Centre du cercle (par défaut au milieu de l'écran)

void fragment() {
    // Calcul de la distance entre le pixel et le centre du cercle
    vec2 uv = SCREEN_UV;
    vec2 centered_uv = uv - center;
    
    // Normalisation en fonction de la taille du ColorRect
    centered_uv.x *= SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
    
    float dist = length(centered_uv);
    
    // Rayon maximum nécessaire pour couvrir tout l'écran
    float max_radius = length(vec2(0.5, 0.5 * SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x));
    
    // Rayon actuel du cercle en fonction de la progression
    float current_radius = progress * max_radius * 1.5; // Un peu plus grand pour s'assurer de tout révéler
    
    // Si la distance est inférieure au rayon actuel, rendre transparent
    // Sinon, afficher la couleur du ColorRect
    if (dist < current_radius) {
        // Rendre cette partie transparente
        COLOR = vec4(0.0, 0.0, 0.0, 0.0);
    } else {
        // Afficher la couleur du ColorRect
        COLOR = color;
    }
}
