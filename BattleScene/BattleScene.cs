using Godot;
using System;
using System.Collections.Generic;

public partial class BattleScene : Node2D
{
    public override void _Ready()
    {
        drawPile = GetChild<CardDeck>(1);
        //Add all cards from deck (inventory) to draw pile
        foreach(var card in save.Cards)
        {
            drawPile.AddCardById(card);
        }
        drawPile.Shuffle(100);
        

        hand = GetChild<CardDeck>(2);
        discardPile = GetChild<CardDeck>(3);
        NTB = GetChild<Button>(4);
        turnCounter= GetChild<TurnCounter>(5);
        //RCB\\
        RCB = GetChild<ReturnCardButton>(6);
        RCB.ButtonUp += ReturnCardInUse;
        RCB.Visible = false;
        //EB\\
        EB = GetChild<EnemyButton>(7);
        EB.ButtonUp += ChooseEnemy;
        EB.Visible = false;
        

        //PLAYER\\
        player = GetChild<BaseEnemy>(8);
        player.CharacterDied += DeathsignalReceiverPlayer;

        //ENEMY\\
        String path;
        if (save.Round == 0)
        {
            path = "res://Enemies/GreenSlime/GreenSlime.tscn";
            save.Round++;
        }
        else if (save.Round == 1)
        {
            path = "res://Enemies/BlueSlime/BlueSlime.tscn";
            save.Round++;
        }
        else { path = "res://Enemies/RedSlime/RedSlime.tscn";  }
        var scene = GD.Load<PackedScene>(path);
        var instance = scene.Instantiate();
        
        AddChild(instance);
        enemy = GetChild<BaseEnemy>(9);
        enemy.Position = new Vector2(760, 250);
        enemy.CharacterDied += DeathsignalReceiverEnemy;
        
        //AP Bar\\
        apBar = GetChild<Control>(0).GetChild<ApBar>(1);
        apBar.MaxAPSetup(player.GetChild<Attribute>(6).getbaseAttribute());
        apBar.setRegenAp(player.GetChild<Attribute>(6).getattributeregeneration());
        apBar.setCurrentAp(player.GetChild<Attribute>(6).getbaseAttribute());

        turnStart = true;
        turnCounter.AddToCounter();

    }
    public override void _Process(double delta)
    {
        
        if (turnStart)
        {
            DrawCards();
            hand.ShowCards(hand.getCards());
            hand.UpdateCardsBasedOnStats(player.getAttributes());
            player.GetChild<Attribute>(6).RegenerateAttribute();
            apBar.setCurrentAp(player.GetChild<Attribute>(6).getcurrentAttribute());
            turnStart = false;
        }
        if (enemyTurn)
        {
            EnemyTurn();
        }
        if (Input.IsActionJustPressed("LeftMouseClick") && turnEnd)
        {

            RemoveChild(cardInUse);
            player.ReceiveCardPlayedByOpponet(cardInUse, enemy.getAttributes());
            NTB.Visible = true;
            turnEnd = false;
            turnStart = true;
        };//Waits until player is able to read text on the card played by enemy
        if (Input.IsActionJustPressed("LeftMouseClick") && enemyDied)
        {
            GetTree().ChangeSceneToFile("res://BattleVictory/BattleVictory.tscn");
        };//Waits

    }
    private void EnemyTurn()
    {
        BaseCard temp = enemy.ChooseCardToPlayC();
        this.cardInUse = temp;
        AddChild(cardInUse);
        int damage =
             (int)(temp.Stats.AttackValue * temp.Stats.StrengthScaling * enemy.getAttributes()[0]) +
            +(int)(temp.Stats.AttackValue * temp.Stats.AgilityScaling * enemy.getAttributes()[1]) +
            +(int)(temp.Stats.AttackValue * temp.Stats.IntelligenceScaling * enemy.getAttributes()[2]);
        cardInUse.UpdateVisibleStats(damage);
        cardInUse.SetGlobalPosition(new Vector2((1152 / 2) - 50, 100));
        NTB.Visible=false;


        turnCounter.AddToCounter();
        enemyTurn = false;
        turnEnd= true;
    }
    private void OnButtonClick()  //next turn\\
    {
        hand.RemoveChildren();
        while (!hand.IsEmpty())
        {
            discardPile.AddCard(hand.TransferCard(hand.getCards().Count - 1));
        }
        hand.EnableButtons();
        enemyTurn = true;
    }
    public bool setCardInUse(BaseCard cardInUse)
    {
        if(cardInUse.Stats.ApCost > player.GetChild<Attribute>(6).getcurrentAttribute()) return false;
        //creates card to be displayed\\
        this.cardInUse = cardInUse;
        AddChild(cardInUse);
        cardInUse.SetGlobalPosition(new Vector2((1152 / 2) - 50, 100));
        
        RCB.Visible = true; //sets the return button to visible so it can be clicked\\
        NTB.Visible = false;
        EB.Visible = true;
        //savings buttons to buffer and turns them off\\
        handButtonBuffer=hand.getCurrentActiveButtons();
        hand.DisableButtons();
        return true;

    }
    private void ReturnCardInUse()
    {
        hand.RemoveChildren();
        
        hand.AddCardById(cardInUse.ID);

        RemoveChild(cardInUse);

        RCB.Visible=false;
        NTB.Visible = true;
        EB.Visible=false;

        GD.Print("HAND HAS" + hand.getCards().Count);
        //turning the buttons back on\\
        hand.ActivateSelectButtons(handButtonBuffer);
        hand.SetLastButtonVisible();
        hand.ShowCards(hand.getCards());
    }
    private void ChooseEnemy()
    {
        enemy.ReceiveCardPlayedByOpponet(cardInUse, player.getAttributes());//placeholder stats, not anymore!
        player.GetChild<Attribute>(6).DebuffAttribute(cardInUse.Stats.ApCost);
        apBar.setCurrentAp(player.GetChild<Attribute>(6).getcurrentAttribute());
        discardPile.AddCard(cardInUse);
        RemoveChild(cardInUse);
        RCB.Visible = false;
        NTB.Visible = true;
        EB.Visible = false;
        
        GD.Print("HAND HAS" + hand.getCards().Count);
        //turning the buttons back on\\
        hand.ActivateSelectButtons(handButtonBuffer);
    }
    private void DrawCards()
    {
        for (int i = 0; i < 5; i++)         //player can't have less than 5 cards
        {
            if (!drawPile.IsEmpty())
            {
                hand.AddCard(drawPile.TransferCard(drawPile.getCards().Count - 1));
            }
            else
            {
                while (!discardPile.IsEmpty())
                {
                    drawPile.AddCard(discardPile.TransferCard(discardPile.getCards().Count - 1));
                }
                drawPile.Shuffle(100);
                hand.AddCard(drawPile.TransferCard(drawPile.getCards().Count - 1));
            }
        }
    }
    public void DeathsignalReceiverPlayer()
    {
        GD.Print("Player Died");
        save.Round = 0;
        GetTree().ChangeSceneToFile("res://GameOverScene/GameOverScene.tscn");
    }
    public void DeathsignalReceiverEnemy()
    {
        GD.Print("Enemy Died");
        save.Upgrade = true;
        enemyDied = true;
        NTB.Visible = false;
        hand.DisableButtons();
    }

    CardDeck drawPile, hand, discardPile;
    Button NTB;
    ReturnCardButton RCB;
    EnemyButton EB;
    TurnCounter turnCounter;
    private BaseCard cardInUse;
    private bool turnStart=true, enemyTurn, turnEnd=false, enemyDied=false;
    private BaseEnemy enemy, player;
    private int[] handButtonBuffer;
    private ApBar apBar;
    private SaveFileResource save = GD.Load<SaveFileResource>("res://Saves/save1.tres");
}
