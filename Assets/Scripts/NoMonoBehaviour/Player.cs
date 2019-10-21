using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player {

    private int gold;
    private int diamonds;
    private Text textGold;
    private Text textDiamonds;

    public float ChanceOfMutation { get; set; }
    public int MaxInQueue { get; set; }

    public int PlantTimeIncrease {get; set;}

    public Player(Text textGold, Text textDiamonds)
    {
        this.textGold = textGold;
        this.textDiamonds = textDiamonds;
        PlantTimeIncrease = 0;
        gold = PlayerPrefs.GetInt("gold", 0);
        diamonds = PlayerPrefs.GetInt("diamonds", 0);
        ChanceOfMutation = PlayerPrefs.GetFloat("chanceOfMutation", 1);
        MaxInQueue = PlayerPrefs.GetInt("maxInQueue", 3);
        setGold(gold);
        setDiamonds(diamonds);
    }

    public void setGold(int g)
    {
        gold = g;
        textGold.text = g.ToString();
        PlayerPrefs.SetInt("gold", g);
    }
    public void addGold(int g)
    {
        gold += g;
        textGold.text = gold.ToString();
        PlayerPrefs.SetInt("gold", gold);
    }

    public int getGold()
    {
        return gold;
    }

    public void setDiamonds(int d)
    {
        diamonds = d;
        textDiamonds.text = d.ToString();
        PlayerPrefs.SetInt("diamonds", d);
    }
    public void addDiamonds(int d)
    {
        diamonds += d;
        textDiamonds.text = diamonds.ToString();
        PlayerPrefs.SetInt("diamonds", diamonds);
    }
    public int getDiamonds()
    {
        return diamonds;
    }
    public void setChanceOfMutation(float c)
    {
        ChanceOfMutation = c;
        PlayerPrefs.SetFloat("chanceOfMutation", c);
    }
    public float getChanceOfMutation()
    {
        return ChanceOfMutation;
    }

}
