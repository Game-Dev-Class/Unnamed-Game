using Godot;

public partial class Boss : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public int MaxHealth = 3;
    [Export] public float DamageCooldown = 0.5f;
    [Export] public AnimatedSprite2D EnemySprite;

    [Export] public AudioStream DamageSound; // <-- assign in inspector

    public int Health { get; private set; }

    private bool _movingRight = true;
    private bool _canTakeDamage = true;

    private AudioStreamPlayer2D _audioPlayer;

    public override void _Ready()
    {
        Health = MaxHealth;

        if (EnemySprite == null)
            EnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        // Create AudioStreamPlayer2D if we have a sound assigned
        if (DamageSound != null)
        {
            _audioPlayer = new AudioStreamPlayer2D();
            AddChild(_audioPlayer);
            _audioPlayer.Stream = DamageSound;
        }
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

            if (collider is Player player)
            {
                player.TakeEnemyHit(GlobalPosition);
                continue;
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
        if (EnemySprite != null)
            EnemySprite.FlipH = !_movingRight;
    }

    public void TakeDamage(int amount = 1)
    {
        if (!_canTakeDamage)
            return;

        Health = Mathf.Max(Health - amount, 0);
        GD.Print("Boss HP: ", Health);

        // Play damage sound
        if (_audioPlayer != null)
            _audioPlayer.Play();

        if (Health <= 0)
        {
            _audioPlayer.Play();
            QueueFree();
            return;
        }

        _canTakeDamage = false;

        var timer = GetTree().CreateTimer(DamageCooldown);
        timer.Timeout += () => _canTakeDamage = true;
    }
}