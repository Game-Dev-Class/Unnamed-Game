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
        _enemy2.EnableMovement();
		_player.EnableMovement(); 
		_enemy1.EnableMovement();
	}
	
	// public void StartStage()
	// {
	// 	GD.Print("The stage started.");
	// 	_tileMap.Visible = true;
	// 	_player.Visible = true;
	// 	_player.EnableMovement();
	// 	_enemy1.EnableMovement();
	// 	_enemy2.EnableMovement();
	// }

	private void PauseGame()
	{
		if (_player.GetCanMove() == true)
		{
			_player.DisableMovement();
			_enemy1.DisableMovement();
			_enemy2.DisableMovement();
		}

		else if (_player.GetCanMove() == false)
		{
			_player.EnableMovement();
			_enemy1.EnableMovement();
			_enemy2.EnableMovement();
		}
	}	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Input.IsActionJustPressed("pause"))
		{
			PauseGame();
		}
	}
}
