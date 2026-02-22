using Godot;

public partial class Character : CharacterBody2D
{
    [Export] public float Speed = 200f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("move_left"))
            direction.X -= 1;

        if (Input.IsActionPressed("move_right"))
            direction.X += 1;

        if (Input.IsActionPressed("move_up"))
            direction.Y -= 1;

        if (Input.IsActionPressed("move_down"))
            direction.Y += 1;

        if (direction != Vector2.Zero)
            direction = direction.Normalized();

        Velocity = direction * Speed;
        MoveAndSlide();
    }
}