using Godot;

public partial class Whip : Node2D
{
    private Area2D _area;

    public override void _Ready()
    {
        // Find the Area2D child
        _area = GetNodeOrNull<Area2D>("Area2D");

        if (_area == null)
        {
            GD.PrintErr("Whip: No Area2D found.");
            return;
        }

        _area.BodyEntered += OnBodyEntered;

        // Check if any bodies are already overlapping (spawn inside)
        CallDeferred(nameof(CheckOverlaps));

        // Auto-remove the whip after 0.5 seconds
        var timer = GetTree().CreateTimer(0.5f);
        timer.Timeout += () => QueueFree();
    }

    private void CheckOverlaps()
    {
        foreach (var body in _area.GetOverlappingBodies())
        {
            OnBodyEntered(body);
        }
    }

    private void OnBodyEntered(Node body)
    {
        // Generic handling: kill enemies
        if (body is Enemy enemy)
        {
            enemy.QueueFree();
        }
        else if (body is WhipEnemy whipEnemy)
        {
            whipEnemy.QueueFree();
        }
        else if (body is Boss bossEnemy) // <-- added
        {
            bossEnemy.QueueFree();
        }
        else
        {
            // You can add more types here later, e.g. destructible objects
            GD.Print("Whip hit something else: ", body.Name);
        }
    }
}