using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;

	[Export] public int MaxHealth = 3;
	public int Health { get; private set; }

	private bool _canMove = true;
	private bool _facingRight = true;
	private bool _isKnocked = false;
	private bool _isIFrames = false;
	private bool _canWhip = true;

	public int _killCount;

	[ExportGroup("Scenes and Nodes")]
	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;
	[Export] public Area2D FeetArea;

	[ExportGroup("Audio")]
	[Export] public AudioStreamPlayer2D JumpSound;
	[Export] public AudioStreamPlayer2D WhipSound;
	[Export] public AudioStreamPlayer2D HurtSound;
	[Export] public AudioStreamPlayer2D EnemyDeathSound;

	[ExportGroup("Combat Mechanics")]
	[Export] public float WhipCooldown = 0.5f;
	[Export] public float KnockbackHorizontal = 320f;
	[Export] public float KnockbackVertical = 260f;
	[Export] public float KnockbackDuration = 0.45f;
	[Export] public float IFramesDuration = 1f;
	[Export] public float PushSpeed = 150.0f;

	[Signal] public delegate void HealthChangedEventHandler(int health);

	public override void _Ready()
	{
		Health = MaxHealth;
		EmitSignal(SignalName.HealthChanged, Health);

		PlayerSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		FeetArea ??= GetNodeOrNull<Area2D>("FeetArea");

		JumpSound ??= GetNodeOrNull<AudioStreamPlayer2D>("JumpSound");
		WhipSound ??= GetNodeOrNull<AudioStreamPlayer2D>("WhipSound");
		HurtSound ??= GetNodeOrNull<AudioStreamPlayer2D>("HurtSound");
		EnemyDeathSound ??= GetNodeOrNull<AudioStreamPlayer2D>("EnemyDeathSound");

		if (FeetArea != null)
			FeetArea.BodyEntered += OnFeetBodyEntered;
	}

	public void EnableMovement() => _canMove = true;
	public void DisableMovement()
	{
		_canMove = false;
		Velocity = Vector2.Zero;
	}
	public bool GetCanMove() => _canMove;

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

		// --- JUMP ---
		if (!_isKnocked && Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);
			JumpSound?.Play();
		}

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

		// --- WHIP ---
		if (Input.IsActionJustPressed("whip") && _canWhip)
			SpawnWhip();

		MoveAndSlide();

		// --- COLLISIONS ---
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();
			Vector2 normal = collision.GetNormal();

			if (collider is Enemy normalEnemy)
			{
				if (normal.Y < -0.7f)
				{
					KillEnemy(normalEnemy);
					Bounce(350);
					_killCount += 1;
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
			else if (collider is Boss bossEnemy)
			{
				if (normal.Y < -0.7f)
				{
					KillEnemy(bossEnemy);
					Bounce(350);
					continue;
				}
				if (Mathf.Abs(normal.X) > 0.7f)
					TakeEnemyHit(bossEnemy.GlobalPosition);
			}
			else if (collider is Spike spike)
			{
				PlayerDies();
				return;
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

		_canWhip = false;
		WhipSound?.Play();

		float distance = DefaultWhipDistance;
		if (WhipDistanceSlider != null)
			distance = (float)WhipDistanceSlider.Value;

		var whipNode = WhipScene.Instantiate<Node2D>();
		GetParent().AddChild(whipNode);

		float dir = _facingRight ? 1f : -1f;
		whipNode.GlobalPosition = GlobalPosition + new Vector2(distance * dir, 0);
		whipNode.Scale = new Vector2(_facingRight ? -1f : 1f, 1f);

		var timer = GetTree().CreateTimer(WhipCooldown);
		timer.Timeout += () => _canWhip = true;
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

		HurtSound?.Play();
		Health = Mathf.Max(Health - 1, 0);
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
			KillEnemy(normalEnemy);
			Bounce(350);
			return;
		}

		if (body is WhipEnemy whipEnemy)
		{
			TakeEnemyHit(whipEnemy.GlobalPosition);
			return;
		}

		if (body is Boss bossEnemy)
		{
			KillEnemy(bossEnemy);
			Bounce(350);
			return;
		}

		if (body is Spike spike)
		{
			PlayerDies();
			return;
		}
	}

	private void KillEnemy(Node enemy)
	{
		EnemyDeathSound?.Play();
		enemy.QueueFree();
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