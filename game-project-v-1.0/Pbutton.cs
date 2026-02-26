using Godot;
using System;

public partial class Pbutton : Area2D
{
	private Area2D _area2D;
	public override void _Ready()
	{
		_area2D = GetNode<Area2D>("Area2D");
		_area2D.BodyEntered += OnBodyEntered;
		Monitoring = true;
	}
	
	public void OnBodyEntered(Node2D body)
	{
		if(body is CharacterBody2D character)
		{
			GD.Print("Player has pressed the button");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
