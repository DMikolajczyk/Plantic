using UnityEngine;

public abstract class Item {

    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public bool Locked { get; set; }
    public bool Premium { get; set; }
    public string Type { get; set; }
    public Texture Icon { get; set; }

}
