using Godot;
using System;

public partial class MainScene : Node2D
{
	private TileMapLayer _tileMap;
	private Hud _hud;
	private Player _player;
	private Pbutton _button;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>("Stage");
		_hud = GetNode<Hud>("HUD");
		_player = GetNode<Player>("Player");
		_button = GetNode<Pbutton>("button");
		
		_hud.StartGame += StartStage;
		_player.DisableMovement();
	}
	
	private void StartStage()
	{
		_tileMap.Visible = true;
		_player.Visible = true;
		_player.EnableMovement();
	}
		
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
