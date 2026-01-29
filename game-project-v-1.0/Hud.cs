using Godot;
using System;

public partial class Hud : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void _on_button_pressed()
	{
		// Logic to switch to the game scene
		//if (GameScene != null)
		//{
			////GetTree().ChangeSceneToPacked(GameScene); // Change the current scene to the game scene
	 	//}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
