using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Back : MonoBehaviour
{
    public GameObject over;
    public GameObject win;
    public GameObject loss;
    public Button back;

    void Start()
    {
        int result = PlayerPrefs.GetInt("result"); Debug.Log(result);
        GameObject g = new GameObject();
        string histroy = "";
        switch (result)
        {
            case 1:
                histroy = "\nHistor: " + PlayerPrefs.GetInt("history");
                g = over;
                break;
            case 2:
                g = win;
                break;
            case 3:
                g = loss;
                break;
            default:
                break;
        }
        g.SetActive(true);
        g.transform.Find("bg/score").GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("score") + histroy;
        back.onClick.AddListener(BackHome);
    }

    void BackHome()
    {
        SceneManager.LoadScene(0);
    }
}
