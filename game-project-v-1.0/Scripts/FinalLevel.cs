using Godot;
using System;

public partial class FinalLevel : Node2D
{
    private TileMapLayer _tileMap;
    private Hud _hud;
    private Player _player;
    private Pbutton _button;
    private PDoor _door;
    private Enemy _enemy1;
    private Enemy _enemy2;
    private Pbutton2 _button2;
    private PDoor2 _door2;

    public override void _Ready()
    {
        _tileMap = GetNode<TileMapLayer>("Stage");
        _hud = GetNode<Hud>("HUD");
        _player = GetNode<Player>("Player");
        _button = GetNode<Pbutton>("Pbutton");
        _door = GetNode<PDoor>("PDoor");
        _enemy1 = GetNode<Enemy>("Enemy");
        _enemy2 = GetNode<Enemy>("enemy");
        _button2 = GetNode<Pbutton2>("Pbutton2");
        _door2 = GetNode<PDoor2>("PDoor2");

        _button.ButtonTrigger += _door.OnButtonTrigger;
        _button2.Button2Trigger += _door2.OnButtonTrigger;
        _hud.StartGame += StartStage;
    }

    public void StartStage()
    {
        GD.Print("The stage started.");
        _tileMap.Visible = true;
        _player.Visible = true;
    }
}