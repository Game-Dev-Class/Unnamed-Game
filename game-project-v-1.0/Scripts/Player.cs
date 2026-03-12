using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -400.0f; // Updated from Script 2

	private bool _canMove = true;
	private bool _facingRight = true;

	// --- Exports from Script 1 ---
	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

	// --- Exports from Script 2 ---
	[Export] public float PushSpeed = 150.0f; 

	public override void _Ready()
	{
		if (PlayerSprite == null)
			PlayerSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void EnableMovement() => _canMove = true;
	
	public void DisableMovement()
	{
		_canMove = false;
		Velocity = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Even if movement is disabled, we still want gravity to apply
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
		
		// Update facing direction for the whip and sprite
		if (direction > 0)
			_facingRight = true;
		else if (direction < 0)
			_facingRight = false;

		if (direction != 0)
			velocity.X = direction * SPEED;
		else
			velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);

		// Whip logic from Script 1
		if (Input.IsActionJustPressed("whip"))
			SpawnWhip();

		Velocity = velocity;
		
		MoveAndSlide();

		// --- Stomp Detection (From Script 1) ---
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			if (collision.GetCollider() is Enemy enemy)
			{
				if (collision.GetNormal().Y < -0.7f)
				{
					enemy.QueueFree();
				}
			}
		}

		// --- Handle Pushing (From Script 2) ---
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

	// Flip sprite AFTER movement
	public override void _Process(double delta)
	{
		if (PlayerSprite != null)
			PlayerSprite.FlipH = !_facingRight;
	}

	private void SpawnWhip()
	{
		if (WhipScene == null)
			return;

		float distance = DefaultWhipDistance;
		if (WhipDistanceSlider != null)
			distance = (float)WhipDistanceSlider.Value;

		var whipNode = WhipScene.Instantiate<Node2D>();
		GetParent().AddChild(whipNode);

		float dir = _facingRight ? 1f : -1f;
		whipNode.GlobalPosition = GlobalPosition + new Vector2(distance * dir, 0);

		whipNode.Scale = new Vector2(_facingRight ? -1f : 1f, 1f);
	}

	public void Bounce(float baseForce, float momentumMultiplier = 0.5f)
	{
		float fallingSpeed = Mathf.Max(0, Velocity.Y);
		float finalVelocityY = baseForce - (fallingSpeed * momentumMultiplier);
		Velocity = new Vector2(Velocity.X, finalVelocityY);
	}
}
