using Godot;
using System;

public partial class PDoor2 : StaticBody2D
{
    private StaticBody2D _door;
    private Sprite2D _sprite;
    private CollisionShape2D _collision;
    private bool _isOpen;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("DoorSprite");
        _collision = GetNode<CollisionShape2D>("DoorCollision");

        var pbutton2 = GetNode<Pbutton2>("Pbutton2");
        pbutton2.Button2Trigger += OnButtonTrigger;

        _door = GetNode<StaticBody2D>("DoorBody");

        _isOpen = false;
    }

    public void OnButtonTrigger()
    {
        GD.Print("PDoor2 Recognizes trigger");

        if (_isOpen == false)
        {
            _sprite.Visible = false;
            _collision.SetDeferred("disabled", true);
            _isOpen = true;
        }
        else
        {
            _sprite.Visible = true;
            _collision.SetDeferred("disabled", false);
            _isOpen = false;
        }
    }

    public override void _Process(double delta)
    {
    }
}