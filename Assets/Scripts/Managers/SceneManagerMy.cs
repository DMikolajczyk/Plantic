using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsEnum;
using PartsOfPlantEnum;
using System.Linq;

namespace PartsOfPlantEnum
{
    enum PartsOfPlant { Stalk = 0, Cup = 1, Leaf = 2, Spike = 4 }
}

public class SceneManagerMy : MonoBehaviour {
    public Player Player { get; set; }
    
    public GameObject grassLeaf;
    public GameObject stone;

    public ManagersContainer managers;
    public WindowsManager windowsManager;
    
    // Use this for initialization
    void Start () {
        managers.planesManager.initPlanes();
        loadPlants();
        loadGarbages();
        loadPlantBuilders();
        loadActionCrosses();
        Player = new Player(managers.guiManager.textGold, managers.guiManager.textDiamonds);
        initBag();
        setCountsOfPlantParts();

        
        

        generateGrass();
        generateGrass();
        generateStones();
    }
    public void loadPlants()
    {
        List<PlantOnField> plants;
        plants = managers.databaseManager.getPlantsOnField();
        foreach (PlantOnField p in plants)
        {
            managers.plantManager.createPlantOnField(p);
        }
    }
    public void loadGarbages()
    {
        List<Vector2> garbages = managers.databaseManager.getGarbagesPositions();
        foreach (Vector2 g in garbages)
        {
            managers.plantManager.createGarbage(g, false, false);
        }
    }
    public void loadPlantBuilders()
    {
        List<ActionPlant> actionPlants = managers.databaseManager.getPlantBuilders();
        foreach (ActionPlant ap in actionPlants)
        {
            managers.plantManager.createPlantBuilder(ap, false);
        }
    }
    public void loadActionCrosses()
    {
        List<ActionCross> actionCrosses = managers.databaseManager.getActionCrosses();
        foreach (ActionCross ac in actionCrosses)
        {
            managers.guiManager.createTab(ac);
        }
    }
    public void initBag()
    {
        if (managers.databaseManager.getPlantsInBag().Count == 0)
        {
            List<int[]> plantsOnStart = new List<int[]>();
            plantsOnStart.Add(Utilities.basicDnas[0]);
            plantsOnStart.Add(Utilities.basicDnas[1]);
            //Player.setGold(1000);
            foreach (int[] p in plantsOnStart)
            {
                managers.databaseManager.addPlantToBag(p, 100);
                managers.databaseManager.addPlantToDiscovered(p);
            }
            addMission();
        }
    }
    private void setCountsOfPlantParts()
    {
        Utilities.countsOfParts.Add((int)PartsOfPlant.Stalk, managers.plantManager.stalks.Length - 1);
        Utilities.countsOfParts.Add((int)PartsOfPlant.Cup, managers.plantManager.cups.Length - 1);
        Utilities.countsOfParts.Add((int)PartsOfPlant.Spike, managers.plantManager.spikes.Length - 1);
        Utilities.countsOfParts.Add((int)PartsOfPlant.Leaf, managers.plantManager.leaves.Length - 1);
    }
    public void generateGrass()
    {
        Vector2 randPos;
        Vector2 randScale;
        Vector3 randRot;
        GameObject parentOfGrass = Instantiate(grassLeaf);
        for (int i = 0; i < 100; i++) {
            randPos = new Vector2(Random.Range(-2.25F, 4.5F * 8 - 2.25F), Random.Range(-2.25F, 4.5F * 8 - 2.25F));
            randScale = new Vector2(Random.Range(1F, 2F), Random.Range(0.5F, 1.4F));
            randRot = new Vector3(Random.Range(-20F, 20F), Random.Range(-20F, 20F), Random.Range(-20F, 20F));
            GameObject grassTmp = Instantiate(grassLeaf);
            grassTmp.transform.position = new Vector3(randPos.x, 0, randPos.y);
            grassTmp.transform.localScale = new Vector3(randScale.x, randScale.x, randScale.y);
            grassTmp.transform.Rotate(randRot.x, randRot.y, randRot.z);
            grassTmp.transform.parent = parentOfGrass.transform;
        }

        mergeMeshes(parentOfGrass);
        
    }

    public void generateStones()
    {
        Vector2 randPos;
        float randScale;
        float randAngleZ;

        GameObject parentOfStones = Instantiate(stone);
        for (int i = 0; i < 20; i++)
        {
            randPos = new Vector2(Random.Range(-2.25F, 4.5F * 8 - 2.25F), Random.Range(-2.25F, 4.5F * 8 - 2.25F));
            randScale = Random.Range(0.2F, 1.5F);
            randAngleZ = Random.Range(-20F, 20F);
            GameObject stoneTmp = Instantiate(stone);
            stoneTmp.transform.position = new Vector3(randPos.x, 0, randPos.y);
            stoneTmp.transform.localScale = new Vector3(randScale, randScale, randScale);
            stoneTmp.transform.Rotate(0, 0, randAngleZ);
            stoneTmp.transform.parent = parentOfStones.transform;
        }
        for (int i = 0; i < 100; i++)
        {
            randPos = new Vector2(Random.Range(-2.25F, 4.5F * 8 - 2.25F), Random.Range(-2.25F, 4.5F * 8 - 2.25F));
            randScale = Random.Range(0.2F, 0.5F);
            randAngleZ = Random.Range(-20F, 20F);
            GameObject stoneTmp = Instantiate(stone);
            stoneTmp.transform.position = new Vector3(randPos.x, 0, randPos.y);
            stoneTmp.transform.localScale = new Vector3(randScale, randScale, randScale);
            stoneTmp.transform.Rotate(0, 0, randAngleZ);
            stoneTmp.transform.parent = parentOfStones.transform;
        }

        mergeMeshes(parentOfStones);
    }
    
