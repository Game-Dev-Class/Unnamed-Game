using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;
	
	[Export] public float PushForce = 15.0f; // Godot 4 uses different force scaling; start low
	
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
			// Add gravity
			if (!IsOnFloor())
				Velocity += GetGravity() * (float)delta;

			// Handle jump
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
				Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

			// Horizontal movement
			float direction = Input.GetAxis("ui_left", "ui_right");
			if (direction != 0)
				Velocity = new Vector2(direction * SPEED, Velocity.Y);
			else
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
		}

		// 1. Run the movement
		MoveAndSlide();

		// 2. Handle Pushing
		HandlePushing();
	}

	private void HandlePushing()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			
			if (collision.GetCollider() is RigidBody2D block)
			{
				// We only push horizontally to avoid the "rocket" air effect
				Vector2 pushDirection = new Vector2(-collision.GetNormal().X, 0);

				// ApplyImpulse is great for one-time kicks; 
				// ApplyCentralForce is better if you want a heavy 'leaning' push
				block.ApplyCentralImpulse(pushDirection * PushForce);
			}
		}
	}

	/// <summary>
	/// Called by the BouncePad. 
	/// momentumMultiplier (0.0 to 1.0) determines how much fall speed is kept.
	/// </summary>
	public void Bounce(float baseForce, float momentumMultiplier = 0.5f)
	{
		// Capture current downward speed (positive Y is down)
		float fallingSpeed = Mathf.Max(0, Velocity.Y);
		
		// Calculate final bounce: The base jump + a percentage of how fast we fell
		// We use negative because Up is negative Y in Godot
		float finalVelocityY = baseForce - (fallingSpeed * momentumMultiplier);
		
		Velocity = new Vector2(Velocity.X, finalVelocityY);
		
		GD.Print($"Trampoline Bounce! Final Force: {finalVelocityY}");
	}
}
