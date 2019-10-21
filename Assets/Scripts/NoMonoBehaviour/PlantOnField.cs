using System;
using UnityEngine;

public class PlantOnField:Plant{
    
    public Vector3 Position { get; set; }
    public int LifeTimeMinutes { get; set; }
    public int LifeBonus { get; set; }
    public DateTime FruitTime { get; set; }

    public PlantOnField(Vector3 position, int[] dna):base(dna)
    {
        Position = position;
        LifeTimeMinutes = Utilities.getPlantLifeInMinutes(dna);
    }
    public PlantOnField(Vector3 position, int lifeBonus, DateTime fruitTime, int[] dna) : base(dna)
    {
        Position = position;
        LifeBonus = lifeBonus;
        LifeTimeMinutes = (int)(Utilities.getPlantLifeInMinutes(dna) + (Utilities.getPlantLifeInMinutes(dna) * (((float)lifeBonus)/100)));
        FruitTime = fruitTime;
    }
}
