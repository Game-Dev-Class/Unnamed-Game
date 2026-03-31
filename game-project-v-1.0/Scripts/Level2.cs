using Godot;
using System;

public partial class Level2 : Node2D
{
	private TileMapLayer _tileMap;
	// private Hud _hud;
	private Player _player;
	private Pbutton _button;
	private PDoor _door;
	private Enemy _enemy1;
	private WhipEnemy _enemy2;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>("Stage");
		// _hud = GetNode<Hud>("HUD");
		_player = GetNode<Player>("Player");
		_button = GetNode<Pbutton>("Pbutton");
		_door = GetNode<PDoor>("PDoor");
		_enemy1 = GetNode<Enemy>("Enemy");
		_enemy2 = GetNode<WhipEnemy>("WhipEnemy");
		
		_button.ButtonTrigger += _door.OnButtonTrigger;
		// _hud.StartGame += StartStage;
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
