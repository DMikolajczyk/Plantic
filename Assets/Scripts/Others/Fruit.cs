using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour {

    SceneManagerMy sceneManager;
    DatabaseManager dbManager;
    PlantCreating plantParams;

    // Use this for initialization
    void Start () {
        sceneManager = GameObject.Find("Scene_Manager").GetComponent<SceneManagerMy>();
        dbManager = GameObject.Find("DB_Manager").GetComponent<DatabaseManager>();
        plantParams = transform.root.gameObject.GetComponent<PlantCreating>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClick()
    {
        Debug.Log("fruit clicked");
        //Destroy(transform.root.gameObject);
        plantParams.fruited = false;
        plantParams.plantInfo.FruitTime = Utilities.getCurrentTime();
        Utilities.isAfterButtonClick = true;
        Destroy(gameObject);
        Debug.Log("Dostajesz: " + Utilities.getPriceOfFruit(plantParams.plantInfo.Dna) + " golda");
        sceneManager.Player.addGold(Utilities.getPriceOfFruit(plantParams.plantInfo.Dna));
        dbManager.updateFruitTimeForCurrent(transform.root.position.x, transform.root.position.z);
    }

}
