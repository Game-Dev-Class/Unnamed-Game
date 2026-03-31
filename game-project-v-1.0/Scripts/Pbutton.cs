using Godot;
using System;

public partial class Pbutton : Area2D
{
	[Signal]
	public delegate void ButtonTriggerEventHandler();

	[Export] public AudioStreamPlayer2D ClickSound; // Exported for the Inspector

	private Area2D _area2D;

	public override void _Ready()
	{
		// Fallback: If you didn't drag it in the inspector, try to find a child named "ClickSound"
		ClickSound ??= GetNodeOrNull<AudioStreamPlayer2D>("ClickSound");

		_area2D = GetNodeOrNull<Area2D>("Area2D");
		
		if (_area2D != null)
		{
			_area2D.BodyEntered += OnBodyEntered;
		}

		Monitoring = true;
	}
	
	public void OnBodyEntered(Node2D body)
	{
		// Check for Player specifically (or any CharacterBody2D like your enemies)
		if (body is CharacterBody2D character)
		{
			GD.Print("Player has pressed the button");
			
			// The ?. ensures it only plays if the node exists
			ClickSound?.Play();
			
			EmitSignal(SignalName.ButtonTrigger);
		}
	}

	public override void _Process(double delta)
	{
	}
}
