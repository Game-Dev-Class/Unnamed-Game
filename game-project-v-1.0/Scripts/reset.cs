using Godot;
using System;

public partial class ResetButton : Node
{
	// This runs every frame
	public override void _Process(double delta)
	{
		// "IsActionJustPressed" ensures it only triggers once per tap
		if (Input.IsActionJustPressed("reset_level"))
		{
			ResetScene();
		}
	}

	private void ResetScene()
	{
		// This clears the current level and reloads it fresh
		GetTree().ReloadCurrentScene();
		
		// Optional: Print to the console to confirm it's working
		GD.Print("Level Reset!");
	}
}
