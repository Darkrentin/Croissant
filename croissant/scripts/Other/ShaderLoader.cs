using Godot;
using System.Collections.Generic;

public partial class ShaderLoader : Node
{
    private bool loadingComplete = false;
    private List<Node> tempObjects = new List<Node>();

    public override void _Ready()
    {
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

        foreach (string shaderPath in shaderPaths)
        {
            try
            {
                var shader = GD.Load<Shader>(shaderPath);
                if (shader != null)
                {
                    var material = new ShaderMaterial();
                    material.Shader = shader;

                    var meshInstance = new MeshInstance3D();
                    var quadMesh = new QuadMesh();
                    meshInstance.Mesh = quadMesh;
                    meshInstance.MaterialOverride = material;
                    AddChild(meshInstance);
                    tempObjects.Add(meshInstance);
                }
            }
            catch (System.Exception)
            {

            }
        }

        foreach (string particlePath in particlePaths)
        {
            try
            {
                var particleScene = GD.Load<PackedScene>(particlePath);
                if (particleScene != null)
                {
                    var particleInstance = particleScene.Instantiate();
                    AddChild(particleInstance);
                    tempObjects.Add(particleInstance);
                }
            }
            catch (System.Exception)
            {

            }
        }
        loadingComplete = true;
        CallDeferred(nameof(CleanupAndDestroy));
    }

    private void CleanupAndDestroy()
    {
        foreach (Node obj in tempObjects)
        {
            if (IsInstanceValid(obj))
            {
                obj.QueueFree();
            }
        }
        tempObjects.Clear();
        QueueFree();
    }
}