    private void mergeMeshes(GameObject parent)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int j = 0;
        while (j < meshFilters.Length)
        {
            combine[j].mesh = meshFilters[j].sharedMesh;
            combine[j].transform = meshFilters[j].transform.localToWorldMatrix;
            meshFilters[j].gameObject.SetActive(false);
            j++;
        }
        parent.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        parent.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        parent.transform.gameObject.SetActive(true);
        parent.transform.Rotate(90, 0, 0);
    }

    public void addMission()
    {
        if (managers.databaseManager.getCountOfMissions() < 2)
        {
            int award = Utilities.getAward(managers.databaseManager);

            List<int[]> discoveredDnas = managers.databaseManager.getDiscoveredDnas();
            int dnaLength = discoveredDnas[0].Length;
            List<int>[] dnaNumbers = new List<int>[dnaLength];
            int[] lastDna = discoveredDnas[0];

            int countOfLongest = 0;
            ///// Tworzenie list unikalnych cech
            foreach (int[] d in discoveredDnas)
            {
                for (int i = 0; i < d.Length; i++)
                {
                    if (dnaNumbers[i] == null)
                    {
                        dnaNumbers[i] = new List<int>();
                    }
                    if (!dnaNumbers[i].Contains(d[i]))
                    {
                        dnaNumbers[i].Add(d[i]);
                        if (dnaNumbers[i].Count > countOfLongest)
                        {
                            countOfLongest = dnaNumbers[i].Count;
                        }
                    }
                }
                lastDna = d;
            }
            Debug.Log("Najdluzszy lancych: " + countOfLongest);
            ///// Tworzenie nowych łańcuchów dna
            foreach (List<int> list in dnaNumbers)
            {
                int countList = list.Count;
                for (int i = countList; i < countOfLongest; i++)
                {
                    list.Add(list[0]);
                }
            }

            List<int[]> dnasMerges = new List<int[]>();
            for (int i = 0; i < countOfLongest; i++)
            {
                int[] tmp = new int[dnaLength];
                for (int j = 0; j < dnaLength; j++)
                {
                    tmp[j] = dnaNumbers[j][i];
                }
                dnasMerges.Add(tmp);
            }

            ///DISPLAY MARGES
            for (int i = 0; i < dnasMerges.Count; i++)
            {
                string text = "DNA nr " + i + ": ";
                for (int j = 0; j < dnaLength; j++)
                {
                    text += dnasMerges[i][j];
                }
                Debug.Log(text);
            }

            List<int[]> finalDnas = new List<int[]>();
            for (int i = 1, j = 0; i < dnasMerges.Count; i++)
            {
                finalDnas.AddRange(getAllCrosses(dnasMerges[i], dnasMerges[j], finalDnas, discoveredDnas));

                if (i == dnasMerges.Count - 1)
                {
                    j++;
                    i = j;
                }
            }
            int[] dnaUsed = null;
            List<int[]> dnasMissions = managers.databaseManager.getMissionsDnas();
            foreach (int[] d in finalDnas)
            {
                if (Utilities.ListContainsDna(dnasMissions, d))
                {
                    dnaUsed = d;
                }
            }
            if (dnaUsed != null)
            {
                finalDnas.Remove(dnaUsed);
            }
            Debug.Log("Count of all crosses to mission: " + finalDnas.Count);
            /// losowanie
            if (finalDnas.Count > 0)
            {
                int rand = Random.Range(0, finalDnas.Count);
                int[] missionDna = finalDnas[rand];
                managers.databaseManager.addMission(missionDna, award);
                if (managers.databaseManager.getCountOfMissions() < 2)
                {
                    addMission();
                }
                if (!windowsManager.isOpen((int)Windows.Missions))
                {
                    windowsManager.openMissions();
                }
            }
            
        }
    }

    private List<int[]> getAllCrosses(int[] dnaOne, int[] dnaTwo, List<int[]> finalDnas, List<int[]> discoveredDnas)
    {
        List<int[]> crosses = new List<int[]>();
        crosses.Add(dnaOne);
        for (int i = 0; i < dnaOne.Length; i++)
        {
            List<int[]> crossesTmp = new List<int[]>();
            foreach (int[] c in crosses)
            {
                int[] tmp = new int[c.Length];
                System.Array.Copy(c, tmp, c.Length);
                tmp[i] = dnaTwo[i];
                if (!Enumerable.SequenceEqual(tmp, dnaTwo))
                {
                    if (!Utilities.ListContainsDna(crosses, tmp) && !Utilities.ListContainsDna(finalDnas, tmp) && !Utilities.ListContainsDna(discoveredDnas, tmp))
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
            if (Utilities.isCorrectDna(dna))
            {
                crossesAfterCheck.Add(dna);
            }
        }

        return crossesAfterCheck;
    }

}
