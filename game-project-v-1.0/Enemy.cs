using Godot;

public partial class Enemy : RigidBody2D
{
	[Export] public int HeadShapeIndex = 1;
	[Export] public int BodyShapeIndex = 0;

	public override void _Ready()
	{
		Connect(
			"body_shape_entered",
			new Callable(this, nameof(OnBodyShapeEntered))
		);
	}

	private void OnBodyShapeEntered(
		Rid bodyRid,
		Node body,
		int bodyShapeIndex,
		int localShapeIndex
	)
	{
		if (!body.IsInGroup("player"))
			return;

		if (localShapeIndex == HeadShapeIndex)
		{
			QueueFree(); // enemy dies
		}
		else if (localShapeIndex == BodyShapeIndex)
		{
			body.QueueFree(); // player dies
		}
	}
}
