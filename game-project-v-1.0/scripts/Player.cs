using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;
	
	// Add this to control how much "oomph" your player has
	[Export] public float PushForce = 100.0f; 
	
	private bool _canMove = true;

	public void EnableMovement() => _canMove = true;
	public void DisableMovement()
	{
		_canMove = false;
		Velocity = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_canMove)
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			if (!IsOnFloor())
				Velocity += GetGravity() * (float)delta;

			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
				Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

			float direction = Input.GetAxis("ui_left", "ui_right");

			if (direction != 0)
				Velocity = new Vector2(direction * SPEED, Velocity.Y);
			else
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
		}

		// 1. Run the movement first
		MoveAndSlide();

		// 2. Handle Pushing (New Logic)
		HandlePushing();
	}

	private void HandlePushing()
	{
		// We check every object the player is currently touching
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			
			// Check if we hit a RigidBody2D
			if (collision.GetCollider() is RigidBody2D block)
			{
				// Push logic: We multiply the collision direction (Normal) 
				// by our force. We use negative normal because the normal 
				// points AWAY from the block.
				Vector2 pushDirection = -collision.GetNormal();
				
				// Optional: Stop the block from flying upward if you bump it
				pushDirection.Y = 0; 

				block.ApplyCentralImpulse(pushDirection * PushForce);
			}
		}
	}
	// Inside Player.cs

	public void Bounce(float force)
	{
		// Set the Y velocity to the bounce force
		// We use a new Vector2 to preserve horizontal momentum
		Velocity = new Vector2(Velocity.X, force);
	
		// Optional: Play a sound or animation here!
		GD.Print("Bounced!");
	}
}
