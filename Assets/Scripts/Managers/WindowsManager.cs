using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;

namespace WindowsEnum
{
    public enum Windows { InfoCrossing, Bag, PlantManager, Crossing, Shop, InfoDeath,
                            Items, NoItem, InfoDigged, NewPlant, NoCross, MissionCompleted,
                            Missions};
}

public class WindowsManager : MonoBehaviour {

    public Dictionary<int, Canvas> canvasesForWindowsEnum = new Dictionary<int, Canvas>();

    public Canvas canvas_infoCrossing;
    public Canvas canvas_bag;
    public Canvas canvas_plantManager;
    public Canvas canvas_crossing;
    public Canvas canvas_shop;
    public Canvas canvas_infoDeath;
    public Canvas canvas_items;
    public Canvas canvas_noItem;
    public Canvas canvas_infoDigged;
    public Canvas canvas_newPlant;
    public Canvas canvas_noCross;
    public Canvas canvas_missionCompleted;
    public Canvas canvas_missions;

    public ManagersContainer managers;

    public Text awardText;

    private UIPlantManager uiManager_plant;
    private UIBagManager uiManager_bag;
    private UICrossingManager uiManager_crossing;
    private UIShopManager uiManager_shop;
    private UIInfoDeath uiInfo_death;
    private UIItemsManager uiManager_items;
    private UIInfoNoItem uiInfo_noItem;
    private UINewPlantManager uiManager_newPlant;
    private UIMissionManager uiManager_mission;

    private void Awake()
    {
        uiManager_plant = canvas_plantManager.GetComponent<UIPlantManager>();
        uiManager_bag = canvas_bag.GetComponent<UIBagManager>();
        uiManager_crossing = canvas_crossing.GetComponent<UICrossingManager>();
        uiManager_shop = canvas_shop.GetComponent<UIShopManager>();
        uiInfo_death = canvas_infoDeath.GetComponent<UIInfoDeath>();
        uiManager_items = canvas_items.GetComponent<UIItemsManager>();
        uiInfo_noItem = canvas_noItem.GetComponent<UIInfoNoItem>();
        uiManager_newPlant = canvas_newPlant.GetComponent<UINewPlantManager>();
        uiManager_mission = canvas_missions.GetComponent<UIMissionManager>();
    }
    // Use this for initialization
    void Start () {
        canvasesForWindowsEnum.Add((int)Windows.InfoCrossing, canvas_infoCrossing);
        canvasesForWindowsEnum.Add((int)Windows.Bag, canvas_bag);
        canvasesForWindowsEnum.Add((int)Windows.PlantManager, canvas_plantManager);
        canvasesForWindowsEnum.Add((int)Windows.Crossing, canvas_crossing);
        canvasesForWindowsEnum.Add((int)Windows.Shop, canvas_shop);
        canvasesForWindowsEnum.Add((int)Windows.InfoDeath, canvas_infoDeath);
        canvasesForWindowsEnum.Add((int)Windows.Items, canvas_items);
        canvasesForWindowsEnum.Add((int)Windows.NoItem, canvas_noItem);
        canvasesForWindowsEnum.Add((int)Windows.InfoDigged, canvas_infoDigged);
        canvasesForWindowsEnum.Add((int)Windows.NewPlant, canvas_newPlant);
        canvasesForWindowsEnum.Add((int)Windows.NoCross, canvas_noCross);
        canvasesForWindowsEnum.Add((int)Windows.MissionCompleted, canvas_missionCompleted);
        canvasesForWindowsEnum.Add((int)Windows.Missions, canvas_missions);

        disabledCanvases();
    }
	
    public bool isOpen(int window)
    {
        if(canvasesForWindowsEnum[window].enabled)
        {
            return true;
        }
        return false;
    }

