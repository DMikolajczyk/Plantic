using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCross : Action {
    public int[] DnaOne { get; set; }
    public int[] DnaTwo { get; set; }
    public float CrossChance { get; set; }

    public ActionCross(int[] dnaOne, int[] dnaTwo)
    {
        StartTime = Utilities.getCurrentTime();
        LifeTimeInMinutes = Utilities.getCrossingTime(dnaOne, dnaTwo);
        DnaOne = dnaOne;
        DnaTwo = dnaTwo;
        CrossChance = (Utilities.getChanceToCross(dnaOne) + Utilities.getChanceToCross(dnaTwo)) / 2;
    }
    public ActionCross(int[] dna, float crossChance, float crossingTime)
    {
        StartTime = Utilities.getCurrentTime();
        LifeTimeInMinutes = crossingTime;
        DnaOne = dna;
        DnaTwo = dna;
        CrossChance = crossChance;
    }
    public ActionCross(int[] dnaOne, int[] dnaTwo, DateTime startTime, float crossChance, float crossingTime)
    {
        StartTime = startTime;
        LifeTimeInMinutes = crossingTime;
        DnaOne = dnaOne;
        DnaTwo = dnaTwo;
        CrossChance = crossChance;
    }


}
