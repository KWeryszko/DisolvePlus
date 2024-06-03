using Godot;
using System;

public partial class GameOverScene : Node2D
{
    public override void _Ready()
    {
        retryButton = GetChild<Button>(2);
        retryButton.ButtonDown += RestartGame;
    }
    private void RestartGame()
    {
        GetTree().ChangeSceneToFile("res://BattleScene/BattleScene.tscn");
    }
    private Button retryButton;
}
