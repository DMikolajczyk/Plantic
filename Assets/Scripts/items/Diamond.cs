using UnityEngine;

public class Diamond : Item {

    public int Count { get; set; }

    public Diamond(string name, int count, int price, Texture icon)
    {
        Name = name;
        Count = count;
        Price = price;
        Locked = false;
        Premium = false;
        Description = "Ilość diamentów: " + count;
        Type = GetType().ToString();
        Icon = icon;
    }
}
