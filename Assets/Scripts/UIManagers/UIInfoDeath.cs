using UnityEngine;
using UnityEngine.UI;
using DialogOptionsEnum;

namespace DialogOptionsEnum
{
    public enum Options { Dig, DigWithShovelBreak, Time };
}

public class UIInfoDeath : MonoBehaviour {

    public GameObject garbageSpawner;
    public ManagersContainer managers;
    public Text txt_description;

    private GameObject garbage;
    
    public void open(int option)
    {
        garbage = managers.plantManager.createGarbage(new Vector2(garbageSpawner.transform.position.x, garbageSpawner.transform.position.z), false, true);
        garbage.AddComponent<RotatePlant>();
        switch(option)
        {
            case (int)Options.Dig:
                txt_description.text = "Niestety uszkodziłeś roślinę podczas przenoszenia i do niczego już się nie nadaje.";
                break;
            case (int)Options.DigWithShovelBreak:
                txt_description.text = "Niestety uszkodziłeś roślinę podczas przenoszenia i do niczego już się nie nadaje. Na dodatek szpadel się połamał.";
                break;
            case (int)Options.Time:
                txt_description.text = "Jedna z Twoich roślin nie była podlewana zbyt długo i uschła. Musisz bardziej dbać o swoje rośliny.";
                break;
        }
    }

    public void close()
    {
        Destroy(garbage);
    }
}
