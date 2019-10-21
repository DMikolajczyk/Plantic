using UnityEngine;

public class Conditioner : Item {

    public int LifTimeIncrease { get; set; }

    public Conditioner(string name, int lifTimeIncrease, int price, bool locked, bool premium, Texture icon)
    {
        Name = name;
        LifTimeIncrease = lifTimeIncrease;
        Price = price;
        Locked = locked;
        Premium = premium;
        Description = "Wzrost witalności o: " + lifTimeIncrease + "%";
        Type = GetType().ToString();
        Icon = icon;
    }

}
