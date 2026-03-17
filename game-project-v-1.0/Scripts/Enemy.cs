using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public AnimatedSprite2D EnemySprite;

    private bool _movingRight = true;

    private bool _canMove;

    public void EnableMovement() => _canMove = true;

    public void DisableMovement()
	{
		_canMove = false;
		Velocity = Vector2.Zero;
	}

    public override void _Ready()
    {
        if (EnemySprite == null)
            EnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Apply gravity (same as player)
        if (!IsOnFloor())
            Velocity += GetGravity() * (float)delta;

        if (!_canMove)
		{
			// Stop horizontal movement
			Velocity = Vector2.Zero;
			MoveAndSlide();
			return;
        }

        // Move only while on floor
        if (IsOnFloor())
        {
            float direction = _movingRight ? 1f : -1f;
            Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);
        }
        else
        {
            Velocity = new Vector2(0, Velocity.Y);
        }

        MoveAndSlide();

        // Check collisions to turn around
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);

            if (collision.GetCollider() is PhysicsBody2D collider)
            {
                // Check if collider is on Layer 3
                if ((collider.CollisionLayer & (1 << 2)) != 0)
                {
                    // Make sure it was a side collision
                    if (Mathf.Abs(collision.GetNormal().X) > 0.7f)
                    {
                        _movingRight = !_movingRight;
                        break;
                    }
                }
            }
        }
    }

    // Flip sprite AFTER physics (same system as Player)
    public override void _Process(double delta)
    {
        if (EnemySprite != null)
            EnemySprite.FlipH = !_movingRight;
    }
}