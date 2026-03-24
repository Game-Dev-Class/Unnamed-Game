using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;
	public int Health = 3;

	private bool _canMove = true;
	private bool _facingRight = true;
	private bool _isKnocked = false;
	private bool _isIFrames = false;

	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

	[Export] public float KnockbackHorizontal = 320f;
	[Export] public float KnockbackVertical = 260f;
	[Export] public float KnockbackDuration = 0.45f;
	[Export] public float IFramesDuration = 1f;

	[Export] public Area2D FeetArea;

	[Export] public float PushSpeed = 150.0f;

	[Signal] public delegate void HealthChangedEventHandler(int health);

	public override void _Ready()
	{
		if (PlayerSprite == null)
			PlayerSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

		if (FeetArea == null)
			FeetArea = GetNodeOrNull<Area2D>("FeetArea");

		if (FeetArea != null)
			FeetArea.BodyEntered += OnFeetBodyEntered;
	}

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
			MoveAndSlide();
			return;
		}

		if (!IsOnFloor())
			Velocity += GetGravity() * (float)delta;

		if (!_isKnocked && Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

		float direction = 0f;
		if (!_isKnocked)
			direction = Input.GetAxis("ui_left", "ui_right");

		if (direction > 0)
			_facingRight = true;
		else if (direction < 0)
			_facingRight = false;

		if (!_isKnocked)
		{
			if (direction != 0)
				Velocity = new Vector2(direction * SPEED, Velocity.Y);
			else
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
		}

		if (Input.IsActionJustPressed("whip"))
			SpawnWhip();

		MoveAndSlide();

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();
			Vector2 normal = collision.GetNormal();

			if (collider is Enemy normalEnemy)
			{
				if (normal.Y < -0.7f)
				{
					normalEnemy.QueueFree();
					continue;
				}

				if (Mathf.Abs(normal.X) > 0.7f)
					TakeEnemyHit(normalEnemy.GlobalPosition);
			}
			else if (collider is WhipEnemy whipEnemy)
			{
				if (!_isKnocked)
					TakeEnemyHit(whipEnemy.GlobalPosition);
			}

			if (collider is RigidBody2D block)
			{
				if (Mathf.Abs(normal.X) > 0.5f)
				{
					block.Sleeping = false;
					float pushDir = -normal.X;
					block.LinearVelocity = new Vector2(pushDir * PushSpeed, block.LinearVelocity.Y);
				}
			}
		}
	}

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

	private void SetInvulnerableCollision(bool enabled)
	{
		SetCollisionLayerValue(1, enabled);
		SetCollisionMaskValue(2, enabled);
		SetCollisionMaskValue(4, enabled);
	}

	public void TakeEnemyHit(Vector2 enemyPosition)
	{
		if (_isIFrames)
			return;

		GD.Print("damage taken");
		Health -= 1;
		EmitSignal(SignalName.HealthChanged, Health);

		if (Health <= 0)
		{
			PlayerDies();
			return;
		}

		if (_isKnocked)
			return;

		float dir = Mathf.Sign(GlobalPosition.X - enemyPosition.X);

		Velocity = new Vector2(dir * KnockbackHorizontal, -KnockbackVertical);

		_facingRight = dir > 0;
		_isKnocked = true;
		_isIFrames = true;

		SetInvulnerableCollision(false);

		var timer = GetTree().CreateTimer(KnockbackDuration);
		var iTimer = GetTree().CreateTimer(IFramesDuration);

		timer.Timeout += () => { _isKnocked = false; };
		iTimer.Timeout += () =>
		{
			_isIFrames = false;
			SetInvulnerableCollision(true);
		};
	}

	private void OnFeetBodyEntered(Node body)
	{
		if (body is Enemy normalEnemy)
		{
			normalEnemy.QueueFree();
			return;
		}

		if (body is WhipEnemy whipEnemy)
		{
			TakeEnemyHit(whipEnemy.GlobalPosition);
			return;
		}
	}

	private void PlayerDies()
	{
		QueueFree();
	}

	public void Bounce(float baseForce, float momentumMultiplier = 0.5f)
	{
		float fallingSpeed = Mathf.Max(0, Velocity.Y);
		float finalVelocityY = baseForce - (fallingSpeed * momentumMultiplier);
		Velocity = new Vector2(Velocity.X, finalVelocityY);
	}
}