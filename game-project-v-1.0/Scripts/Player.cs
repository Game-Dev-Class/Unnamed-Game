using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;

	private bool _canMove = true;
	private bool _facingRight = true;

	// From Script 1
	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

	// From Script 2
	[Export] public float PushForce = 15.0f; 

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
		if (!_canMove)
		{
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
		}

		// Gravity
		if (!IsOnFloor())
			Velocity += GetGravity() * (float)delta;

		// Jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

		// Movement input
		float direction = Input.GetAxis("ui_left", "ui_right");

		if (direction > 0)
			_facingRight = true;
		else if (direction < 0)
			_facingRight = false;

		if (direction != 0)
			Velocity = new Vector2(direction * SPEED, Velocity.Y);
		else
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);

		if (Input.IsActionJustPressed("whip"))
			SpawnWhip();

		// 1. Run the movement
		MoveAndSlide();

		// 2. Stomp Detection (From Script 1)
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

		// 3. Handle Pushing (From Script 2)
		HandlePushing();
	}

	private void HandlePushing()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			
			if (collision.GetCollider() is RigidBody2D block)
			{
				Vector2 pushDirection = new Vector2(-collision.GetNormal().X, 0);
				block.ApplyCentralImpulse(pushDirection * PushForce);
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
		
		GD.Print($"Trampoline Bounce! Final Force: {finalVelocityY}");
	}
}
