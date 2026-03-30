using Godot;

public partial class Boss : CharacterBody2D
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

        if (!IsOnFloor())
            Velocity += GetGravity() * dt;
        else
            Velocity = new Vector2(Velocity.X, 0);

        if (IsOnFloor())
        {
            float direction = _movingRight ? 1f : -1f;
            Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);
        }

        MoveAndSlide();

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();
            Vector2 normal = collision.GetNormal();

            // Hit player → knockback
            if (collider is Player player)
            {
                player.TakeEnemyHit(GlobalPosition);
            }

            // Flip on invisible wall (layer 3)
            if (collider is PhysicsBody2D body)
            {
                if ((body.CollisionLayer & (1 << 2)) != 0)
                {
                    if (Mathf.Abs(normal.X) > 0.7f)
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