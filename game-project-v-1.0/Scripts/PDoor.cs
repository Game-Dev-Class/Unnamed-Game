using Godot;
using System;

public partial class PDoor : StaticBody2D
{
	private StaticBody2D _door;
	private Sprite2D _sprite;
	private CollisionShape2D _collision;
	private bool _isOpen;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("DoorSprite");
		_collision = GetNode<CollisionShape2D>("DoorCollision");

		var pbutton = GetNode<Pbutton>("Pbutton");
		pbutton.ButtonTrigger += OnButtonTrigger;

		_door = GetNode<StaticBody2D>("DoorBody");

		_isOpen = false;
	}

	public void OnButtonTrigger()
	{
		GD.Print("Door Recognises trigger");
		
		if (_isOpen == false)
		{
			_sprite.Visible = false;
			_collision.SetDeferred("disabled", true);
			_isOpen = true;
		}

		else if (_isOpen == true)
		{
			_sprite.Visible = true;
			_collision.SetDeferred("disabled", false);
			_isOpen = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
