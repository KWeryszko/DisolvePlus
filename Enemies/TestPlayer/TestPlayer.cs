using Godot;
using System;

public partial class TestPlayer : BaseEnemy
{
    public override void _Ready()
    {
        ConnectAttributesToChildren();
        sprite.FlipH = true;
        SetHpBar();
    }
}
