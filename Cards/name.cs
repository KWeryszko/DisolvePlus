using Godot;
using System;

public partial class name : Label
{
    public override void _Ready()
    {
        baseCard = GetNode<BaseCard>(GetPathTo(GetParent().GetParent().GetParent()));
        string str = baseCard.Stats.Name;
        Text = str;
    }
    public override void _Process(double delta)
    {
        Update();
    }
    public void Update()
    {
        string str = baseCard.Stats.Name;
        Text = str;
    }
    BaseCard baseCard;
}
