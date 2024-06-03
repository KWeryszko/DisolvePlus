using Godot;
using System;

public partial class BattleVictory : Node2D
{
    public override void _Ready()

    {
        addCardButton = GetChild<Button>(1);
        addCardButton.ButtonUp += UpgradeCard;
        upgradeCardButton = GetChild<Button>(2);
        upgradeCardButton.ButtonUp += AddCard;
        nextBattleButton = GetChild<Button>(3);
        nextBattleButton.ButtonUp += NextBattle;

    }
    private void AddCard()
    {

        int[] tempList = new int[save.Cards.Length+1];
        for (int i = 0; i < save.Cards.Length; i++)
        {
            tempList[i] = save.Cards[i];
        }
        tempList[save.Cards.Length] = 1;
        save.Cards = tempList;
        //ResourceSaver.Save(save, "res://Saves/save1.tres"); //with save, changes are permanent
        

    }
    private void UpgradeCard()
    {
        GetTree().ChangeSceneToFile("res://CardUpgradeScene/CardUpgradeScene.tscn");
    }
    private void NextBattle()
    {
        GetTree().ChangeSceneToFile("res://BattleScene/BattleScene.tscn");
    }
    private Button addCardButton, upgradeCardButton, nextBattleButton;
    private SaveFileResource save = GD.Load<SaveFileResource>("res://Saves/save1.tres");
}
