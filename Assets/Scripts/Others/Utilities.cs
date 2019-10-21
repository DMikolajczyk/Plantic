using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class Utilities {

    public const int fieldSize = 7;
    public const float scaleUnit = 4.5F;
    public const float stepForMutationChance = 1.0F;
    static public bool isAfterButtonClick = false;
    public const int fieldGarbageBonus = 40;
    static public Dictionary<int, int> countsOfParts = new Dictionary<int, int>();
    static public List<int[]> basicDnas = new List<int[]>()
    {
        new int[] { 1, 1, 0, 0, 0, 0, 0 },
        new int[] { 0, 2, 0, 0, 0, 0, 0 },
        new int[] { 1, 3, 0, 0, 1, 0, 0 },
        new int[] { 1, 4, 1, 0, 0, 0, 0 }
    };

    static public DateTime getCurrentTime()
    {
        return DateTime.Now;
    }
    static public bool isCorrectDna(int[] dna)
    {
        if (!(dna[0] == 0 && (dna[4] != 0 || dna[2] != 0)))
        {
            return true;
        }
        return false;
    }
    static public bool isBasicDna(int[] dna)
    {
        if(ListContainsDna(basicDnas, dna))
        {
            return true;
        }
        return false;
    }
    static public bool ListContainsDna(List<int[]> dnas, int[] dna)
    {
        if(dnas.Any(el => Enumerable.SequenceEqual(el, dna)))
        {
            return true;
        }
        return false;
    }
    static public int getPlantLifeInMinutes(int[] dna)
    {
        //return (int)(((10 * 60) * (1 + dna[0] * 0.5 + dna[1] * 0.05 + dna[2] * 0.5 + dna[3] * 0.2 + dna[4] * 0.05 + dna[5] * 0.05 + dna[6] * (-0.5)))/900);
        return (int)(((10 * 60) * (1 + dna[0] * 0.5 + dna[1] * 0.05 + dna[2] * 0.5 + dna[3] * 0.2 + dna[4] * 0.05 + dna[5] * 0.05 + dna[6] * (-0.5))) / 2);
    }
    static public int getPriceOfPlant(int[] dna)
    {
        int price = (int)(100 * (1 + dna[0] * 1.8F + dna[1] * 1.4F + dna[2] * 1.3F + dna[3] * 1.5F + dna[4] * 1.2F + dna[5] * 1.6F + dna[6] * 1.1F));
        return price - price % 10;
    }
    static public int getPriceOfFruit(int[] dna)
    {
        int price = (int)((100 * (1 + dna[0] * 1.8F + dna[1] * 1.4F + dna[2] * 1.3F + dna[3] * 1.5F + dna[4] * 1.2F + dna[5] * 1.6F + dna[6] * 1.1F))/10);
        return price - price % 10;
    }
    static public float getChanceToCross(int[] dna)
    {
        float sum = dna[0] + dna[1] + dna[2] * 0.9F + dna[3] * 1.5F + dna[4] * 1.3F + dna[5] * 1.5F + dna[6];
        if (sum < 2)
        {
            return 1;
        }
        else
        {
            float chance = Mathf.Sqrt(1.0F / (sum * 5000)) * 100;
            return chance;
        }

        
    }
    static public float getPlantingTime(int[] dna)
    {
        //// do przeróbki
        //return (((10 * 60) * (1 + dna[0] * 0.5F + dna[1] * 0.05F + dna[2] * 0.5F + dna[3] * 0.2F + dna[4] * 0.05F + dna[5] * 0.05F + dna[6] * (-0.5F))) / 600);
        return (((10 * 60) * (1 + dna[0] * 0.5F + dna[1] * 0.05F + dna[2] * 0.5F + dna[3] * 0.2F + dna[4] * 0.05F + dna[5] * 0.05F + dna[6] * (-0.5F))) / 6000);
    }
    static public float getFruitTime(int[] dna)
    {
        //// do przeróbki
        return (((10 * 60) * (1 + dna[0] * 0.5F + dna[1] * 0.05F + dna[2] * 0.5F + dna[3] * 0.2F + dna[4] * 0.05F + dna[5] * 0.05F + dna[6] * (-0.5F))) / 6000);
    }
    static public float getCrossingTime(int[] dnaOne, int[] dnaTwo)
    {
        //// do przeróbki
        float a = ((10 * 60) * (1 + dnaOne[0] * 0.5F + dnaOne[1] * 0.05F + dnaOne[2] * 0.5F + dnaOne[3] * 0.2F + dnaOne[4] * 0.05F + dnaOne[5] * 0.05F + dnaOne[6] * (-0.5F))) / 600;
        float b = ((10 * 60) * (1 + dnaTwo[0] * 0.5F + dnaTwo[1] * 0.05F + dnaTwo[2] * 0.5F + dnaTwo[3] * 0.2F + dnaTwo[4] * 0.05F + dnaTwo[5] * 0.05F + dnaTwo[6] * (-0.5F))) / 600;
        return (a + b) / 10;
    }

    static public int getAward(DatabaseManager dbManager)
    {
        return dbManager.getCountOfDiscoveredPlants() * dbManager.getCountOfDiscoveredPlants() * 20;

    }
    static public string convertTimeToString(float minutes)
    {
        string text = "";
        if (minutes > 60)
        {
            text = (((int)minutes) / 60).ToString() + "h " + (((int)minutes) % 60).ToString() + "min";
        }
        else if (minutes > 1)
        {
            text = (((int)minutes) % 60).ToString() + "m " + ((int)((minutes * 60) % 60)).ToString() + "s";
        }
        else
        {
            text = ((int)(minutes * 60 )).ToString() + "s";
        }
        return text;
    }

}
