using Godot;

public partial class WhipEnemy : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public AnimatedSprite2D WhipEnemySprite;

    [ExportGroup("Audio")]
    [Export] public AudioStreamPlayer2D HurtSound;
    [Export] public AudioStreamPlayer2D DeathSound;

    private bool _movingRight = true;

    public override void _Ready()
    {
        if (WhipEnemySprite == null)
            WhipEnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        // Auto-grab audio nodes if not assigned
        HurtSound ??= GetNodeOrNull<AudioStreamPlayer2D>("HurtSound");
        DeathSound ??= GetNodeOrNull<AudioStreamPlayer2D>("DeathSound");
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // Gravity always applies
        if (!IsOnFloor())
            Velocity += GetGravity() * dt;
        else
            Velocity = new Vector2(Velocity.X, 0);

        // Horizontal movement only on floor
        if (IsOnFloor())
        {
            float direction = _movingRight ? 1f : -1f;
            Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);
        }

        MoveAndSlide();

        // Check collisions for turning and player contact
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            var collider = collision.GetCollider();
            Vector2 normal = collision.GetNormal();

            if (collider is Player player)
            {
                player.TakeEnemyHit(GlobalPosition);
            }

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

    // --- Combat methods with sounds ---
    public void TakeDamage(int amount)
    {
        HurtSound?.Play();
        // Optional: add health tracking here
        // Die() can be called if health <= 0
    }

    public void Die()
    {
        DeathSound?.Play();
        QueueFree();
    }
}