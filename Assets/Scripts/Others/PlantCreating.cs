using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DialogOptionsEnum;

public class PlantCreating : MonoBehaviour {
    
    private float actualRot;
    private float speedRot;
    private int randomStartRotate;
    private float sum;
    private bool created;
    private bool barCreated;
    private int counter = 0;
    public bool fruited { get; set; }
    public PlantOnField plantInfo { get; set; }
    public WindowsManager windowsManager { get; set; }
    public ManagersContainer managers { get; set; }

    private TimeSpan timeDifference;
    private float minutesToDeath;
    public Material[] originalMaterialsOfPlant;
    private Dictionary<GameObject, Material[]> childsAndMaterials;

    public bool isCrossing { get; set; }

    public DateTime bornTime { get; set; }

    // Use this for initialization
    void Start () {
        actualRot = 0;
        sum = 0;
        counter = 0;
        created = false;
        barCreated = false;
        isCrossing = false;
        fruited = false;
        originalMaterialsOfPlant = gameObject.GetComponent<Renderer>().sharedMaterials;
        childsAndMaterials = new Dictionary<GameObject, Material[]>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Renderer>() != null && (child.CompareTag("plant") || child.CompareTag("plantRoot")))
            {
                childsAndMaterials.Add(child.gameObject, child.gameObject.GetComponent<Renderer>().sharedMaterials);
            }
        }
        randomStartRotate = UnityEngine.Random.Range(300, 359);
        speedRot = UnityEngine.Random.Range(0.07F, 0.13F);
        transform.Rotate(0, 0, randomStartRotate);
		transform.localScale = Vector3.zero;
        if (CompareTag("plantRoot"))
        {
            GameObject.Find("PlaneToTouch").GetComponent<PlanesManager>().setFree(new Vector2(transform.position.x, transform.position.z), false);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!created)
        {
            created = true;
            if (transform.localScale.x < 1)
            {
                transform.localScale += new Vector3(0.02F, 0.02F, 0);
                created = false;
            }
            if (transform.localScale.z < 1)
            {
                transform.localScale += new Vector3(0, 0, 0.015F);
                created = false;
            }
            if (transform.rotation.z > 0 && CompareTag("plantRoot"))
            {
                transform.Rotate(new Vector3(0, 0, -actualRot));
                if (sum < randomStartRotate / 2)
                {
                    actualRot += speedRot;
                }
                else if (actualRot > 1)
                {
                    actualRot -= speedRot;
                }
                sum += actualRot;
                created = false;
            }
        }
        else if(!barCreated)
        {
            Canvas hpbar = Instantiate(managers.plantManager.canvas_hpBar);
            hpbar.transform.parent = transform;
            hpbar.name = "PlantBar_Canvas";
            hpbar.transform.localRotation = new Quaternion(0, 0, 0, 0);
            hpbar.transform.Rotate(new Vector3(-42, -121, -70));
            hpbar.transform.localPosition = new Vector3(1.9F, -1.5F, 5.8F);

            barCreated = true;
        }
        
        else
        {
            animate();
            updateBar();
            if (isCrossing)
            {
                changeColor();
            }
        }
        if (!fruited && created)
        {
            TimeSpan timeDifference = (Utilities.getCurrentTime() - plantInfo.FruitTime);
            float minutesToFruit = Utilities.getFruitTime(plantInfo.Dna) - (float)timeDifference.TotalMinutes;
            if (minutesToFruit <= 0)
            {
                /// sprawdź czy stworzyć owoce
                Canvas fruitBox = Instantiate(managers.plantManager.canvas_fruitBox);
                fruitBox.transform.parent = transform;
                fruitBox.transform.localRotation = new Quaternion(0, 0, 0, 0);
                fruitBox.transform.Rotate(new Vector3(-42, -121, -70));
                fruitBox.transform.localPosition = new Vector3(1.9F, -1.5F, 5.8F);
                fruited = true;
            }
        }
        checkLife();
    }

    void animate()
    {
        switch(counter)
        {
            case 0:
                if(transform.localScale.z > 0.95)
                {
                    transform.localScale += new Vector3(0, 0, -0.001F);
                }
                else
                {
                    counter++;
                }
                break;
            case 1:
                if (transform.localScale.z < 1)
                {
                    transform.localScale += new Vector3(0, 0, 0.001F);
                }
                else
                {
                    counter--;
                }
                break;
        }
        
    }
    void updateBar()
    {
        GameObject bar = transform.Find("PlantBar_Canvas").Find("bar_hp_background").Find("bar_hp").gameObject;
        GameObject barText = transform.Find("PlantBar_Canvas").Find("bar_hp_background").Find("bar_hp_text").gameObject;
        if (bar != null && barText != null)
        {
            TimeSpan timeDifference =   (Utilities.getCurrentTime() - bornTime);
            float minutesToDeath = plantInfo.LifeTimeMinutes - (float)timeDifference.TotalMinutes;

            bar.transform.localPosition = new Vector3(-(((plantInfo.LifeTimeMinutes - minutesToDeath) / plantInfo.LifeTimeMinutes) * bar.GetComponent<RectTransform>().rect.width), bar.transform.localPosition.y, bar.transform.localPosition.z);
            barText.GetComponent<Text>().text = Utilities.convertTimeToString(minutesToDeath);
            if(Mathf.Abs(bar.transform.localPosition.x) > 0.8 * bar.GetComponent<RectTransform>().rect.width)
            {
                bar.GetComponent<RawImage>().color = new Color32(255, 50, 50, 255);
            }
            else if (Mathf.Abs(bar.transform.localPosition.x) > 0.5 * bar.GetComponent<RectTransform>().rect.width)
            {
                bar.GetComponent<RawImage>().color = new Color32(240, 205, 46, 255);
            }
            else
            {
                bar.GetComponent<RawImage>().color = new Color32(85, 214, 112, 255);
            }

        }
        
    }

    public void checkLife()
    {
        timeDifference = (Utilities.getCurrentTime() - bornTime);
        minutesToDeath = plantInfo.LifeTimeMinutes - (float)timeDifference.TotalMinutes;
        if (minutesToDeath <= 0)
        {
            GameObject.Find("Plant_Manager").GetComponent<PlantManager>().createGarbage(new Vector2(transform.position.x, transform.position.z), true, false);
            GameObject.Find("Plant_Manager").GetComponent<PlantManager>().removePlant(gameObject);
            windowsManager.openInfoDeath((int)Options.Time);
        }
    }

    void changeColor()
    {
        Material[] oldMaterials = gameObject.GetComponent<Renderer>().materials;
        foreach (Material m in oldMaterials)
        {
            m.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 0.3F));
        }

        foreach(GameObject child in childsAndMaterials.Keys)
        {
            Material[] oldMaterialsChild = child.GetComponent<Renderer>().materials;
            foreach (Material m in oldMaterialsChild)
            {
                m.color = Color.Lerp(Color.yellow, Color.black, Mathf.PingPong(Time.time, 0.3F));
            }
        }        
    }

    public void stopChangingColor()
    {
        isCrossing = false;
        gameObject.GetComponent<Renderer>().materials = originalMaterialsOfPlant;
        foreach (GameObject child in childsAndMaterials.Keys)
        {
            child.GetComponent<Renderer>().materials = childsAndMaterials[child];
        }
    }

}
