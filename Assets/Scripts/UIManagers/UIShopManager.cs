using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour {

    public ManagersContainer managers;

    public Texture diamondIcon;

    public GameObject elementSchema;
    private List<GameObject> elementsOnList = new List<GameObject>();

    public RawImage[] tabs;
    public GameObject tabContainer;
    
    public GameObject content;
    
    private Color32 blackColor;
    private Color32 whiteColor;
    private int index;

    public Texture[] textures_shovel;
    public Texture[] textures_fertilizer;
    public Texture[] textures_diamonds;
    public Texture[] textures_conditioners;

    public List<Shovel> shovels = new List<Shovel>();
    public List<Fertilizer> fertilizers = new List<Fertilizer>();
    public List<Conditioner> conditioners = new List<Conditioner>();
    public List<Diamond> diamonds = new List<Diamond>();

    // Use this for initialization
    void Start () {
        setTabsPosition();
        addAllProducts();
        whiteColor = new Color32(255, 255, 255, 220);
        blackColor = new Color32(0, 0, 0, 220);
        index = 0;
    }
    private void setTabsPosition()
    {
        int countOfTabs = tabs.Length;
        float widthOfTabBar = tabContainer.GetComponent<RectTransform>().rect.width;
        float widthTab = widthOfTabBar / countOfTabs;
        float holeSize = widthTab * 0.05F;
        widthTab -= holeSize;
        holeSize = (holeSize * countOfTabs) / (countOfTabs + 1);
        foreach (RawImage t in tabs)
        {
            t.rectTransform.localScale = new Vector3(1, 1, 1);
            t.GetComponent<RectTransform>().sizeDelta = new Vector2(widthTab, t.GetComponent<RectTransform>().rect.height);
        }
        float pos = -(widthOfTabBar / 2) + holeSize + (tabs[0].rectTransform.rect.width / 2);
        foreach (RawImage t in tabs)
        {
            t.transform.localPosition = new Vector3(pos, t.transform.localPosition.y, t.transform.localPosition.z);
            pos += (widthTab + holeSize);
        }
    }
    private void addAllProducts()
    {
        ///// Shovels
        shovels.Add(new Shovel("Drewniany szpadel", 70, 50, 100, false, false, textures_shovel[0]));
        shovels.Add(new Shovel("Kamienny szpadel", 75, 45, 200, false, false, textures_shovel[1]));
        shovels.Add(new Shovel("Metalowy szpadel", 80, 40, 400, false, false, textures_shovel[2]));
        shovels.Add(new Shovel("Diamentowy szpadel", 100, 20, 5, false, true, textures_shovel[3]));

        ///// Fertilizers
        fertilizers.Add(new Fertilizer("Nawóz 1", 5, 200, false, false, textures_fertilizer[0]));
        fertilizers.Add(new Fertilizer("Nawóz 2", 8, 400, false, false, textures_fertilizer[1]));
        fertilizers.Add(new Fertilizer("Nawóz 3", 12, 800, false, false, textures_fertilizer[2]));
        fertilizers.Add(new Fertilizer("Nawóz premium", 20, 6, false, true, textures_fertilizer[3]));

        ///// Conditioners
        conditioners.Add(new Conditioner("Odżywka 1", 10, 150, false, false, textures_conditioners[0]));
        conditioners.Add(new Conditioner("Odżywka 2", 15, 300, false, false, textures_conditioners[1]));
        conditioners.Add(new Conditioner("Odżywka 3", 20, 600, false, false, textures_conditioners[2]));
        conditioners.Add(new Conditioner("Odżywka premium", 40, 4, false, true, textures_conditioners[3]));
        conditioners.Add(new Conditioner("Odżywka testowa", 80, 10, false, false, textures_conditioners[4]));

        ///// Diamonds
        diamonds.Add(new Diamond("Garść diamentów", 5, 2, textures_diamonds[0]));
        diamonds.Add(new Diamond("Worek diamentów", 20, 5, textures_diamonds[1]));
        diamonds.Add(new Diamond("Skrzynia diamentów", 100, 20, textures_diamonds[2]));
        diamonds.Add(new Diamond("Ciężarówka diamentów", 500, 80, textures_diamonds[3]));
    }

    private void  deselectAllTabs()
    {
        foreach(RawImage tab in tabs)
        {
            tab.color = blackColor;
            tab.transform.GetChild(0).GetComponent<Text>().color = whiteColor;
        }
    }
    public void setTab(int i)
    {
        deselectAllTabs();
        tabs[i].color = whiteColor;
        tabs[i].transform.GetChild(0).GetComponent<Text>().color = blackColor;
        fillItemList(i);
        index = i;
    }
    
    void fillItemList(int i)
    {
        clearList();
        switch (i)
        {
            case 0:
                foreach(Shovel shovel in shovels)
                {
                    addElementToList(shovel);
                }
                break;
            case 1:
                foreach(Fertilizer fertelizer in fertilizers)
                {
                    addElementToList(fertelizer);
                }
                break;
            case 2:
                foreach (Conditioner conditioner in conditioners)
                {
                    addElementToList(conditioner);
                }
                break;
            case 3:
                foreach (Diamond diamond in diamonds)
                {
                    addElementToList(diamond);
                }
                break;
        }
    }

    private void addElementToList(Item item)
    {
        GameObject el = Instantiate(elementSchema);
        el.transform.parent = content.transform;
        el.transform.localScale = new Vector3(1, 1, 1);
        el.transform.Find("txt_name").GetComponent<Text>().text = item.Name;
        el.transform.Find("txt_description").GetComponent<Text>().text = item.Description;
        if (item is Diamond) 
        {
            el.transform.Find("field_price").transform.Find("txt_price").GetComponent<Text>().text = (item.Price - 1).ToString() + ",99zł";
            el.transform.Find("btn_buy").GetComponent<Button>().interactable = true;
        }
        else if(item.Premium)
        {
            el.transform.Find("field_price").transform.Find("txt_price").GetComponent<Text>().text = item.Price.ToString();
            if(item.Price > managers.sceneManager.Player.getDiamonds())
            {
                el.transform.Find("btn_buy").GetComponent<Button>().interactable = false;
            }
            else
            {
                el.transform.Find("btn_buy").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            el.transform.Find("field_price").transform.Find("txt_price").GetComponent<Text>().text = item.Price.ToString();
            if ((item.Locked) || (item.Price > managers.sceneManager.Player.getGold()))
            {
                el.transform.Find("btn_buy").GetComponent<Button>().interactable = false;
            }
            else
            {
                el.transform.Find("btn_buy").GetComponent<Button>().interactable = true;
            }
        }
        el.transform.Find("btn_buy").GetComponent<Button>().onClick.AddListener(delegate { buyItem(item); });
        if (item.Icon != null)
        {
            el.transform.Find("background_item").transform.Find("icon_item").GetComponent<RawImage>().texture = item.Icon;
        }
        
        elementsOnList.Add(el);
        if (item.Premium)
        {
            el.transform.Find("field_price/img_gold").GetComponent<RawImage>().texture = diamondIcon;
        }
    }
    

    void clearList()
    {
        foreach(GameObject el in elementsOnList)
        {
            Destroy(el);
        }
        elementsOnList.Clear();
    }

    public void buyItem(Item item)
    {
        if (item is Diamond)
        {
            switch(((Diamond)item).Count) {
                case 5:
                    IAPManager.Instance.Buy5diamonds();
                    break;
                case 20:
                    IAPManager.Instance.Buy20diamonds();
                    break;
                case 100:
                    IAPManager.Instance.Buy100diamonds();
                    break;
                case 500:
                    IAPManager.Instance.Buy500diamonds();
                    break;
            }
        }
        else if(item.Premium)
        {
            managers.sceneManager.Player.addDiamonds(-item.Price);
            managers.databaseManager.addItem(item);
        }
        else
        {
            managers.sceneManager.Player.addGold(-item.Price);
            managers.databaseManager.addItem(item);
        }
        setTab(index);
    }

}
