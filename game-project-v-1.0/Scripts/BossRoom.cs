using Godot;
using System;

public partial class BossRoom : Node2D
{
	private TileMapLayer _tileMap;
	private Hud _hud;
	private Player _player;
	private Pbutton _button;
	private PDoor _door;
	private Enemy _enemy1;
	private Enemy _enemy2;
	private WhipEnemy _whipEnemy;
	private Boss _boss;

	private bool _isPaused = false;

	public override void _Ready()
	{
		_tileMap = GetNodeOrNull<TileMapLayer>("Stage");
		_hud = GetNodeOrNull<Hud>("HUD");
		_player = GetNodeOrNull<Player>("Player");
		_button = GetNodeOrNull<Pbutton>("Pbutton");
		_door = GetNodeOrNull<PDoor>("PDoor");
		_enemy1 = GetNodeOrNull<Enemy>("Enemy");
		_enemy2 = GetNodeOrNull<Enemy>("enemy");
		_whipEnemy = GetNodeOrNull<WhipEnemy>("WhipEnemy");
		_boss = GetNodeOrNull<Boss>("Boss");

		if (_button != null && _door != null)
			_button.ButtonTrigger += _door.OnButtonTrigger;

		if (_hud != null)
		{
			_hud.StartGame += StartStage;
			_player?.DisableMovement();
			_enemy1?.DisableMovement();
			_enemy2?.DisableMovement();
			_whipEnemy?.DisableMovement();
			_boss?.DisableMovement();
		}
		else
		{
			StartStage();
		}
	}

	public void StartStage()
	{
		GD.Print("The stage started.");

		if (_tileMap != null)
			_tileMap.Visible = true;

		if (_player != null)
		{
			_player.Visible = true;
			_player.EnableMovement();
		}

		_enemy1?.EnableMovement();
		_enemy2?.EnableMovement();
		_whipEnemy?.EnableMovement();
		_boss?.EnableMovement();

		_isPaused = false;
	}

	private void PauseGame()
	{
		_isPaused = !_isPaused;

		if (_isPaused)
		{
			_player?.DisableMovement();
			_enemy1?.DisableMovement();
			_enemy2?.DisableMovement();
			_whipEnemy?.DisableMovement();
			_boss?.DisableMovement();
		}
		else
		{
			_player?.EnableMovement();
			_enemy1?.EnableMovement();
			_enemy2?.EnableMovement();
			_whipEnemy?.EnableMovement();
			_boss?.EnableMovement();
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
			PauseGame();
	}
}