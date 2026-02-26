using Godot;

public partial class Player : CharacterBody2D
{
	private const float SPEED = 300.0f;
	private const float JUMP_VELOCITY = -350.0f;

	private bool _canMove = true;
	private bool _facingRight = true;

	[Export] public PackedScene WhipScene;
	[Export] public float DefaultWhipDistance = 48.0f;
	[Export] public Range WhipDistanceSlider;
	[Export] public AnimatedSprite2D PlayerSprite;

	public override void _Ready()
	{
		// auto-find sprite if not assigned
		if (PlayerSprite == null)
			PlayerSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
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

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			Velocity = new Vector2(Velocity.X, JUMP_VELOCITY);

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

		MoveAndSlide();
	}

	// IMPORTANT: flip AFTER animation updates
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
}