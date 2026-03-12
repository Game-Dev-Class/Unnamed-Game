using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -400.0f;
	
	[Export] public float PushSpeed = 150.0f; 
	
	private bool _canMove = true;

	// --- ADDED THESE BACK TO FIX YOUR COMPILER ERRORS ---
	public void EnableMovement() => _canMove = true;
	public void DisableMovement()
	{
		_canMove = false;
		Velocity = Vector2.Zero;
	}
	// ----------------------------------------------------

	public override void _PhysicsProcess(double delta)
	{
		// Even if movement is disabled, we still want gravity to apply
		// so the player doesn't float in mid-air.
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		if (!_canMove)
		{
			velocity.X = 0; // Stop horizontal movement
			Velocity = velocity;
			MoveAndSlide();
			return;
		}

		// Handle jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JUMP_VELOCITY;

		// Get input direction
		float direction = Input.GetAxis("ui_left", "ui_right");
		if (direction != 0)
			velocity.X = direction * SPEED;
		else
			velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);

		Velocity = velocity;
		
		MoveAndSlide();
		HandlePushing();
	}

	private void HandlePushing()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			Node2D collider = (Node2D)collision.GetCollider();

			if (collider is RigidBody2D block)
			{
				if (Mathf.Abs(collision.GetNormal().X) > 0.5f)
				{
					block.Sleeping = false;
					float pushDir = -collision.GetNormal().X; 
					block.LinearVelocity = new Vector2(pushDir * PushSpeed, block.LinearVelocity.Y);
				}
			}
		}
	}

	public void Bounce(float baseForce, float momentumMultiplier = 0.5f)
	{
		float fallingSpeed = Mathf.Max(0, Velocity.Y);
		float finalVelocityY = baseForce - (fallingSpeed * momentumMultiplier);
		Velocity = new Vector2(Velocity.X, finalVelocityY);
	}
}
