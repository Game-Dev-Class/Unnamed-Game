using Godot;
using System;

public partial class LevelExitButton : Button
{
    [Export]
    public string NextLevelPath = "res://Scenes/Level1.tscn";

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            OnPressed();
        }
    }

    private void OnPressed()
    {
        GD.Print("Button pressed! Queuing scene switch...");

        // Deferred call is still a safe habit for scene switching
        CallDeferred(MethodName.SwitchScene);
    }

    private void SwitchScene()
    {
        Error result = GetTree().ChangeSceneToFile(NextLevelPath);

        if (result != Error.Ok)
        {
            GD.PrintErr("CRITICAL ERROR: Could not find the file at " + NextLevelPath);
        }
    }
}