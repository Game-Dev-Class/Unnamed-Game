using Godot;
using System;

public partial class Pbutton2 : Area2D
{
    [Signal]
    public delegate void Button2TriggerEventHandler();

    [Export] public AudioStreamPlayer2D ClickSound;

    private Area2D _area2D;

    public override void _Ready()
    {
        ClickSound ??= GetNodeOrNull<AudioStreamPlayer2D>("ClickSound");
        _area2D = GetNodeOrNull<Area2D>("Area2D");

        if (_area2D != null)
            _area2D.BodyEntered += OnBodyEntered;

        Monitoring = true;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D)
        {
            GD.Print("Player has pressed the button for PDoor2");

            ClickSound?.Play();

            EmitSignal(SignalName.Button2Trigger);
        }
    }

    public override void _Process(double delta)
    {
    }
}