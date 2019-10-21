using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionManager : MonoBehaviour {

    public GameObject spawnerLeft;
    public GameObject spawnerRight;
    
    public Text awardFieldLeft;
    public Text awardFieldRight;

    public RawImage leftViewPlant;
    public RawImage rightViewPlant;

    public Texture askSignSquare;
    public RenderTexture leftRenderTexture;
    public RenderTexture rightRenderTexture;

    public GameObject panelLeft;
    public GameObject paneRight;

    public ManagersContainer managers;

    private GameObject plantLeft;
    private GameObject plantRight;

    private void Update()
    {

    }
    
    public void open()
    {
        
        List<int[]> missionPlants = managers.databaseManager.getMissionsDnas();
        if (missionPlants.Count > 0)
        {
            plantLeft = managers.plantManager.createPlantInView(new PlantInView(spawnerLeft.transform.position, missionPlants[0]), true);
            awardFieldLeft.text = managers.databaseManager.getAwardValue(missionPlants[0]).ToString();
            if(leftViewPlant.texture != leftRenderTexture)
            {
                leftViewPlant.texture = leftRenderTexture;
            }
        }
        else
        {
            awardFieldLeft.text = "niedostępne";
            if (leftViewPlant.texture != askSignSquare)
            {
                leftViewPlant.texture = askSignSquare;
            }
        }
        if (missionPlants.Count == 2)
        {
            plantRight = managers.plantManager.createPlantInView(new PlantInView(spawnerRight.transform.position, missionPlants[1]), true);
            awardFieldRight.text = managers.databaseManager.getAwardValue(missionPlants[1]).ToString();
            if(rightViewPlant.texture != rightRenderTexture)
            {
                rightViewPlant.texture = rightRenderTexture;
            }
            panelLeft.transform.localPosition = new Vector3(-143, panelLeft.transform.localPosition.y, panelLeft.transform.localPosition.z);
            paneRight.SetActive(true);
        }
        else
        {
            panelLeft.transform.localPosition = new Vector3(0, panelLeft.transform.localPosition.y, panelLeft.transform.localPosition.z);
            paneRight.SetActive(false);
        }

    }
    public void close()
    {
        Destroy(plantLeft);
        Destroy(plantRight);
    }

}
