using Godot;
using System;

public partial class FallState : State
{
	public override void Enter()
		{
			GD.Print("Falling");

			var fsm = GetParent();
			var player = fsm?.GetParent() as PlayerCharacter;
			if (player == null)
				return;
		}

		public override void Update(float delta)
		{
			var fsm = GetParent();
			var player = fsm?.GetParent() as PlayerCharacter;
			if (player == null)
				return;

			Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

			Vector2 velocity = player.Velocity;
			velocity.X = direction.X * PlayerCharacter.Speed;
			player.Velocity = velocity;

			if (player.IsOnFloor())
			{

				if (Mathf.Abs(direction.X) > 0.1f)
					EmitSignal(SignalName.StateTransition, this, "WalkState");
				else
					EmitSignal(SignalName.StateTransition, this, "IdleState");
			}
		
			if (!player.IsOnFloor() && player.IsOnWall())
			{
				for (int i = 0; i < player.GetSlideCollisionCount(); i++)
				{
					KinematicCollision2D collision = player.GetSlideCollision(i);
					Vector2 normal = collision.GetNormal();

					if (normal.X > 0)
					{
						EmitSignal(SignalName.StateTransition, this, "WallSlideRight");
						return;
					}
					else
					{
						EmitSignal(SignalName.StateTransition, this, "WallSlideLeft");
						return;
					}
				}   
			}
		}

		public override void Exit() { }
}
