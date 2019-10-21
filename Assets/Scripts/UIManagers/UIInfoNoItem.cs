using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;

public class UIInfoNoItem : MonoBehaviour {
    
    public SceneManagerMy sceneManager;
    public WindowsManager windowsManager;
    public Text txt_info;

    private int index;
    
    public void open(string type)
    {
        switch(type)
        {
            case "Shovel":
                txt_info.text = "Nie posiadasz żadnego szpadla.\n" +
                                "Czy chcesz przejść do sklepu aby kupić ?";
                index = 0;
                break;
            case "Fertilizer":
                txt_info.text = "Nie posiadasz żadnego nawozu.\n" +
                                "Czy chcesz przejść do sklepu aby kupić ?";
                index = 1;
                break;
            case "Conditioner":
                txt_info.text = "Nie posiadasz żadnych odżywek.\n" +
                                "Czy chcesz przejść do sklepu aby kupić ?";
                index = 2;
                break;
        }
    }
    
    public void goToShop()
    {
        windowsManager.close((int)Windows.NoItem);
        windowsManager.openShop(index);
    }

}
