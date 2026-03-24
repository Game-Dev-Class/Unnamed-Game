using Godot;

public partial class Hearts : AnimatedSprite2D
{
	public void OnHealthChanged(int health)
	{
		health = Mathf.Clamp(health, 0, 3);
		Frame = 3 - health;
	}
}
