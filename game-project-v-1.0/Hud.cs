using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void _on_button_pressed()
	{
		Visible = false;
		EmitSignal(SignalName.StartGame);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
