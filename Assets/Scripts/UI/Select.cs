using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour
{
    public Image bg;

    public Button M1;
    public Button M2;

    public Button L1;
    public Button L2;
    public Button L3;

    int mode;
    int level;

    public GameObject start;
    Animator click;
    Button startBtn;

    void Start()
    {
        click = start.GetComponent<Animator>();
        startBtn = start.GetComponent<Button>();
        startBtn.onClick.AddListener(Clicked);
        M1.onClick.AddListener(Mode_1);
        M2.onClick.AddListener(Mode_2);
        L1.onClick.AddListener(Level_1);
        L2.onClick.AddListener(Level_2);
        L3 .onClick.AddListener(Level_3);
    }

    public void Clicked()
    {
        PlayerPrefs.SetInt("Mode", mode);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.Save();
        click.SetBool("clicked", true);
        AnimatorStateInfo info = click.GetCurrentAnimatorStateInfo(0);
        Invoke("Next", info.length);
    }

    void Next()
    {
        SceneManager.LoadScene(1);
    }

    void Mode_1()
    {
        mode = 0;
        level = 0;
        bg.sprite = Resources.Load<Sprite>("UI/picture/1-1");
    }

    void Mode_2()
    {
        mode = 1;
        level = 0;
        bg.sprite = Resources.Load<Sprite>("UI/picture/2-1");
    }

    void Level_1()
    {
        level = 0;
        string path = mode == 0 ? "UI/picture/1-1" : "UI/picture/2-1";
        bg.sprite = Resources.Load<Sprite>(path);
    }

    void Level_2()
    {
        level = 1;
        string path = mode == 0 ? "UI/picture/1-2" : "UI/picture/2-2";
        bg.sprite = Resources.Load<Sprite>(path);
    }

    void Level_3()
    {
        level = 2;
        string path = mode == 0 ? "UI/picture/1-3" : "UI/picture/2-3";
        bg.sprite = Resources.Load<Sprite>(path);
    }
}
