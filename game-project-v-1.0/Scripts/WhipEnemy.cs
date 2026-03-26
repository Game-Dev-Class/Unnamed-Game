using Godot;

public partial class WhipEnemy : CharacterBody2D
{
    [Export] public float MoveSpeed = 100f;
    [Export] public AnimatedSprite2D WhipEnemySprite;

    [Export] public bool StartCanMove = true;

    private bool _movingRight = true;
    private bool _canMove;

    public void EnableMovement()
    {
        _canMove = true;
    }

    public void DisableMovement()
    {
        _canMove = false;
    }

    public bool GetCanMove()
    {
        return _canMove;
    }

    public override void _Ready()
    {
        if (WhipEnemySprite == null)
            WhipEnemySprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        _canMove = StartCanMove;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_canMove)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

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
}