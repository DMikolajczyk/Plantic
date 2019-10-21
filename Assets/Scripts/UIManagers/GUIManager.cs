using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class GUIManager : MonoBehaviour {
    
    public GameObject queue_background;
    public GameObject queue_tabObject;
    public GameObject queue_hideButton;
    public GameObject queue_holder;
    public Text queue_counter;
    public Text textGold;
    public Text textDiamonds;
    public bool isOpen;

    public ManagersContainer managers;
    public UICrossingManager uiCrossingManager;

    public Dictionary<GameObject, Action> tabsAndActions = new Dictionary<GameObject, Action>();

    private Vector3 queue_startPosition;
    private RectTransform rectTransform_queueBackground;
    private RectTransform rectTransform_tabObject;

    // Use this for initialization
    void Start () {
        isOpen = true;
        rectTransform_queueBackground = queue_background.GetComponent<RectTransform>();
        rectTransform_tabObject = queue_tabObject.GetComponent<RectTransform>();

        queue_startPosition = new Vector3(  queue_holder.transform.localPosition.x, 
                                            queue_holder.transform.localPosition.y, 
                                            queue_hideButton.transform.localPosition.z
                                            );
        refreshActions();
    }
	
	// Update is called once per frame
	void Update () {
        updateButtons();
	}

    public void updateButtons()
    {
        {
            int i = 0;
            GameObject[] tabsTmp = new GameObject[tabsAndActions.Count];
            tabsAndActions.Keys.CopyTo(tabsTmp, 0);
            foreach (GameObject tab in tabsTmp)
            {
                TimeSpan timeDifference = Utilities.getCurrentTime() - tabsAndActions[tab].StartTime;
                float minutesToEnd = tabsAndActions[tab].LifeTimeInMinutes - (float)timeDifference.TotalMinutes;
                
                if (tabsAndActions[tab] is ActionPlant)
                {
                    minutesToEnd -= (((float)((ActionPlant)tabsAndActions[tab]).PlantIncreaseBonus)/100) * tabsAndActions[tab].LifeTimeInMinutes;
                }
                tab.transform.Find("queueTab_time").GetComponent<Text>().text = Utilities.convertTimeToString(minutesToEnd);
                if(minutesToEnd < 0)
                {
                    if (tabsAndActions[tab] is ActionPlant) {
                        ActionPlant actionPlant = (ActionPlant)tabsAndActions[tab];
                        GameObject pb = null;
                        foreach(GameObject plantBuilder in managers.plantManager.plantBuilders)
                        {
                            if(plantBuilder.transform.position == actionPlant.Position)
                            {
                                pb = plantBuilder;
                                
                            }
                        }
                        if (pb != null)
                        {
                            managers.plantManager.removePlantBuilder(pb, actionPlant);
                        }
                    }
                    else if (tabsAndActions[tab] is ActionCross)
                    {
                        ActionCross actionCross = (ActionCross)tabsAndActions[tab];
                        managers.databaseManager.removeActionCross(actionCross);
                        uiCrossingManager.crossPlants(actionCross.DnaOne, actionCross.DnaTwo, actionCross.CrossChance);
                    }

                    tabsAndActions.Remove(tab);
                    Destroy(tab);
                    refreshActions();
                }
                i++;
            }
            queue_counter.text = i.ToString() + " / " + managers.sceneManager.Player.MaxInQueue;
        }
    }

    public void tabMoreInfo()
    {
        IAPManager.Instance.Buy5diamonds();
        /*tabsAndActions.Remove(EventSystem.current.currentSelectedGameObject);
        Destroy(EventSystem.current.currentSelectedGameObject);
        refreshActions();*/
    }

    public void onClickUnhider()
    {
        isOpen = !isOpen;
        refreshActions();
        Utilities.isAfterButtonClick = true;
    }

    public void createTab(Action action)
    {
        GameObject tabObject = Instantiate(queue_tabObject);
        tabObject.GetComponent<Button>().onClick.AddListener(tabMoreInfo);
        tabsAndActions.Add(tabObject, action);
        string nameTab = "";
        if (action is ActionPlant)
        {
            nameTab = "sadzenie";
        }
        else if (action is ActionCross)
        {
            nameTab = "krzyzowanie";
        }
        tabObject.transform.Find("queueTab_type").GetComponent<Text>().text = nameTab;
        refreshActions();
    }

    public void refreshActions()
    {
        queue_holder.transform.localPosition = queue_startPosition;
        if (isOpen)
        {
            float offset = 0;
            float posVerticalOfTab = -rectTransform_queueBackground.rect.height / 2 + rectTransform_tabObject.rect.height / 2;
            foreach (GameObject tabbb in tabsAndActions.Keys)
            {
                tabbb.transform.SetParent(queue_background.transform, false);
                tabbb.transform.localPosition = new Vector3(0, posVerticalOfTab, 0);
                posVerticalOfTab += rectTransform_tabObject.rect.height * 0.92F;
                offset += rectTransform_tabObject.rect.height * 0.92F * queue_hideButton.transform.localScale.y;
            }
            if (tabsAndActions.Count > 0)
            {
                queue_holder.transform.localPosition = new Vector3(queue_holder.transform.localPosition.x, queue_holder.transform.localPosition.y - offset - 7, queue_holder.transform.localPosition.z);
            }
        }

    }
    
}
