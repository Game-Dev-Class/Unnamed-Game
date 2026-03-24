using Godot;

public partial class Sign : Sprite2D
{
    [Export]
    public string SignText { get; set; } = "Default sign text";

    [Export]
    public NodePath TextLabelPath { get; set; }

    private Area2D _area;
    private Label _label;

    public override void _Ready()
    {
        _area = GetNode<Area2D>("Area2D");
        _label = GetNodeOrNull<Label>(TextLabelPath);

        if (_label != null)
        {
            _label.Text = SignText;
            _label.Visible = false;
        }

        _area.BodyEntered += OnBodyEntered;
        _area.BodyExited += OnBodyExited;
        _area.AreaEntered += OnAreaEntered;
        _area.AreaExited += OnAreaExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        ShowText();
    }

    private void OnBodyExited(Node2D body)
    {
        HideText();
    }

    private void OnAreaEntered(Area2D area)
    {
        ShowText();
    }

    private void OnAreaExited(Area2D area)
    {
        HideText();
    }

    private void ShowText()
    {
        if (_label != null)
        {
            _label.Text = SignText;
            _label.Visible = true;
        }
    }

    private void HideText()
    {
        if (_label != null)
        {
            _label.Visible = false;
        }
    }
}