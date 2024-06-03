using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

public partial class CardDeck : Node2D
{
    [Signal]
    public delegate int CardClickedEventHandler(); //returns clicked card id\\
    public CardDeck() { }
    public CardDeck(int[] cardIDs)
    {
        foreach (var id in cardIDs)
        {
            cards.Add(new BaseCard(id));
        }
    }
    public CardDeck(BaseCard[] cards)
    {
        foreach (var card in cards)
        {
            this.cards.Add(card);
        }
    }
    public override void _Ready()
    {
        //Creates buttons and connects signals\\  //temporary solution
        funkcje.Add(Card1ButtonHoverStart);
        funkcje.Add(Card2ButtonHoverStart);
        funkcje.Add(Card3ButtonHoverStart);
        funkcje.Add(Card4ButtonHoverStart);
        funkcje.Add(Card5ButtonHoverStart);
        funkcje.Add(Card1ButtonHoverEnd);
        funkcje.Add(Card2ButtonHoverEnd);
        funkcje.Add(Card3ButtonHoverEnd);
        funkcje.Add(Card4ButtonHoverEnd);
        funkcje.Add(Card5ButtonHoverEnd);
        for (int i = 0; i < 5;i++) 
        { 
            buttons.Add(new CardButton());
            buttons[i].OnButtonPressed += CardButtonClick;
            buttons[i].MouseEntered += funkcje[i];
            buttons[i].MouseExited += funkcje[5+i];  
            buttons[i].Flat = true;
        }
        funkcje.Clear();
    }
    public void AddCardById(int id)
    {
        cards.Add(new BaseCard(id));
    }
    public void AddCard(BaseCard card)
    {
        cards.Add(card);
    }
    public void RemoveCard(int index) 
    {
        try
        {
            cards.RemoveAt(index);
        }
        catch (IndexOutOfRangeException)
        {
            
            GD.Print("IndexOutOfRange");
        }
    }
    public void ChooseCardUpgrade(int upgradeableCardIndex)
    {
        List<BaseCard> tempCards = new();
        foreach (int id in cards[upgradeableCardIndex].Stats.NextCardsID)
        {
            tempCards.Add(new BaseCard(id));
        }
        var upgradeID = tempCards[ShowCards(tempCards)].Stats.ID;
        UpgradeCard(upgradeableCardIndex, upgradeID);
        //input function? ex. Create a new list of Cards consisting of possible upgrades and show them to the player once upgrade is chosen the card updates to it
    }
    public int ShowCards(List<BaseCard> cards, int verticalOffset = 0) //return index number of chosen card // VO - number of pixels from the bottom
    {
        int verticalPosition = 648 - 150 - verticalOffset;
        List<Vector2> positions = new List<Vector2>(0);
        childPaths = new string[cards.Count];

        switch (cards.Count)
        {   
            case 0:
                break;
            case 1:                //display 1     [       []       ]
                positions.Add(new Vector2((1152 / 2) - 50, verticalPosition)); //648 //1152 to akurat rozmiar okienka      
                break;
            case 2:                //display 2     [      [][]      ]
                positions.Add(new Vector2((1152 / 2) - 100, verticalPosition));
                positions.Add(new Vector2((1152 / 2), verticalPosition));
                break;
            case 3:                //display 3     [     [][][]     ]
                positions.Add(new Vector2((1152 / 2) - 150, verticalPosition));
                positions.Add(new Vector2((1152 / 2) - 50, verticalPosition));
                positions.Add(new Vector2((1152 / 2) + 50, verticalPosition));
                break;
            case 4:                //display 4     [    [][][][]    ]
                positions.Add(new Vector2((1152 / 2) - 200, verticalPosition));
                positions.Add(new Vector2((1152 / 2) - 100, verticalPosition));
                positions.Add(new Vector2((1152 / 2), verticalPosition));
                positions.Add(new Vector2((1152 / 2) + 100, verticalPosition));
                break;
            case 5:                //display 5     [   [][][][][]   ]
                positions.Add(new Vector2((1152 / 2) - 250, verticalPosition));
                positions.Add(new Vector2((1152 / 2) - 150, verticalPosition));
                positions.Add(new Vector2((1152 / 2) - 50, verticalPosition));
                positions.Add(new Vector2((1152 / 2) + 50, verticalPosition));
                positions.Add(new Vector2((1152 / 2) + 150, verticalPosition));
                break;
            default:
                throw new Exception("CardCountOverflowException");

        }
        for (int i = 0; i < cards.Count; i++)
        {
            AddChild(cards[i]);
            childPaths[i] = GetPathTo(cards[i]);
            cards[i].SetGlobalPosition(positions[i]);

            //adding and setting the position of buttons\\
            if (buttons[i].GetParent() == null) { cards[i].AddChild(buttons[i]); }
            else { buttons[i].Reparent(cards[i]); }
            buttons[i].SetGlobalPosition(cards[i].GetGlobalPosition());
            buttons[i].setParentCardIndex(i);
            
            GD.Print(cards[i].ID);
        }
        

        return 0;
    }
    public void Card1ButtonHoverStart() { CardButtonHoverStart(0); }//temporary solution, find a way to override hover signal
    public void Card2ButtonHoverStart() { CardButtonHoverStart(1); }
    public void Card3ButtonHoverStart() { CardButtonHoverStart(2); }
    public void Card4ButtonHoverStart() { CardButtonHoverStart(3); }
    public void Card5ButtonHoverStart() { CardButtonHoverStart(4); }
    public void Card1ButtonHoverEnd() {  CardButtonHoverEnd(0); }
    public void Card2ButtonHoverEnd() { CardButtonHoverEnd(1); }
    public void Card3ButtonHoverEnd() { CardButtonHoverEnd(2); }
    public void Card4ButtonHoverEnd() { CardButtonHoverEnd(3); }
    public void Card5ButtonHoverEnd() { CardButtonHoverEnd(4); }
    private void CardButtonClick(int indx) {
        RemoveChildren();
        if (GetParent<BattleScene>().setCardInUse(GetCard(indx)))
        {
            TransferCard(indx);
            ShowCards(cards);
            buttons[getCards().Count].Visible = false;
        }
        else
        {
            ShowCards(cards);
            buttons[getCards().Count].Visible = false;
        };//sets card in the middle of the screen, if successful removes card\\ 

    }
    private void CardButtonHoverStart(int indx)
    { 
        buttons[indx].GetParent<BaseCard>().Position -= new Vector2(0, 20);
        //buttons[indx].Position -= new Vector2(0, 20);//nothing happens
    } //Lifts card up by Vector when mouse hovers over
    private void CardButtonHoverEnd(int indx) 
    { 
        buttons[indx].GetParent<BaseCard>().Position += new Vector2(0, 20);
        //buttons[indx].Position += new Vector2(0, 20);//nothing happens
    }   //Lowers card down to default position when mouse leaves
    public void SetLastButtonVisible()
    {
        buttons[getCards().Count-1].Visible = true; // can be without -1 but battlescene has to have a change made\\
    }
    public void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.Visible = false;
        }

    }//turns buttons invisible\\
    public void EnableButtons()
    {
        foreach (var button in buttons)
        {
            button.Visible = true;
        }
    }//turns buttons visible\\
    public void Shuffle(int count)
    {
        Random rand = new Random();
        int indx1, indx2;
        BaseCard temp;
        for (int i = 0; i < count; i++)
        {
            indx1=rand.Next(cards.Count);
            indx2=rand.Next(cards.Count);

            temp = cards[indx1];
            cards[indx1] = cards[indx2];
            cards[indx2] = temp;
        }
    }
    private void UpgradeCard(int cardIndex, int upgradedCardID)
    {
        try
        {
            cards[cardIndex].ID = upgradedCardID;
            cards[cardIndex].Update();
        }
        catch (IndexOutOfRangeException)
        { 
            GD.Print("index out of bounds");
        }
    }
    public BaseCard TransferCard(int index)
    {
        BaseCard tempCard=GetCard(index);
        RemoveCard(index);
        return tempCard;
    }
    public int TransferCardById(int index)
    {
        BaseCard tempCard = GetCard(index);
        RemoveCard(index);
        return tempCard.ID;
    }
    public List<BaseCard> getCards()
    {
        return cards;
    }
    public void RemoveChildren()    { 
        foreach (var path in childPaths)
        {
            RemoveChild(GetNode(path)); 
        }   
    }
    public int[] getCurrentActiveButtons() //check how it actually works
    {
        List<int> activeButtons= new(0);
        for (int i = 0; i < cards.Count; i++)
        {
            activeButtons.Add(i);
        }
        return activeButtons.ToArray();
    }
    public void ActivateSelectButtons(int[] buttonsToActivate)
    {
        foreach (var index in buttonsToActivate)
        {
            buttons[index].Visible = true;
        }
    }
    public bool IsEmpty() {  return cards.Count == 0; }
    public BaseCard GetCard(int index)
    {
        BaseCard tempCard=new BaseCard();
        try
        {
            tempCard = cards[index];
            return tempCard;
        }
        catch (IndexOutOfRangeException)
        { 
            GD.Print("IndexOutOfRange"); 
            return tempCard;
        }
    }
    private List<BaseCard> cards=new(0);
    List<CardButton> buttons = new List<CardButton>(0);
    List<Action> funkcje=new List<Action>(0);
    private string[] childPaths;
    private int lastButton;
}
