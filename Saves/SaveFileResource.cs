using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class SaveFileResource : Resource
{
    [Export]
    public int[] Cards { get => cards; set => cards = value; }
    [Export]
    public bool Upgrade { get => upgrade; set => upgrade = value; }

    private bool upgrade=false; //battle win -> true, draw/upgrade -> false
    private int[] cards;
}
