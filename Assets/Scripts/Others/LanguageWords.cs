using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageWords : MonoBehaviour {

    public Text GUI_shop;
    public Text GUI_gold;
    public Text GUI_diamonds;

    public Dictionary<string, string> words { get; set; } 

	// Use this for initialization
	void Start () {
        ///////////////////// ZOSTAWIONE NA PÓŹNIEJ //////
        //initDictionary(); //////////////////////////////
        //setLanguage("PL"); /////////////////////////////
        //////////////////////////////////////////////////
	}
	
    private void initDictionary()
    {
        words = new Dictionary<string, string>();
        words.Add("K_shop", "");
        words.Add("K_gold", "");
        words.Add("K_diamonds", "");
    }

    public void setLanguage(string language)
    {
        switch (language)
        {
            case "PL":
                words["K_shop"] = "Sklep";
                words["K_gold"] = "Złoto:";
                words["K_diamonds"] = "Diamenty:";
                break;
            case "ENG":
                words["K_shop"] = "Shop";
                words["K_gold"] = "Gold:";
                words["K_diamonds"] = "Diamonds:";
                break;
        }
        setTexts();
    }
    public void setTexts()
    {
        GUI_shop.text = words["K_shop"];
        GUI_gold.text = words["K_gold"];
        GUI_diamonds.text = words["K_diamonds"];
    }

}
