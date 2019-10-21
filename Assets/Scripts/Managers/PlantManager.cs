using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    ///////////////////////////////////////// Parts of plants ///////////////////////
    public GameObject[] stalks;
    public GameObject[] cups;
    public GameObject[] spikes;
    public GameObject[] leaves;
    ///////////////////////////////////////// Canvases to Plant //////////////////////////////
    public Canvas canvas_hpBar;
    public Canvas canvas_fruitBox;
    ///////////////////////////////////////// Managers //////////////////////////////
    public WindowsManager windowsManager;
    public ManagersContainer managers;
    ///////////////////////////////////////// Other GameObjects /////////////////////
    public GameObject garbage;
    public GameObject plantBuilder;
    ///////////////////////////////////////// To cross ////////////////////////////////
    public int[] dnaOneToCross { get; set; }
    public int[] dnaTwoToCross { get; set; }
    public bool CrossMode { get; set; }
    ///////////////////////////////////////// Lists ////////////////////////////////////
    public List<GameObject> plantBuilders = new List<GameObject>();
    void Start()
    {
        CrossMode = false;
    }
    public void removePlantBuilder(GameObject pb, ActionPlant actionPlant)
    {
        managers.databaseManager.removePlantBuilder(pb.transform.position.x, pb.transform.position.z);
        plantBuilders.Remove(pb);
        Destroy(pb);
        addPlantToDatabaseAndField(actionPlant.Position, actionPlant.Dna);
    }
    public void addPlantToDatabaseAndField(Vector3 position, int[] dna)
    {
            PlantOnField plant = new PlantOnField(position, 0, Utilities.getCurrentTime() ,dna);
            managers.databaseManager.addPlant(plant.Position.x, plant.Position.z, dna);
            createPlantOnField(plant);
    }
    public void createPlantOnField(PlantOnField plant)
    {
        GameObject newPlant = generatePlantMesh(plant.Dna, plant.Position.x, plant.Position.z);
        newPlant.AddComponent<PlantCreating>();
        PlantCreating plantParams = newPlant.GetComponent<PlantCreating>();
        plantParams.bornTime = managers.databaseManager.getBornTimeOfPlantOnField(newPlant.transform.position.x, newPlant.transform.position.z);
        plantParams.plantInfo = plant;
        plantParams.managers = managers;
        plantParams.windowsManager = windowsManager;
        managers.planesManager.setFree(new Vector2(plant.Position.x, plant.Position.z), false);
    }
    public GameObject createGarbage(Vector2 garbagePosition, bool saveToDB, bool toShowInView)
    {
        GameObject garbageTmp = Instantiate(garbage);
        garbageTmp.transform.position = new Vector3(garbagePosition.x, 0, garbagePosition.y);
        if (!toShowInView)
        {
            garbageTmp.tag = "garbage";
            managers.planesManager.setFree(new Vector2(garbagePosition.x, garbagePosition.y), false);
        }
        if(saveToDB)
        {
            managers.databaseManager.addGarbage(garbagePosition.x, garbagePosition.y);
        }
        return garbageTmp;
    }
    public void removeGarbage(GameObject g)
    {
        managers.databaseManager.removeGarbage(g.transform.position.x, g.transform.position.z);
        managers.planesManager.setFree(new Vector2(g.transform.position.x, g.transform.position.z), true);
        Destroy(g);
    }
    public void createPlantBuilder(ActionPlant actionPlant, bool saveToDB)
    {
        GameObject ap = Instantiate(plantBuilder);
        plantBuilders.Add(ap);
        managers.guiManager.createTab(actionPlant);
        ap.transform.position = new Vector3(actionPlant.Position.x, 0, actionPlant.Position.z);
        managers.planesManager.setFree(new Vector2(actionPlant.Position.x, actionPlant.Position.z), false);
        if (saveToDB)
        {
            managers.databaseManager.addPlantBuilder(actionPlant);
        }
    }
    
    public GameObject generatePlantMesh(int[] dna, float x, float z)
    {
        GameObject newStalk = null;
        //GameObject newCup = null;

        if(stalks[dna[0]] != null)
        {
            newStalk = Instantiate(stalks[dna[0]]);
            newStalk.transform.position = new Vector3(x, 0, z);
            newStalk.tag = "plantRoot";

            foreach (Transform cupHolder in newStalk.transform)
            {
                if (cupHolder.tag == "respCup")
                {
                    GameObject cup = Instantiate(cups[dna[1]]);
                    cup.transform.position = cupHolder.transform.position;
                    cup.transform.rotation = cupHolder.transform.rotation;
                    cup.transform.parent = newStalk.transform;
                    Destroy(cupHolder.gameObject);
                }
            }
            
            if (spikes[dna[4]] != null)
            {
                foreach (Transform spikeHolder in newStalk.transform)
                {
                    if (spikeHolder.tag == "respSpike")
                    {
                        GameObject spike = Instantiate(spikes[dna[4]]);
                        spike.transform.position = spikeHolder.transform.position;
                        spike.transform.rotation = spikeHolder.transform.rotation;
                        spike.transform.parent = newStalk.transform;
                        Destroy(spikeHolder.gameObject);
                    }
                }
            }
            if (leaves[dna[2]] != null)
            {
                foreach (Transform leafHolder in newStalk.transform)
                {
                    if (leafHolder.tag == "respLeaf")
                    {
                        GameObject leaf = Instantiate(leaves[dna[2]]);
                        leaf.transform.position = leafHolder.transform.position;
                        leaf.transform.rotation = leafHolder.transform.rotation;
                        leaf.transform.parent = newStalk.transform;
                        Destroy(leafHolder.gameObject);
                    }
                }
            }
            return newStalk;
        }
        else
        {
            GameObject newCup = Instantiate(cups[dna[1]]);
            newCup.transform.position = new Vector3(x, 0, z);
            newCup.tag = "plantRoot";
            return newCup;
        }        
    }
    public GameObject createPlantInView(PlantInView plant, bool isMoving)
    {
        GameObject p = generatePlantMesh(plant.Dna, plant.Position.x, plant.Position.z);
        if (isMoving)
        {
            p.AddComponent<RotatePlant>();
        }
        return p;        
    }
    public int[] getDna(float x, float z)
    {
        return managers.databaseManager.getDnaOfPlantOnField(x, z);
    }
    public void removePlant(GameObject plantObject)
    {
        managers.databaseManager.removePlant(plantObject.transform.position.x, plantObject.transform.position.z);
        managers.planesManager.setFree(new Vector2(plantObject.transform.position.x, plantObject.transform.position.z), true);
        Destroy(plantObject);
    }
    public void restorePlantHealth(GameObject plant)
    {
        managers.databaseManager.setBornTimeOfPlantOnFieldToActual(plant);
    }

    public void zeroCrossing()
    {
        dnaOneToCross = null;
        dnaTwoToCross = null;
        CrossMode = false;
    }


}
