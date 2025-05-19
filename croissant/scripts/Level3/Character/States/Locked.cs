using Godot;
using System;

public partial class Locked : PlayerState
{
    // Mémoire de la direction du mur (nue)
    private Vector2 stuckWallDir;

    public override void EnterState()
    {
        Name = "Locked";

        Player._hasJumped = false;

        // Arrêt horizontal + petite poussée pour rester collé
        Player.Velocity = new Vector2(0, PlayerCharacter.GravityWallSlide*3);

        Player.jumps = PlayerCharacter.MaxJumps;

        // On enregistre immédiatement la direction du mur
        Player.GetWallDirection();
        stuckWallDir = Player.wallDirection;

        // **On vide les états « just pressed » pour ne pas déclencher**
        // Godot ne permet pas de réinitialiser Input, mais on peut
        // consommer le prochain appui en ignorant tout IsActionPressed()
        // jusqu’à un nouveau JustPressed().
    }

    public override void ExitState()
    {
        // Rien à faire
    }

    public override void Update(double delta)
    {
        // Mise à jour de la direction murale
        Player.GetWallDirection();
        var dir = Player.wallDirection;

        // Descente lente
        Player.HandleGravity(delta, PlayerCharacter.GravityWallSlide);

        // Animation
        Player.Animator.Play("WallSlideLeft");
        Player.Sprite.FlipH = (dir == Vector2.Right);

        // **Wall jump sur NOUVELLE pression** :
        // - Nouvelle pression SAUT
        // - Nouvelle pression de direction opposée
        if (Input.IsActionJustPressed("ui_up"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }
        if (stuckWallDir == Vector2.Left && Input.IsActionJustPressed("ui_right"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }
        if (stuckWallDir == Vector2.Right && Input.IsActionJustPressed("ui_left"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }

        // Dès qu’on touche un sol ou qu’on lâche le mur, on tombe
        if (dir == Vector2.Zero || Player.IsOnFloor())
        {
            Player.ChangeState((Node)States.Get("Fall"));
            return;
        }
    }
}
