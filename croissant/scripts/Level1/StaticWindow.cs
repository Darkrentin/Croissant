using Godot;
using System;

[Tool]
public partial class StaticWindow : PopUpWindow
{

    [Export] public CompressedTexture2D Texture1;
    [Export] public CompressedTexture2D Texture2;

    [Export] public TextureRect textureRect;

    [Export] public bool SetText1{
        get
        {
            return false;
        }
        set
        {
            if (value)
            {
                SetTexture1();
            }
        }
    }
    [Export] public bool SetText2
    {
        get
        {
            return false;
        }
        set
        {
            if (value)
            {
                SetTexture2();
            }
        }
    }

    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 300));
        int rand = Lib.rand.Next(0, 3);
    }
    public void SetTexture1()
    {
        Lib.Print("SetTexture1");
        textureRect.Texture = Texture1;
    }

    public void SetTexture2()
    {
        Lib.Print("SetTexture2");
        textureRect.Texture = Texture2;
    }

    public override void OnClose()
    {
        Level1.WindowKill();
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
