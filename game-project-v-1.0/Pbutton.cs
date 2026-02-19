using Godot;
using System;

public partial class Pbutton : Area2D
{
	[Signal]
	public delegate void ButtonPressedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public void _on_body_entered()
	{
		EmitSignal(SignalName.ButtonPressed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
