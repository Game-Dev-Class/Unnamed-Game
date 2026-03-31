using Godot;
using System;

public partial class LevelBeta : Node2D
{
	private TileMapLayer _tileMap;
	// private Hud _hud;
	private Player _player;
	private PDoor _door;
	private Enemy _enemy1;
	private Enemy _enemy2;
	private WhipEnemy _whipEnemy;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>("Stage");
		_player = GetNode<Player>("Player");
		_door = GetNode<PDoor>("PDoor");
		_enemy1 = GetNode<Enemy>("Enemy1");
		_enemy2 = GetNode<Enemy>("Enemy2");
		_whipEnemy = GetNode<WhipEnemy>("WhipEnemy");
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
