using UnityEngine;
using UnityEngine.UI;
using NewPlantOptionEnum;

namespace NewPlantOptionEnum
{
    public enum NewPlantOptions { ByCross, ByMutation, Award };
}

public class UINewPlantManager : MonoBehaviour {

    public ManagersContainer managers;
    public GameObject plantSpawner;
    public Text newPlantDescription;
    
    private GameObject newPlant;
        
    public void open(int[] dna, int option)
    {
        switch (option)
        {
            case (int)NewPlantOptions.ByCross:
                newPlantDescription.text = "Właśnie udało Ci się uzyskać nową roślinę w wyniku krzyżowania. Znajdziesz ją w plecaku.";
                break;
            case (int)NewPlantOptions.ByMutation:
                newPlantDescription.text = "Niesamowite! Roślina otrzymana w wyniku krzyżowania dodatkowo ewoluowała. Znajdziesz ją w plecaku.";
                break;
            case (int)NewPlantOptions.Award:
                newPlantDescription.text = "Dobrze Ci idzie! Z tej okazji dostajesz nową roślinę. Znajdziesz ją w plecaku.";
                break;
        }
        if(newPlant != null)
        {
            Destroy(newPlant);
        }
        newPlant = managers.plantManager.createPlantInView(new PlantInView(plantSpawner.transform.position, dna), true);
        
    }
    public void close()
    {
        Destroy(newPlant);
    }

}
