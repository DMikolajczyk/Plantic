using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private AsyncOperation asyncLoad = null;
    public Image bar;
    public float endWidth;
    public Text percentText;

    void Start()
    {
        endWidth = bar.rectTransform.rect.width;
        bar.rectTransform.sizeDelta = new Vector2(0, bar.rectTransform.rect.height);
        StartCoroutine(LoadYourAsyncScene());
    }

    // Updates once per frame
    void Update()
    {

    }

    IEnumerator LoadYourAsyncScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(1);
        while (!asyncLoad.isDone)
        {
            Debug.Log("Progress: " + asyncLoad.progress);
            percentText.text = (asyncLoad.progress * 100).ToString() + "%";
            bar.rectTransform.sizeDelta = new Vector2((asyncLoad.progress * endWidth), bar.rectTransform.rect.height);
            yield return null;
        }
    }

}