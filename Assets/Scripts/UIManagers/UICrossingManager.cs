using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;
using NewPlantOptionEnum;

public class UICrossingManager : MonoBehaviour {
    
    public GameObject spawnOne;
    public GameObject spawnTwo;
    private GameObject plantOne;
    public GameObject plantTwo;
    public Text txt_crossChanceOne;
    public Text txt_crossChanceTwo;
    //public RawImage plantImage;
    public RenderTexture renderTexture;
    public Camera cameraPrefab;
    public Vector3 spawnPosition;
    public RawImage plant_btn;
    public RawImage plant_btn_noPlant;
    
    public GameObject spawnerOne;
    public GameObject plantSpawner;
    public Button btn_crossAccept;
    
    public ManagersContainer managers;
    public WindowsManager windowsManager;

    public GameObject content;
    public GameObject item_crossPlant;
    public GameObject item_crossAskSign;
    public RenderTexture plantTexture;

    //public Text newPlantDescription;

    private List<int[]> listAllCrosses;
    private List<int[]> plantsToShow;

    float crossChanceOne;
    float crossChanceTwo;
    
    List<GameObject> askSignSquares = new List<GameObject>();
    List<GameObject> plantSquares = new List<GameObject>();
    List<RenderTexture> plantTextures = new List<RenderTexture>();
    List<Camera> plantCameras = new List<Camera>();
    List<GameObject> plants = new List<GameObject>();

    // Use this for initialization
    void Start () {
        checkNewPlants();
    }
	