    public void open(int window)
    {
        if (window == (int)Windows.Crossing)
        {
            uiManager_crossing.open();
            canvas_crossing.enabled = true;
        }
        else if ((window != (int)Windows.Bag) && (window != (int)Windows.PlantManager)
             && (window != (int)Windows.Shop) && (window != (int)Windows.InfoDeath)
             && (window != (int)Windows.Items) && (window != (int)Windows.NoItem)
             && (window != (int)Windows.NewPlant) )
        {
            canvasesForWindowsEnum[window].enabled = true;
        }
        else
        {
            Debug.Log("MY_ERROR: Bad use of function open(int window). Check if window wich you want to open has specify function.");
        }
    }
    public void openBag(Vector3 pos, GameObject g = null)
    {
        uiManager_bag.openBagWindow(pos, g);
        canvas_bag.enabled = true;
    }
    public void openPlantManager(GameObject p, Vector3 pos)
    {
        uiManager_plant.open(p, pos);
        canvas_plantManager.enabled = true;
    }
    public void openShop(int tab)
    {
        uiManager_shop.setTab(tab);
        canvas_shop.enabled = true;
    }
    public void openInfoDeath(int option)
    {
        closeAllWindows();
        uiInfo_death.open(option);
        canvas_infoDeath.enabled = true;
    }
    public void openItems(string type, GameObject p = null)
    {
        uiManager_items.open(type, p);
        canvas_items.enabled = true;
    }
    public void openNoItem(string type)
    {
        closeAllWindows();
        uiInfo_noItem.open(type);
        canvas_noItem.enabled = true;
    }
    public void openNewPlant(int[] dna, int option)
    {
        if (Utilities.ListContainsDna(managers.databaseManager.getMissionsDnas(), dna))
        {
            int award = managers.databaseManager.getAwardValue(dna);
            managers.sceneManager.Player.addGold(award);
            managers.databaseManager.removeMission(dna);
            Debug.Log("wykonano misję");
            openMissionCompleted(award);
        }
        else
        {
            uiManager_newPlant.open(dna, option);
            canvas_newPlant.enabled = true;
        }
    }
    public void openMissionCompleted(int award)
    {
        awardText.text = award.ToString();
        canvas_missionCompleted.enabled = true;
    }
    public void openMissions()
    {
        uiManager_mission.open();
        Utilities.isAfterButtonClick = true;
        canvas_missions.enabled = true;
    }
    public void close(int window)
    {
        if (isOpen(window))
        {
            canvasesForWindowsEnum[window].enabled = false;
            switch (window)
            {
                case (int)Windows.InfoCrossing:
                    if (uiManager_plant.plant != null)
                    {
                        GameObject plant = uiManager_plant.plant;
                        if (plant.transform.parent != null)
                        {
                            plant.transform.parent.GetComponent<PlantCreating>().stopChangingColor();
                        }
                        else
                        {
                            plant.GetComponent<PlantCreating>().stopChangingColor();
                        }
                    }
                    managers.plantManager.CrossMode = false;
                    break;
                case (int)Windows.Bag:
                    uiManager_bag.closeBagWindow();
                    break;
                case (int)Windows.Crossing:
                    uiManager_crossing.close();
                    break;
                case (int)Windows.InfoDeath:
                    uiInfo_death.close();
                    break;
                case (int)Windows.NoItem:
                    Utilities.isAfterButtonClick = true;
                    break;
                case (int)Windows.NewPlant:
                    uiManager_newPlant.close();
                    //uiManager_crossing.checkNewPlants();
                    break;
                case (int)Windows.NoCross:
                    uiManager_crossing.checkNewPlants();
                    break;
                case (int)Windows.MissionCompleted:                    
                    /// zamiast tego może po każdym zamknięciu okna sprawdzane
                    /// to czy jest mniej niż 2 misje w DB.
                    break;
                case (int)Windows.Missions:
                    uiManager_mission.close();
                    break;
            }
            uiManager_crossing.checkNewPlants();
            managers.sceneManager.addMission();
            Utilities.isAfterButtonClick = true;
        }
    }
    public void disabledCanvases()
    {
        foreach (Canvas c in canvasesForWindowsEnum.Values)
        {
            if(c.enabled)
            {
                c.enabled = false;
            }
        }
    }

    /// Using in Editor - don't remove when 0 references !
    public void close(Canvas c)
    {
        foreach (int i in canvasesForWindowsEnum.Keys)
        {
            if (c == canvasesForWindowsEnum[i])
            {
                close(i);
            }
        }
    }
    public void closeAllWindows()
    {
        foreach(int i in canvasesForWindowsEnum.Keys)
        {
            close(i);
        }
    }
    public bool isAnyBlockCameraWindowOpen()
    {
        if (isOpen((int)Windows.Bag) ||
            isOpen((int)Windows.Crossing) ||
            isOpen((int)Windows.NewPlant) ||
            isOpen((int)Windows.NoCross) ||
            isOpen((int)Windows.Shop) ||
            isOpen((int)Windows.InfoDeath) ||
            isOpen((int)Windows.Items))
        {
            return true;
        }
        return false;
    }
    public bool isAnyInfoDialogOpen()
    {
        if (isOpen((int)Windows.InfoCrossing) ||
            isOpen((int)Windows.NoItem) ||
            isOpen((int)Windows.InfoDigged))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
