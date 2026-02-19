using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;
	private bool _canMove = true; // Set to true so you can move by default
	
	public void EnableMovement()
	{
		_canMove = true;
	}
	
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
			// Add the gravity.
			if (!IsOnFloor())
			{
				Velocity += GetGravity() * (float)delta;
			}
	
			// Handle jump.
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			{
				Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
			}
	
			// Get input direction.
			float direction = Input.GetAxis("ui_left", "ui_right");
	
			if (direction != 0)
			{
				Velocity = new Vector2(direction * SPEED, Velocity.Y);
			}
			else
			{
				Velocity = new Vector2(
					Mathf.MoveToward(Velocity.X, 0, SPEED),
					Velocity.Y
				);
			}
		}

		MoveAndSlide();
	}
}
