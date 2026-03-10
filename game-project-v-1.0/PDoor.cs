using Godot;
using System;

public partial class PDoor : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pbutton = GetNode<Pbutton>("Pbutton");
		pbutton.ButtonTrigger += OnButtonTrigger;
	}

	private void OnButtonTrigger()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
