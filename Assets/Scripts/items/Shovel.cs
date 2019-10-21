using UnityEngine;

public class Shovel : Item {
    
    public int MoveChance { get; set; }
    public int BreakChance { get; set; }

    public Shovel (string name, int moveChance, int breakChance, int price, bool locked, bool premium, Texture icon)
    {
        Name = name;
        MoveChance = moveChance;
        BreakChance = breakChance;
        Price = price;
        Locked = locked;
        Premium = premium;
        Description = "Szansa na przeniesienie: " + moveChance + "%\nSzansa złamania przedmiotu: " + breakChance + "%";
        Type = GetType().ToString();
        Icon = icon;
    }

}
