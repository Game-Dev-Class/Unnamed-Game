using Godot;

public partial class Whip : Node2D
{
    private Area2D _area;

    public override void _Ready()
    {
        _area = GetNodeOrNull<Area2D>("Area2D");

        if (_area == null)
        {
            GD.PrintErr("Whip: No Area2D found.");
            return;
        }

        _area.BodyEntered += OnBodyEntered;

        CallDeferred(nameof(CheckOverlaps));

        var timer = GetTree().CreateTimer(0.25f);
        timer.Timeout += () => QueueFree();
    }

    private void CheckOverlaps()
    {
        foreach (var body in _area.GetOverlappingBodies())
            OnBodyEntered(body);
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Boss bossEnemy)
        {
            bossEnemy.TakeDamage(1);
            return;
        }

        if (body is Enemy enemy)
        {
            enemy.QueueFree();
            return;
        }

        if (body is WhipEnemy whipEnemy)
        {
            whipEnemy.QueueFree();
            return;
        }

        GD.Print("Whip hit something else: ", body.Name);
    }
}