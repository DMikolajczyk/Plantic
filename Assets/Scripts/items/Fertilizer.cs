using UnityEngine;

public class Fertilizer : Item {
    public int PlantTimeIncrease { get; set; }

    public Fertilizer(string name, int crossingIncrease, int price, bool locked, bool premium, Texture icon)
    {
        Name = name;
        PlantTimeIncrease = crossingIncrease;
        Price = price;
        Locked = locked;
        Premium = premium;
        Description = "Skrócenie czasu sadzenia o: " + crossingIncrease + "%";
        Type = GetType().ToString();
        Icon = icon;
    }

}
