using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;

public class UIPlantManager : MonoBehaviour {

    private Vector3 clickedPositionWorld;
    private Vector3 positionOnScreen;
    private int price;

    public GameObject plant { get; set; }

    public Button btn_sell;
    public Button btn_cross;
    public Image img_circle;
    public GameObject waterRain;
    
    public ManagersContainer managers;
    public WindowsManager windowsManager;
    	
    void Update ()
    {
        if (windowsManager.isOpen((int)Windows.PlantManager))
        {
            if (managers.guiManager.tabsAndActions.Count < managers.sceneManager.Player.MaxInQueue)
            {
                btn_cross.interactable = true;
            }
            else
            {
                btn_cross.interactable = false;
            }
            if (plant != null)
            {
                positionOnScreen = Camera.main.WorldToScreenPoint(clickedPositionWorld);
                img_circle.transform.position = new Vector3(positionOnScreen.x, positionOnScreen.y);
            }
        }
    }

    public void open(GameObject p, Vector3 position)
    {
        managers.plantManager.zeroCrossing();
        
        if (p.transform.parent != null)
        {
            plant = p.transform.parent.gameObject;
        }
        else
        {
            plant = p;
        }
        
        clickedPositionWorld = position;
        positionOnScreen = Camera.main.WorldToScreenPoint(clickedPositionWorld);
        setPositionOfButtons();
    }
    
    public void setPositionOfButtons()
    {
        price = Utilities.getPriceOfPlant(managers.plantManager.getDna(plant.transform.position.x, plant.transform.position.z));
        btn_sell.transform.Find("txt_sell").GetComponent<Text>().text = "Sprzedaj za " + price;
        img_circle.transform.position = new Vector3(positionOnScreen.x, positionOnScreen.y);
    }

    public void sellPlant()
    {
        managers.plantManager.removePlant(plant.transform.root.gameObject);
        managers.sceneManager.Player.addGold(price);
        windowsManager.close((int)Windows.PlantManager);
    }

    public void crossPlant()
    {
        managers.plantManager.dnaOneToCross = managers.plantManager.getDna(plant.transform.root.transform.position.x, plant.transform.root.transform.position.z);
        plant.transform.root.GetComponent<PlantCreating>().isCrossing = true;

        windowsManager.close((int)Windows.PlantManager);
        windowsManager.open((int)Windows.InfoCrossing);
        managers.plantManager.CrossMode = true;
    }

    public void waterPlant()
    {
        waterPlant(plant);
    }
    public void waterPlant(GameObject p, int bonus = 0)
    {
        PlantOnField pof = plant.GetComponent<PlantCreating>().plantInfo;
        pof.LifeBonus = bonus;
        pof.LifeTimeMinutes = Utilities.getPlantLifeInMinutes(pof.Dna) + (int)(Utilities.getPlantLifeInMinutes(pof.Dna) * ((float)(bonus) / 100));
        managers.databaseManager.updateLifeBonusForPlant(plant.transform.position.x, plant.transform.position.z, pof.LifeBonus);

        managers.plantManager.restorePlantHealth(p);
        GameObject rain = Instantiate(waterRain);
        rain.transform.position = new Vector3(p.transform.position.x, p.transform.position.y + 7, p.transform.position.z);
        windowsManager.close((int)Windows.PlantManager);
    }

    public void waterPlantPlus()
    {
        if (managers.databaseManager.getItemsByType("Conditioner").Count > 0)
        {
            windowsManager.openItems("Conditioner", plant);
        }
        else
        {
            windowsManager.openNoItem("Conditioner");
        }
        windowsManager.close((int)Windows.PlantManager);
    }

    public void digPlant()
    {
        if (managers.databaseManager.getItemsByType("Shovel").Count > 0)
        {
            windowsManager.openItems("Shovel", plant);
        }
        else
        {
            windowsManager.openNoItem("Shovel");
        }
        windowsManager.close((int)Windows.PlantManager);
    }

}
