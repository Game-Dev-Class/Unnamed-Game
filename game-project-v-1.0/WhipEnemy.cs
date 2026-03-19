using Godot;

public partial class WhipEnemy : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public AnimatedSprite2D WhipEnemySprite;

    private bool _movingRight = true;

    public override void _Ready()
    {
        if (WhipEnemySprite == null)
            WhipEnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // ✅ ALWAYS apply gravity
        if (!IsOnFloor())
            Velocity += GetGravity() * dt;
        else
            Velocity = new Vector2(Velocity.X, 0); // lock to floor

        // ✅ Horizontal movement ONLY when grounded
        if (IsOnFloor())
        {
            float direction = _movingRight ? 1f : -1f;
            Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);
        }

        MoveAndSlide();

        // ✅ Handle collisions AFTER movement
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();
            Vector2 normal = collision.GetNormal();

            // 🔹 Player hit → knockback ONLY (no physics reaction)
            if (collider is Player player)
            {
                player.TakeEnemyHit(GlobalPosition);
            }

            // 🔹 Flip ONLY on invisible wall (layer 3)
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
        if (WhipEnemySprite != null)
            WhipEnemySprite.FlipH = !_movingRight;
    }
}