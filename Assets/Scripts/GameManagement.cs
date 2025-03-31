using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Mode
{
    ColorWar,
    Endless
}

public enum Level
{
    Easy,
    Normal,
    Hard
}

public class GameManagement : MonoBehaviour
{
    static public GameManagement instance;

    public Camera myCamera;
    public Mode mode;
    public Level level;
    public float range;
    public GameObject wall;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI timeTxt;

    List<GameObject> W;
    int second;
    float nextTime;
    int score;

    void Awake()
    {
        instance = this;
    }

    public void GameStart(int m, int l)
    {
        level = (Level)l;
        mode = (Mode)m;
        range = 100;
        score = 0;

        switch (mode)
        {
            case Mode.ColorWar:
                gameObject.AddComponent<ColorWar>();
                GetComponent<ColorWar>().Init();
                second = 300;
                break;
            case Mode.Endless:
                gameObject.AddComponent<EndLess>();
                GetComponent<EndLess>().Init();
                scoreTxt.text = "Score: " + 0;
                second = 60;
                break;
            default:
                break;
        }
        
        for (int i = 0; i < 4; i++)
        {
            W.Add(Instantiate(wall));
        }
        SetArea();
    }

    void Update()
    {
        myCamera.transform.position = new Vector3(0, 0, -30) + gameObject.transform.position;
        myCamera.orthographicSize = 10 + transform.localScale.x;
        range = Mathf.Max(transform.localScale.x * 10, range);
        SetArea();
        TimeCount();
    }


    void Start()
    {
        W = new List<GameObject>();
        GameStart(PlayerPrefs.GetInt("Mode"), PlayerPrefs.GetInt("Level"));
        //GameStart(0, 0);
    }

    void SetArea()
    {
        Vector2[] pos = { Vector2.up, Vector2.left ,Vector2.down, Vector2.right };
        int angle = 0;
        for (int i = 0; i < W.Count; i++)
        {
            W[i].transform.position = pos[i] * range;
            W[i].transform.localScale = new Vector2(2 * range, 1);
            W[i].transform.rotation=Quaternion.Euler(0, 0, angle);
            angle += 90;
        }
    }

    public void CellEat(GameObject cell)
    {
        if (mode == Mode.Endless)
        {
            if (cell.tag == "Player")
                GameOver();
            else
                Destroy(cell);
        }
        else
            ReLive(cell);
    }

    void TimeCount()
    {
        if (second > 0)//second < 0 停止倒计时
        {
            if (second <= 10)//小于10秒则倒计时字体变红
            {
                timeTxt.color = Color.red;
            }
            if (Time.time >= nextTime)
            {
                second--;//减一秒钟
                timeTxt.text = "Time: " + string.Format("{0:d2}:{1:d2}", second / 60, second % 60);
                nextTime = Time.time + 1;//变成当前时间的后一秒
            }
            return;
        }
        switch (mode)
        {
            case Mode.ColorWar:
                GameOver();
                break;
            case Mode.Endless:
                second = 60;
                break;
            default:
                break;
        }
    }

    void ReLive(GameObject cell)
    {
        float r, x, y;
        x = Random.Range(-range, range);
        y = Random.Range(-range, range);
        r = cell.tag == "Player" ? 4 : 2;
        Vector3 pos = new Vector2(x, y);
        while (isExist(r, pos))
        {
            x = Random.Range(-range, range);
            y = Random.Range(-range, range);
            pos = new Vector2(x, y);
        }
        cell.transform.localScale = new Vector2(r, r);
        cell.transform.position = pos;
    }

    bool isExist(float r, Vector2 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, r);
        if (colliders.Length > 0)
            return true;
        else
            return false;
    }

    public void Display(int s)
    {
        Debug.Log(s);
        score = s;
        scoreTxt.text = "Score: "+ s.ToString();
    }

    public void Display(float f, float e)
    {
        //Debug.Log(f + ":" + e);
        scoreTxt.text = "You: " + f.ToString() + "\nEnemy: " + e.ToString();
        PlayerPrefs.SetInt("result", (f > e) ? 2 : 3);
        PlayerPrefs.Save();
    }

    void GameOver()
    {
        PlayerPrefs.SetString("score", scoreTxt.text);
        if (mode == Mode.Endless)
        {
            PlayerPrefs.SetInt("history", Mathf.Max(score, PlayerPrefs.GetInt("history")));
            PlayerPrefs.SetInt("result", 1);
        }
        PlayerPrefs.Save();
        SceneManager.LoadScene(2);
    }
}
