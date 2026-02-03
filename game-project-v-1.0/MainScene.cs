using Godot;
using System;

public partial class MainScene : Node2D
{
	private TileMap _tileMap;
	private Hud _hud;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<TileMap>("Stage");
		_hud = GetNode<Hud>("HUD");
		
		_hud.StartGame += StartStage;
	}

	private void StartStage(){
		_tileMap.Visible = true;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
