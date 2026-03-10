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
        // Apply gravity (same as player)
        if (!IsOnFloor())
            Velocity += GetGravity() * (float)delta;

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

        // Check collisions
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);

            // -------- PLAYER HIT DETECTION (NEW) --------
            if (collision.GetCollider() is Player player)
            {
                player.TakeEnemyHit(GlobalPosition);
            }
            // --------------------------------------------

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
        if (WhipEnemySprite != null)
            WhipEnemySprite.FlipH = !_movingRight;
    }
}