    public void open()
    {        
        plantOne = managers.plantManager.createPlantInView(new PlantInView(spawnOne.transform.position, managers.plantManager.dnaOneToCross), true);
        plantTwo = managers.plantManager.createPlantInView(new PlantInView(spawnTwo.transform.position, managers.plantManager.dnaTwoToCross), true);
        crossChanceOne = Utilities.getChanceToCross(managers.plantManager.dnaOneToCross);
        crossChanceTwo = Utilities.getChanceToCross(managers.plantManager.dnaTwoToCross);
        txt_crossChanceOne.text = crossChanceOne.ToString("P");
        txt_crossChanceTwo.text = crossChanceTwo.ToString("P");
        spawnPosition = spawnerOne.transform.position;

        listAllCrosses = getAllCrosses(managers.plantManager.dnaOneToCross, managers.plantManager.dnaTwoToCross);

        List<Plant> plantsDiscovered = managers.databaseManager.getDiscoveredPlants();
        plantsToShow = new List<int[]>();
        foreach (Plant p in plantsDiscovered)
        {
            if(Utilities.ListContainsDna(listAllCrosses, p.Dna)) {
                plantsToShow.Add(p.Dna);
            }
        }

        
        for(int i = 0; i < plantsToShow.Count; i++)
        {
            GameObject square = Instantiate(item_crossPlant);
            square.transform.parent = content.transform;
            square.transform.localScale = new Vector3(1, 1, 1);

            RenderTexture textureTmp = Instantiate(plantTexture);
            Camera cameraTmp = Instantiate(cameraPrefab);
            cameraTmp.targetTexture = textureTmp;
            cameraTmp.transform.position += new Vector3(0, ((i * 30) + 30), 0);

            GameObject plantTmp = managers.plantManager.createPlantInView(new PlantInView(new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z), plantsToShow[i]), false);
            plantTmp.transform.position += new Vector3(0, ((i * 30) + 30), 0);

            square.transform.Find("rawImageForPlant").GetComponent<RawImage>().texture = textureTmp;

            PlantImageCrosser plantImageCrosser = square.GetComponent<PlantImageCrosser>();
            plantImageCrosser.Dna = plantsToShow[i];
            plantImageCrosser.CrossChance = (crossChanceOne + crossChanceTwo) / 2;
            plantImageCrosser.managers = managers;
            plantImageCrosser.windowsManager = windowsManager;
            plantImageCrosser.CrossingTime = Utilities.getCrossingTime(managers.plantManager.dnaOneToCross, managers.plantManager.dnaTwoToCross);
            
            plantSquares.Add(square);
            plantTextures.Add(textureTmp);
            plantCameras.Add(cameraTmp);
            plants.Add(plantTmp);
        }        
        for (int j = 0; j < listAllCrosses.Count - plantsToShow.Count; j++)
        {
            GameObject squareNoPlant = Instantiate(item_crossAskSign);
            squareNoPlant.transform.parent = content.transform;
            squareNoPlant.transform.localScale = new Vector3(1, 1, 1);
            askSignSquares.Add(squareNoPlant);
        }
        setButtonInteractable();
    }

    public void close()
    {
        Destroy(plantOne);
        Destroy(plantTwo);
            
        foreach(GameObject el in askSignSquares)
        {
            Destroy(el);
        }
        foreach (GameObject el in plantSquares)
        {
            Destroy(el);
        }
        foreach (RenderTexture el in plantTextures)
        {
            Destroy(el);
        }
        foreach (Camera el in plantCameras)
        {
            Destroy(el);
        }
        foreach (GameObject el in plants)
        {
            Destroy(el);
        }
        askSignSquares.Clear();
        plantSquares.Clear();
        plantTextures.Clear();
        plantCameras.Clear();
        plants.Clear();

        managers.plantManager.zeroCrossing();
    }
    
    public void crossPlantsBtn()
    {
        ActionCross actionCross = new ActionCross(managers.plantManager.dnaOneToCross, managers.plantManager.dnaTwoToCross);
        managers.guiManager.createTab(actionCross);
        managers.databaseManager.addActionCross(actionCross);

        windowsManager.close((int)Windows.Crossing);
    }

    public void crossPlants(int[] dnaOne, int[] dnaTwo, float crossChance)
    {
        float rand = Random.Range(0.0F, 1.0F);
        if(Enumerable.SequenceEqual(dnaOne, dnaTwo))
        {
            if (rand <= crossChance)
            {
                mutationOrCross(dnaOne);
            }
            else
            {
                windowsManager.open((int)Windows.NoCross);
            }            
        }
        else
        {
            List<int[]> allCrosses = getAllCrosses(dnaOne, dnaTwo);
            if (rand <= crossChance)
            {

                List<Plant> plantsDiscovered = managers.databaseManager.getDiscoveredPlants();
                List<int[]> plantsToCross = new List<int[]>();
                foreach (int[] el in allCrosses)
                {
                    bool discovered = false;
                    foreach (Plant pd in plantsDiscovered)
                    {
                        if(Enumerable.SequenceEqual(pd.Dna, el))
                        {
                            discovered = true;
                        }
                    }
                    if (discovered == false)
                    {
                        plantsToCross.Add(el);
                    }
                }

                if (plantsToCross.Count > 0)
                {
                    int randomDna = Random.Range(0, plantsToCross.Count);
                    mutationOrCross(plantsToCross[randomDna]);
                }
                else
                {
                    /// tu jeszcze do przemyślenia czy na pewno dawać nieudane krzyżowanie czy jakoś inaczej to rozwiązać.
                    windowsManager.open((int)Windows.NoCross);
                }

            }
            else
            {
                windowsManager.open((int)Windows.NoCross);
            }
        }
        
    }
    private void mutationOrCross(int[] dna)
    {
        float chanceOfMutation = managers.sceneManager.Player.getChanceOfMutation();
        float rand = Random.Range(0.0F, 100.0F);
        List<int[]> mutations = getAllMutations(dna);
        if ((rand <= chanceOfMutation) && (mutations.Count > 0))
        {
            int mutRand = Random.Range(0, mutations.Count);
            managers.sceneManager.Player.setChanceOfMutation(1);
            managers.databaseManager.addPlantToBag(dna, 1);
            managers.databaseManager.addPlantToDiscovered(dna);
            windowsManager.openNewPlant(dna, (int)NewPlantOptions.ByMutation);
        }
        else
        {
            managers.sceneManager.Player.setChanceOfMutation(chanceOfMutation + Utilities.stepForMutationChance);
            managers.databaseManager.addPlantToBag(dna, 1);
            managers.databaseManager.addPlantToDiscovered(dna);
            windowsManager.openNewPlant(dna, (int)NewPlantOptions.ByCross);
        }
    }

    private void setButtonInteractable()
    {
        if(listAllCrosses.Count == plantsToShow.Count)
        {
            btn_crossAccept.interactable = false;
        }
        else
        {
            btn_crossAccept.interactable = true;
        }
    }

    public List<int[]> getAllMutations(int[] dna)
    {
        List<int[]> mutations = new List<int[]>();
        List<int[]> dnaDiscovered = managers.databaseManager.getDiscoveredDnas();
        for (int i = 0; i < dna.Length; i++)
        {
            if (Utilities.countsOfParts.ContainsKey(i))
            {
                if (dna[i] < Utilities.countsOfParts[i])
                {
                    int[] dnaTmp = new int[dna.Length];
                    for (int k = 0; k < dnaTmp.Length; k++)
                    {
                        dnaTmp[k] = dna[k];
                        if (k == i)
                        {
                            dnaTmp[k]++;
                        }
                    }
                    if (!Utilities.ListContainsDna(dnaDiscovered, dnaTmp))
                    {
                        if (Utilities.isCorrectDna(dnaTmp))
                        {
                            mutations.Add(dnaTmp);
                        }
                    }
                }
            }
        }
        return mutations;
    }
        
    private List<int[]> getAllCrosses(int[] dnaOne, int[] dnaTwo)
    {
        List<int[]> crosses = new List<int[]>();
        crosses.Add(dnaOne);
        for (int i = 0; i< dnaOne.Length; i++)
        {
            List<int[]> crossesTmp = new List<int[]>();
            foreach (int[] c in crosses)
            {
                int[] tmp = new int[c.Length];
                System.Array.Copy(c, tmp, c.Length);
                tmp[i] = dnaTwo[i];
                Debug.Log("DNA: " + tmp[0] + tmp[1] + tmp[2] + tmp[3] + tmp[4] + tmp[5] + tmp[6]);
                if (!Enumerable.SequenceEqual(tmp, dnaTwo))
                {
                    if (!Utilities.ListContainsDna(crosses, tmp))
                    {
                        crossesTmp.Add(tmp);
                    }
                }
            }
            crosses.AddRange(crossesTmp);
        }
        crosses.Remove(dnaOne);
        List<int[]> crossesAfterCheck = new List<int[]>();
        //// kolce i liście nie mogąbyć bez łodygi
        foreach (int[] dna in crosses)
        {
            if(Utilities.isCorrectDna(dna))
            {
                crossesAfterCheck.Add(dna);
            } 
        }

        return crossesAfterCheck;
    }
    
    public void checkNewPlants()
    {
        bool unlocked = false;
        List<Plant> discoveredList = managers.databaseManager.getDiscoveredPlants();
        int countOfDiscovered = discoveredList.Count;
        int[] dna = null;

        if((countOfDiscovered >= 4) && (!discoveredList.Any(el => (el.Dna[0] == Utilities.basicDnas[2][0])
            && (Enumerable.SequenceEqual(el.Dna, Utilities.basicDnas[2]))
            )))
        {
            Debug.Log("Odblokowwane roślinę nr 3");
            dna = Utilities.basicDnas[2];
            managers.databaseManager.addPlantToBag(dna, 100);
            managers.databaseManager.addPlantToDiscovered(dna);
            unlocked = true;
        }
        if ((countOfDiscovered >= 8) && (!discoveredList.Any(el => (el.Dna[0] == Utilities.basicDnas[3][0])
            && (Enumerable.SequenceEqual(el.Dna, Utilities.basicDnas[3]))
            )))
        {
            Debug.Log("Odblokowwane roślinę nr 4");
            dna = Utilities.basicDnas[3];
            managers.databaseManager.addPlantToBag(dna, 100);
            managers.databaseManager.addPlantToDiscovered(dna);
            unlocked = true;
        }
        if(unlocked)
        {
            windowsManager.openNewPlant(dna, (int)NewPlantOptions.Award);
        }
    }

}
