using Godot;
using System.Collections.Generic;

public partial class ShaderLoader : Node
{
    private bool loadingComplete = false;
    private List<Node> tempObjects = new List<Node>();

    public override void _Ready()
    {
        Lib.Print("Démarrage du chargement des shaders et particules...");
        LoadAllShadersAndParticles();
    }

    private void LoadAllShadersAndParticles()
    {
        var shaderPaths = new List<string>
        {
            "res://assets/shaders/3DDithering.gdshader",
            "res://assets/shaders/ChromaticAberration.gdshader", 
            "res://assets/shaders/CombinedGandC.gdshader",
            "res://assets/shaders/Disolve.gdshader",
            "res://assets/shaders/Dithering.gdshader",
            "res://assets/shaders/Indicator.gdshader",
            "res://assets/shaders/IntroGame.gdshader",
            "res://assets/shaders/Lava.gdshader",
            "res://assets/shaders/Melt.gdshader",
            "res://assets/shaders/PlainHighlight.gdshader",
            "res://assets/shaders/PlatformHighlight.gdshader",
            "res://assets/shaders/TriplanarNoise.gdshader",
            "res://assets/shaders/VHS.gdshader"
        };

        var particlePaths = new List<string>
        {
            "res://scenes/FinalLevel/Particles/BulletHit.tscn",
            "res://scenes/FinalLevel/Particles/ObjectiveExplosion.tscn",
            "res://scenes/FinalLevel/Particles/EnemyHit.tscn",
            "res://scenes/FinalLevel/Particles/EnemyExplosion.tscn",
            "res://scenes/Other/WindowExplosion.tscn",
            "res://scenes/Intro/GameExplosion.tscn",
            "res://scenes/Npc/Virus/VirusSplash.tscn"
        };

        Lib.Print($"Chargement de {shaderPaths.Count} shaders...");

        // Charger les shaders
        foreach (string shaderPath in shaderPaths)
        {
            try
            {
                var shader = GD.Load<Shader>(shaderPath);
                if (shader != null)
                {
                    // Forcer la compilation en créant un matériau temporaire
                    var material = new ShaderMaterial();
                    material.Shader = shader;
                    
                    // Créer un mesh temporaire pour forcer la compilation
                    var meshInstance = new MeshInstance3D();
                    var quadMesh = new QuadMesh();
                    meshInstance.Mesh = quadMesh;
                    meshInstance.MaterialOverride = material;
                    
                    // Ajouter temporairement à la scène pour forcer la compilation
                    AddChild(meshInstance);
                    tempObjects.Add(meshInstance);
                    
                    Lib.Print($"Shader chargé: {shaderPath}");
                }
            }
            catch (System.Exception e)
            {
                Lib.Print($"Erreur lors du chargement du shader {shaderPath}: {e.Message}");
            }
        }

        Lib.Print($"Chargement de {particlePaths.Count} particules...");

        // Charger les particules
        foreach (string particlePath in particlePaths)
        {
            try
            {
                var particleScene = GD.Load<PackedScene>(particlePath);
                if (particleScene != null)
                {
                    // Instancier temporairement pour initialiser
                    var particleInstance = particleScene.Instantiate();
                    AddChild(particleInstance);
                    tempObjects.Add(particleInstance);
                    
                    Lib.Print($"Particule chargée: {particlePath}");
                }
            }
            catch (System.Exception e)
            {
                Lib.Print($"Erreur lors du chargement de la particule {particlePath}: {e.Message}");
            }
        }

        loadingComplete = true;
        Lib.Print("Chargement des shaders et particules terminé !");
        
        // Se supprimer après un frame pour laisser le temps aux objets de s'initialiser
        CallDeferred(nameof(CleanupAndDestroy));
    }

    private void CleanupAndDestroy()
    {
        // Nettoyer tous les objets temporaires
        foreach (Node obj in tempObjects)
        {
            if (IsInstanceValid(obj))
            {
                obj.QueueFree();
            }
        }
        tempObjects.Clear();

        Lib.Print("ShaderLoader supprimé après chargement complet");
        
        // Se supprimer
        QueueFree();
    }
}