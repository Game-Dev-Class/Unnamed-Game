using Godot;

public partial class Hearts : AnimatedSprite2D
{
	private Player _player;

	public override void _Ready()
	{
		_player = GetNodeOrNull<Player>("%Player");

		if (_player == null)
		{
			GD.PushError("Hearts could not find the Player node. Make sure the Player node is marked as Unique Name (%Player).");
			return;
		}

		_player.HealthChanged += OnHealthChanged;
		OnHealthChanged(_player.Health);
	}

	public override void _ExitTree()
	{
		if (IsInstanceValid(_player))
			_player.HealthChanged -= OnHealthChanged;
	}

	private void OnHealthChanged(int health)
	{
		health = Mathf.Clamp(health, 0, 3);
		Frame = 3 - health;
	}
}