using Godot;
using System;

public partial class Menu : Node2D
{
    public override void _Ready()
    {
        playButton = GetChild<Button>(2);
        playButton.ButtonDown += PlayButtonClicked;

        //ExitButton = GetChild<Button>(3);
        //ExitButton.ButtonDown += ExitButtonClicked;

        title = GetChild<TextureButton>(1);
        title.ButtonDown += TitleClicked;
        
    }
    private void PlayButtonClicked()
    {
        GetTree().ChangeSceneToFile("res://BattleScene/BattleScene.tscn");
    }
    private void ExitButtonClicked()
    {
        GetTree().Quit();
    }
    private void TitleClicked()
    {
        titleClickedCount++;
        if (titleClickedCount >= 8)
        {
            GD.Print("eluwina");
        }

    }
    private Button playButton, ExitButton, TickTackToe;
    private TextureButton title;
    private int titleClickedCount = 0;
}
