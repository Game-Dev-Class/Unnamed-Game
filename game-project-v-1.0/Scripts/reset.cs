using Godot;
using System;

// The word after 'class' MUST be 'reset' to match 'reset.cs'
public partial class reset : Node 
{
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("reset_level"))
		{
			GetTree().ReloadCurrentScene();
			GD.Print("Level Reset!");
		}
	}
}
