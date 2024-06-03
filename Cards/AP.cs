using Godot;
using System;

public partial class AP : Label
{
    public override void _Ready()
    {
        baseCard = GetNode<BaseCard>(GetPathTo(GetParent().GetParent().GetParent()));
        string str = baseCard.Stats.ApCost.ToString();
        Text = str;
    }
    public override void _Process(double delta)
    {
        Update();
    }

    public void Update()
    {
        string str = baseCard.Stats.ApCost.ToString();
        Text = str;
    }
    BaseCard baseCard;
}
