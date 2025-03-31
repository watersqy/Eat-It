using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorWar : MonoBehaviour
{
    
    public Mode mode;
    public Level level;
    
    float range;
    
    GameObject fCell;
    List<GameObject> friends;
    Color cf;
    int friendNum;

    GameObject eCell;
    List<GameObject> enemy;
    Color ce;
    int enemyNum;

    public void Init()
    {
        switch (level)
        {
            case Level.Easy:
                friendNum = enemyNum = 30;
                break;
            case Level.Normal:
                friendNum = enemyNum = 40;
                break;
            case Level.Hard:
                friendNum = 20;
                enemyNum = 50;
                break;
            default:
                break;
        }
        range = GameManagement.instance.range;
        cf = GetComponent<SpriteRenderer>().color;
        ce = new Color(1.0f - cf.r, 1.0f - cf.g, 1.0f - cf.b);

        eCell = (Resources.Load("Material/enemy")) as GameObject;
        enemy = new List<GameObject>();
        Creat(enemyNum, ce, enemy, eCell);
        
        fCell = (Resources.Load("Material/friend")) as GameObject;
        friends = new List<GameObject>(); 
        Creat(friendNum, cf, friends, fCell);
    }

    void Update()
    {
        calScore();
    }

    void Creat(int n, Color c, List<GameObject> L, GameObject cell)
    {
        Vector3 pos;
        float r, x, y;
        float R = gameObject.transform.localScale.x;
        range = GameManagement.instance.range;
        for (int i = 0; i < n; i++)
        {
            r = Random.Range(R / 2, ((int)level + 3) * R);
            x = Random.Range(-range, range);
            y = Random.Range(-range, range);
            pos = new Vector2(x, y);
            if (isExist(r, pos))
            {
                i--;
                continue;
            }

            GameObject item = Instantiate(cell);
            item.transform.localScale = new Vector2(r, r);
            item.transform.position = pos;
            item.GetComponent<SpriteRenderer>().color = c;
            L.Add(item);
        }
    }

    bool isExist(float r, Vector2 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, r);
        if (colliders.Length > 0)
            return true;
        else
            return false;
    }

    void calScore()
    {
        float f = transform.localScale.x, e = 0;
        foreach (GameObject g in friends)
        {
            f += g.transform.localScale.x;
        }
        foreach (GameObject g in enemy)
        {
            e += g.transform.localScale.x;
        }
        float all = f + e;
        f = Mathf.Round(f / all * 10000) / 100f;
        e = 100f - f;
        
        GameManagement.instance.Display(f, e);
    }
}
