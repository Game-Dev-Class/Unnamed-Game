using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
<<<<<<< HEAD:game-project-v-1.0/Scripts/Player.cs
	private const float JUMP_VELOCITY = -400.0f; // Updated from Script 2

=======
	private const float JUMP_VELOCITY = -350.0f;
	public int Health = 3;
>>>>>>> Combat:game-project-v-1.0/Player.cs
	private bool _canMove = true;
	private bool _facingRight = true;
	private bool _isKnocked = false;
	private bool _isIFrames = false;

	// --- Exports from Script 1 ---
	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

<<<<<<< HEAD:game-project-v-1.0/Scripts/Player.cs
	// --- Exports from Script 2 ---
	[Export] public float PushSpeed = 150.0f; 
=======
	// Knockback settings
	[Export] public float KnockbackHorizontal = 320f;
	[Export] public float KnockbackVertical = 260f;
	[Export] public float KnockbackDuration = 0.45f;
	[Export] public float IFramesDuration = 1f;

	// Feet area to reliably detect stomps on stationary enemies
	[Export] public Area2D FeetArea;

	[Signal] public delegate void HealthChangedEventHandler(int health);
>>>>>>> Combat:game-project-v-1.0/Player.cs

	public override void _Ready()
	{
		if (PlayerSprite == null)
			PlayerSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

		if (FeetArea == null)
			FeetArea = GetNodeOrNull<Area2D>("FeetArea");

		if (FeetArea != null)
			FeetArea.BodyEntered += OnFeetBodyEntered;
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

<<<<<<< HEAD:game-project-v-1.0/Scripts/Player.cs
		// Handle jump
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JUMP_VELOCITY;

		// Get input direction
		float direction = Input.GetAxis("ui_left", "ui_right");
		
		// Update facing direction for the whip and sprite
=======
		// Gravity
		if (!IsOnFloor())
			Velocity += GetGravity() * (float)delta;

		// Jump (disabled while knocked)
		if (!_isKnocked && Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

		// Movement input (disabled while knocked)
		float direction = 0f;
		if (!_isKnocked)
			direction = Input.GetAxis("ui_left", "ui_right");

>>>>>>> Combat:game-project-v-1.0/Player.cs
		if (direction > 0)
			_facingRight = true;
		else if (direction < 0)
			_facingRight = false;

<<<<<<< HEAD:game-project-v-1.0/Scripts/Player.cs
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
=======
		if (!_isKnocked)
		{
			if (direction != 0)
				Velocity = new Vector2(direction * SPEED, Velocity.Y);
			else
				Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, SPEED), Velocity.Y);
		}

		// Whip input
		if (Input.IsActionJustPressed("whip"))
			SpawnWhip();

		// Move and collect collisions
		MoveAndSlide();

		// Slide collisions handle side-hits (and top-hits if they appear here)
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();
			Vector2 normal = collision.GetNormal();

			// Normal enemy logic: top -> kill, side -> knockback
			if (collider is Enemy normalEnemy)
			{
				if (normal.Y < -0.7f)
>>>>>>> Combat:game-project-v-1.0/Player.cs
				{
					// stomp kill
					normalEnemy.QueueFree();
					continue;
				}

				if (Mathf.Abs(normal.X) > 0.7f)
					TakeEnemyHit(normalEnemy.GlobalPosition);
			}
			// WhipEnemy logic: any collision -> knockback (top or side)
			else if (collider is WhipEnemy whipEnemy)
			{
				// If already knocked, skip
				if (!_isKnocked)
					TakeEnemyHit(whipEnemy.GlobalPosition);
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

	// Keep sprite flipping in Process (after physics/animation)
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

<<<<<<< HEAD:game-project-v-1.0/Scripts/Player.cs
	public void Bounce(float baseForce, float momentumMultiplier = 0.5f)
	{
		float fallingSpeed = Mathf.Max(0, Velocity.Y);
		float finalVelocityY = baseForce - (fallingSpeed * momentumMultiplier);
		Velocity = new Vector2(Velocity.X, finalVelocityY);
	}
}
=======
	private void SetInvulnerableCollision(bool enabled)
	{
		SetCollisionLayerValue(1, enabled); // enemies won't detect the player
		SetCollisionMaskValue(2, enabled);  // player won't collide with enemy layer 2
		SetCollisionMaskValue(4, enabled);  // player won't collide with enemy layer 4
	}

	// External call (enemy hits player) or internal use (side/top collisions)
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

	// Feet area callback: stomps on stationary enemies
	private void OnFeetBodyEntered(Node body)
	{
		// If player stomps a normal enemy -> kill
		if (body is Enemy normalEnemy)
		{
			normalEnemy.QueueFree();
			return;
		}

		// If player stomps a whip-only enemy -> knockback (player gets knocked)
		if (body is WhipEnemy whipEnemy)
		{
			TakeEnemyHit(whipEnemy.GlobalPosition);
			return;
		}

		// other bodies ignored here
	}

	private void PlayerDies()
	{
		QueueFree();
	}
}
>>>>>>> Combat:game-project-v-1.0/Player.cs
