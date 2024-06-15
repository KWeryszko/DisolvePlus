using Godot;
using System;

public partial class GameOverScene : Node2D
{
    public override void _Ready()
    {
        retryButton = GetChild<TextureButton>(2);
        retryButton.ButtonUp += RestartGame;
    }
    private void RestartGame()
    {
        GetTree().ChangeSceneToFile("res://BattleScene/BattleScene.tscn");
    }
    private TextureButton retryButton;
}
