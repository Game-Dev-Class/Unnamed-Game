using Godot;
using System;

public partial class Pbutton2 : Area2D
{
    [Signal]
    public delegate void Button2TriggerEventHandler();

    private Area2D _area2D;
    public override void _Ready()
    {
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.BodyEntered += OnBodyEntered;
        Monitoring = true;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D character)
        {
            GD.Print("Player has pressed the button for PDoor2");
            EmitSignal(SignalName.Button2Trigger);
        }
    }

    public override void _Process(double delta)
    {
    }
}