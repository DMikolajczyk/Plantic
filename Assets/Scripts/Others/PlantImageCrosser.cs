using UnityEngine;
using WindowsEnum;

public class PlantImageCrosser : MonoBehaviour {

    public int[] Dna { get; set; }
    public float CrossChance { get; set; }
    public float CrossingTime { get; set; }
    public ManagersContainer managers { get; set; }
    public WindowsManager windowsManager { get; set; }
    

    public void crossBtn()
    {
        ActionCross actionCross = new ActionCross(Dna, CrossChance, CrossingTime);
        managers.guiManager.createTab(actionCross);
        managers.databaseManager.addActionCross(actionCross);

        windowsManager.close((int)Windows.Crossing);
    }
    
}
