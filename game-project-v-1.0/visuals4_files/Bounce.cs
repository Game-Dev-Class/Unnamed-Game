using Godot;
using System;

public partial class BouncePad : Area2D
{
	[Export] public float BaseBounce = -600.0f;
	[Export] public float MomentumMultiplier = 0.5f;
	
	private double _lastBounceTime = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (Time.GetTicksMsec() - _lastBounceTime < 100) return;

		// Check if the node is in the "player" group
		if (body.IsInGroup("player"))
		{
			_lastBounceTime = Time.GetTicksMsec();

			// We still need to call the Bounce method. 
			// This 'as' check ensures the script on the node has that method.
			if (body is Player playerScript) 
			{
				playerScript.Bounce(BaseBounce, MomentumMultiplier);
				GD.Print($"{body.Name} bounced!");
			}
			else
			{
				GD.PrintErr($"Node {body.Name} is in 'player' group but is missing Player.cs script!");
			}
		}
	}
}  
