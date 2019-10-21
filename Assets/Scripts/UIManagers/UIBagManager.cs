using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;

public class UIBagManager : MonoBehaviour {
    public Fertilizer fertilizer { get; set; }
    
    private List<PlantInBag> bag;
    private int bagIndex;
    private GameObject plantInDialog;
    private Vector3 positionToPlant;
    private GameObject garbage;

    public GameObject plantSpawner;

    public Text textCount;
    
    public Button btn_bag_left;
    public Button btn_bag_right;
    public Button btn_confirmPlant;
    public Button btn_addFerilizer;
    public Text txt_description;
    
    public ManagersContainer managers;
    public WindowsManager windowsManager;
        
    void Update ()
    {
        if(windowsManager.isOpen((int)Windows.Bag)) {
            if (managers.guiManager.tabsAndActions.Count < managers.sceneManager.Player.MaxInQueue)
            {
                btn_confirmPlant.interactable = true;
            }
            else
            {
                btn_confirmPlant.interactable = false;
            }
        }
    }

    public void openBagWindow(Vector3 pos, GameObject g = null)
    {
        windowsManager.close((int)Windows.PlantManager);
        positionToPlant = new Vector3(pos.x, 0, pos.z);
        
        
        bag = managers.databaseManager.getPlantsInBag();
        if(bag.Count <= bagIndex)
        {
            bagIndex = 0;
        }
        setActives();
        
        garbage = null;
        if (g != null)
        {
            managers.sceneManager.Player.PlantTimeIncrease = Utilities.fieldGarbageBonus;
            btn_addFerilizer.interactable = false;
            garbage = g;
        }
        else
        {
            btn_addFerilizer.interactable = true;
        }
        updateWindowElements();

    }
    public void closeBagWindow()
    {
        Destroy(plantInDialog);
        managers.sceneManager.Player.PlantTimeIncrease = 0;
        fertilizer = null;
    }

    public void bagNextPlant()
    {
        bagIndex++;
        updateWindowElements();
        setActives();
    }

    public void bagPreviousPlant()
    {
        bagIndex--;
        updateWindowElements();
        setActives();
    }
    public void updateWindowElements() {
        if(plantInDialog != null)
        {
            Destroy(plantInDialog);
        }
        plantInDialog = managers.plantManager.createPlantInView(new PlantInView(plantSpawner.transform.position, bag[bagIndex].Dna), true);
        if (!Utilities.isBasicDna(bag[bagIndex].Dna))
        {
            textCount.text = managers.databaseManager.getCountOfPlantInBag(bag[bagIndex].Dna).ToString();
            textCount.fontSize = 24;
        }
        else
        {
            textCount.text = "∞";
            textCount.fontSize = 30;
        }
        updatePlantInfo();
    }
    public void updatePlantInfo()
    {
        if (managers.sceneManager.Player.PlantTimeIncrease != 0)
        {
            float timeWithFertilizer = Utilities.getPlantingTime(bag[bagIndex].Dna) * ((float)(100 - managers.sceneManager.Player.PlantTimeIncrease) / 100);
            txt_description.text = "Czas posadzenia: " + Utilities.convertTimeToString(Utilities.getPlantingTime(bag[bagIndex].Dna)) +
                "\nCzas z nawozem: " + Utilities.convertTimeToString(timeWithFertilizer);
        }
        else
        {
            txt_description.text = "Czas posadzenia: " + Utilities.convertTimeToString(Utilities.getPlantingTime(bag[bagIndex].Dna));
        }
    }

    public void confirmPlantClick()
    {        
        if (!Utilities.isBasicDna(bag[bagIndex].Dna))
        {
            managers.databaseManager.subtractOnePlantFromBag(bag[bagIndex].Dna);
        }
        if (garbage != null)
        {
            managers.plantManager.removeGarbage(garbage);
        }
        ActionPlant actionPlant = new ActionPlant(bag[bagIndex].Dna, positionToPlant, managers.sceneManager.Player.PlantTimeIncrease);
        managers.plantManager.createPlantBuilder(actionPlant, true);
        if(fertilizer != null)
        {
            managers.databaseManager.subtractOneItem(fertilizer.Name);
        }
        windowsManager.close((int)Windows.Bag);
    }
    

    public void setActives()
    {
        if(bagIndex <= 0)
        {
            btn_bag_left.interactable = false;
        }
        else
        {
            btn_bag_left.interactable = true;
        }
        if (bagIndex >= bag.Count-1)
        {
            btn_bag_right.interactable = false;
        }
        else
        {
            btn_bag_right.interactable = true;
        }
    }

    public void addFertilizer()
    {
        if (managers.databaseManager.getItemsByType("Fertilizer").Count > 0)
        {
            windowsManager.openItems("Fertilizer");
        }
        else
        {
            windowsManager.openNoItem("Fertilizer");
        }
    }
    


}
