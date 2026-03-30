using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public AnimatedSprite2D EnemySprite;

    private bool _movingRight = true;

    public override void _Ready()
    {
        if (EnemySprite == null)
            EnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // Always apply gravity
        if (!IsOnFloor())
            Velocity += GetGravity() * dt;
        else
            Velocity = new Vector2(Velocity.X, 0);

        // Horizontal movement only when grounded
        if (IsOnFloor())
        {
            float direction = _movingRight ? 1f : -1f;
            Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);
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

    public override void _Process(double delta)
    {
        if (EnemySprite != null)
            EnemySprite.FlipH = !_movingRight;
    }
}