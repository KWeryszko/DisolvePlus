using Godot;
using System;

public partial class CardButton : Button
{
    [Signal]
    public delegate void OnButtonPressedEventHandler(int parentCardIndex);
    public override void _Pressed()
    {
        base._Pressed();
        EmitSignal(SignalName.OnButtonPressed, parentCardIndex);
    }
    
    public override void _Ready()
    {
        Size = new Vector2(100, 150); //sets card size
        StyleBoxFlat ferus = GD.Load<StyleBoxFlat>("res://CardButton/Resources/Disabled_Card_Button.tres");
        AddThemeStyleboxOverride("disabled", ferus);
    }
    public override void _Draw()
    {
        DrawSetTransform(GlobalPosition = globalPosition);
    }
    public void SetGlobalPosition(Vector2 position) { globalPosition = position; }
    Vector2 globalPosition;
    public void setParentCardIndex(int parentCardIndex) { this.parentCardIndex = parentCardIndex; }
    public int getParentCardIndex() { return this.parentCardIndex; }
    private int parentCardIndex;
}
