using Godot;

public partial class Camera2d : Camera2D
{
    [Export] public NodePath PlayerPath;
    [Export] public float Smoothing = 0.12f;
    [Export] public bool ToggleFollow = true; // follow state

    private Node2D _player;
    private float _fixedY;

    public override void _Ready()
    {
        MakeCurrent();
        _player = GetNodeOrNull<Node2D>(PlayerPath);
        _fixedY = Position.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;

        // Toggle follow with E key
        if (Input.IsActionJustPressed("screen_lock"))

        {
            ToggleFollow = !ToggleFollow;

            // If turning ON → recenter immediately
            if (ToggleFollow)
            {
                Position = new Vector2(_player.GlobalPosition.X, _fixedY);
            }
        }

        if (!ToggleFollow) return;

        float targetX = _player.GlobalPosition.X;
        float newX = Mathf.Lerp(Position.X, targetX, Smoothing);

        Position = new Vector2(newX, _fixedY);
    }
}
