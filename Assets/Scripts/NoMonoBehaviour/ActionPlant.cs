using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlant : Action {
    public int[] Dna { get; set; }
    public Vector3 Position { get; set; }
    public int PlantIncreaseBonus { get; set; }

    public ActionPlant(int[] dna, Vector3 position, int plantIncrease)
    {
        Debug.Log("bonus: " + plantIncrease);
        StartTime = Utilities.getCurrentTime();
        PlantIncreaseBonus = plantIncrease;
        Debug.Log("test of ceneManager: " + plantIncrease);
        LifeTimeInMinutes = Utilities.getPlantingTime(dna);
        Dna = dna;
        Position = position;
    }
    public ActionPlant(int[] dna, Vector3 position, DateTime startTime, int plantIncreaseBonus)
    {
        StartTime = startTime;
        PlantIncreaseBonus = plantIncreaseBonus;
        LifeTimeInMinutes = Utilities.getPlantingTime(dna);
        Dna = dna;
        Position = position;
    }

}
