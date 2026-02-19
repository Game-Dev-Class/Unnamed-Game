using Godot;
using System;

public partial class LevelExit : Area2D
{
	[Export]
	public string NextLevelPath = "res://scenes/sLevel_2.tscn"; 

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			GD.Print("Player detected! Queuing scene switch...");
			
			// Use CallDeferred to wait for the physics step to finish
			// This fixes the "Removing a CollisionObject during physics callback" error
			CallDeferred(MethodName.SwitchScene);
		}
	}

	// Move the actual loading logic into its own method
	private void SwitchScene()
	{
		Error result = GetTree().ChangeSceneToFile(NextLevelPath);

		if (result != Error.Ok)
		{
			GD.PrintErr("CRITICAL ERROR: Could not find the file at " + NextLevelPath);
		}
	}
}
