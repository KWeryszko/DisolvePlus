using Godot;
using System;

public partial class BlueSlime : BaseEnemy
{
    public override void _Ready()
    {
        SelectEnemyCards(new int[] { 1, 1, 2 });
        ConnectAttributesToChildren();
        SetHpBar();
    }
}
