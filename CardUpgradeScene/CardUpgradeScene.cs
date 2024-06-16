using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public partial class CardUpgradeScene : Node2D
{
    public override void _Ready()
    { 
        GetChild<Button>(1).ButtonDown += OnReturnButtonClick;
        GetChild<Button>(4).ButtonDown += CancelUpgrade;
        GetChild<Button>(4).Visible = false;

        nextPageButton = GetChild<Button>(2);
        nextPageButton.Visible = false;
        nextPageButton.ButtonDown += OnNextButtonPageClick;

        previousPageButton = GetChild<Button>(3);
        previousPageButton.Visible = false;
        previousPageButton.ButtonDown += OnPreviousButtonPageClick;


        ImportCards();
        currentPage = 0;

        SetButtonsPositions();
        displayCards();
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("LeftMouseClick") && upgradePicked)
        {
            upgradePicked = false;
            save.Upgrade = false;
            GetTree().ChangeSceneToFile("res://BattleVictory/BattleVictory.tscn");
        }
    }
    private void displayCards()
    {
        for (int i = 10 * currentPage; i < cards.Count && i < 10 * (currentPage + 1); i++)
        {   

            
            AddChild(cards[i]);
            cards[i].SetGlobalPosition(positionBuffer);


            if (buttons[i % 10].GetParent() == null) cards[i].AddChild(buttons[i % 10]);
            else                                     buttons[i % 10].Reparent(cards[i]);  //some buttons have parents left on previous page, it doesn't break anything\\
            buttons[i % 10].setParentCardIndex(i);

            positionBuffer[0] += 150;
            if (i%5 == 4)
            {
                positionBuffer[0] = 80;
                positionBuffer[1] += 200;
            }


        }
        foreach (var button in buttons.FindAll(button => button.GetParent<BaseCard>().Stats.NextCardsID.Length == 0))
        {
            button.Flat = false;
            button.Disabled = true;
        }
        foreach (var button in buttons.FindAll(button => button.GetParent<BaseCard>().Stats.NextCardsID.Length != 0))
        {
            button.Disabled = false;
            button.Flat = true;
        }

        if (cards.Count > (currentPage+1) * 10) nextPageButton.Visible = true;    
        else                                    nextPageButton.Visible= false;
        
        if (currentPage > 0)                    previousPageButton.Visible = true;  
        else                                    previousPageButton.Visible= false; 
        
        ResetPositionBuffer();
    }

    private void SetButtonsPositions()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetGlobalPosition(positionBuffer);
            buttons[i].Flat = true;
            positionBuffer[0] += 150;
            if (i % 5 == 4)
            {
                positionBuffer[0] = 80;
                positionBuffer[1] += 200;
            }
            buttons[i].OnButtonPressed += OnCardButtonPressed; //connecting functions, can be moved to separate method
            buttons.FindAll(x=>)
        }


        ResetPositionBuffer();
    }
    private void GrayOutButtons(int[] index)
    {
        foreach (int i in index)
        {
            buttons[i].Flat = false;
            buttons[i].Disabled = true;
        }
    }
    private void EnableButtons(int[] index)
    {
        foreach (int i in index)
        {   
            buttons[i].Flat = true;
            buttons[i].Disabled = false;
        }
    }
    private void OnNextButtonPageClick()
    {
        RemoveChildren();
        currentPage++;
        displayCards();
    }
    private void OnPreviousButtonPageClick()
    {
        RemoveChildren();
        currentPage--;
        displayCards();
    }
    private void OnReturnButtonClick()
    {
        GetTree().ChangeSceneToFile("res://BattleVictory/BattleVictory.tscn");
    }
    private void OnCardButtonPressed(int parentCardIndex)
    {
        cards[parentCardIndex].RemoveChild(buttons[parentCardIndex % 10]);//deletes leftover button
        chosenCard = parentCardIndex;
        GD.Print("ulepszasz karte nr "+ parentCardIndex);
        RemoveChildren();
        previousPageButton.Visible = false;
        nextPageButton.Visible = false;
        GetChild<Button>(1).Visible = false;
        GetChild<Button>(4).Visible = true;


        tempCardList = new List<BaseCard>(0);
        tempButtonList = new(0);
        foreach(var id in cards[parentCardIndex].Stats.NextCardsID)
        {
            tempCardList.Add(new BaseCard(id));
        }
        for (int i = 0; i < tempCardList.Count; i++)
        {
            AddChild(tempCardList[i]);
            tempButtonList.Add(new CardButton());
            tempCardList[i].AddChild(tempButtonList[i]);
            tempButtonList[i].setParentCardIndex(tempCardList[i].ID);
            tempButtonList[i].OnButtonPressed += UpgradeCard;
        }
        if (tempCardList.Count==1) 
        {
            tempCardList[0].SetGlobalPosition(new Vector2(380, 200));
            tempButtonList[0].SetGlobalPosition(new Vector2(380, 200));
        }
        else if (tempCardList.Count==2)
        {
            tempCardList[0].SetGlobalPosition(new Vector2(305, 200));
            tempCardList[1].SetGlobalPosition(new Vector2(455, 200));
            tempButtonList[0].SetGlobalPosition(new Vector2(305, 200));
            tempButtonList[1].SetGlobalPosition(new Vector2(455, 200));
        }
        else if(tempCardList.Count==3)
        {
            tempCardList[0].SetGlobalPosition(new Vector2(230, 200));
            tempCardList[1].SetGlobalPosition(new Vector2(380, 200));
            tempCardList[2].SetGlobalPosition(new Vector2(530, 200));
            tempButtonList[0].SetGlobalPosition(new Vector2(230, 200));
            tempButtonList[1].SetGlobalPosition(new Vector2(380, 200));
            tempButtonList[2].SetGlobalPosition(new Vector2(530, 200));
        }
        foreach(CardButton button in tempButtonList)
        {
            button.Flat = true;
            
        }
        AddChild(cards[parentCardIndex]);
        //Karta do ulepszenia na srodku

        cards[parentCardIndex].SetGlobalPosition(new Vector2(380, 400));
    }
    private void ImportCards()
    {
        foreach(var card in save.Cards)
        {
            cards.Add(new BaseCard(card));
        }
    }
    private void RemoveChildren()
    {
        for (int i = 10 * currentPage; i < cards.Count && i < 10 * (currentPage + 1); i++)
        {
            RemoveChild(cards[i]);
        }
    }
    private void ResetPositionBuffer()
    {
        positionBuffer[0] = 80;
        positionBuffer[1] = 200;
    }
    private void UpgradeCard(int parentCardIndex)
    {
        //removes all cards and buttons from screen\\
        for (int i = 0; i < tempCardList.Count; i++)
        {
            RemoveChild(tempCardList[i]);
            tempCardList[i].RemoveChild(tempButtonList[i]);
        }
        RemoveChild(cards[chosenCard]);
        GetChild<Button>(4).Visible = false;

        //sets upgraded card and its upgrade on screen\\
        cards[chosenCard].SetGlobalPosition(new Vector2(200, 200));
        AddChild(cards[chosenCard]);
        BaseCard tempCard = new(parentCardIndex);
        tempCard.SetGlobalPosition(new Vector2(400, 200));
        AddChild(tempCard);
        
        //saving upgraded card to save file\\
        save.Cards[chosenCard] = parentCardIndex;

        upgradePicked = true;
    }
    private void CancelUpgrade()
    {
        for(int i = 0; i< tempCardList.Count; i++)
        {
            RemoveChild(tempCardList[i]);
            tempCardList[i].RemoveChild(tempButtonList[i]);
        }
        RemoveChild(cards[chosenCard]);
        GetChild<Button>(4).Visible = false;
        GetChild<Button>(1).Visible = true;
        displayCards();
    }
    private List<BaseCard> cards = new(0);
    private List<CardButton> buttons = new(0)
    { 
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton(),
        new CardButton()
    };
    private Vector2 positionBuffer = new Vector2(80,200);
    private int currentPage , chosenCard;
    private Button previousPageButton, nextPageButton, cancelUpgradeButton;
    private bool upgradePicked = false;
    private List<BaseCard> tempCardList;
    private List<CardButton> tempButtonList;
    private SaveFileResource save = GD.Load<SaveFileResource>("res://Saves/save1.tres");
}
