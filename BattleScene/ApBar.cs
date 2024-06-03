using Godot;
using System;
using System.Collections.Generic;

public partial class ApBar : ColorRect
{
    public override void _Ready()
    {
        usedAp = ImageTexture.CreateFromImage(GD.Load<CompressedTexture2D>("res://BattleScene/Resources/APBar/APUSEDIMP.png").GetImage());
        currentAP = ImageTexture.CreateFromImage(GD.Load<CompressedTexture2D>("res://BattleScene/Resources/APBar/APIMG.png").GetImage());
        regenaAP = ImageTexture.CreateFromImage(GD.Load<CompressedTexture2D>("res://BattleScene/Resources/APBar/APREGIMG.png").GetImage());
    }
    public void MaxAPSetup(int maxAP) //max 10
    {
        this.maxAP = maxAP;
        for (int i = 0; i < maxAP; i++)
        {
            apBubbles.Add(new TextureRect());
            apBubbles[i].ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            apBubbles[i].Texture = usedAp;
            apBubbles[i].Position = new Vector2(10, 280- 25*i );
            apBubbles[i].Size = new Vector2(20, 20);
            
            AddChild(apBubbles[i]);
        }
    }
    public void setCurrentAp(int currentAP) 
    {
        for (int i = 0; i < currentAP & i < maxAP; i++)
        {
            apBubbles[i].Texture = this.currentAP;
        }
        if (currentAP < maxAP)
        {
            for(int i = currentAP; i < apRegen+currentAP && i < maxAP; i++)
            {
                apBubbles[i].Texture = regenaAP;
            }
            for(int i = apRegen + currentAP; i < maxAP; i++)
            {
                apBubbles[i].Texture = usedAp;
            }
        }
    }
    public void setRegenAp(int apRegen)
    {
        this.apRegen = apRegen;
    }
    private List<TextureRect> apBubbles = new List<TextureRect>(0);
    private int maxAP, apRegen;
    ImageTexture usedAp, currentAP, regenaAP;
}
