using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogOptionsEnum;
using WindowsEnum;

public class UIItemsManager : MonoBehaviour {
    
    private List<Item> items;
    private int index;
    private int countOfItems;
    private GameObject plant;

    public Button btn_left;
    public Button btn_right;
    public Button btn_action;
    public Text txt_count;
    public Text txt_title;
    public Text txt_name;
    public Text txt_description;
    public Text txt_actionButton;
    public RawImage icon;
    public Text txt_info;

    public WindowsManager windowsManager;
    public ManagersContainer managers;
    
    public UIBagManager uiManager_bag;
    public UIPlantManager uiManager_plant;
    	
    public void open(string type, GameObject p = null)
    {
        if (p != null)
        {
            if (p.transform.parent != null)
            {
                plant = p.transform.parent.gameObject;
            }
            else
            {
                plant = p;
            }
        }
        items = managers.databaseManager.getItemsByType(type);
        countOfItems = items.Count;
        index = countOfItems - 1;
        
        updateWindowElements();
    }
    
    public void updateWindowElements()
    {
        switch (items[0].GetType().ToString())
        {
            case "Shovel":
                txt_title.text = "Plecak - Szpadle";
                txt_actionButton.text = "Wykop";
                break;
            case "Fertilizer":
                txt_title.text = "Plecak - Nawozy";
                txt_actionButton.text = "Dodaj nawóz";
                break;
            case "Conditioner":
                txt_title.text = "Plecak - Odżywki";
                txt_actionButton.text = "Podlej z odżywką";
                break;
        }
        icon.texture = items[index].Icon;
        txt_count.text = managers.databaseManager.getCountOfItem(items[index]).ToString();
        txt_description.text = items[index].Description;
        txt_name.text = items[index].Name;
        setButtonsActives();
    }

    public void setButtonsActives()
    {
        if (index == 0)
        {
            btn_left.interactable = false;
        }
        else
        {
            btn_left.interactable = true;
        }
        if (index == countOfItems - 1)
        {
            btn_right.interactable = false;
        }
        else
        {
            btn_right.interactable = true;
        }
    }

    public void nextItem()
    {
        index++;
        updateWindowElements();
    }
    public void previousItem()
    {
        index--;
        updateWindowElements();
    }

    public void action()
    {
        switch (items[0].GetType().ToString())
        {
            case "Shovel":
                actionShovel();
                break;
            case "Fertilizer":
                actionFertilizer();
                break;
            case "Conditioner":
                actionConditioner();
                break;
        }
        windowsManager.close((int)Windows.Items);
    }

    public void  actionShovel()
    {
        int rand = Random.Range(0, 100);
        bool broken = false;
        if (rand <= ((Shovel)items[index]).BreakChance)
        {
            broken = true;
            managers.databaseManager.subtractOneItem(items[index].Name);
        }
        rand = Random.Range(0, 100);
        if (rand <= ((Shovel)items[index]).MoveChance)
        {
            if(broken)
            {
                txt_info.text = "Twoja roślina została przeniesiona do plecaka,\nale szpadel się połamał.";
            }
            else
            {
                txt_info.text = "Twoja roślina została przeniesiona do plecaka.";
            }
            windowsManager.open((int)Windows.InfoDigged);
            managers.databaseManager.addPlantToBag(managers.databaseManager.getDnaOfPlantOnField(plant.transform.position.x, plant.transform.position.z), 1);
        }
        else
        {
            if (broken)
            {
                windowsManager.openInfoDeath((int)Options.DigWithShovelBreak);
            }
            else
            {
                windowsManager.openInfoDeath((int)Options.Dig);
            }
            managers.plantManager.createGarbage(new Vector2(plant.transform.position.x, plant.transform.position.z), true, false);
        }
        managers.plantManager.removePlant(plant);
        
    }

    public void actionFertilizer ()
    {
        managers.sceneManager.Player.PlantTimeIncrease = ((Fertilizer)items[index]).PlantTimeIncrease;
        uiManager_bag.updatePlantInfo();
        uiManager_bag.fertilizer = (Fertilizer)items[index];
    }
    private void actionConditioner()
    {
        uiManager_plant.waterPlant(plant, ((Conditioner)items[index]).LifTimeIncrease);
        managers.databaseManager.subtractOneItem(items[index].Name);
    }

}
