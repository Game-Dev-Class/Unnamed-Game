using Godot;
using System;

public partial class BouncePad : Area2D
{
	[Export] public float BounceVelocity = -700.0f;

	public override void _Ready()
	{
		// Connect the signal in code
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// Check if the thing that touched us is the Player
		if (body is Player player)
		{
			player.Bounce(BounceVelocity);
		}
	}
}
	
