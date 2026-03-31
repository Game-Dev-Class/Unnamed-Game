using Godot;

public partial class Whip : Node2D
{
    private Area2D _area;

    [ExportGroup("Audio")]
    [Export] public AudioStreamPlayer2D HitSound;

    [Export] public float Lifetime = 0.5f; // seconds

    public override void _Ready()
    {
        _area = GetNodeOrNull<Area2D>("Area2D");
        if (_area == null)
        {
            GD.PrintErr("Whip: No Area2D found.");
            return;
        }

        _area.BodyEntered += OnBodyEntered;

        // Check overlaps immediately (in case spawned inside an enemy)
        CallDeferred(nameof(CheckOverlaps));

        // Auto-remove the whip after Lifetime seconds
        var timer = GetTree().CreateTimer(Lifetime);
        timer.Timeout += () => QueueFree();
    }

    private void CheckOverlaps()
    {
        foreach (var body in _area.GetOverlappingBodies())
            OnBodyEntered(body);
    }

    private void OnBodyEntered(Node body)
    {
        // --- Boss takes 1 damage ---
        if (body is Boss bossEnemy)
        {
            bossEnemy.TakeDamage(1);
            HitSound?.Play();
            return;
        }

        // --- Normal Enemy dies ---
        if (body is Enemy enemy)
        {
            enemy.QueueFree();
            HitSound?.Play();
            return;
        }

        // --- Whip-specific enemy dies ---
        if (body is WhipEnemy whipEnemy)
        {
            whipEnemy.QueueFree();
            HitSound?.Play();
            return;
        }

        // --- Unhandled objects ---
        GD.Print("Whip hit something else: ", body.Name);
    }
}