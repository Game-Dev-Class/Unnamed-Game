using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public float MinFallSpeed = 20f;
    [Export(PropertyHint.Range, "0,0.9")] public float VerticalFraction = 0.25f;
    [Export(PropertyHint.Range, "0.1,1")] public float HorizontalMargin = 0.9f;

    [Export] public NodePath DetectionAreaPath;
    [Export] public NodePath CollisionShapePath;

    private Area2D _detectionArea;
    private CollisionShape2D _collisionShape;
    private Vector2 _shapeExtents = new Vector2(16, 16);

    public override void _Ready()
    {
        // Find Area2D
        if (DetectionAreaPath != null && !DetectionAreaPath.IsEmpty)
            _detectionArea = GetNodeOrNull<Area2D>(DetectionAreaPath);
        else
            _detectionArea = GetNodeOrNull<Area2D>("DetectionArea");

        // Find CollisionShape2D
        if (CollisionShapePath != null && !CollisionShapePath.IsEmpty)
            _collisionShape = GetNodeOrNull<CollisionShape2D>(CollisionShapePath);
        else
            _collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

        if (_collisionShape != null && _collisionShape.Shape is RectangleShape2D rect)
            _shapeExtents = rect.Size / 2f;

        if (_detectionArea != null)
            _detectionArea.BodyEntered += OnBodyEntered;
        else
            GD.PrintErr("Enemy: No DetectionArea found.");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Apply gravity same as player
        if (!IsOnFloor())
            Velocity += GetGravity() * (float)delta;

        MoveAndSlide();
    }

    private void OnBodyEntered(Node body)
    {
        if (body is not Player player)
            return;

        // Must be falling
        if (player.Velocity.Y <= 0)
            return;

        // Player must be above enemy center
        if (player.GlobalPosition.Y < GlobalPosition.Y)
        {
            QueueFree();
        }
    }
}