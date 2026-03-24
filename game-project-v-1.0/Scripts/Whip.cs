using Godot;

public partial class Whip : Node2D
{
    public override void _Ready()
    {
        var timer = GetTree().CreateTimer(0.5f);
        timer.Timeout += () => QueueFree();
    }
}