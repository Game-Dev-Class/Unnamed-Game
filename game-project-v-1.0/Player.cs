using Godot;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;

	private bool _canMove = true;
	private bool _facingRight = true;
	private bool _isKnocked = false;

	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Godot.Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

	// Knockback settings
	[Export] public float KnockbackHorizontal = 320f;
	[Export] public float KnockbackVertical = 260f;
	[Export] public float KnockbackDuration = 0.45f;

	// Feet area to reliably detect stomps on stationary enemies
	[Export] public Area2D FeetArea;

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
				{
					// stomp kill
					normalEnemy.QueueFree();
					continue;
				}

				if (Mathf.Abs(normal.X) > 0.7f)
				{
					TakeEnemyHit(normalEnemy.GlobalPosition);
				}
			}
			// WhipEnemy logic: any collision -> knockback (top or side)
			else if (collider is WhipEnemy whipEnemy)
			{
				// If already knocked, skip
				if (!_isKnocked)
					TakeEnemyHit(whipEnemy.GlobalPosition);
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

	// External call (enemy hits player) or internal use (side/top collisions)
	public void TakeEnemyHit(Vector2 enemyPosition)
	{
		if (_isKnocked)
			return;

		float dir = Mathf.Sign(GlobalPosition.X - enemyPosition.X);

		Velocity = new Vector2(dir * KnockbackHorizontal, -KnockbackVertical);

		_facingRight = dir > 0;
		_isKnocked = true;

		var timer = GetTree().CreateTimer(KnockbackDuration);
		timer.Timeout += () => { _isKnocked = false; };
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
}