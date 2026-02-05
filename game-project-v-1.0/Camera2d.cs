using Godot;

public partial class Camera2d : Camera2D
{
    [Export] public NodePath PlayerPath;   // assign the player in inspector
    [Export] public float Smoothing = 0.12f; // 0 = instant, ~0.1–0.2 smooth

    private Node2D _player;
    private float _fixedY;

    public override void _Ready()
    {
        MakeCurrent(); // activate this camera
        _player = GetNodeOrNull<Node2D>(PlayerPath);
        _fixedY = Position.Y; // lock the camera's vertical position
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null) return;

        float targetX = _player.GlobalPosition.X;
        float newX = Mathf.Lerp(Position.X, targetX, Smoothing);

        // Only move horizontally, Y is fixed
        Position = new Vector2(newX, _fixedY);
    }
}
