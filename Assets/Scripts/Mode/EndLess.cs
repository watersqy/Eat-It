using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLess : MonoBehaviour
{
    public Mode mode;
    public Level level;
    
    GameObject example;
    int minNum;
    float range;
    int score;
    float record;

    public void Init()
    {
        example = Resources.Load<GameObject>("Material/enemy");
        range = GameManagement.instance.range;
        switch (level)
        {
            case Level.Easy:
                minNum = 50;
                break;
            case Level.Normal:
                minNum = 60;
                break;
            case Level.Hard:
                minNum = 70;
                break;
            default:
                break;
        }
        score = 0;
        record = gameObject.transform.localScale.x;
        Creat(minNum);
        InvokeRepeating("CalSocre", 60, 60);
    }

    void FixedUpdate()
    {
        Check();
    }

    void Creat(int n)
    {
        Vector3 pos;
        float r, x, y;
        float R = gameObject.transform.localScale.x;
        range = GameManagement.instance.range;
        while (n > 0)
        {
            r = Random.Range(R / 2, ((int)level + 3) * R);
            x = Random.Range(-range, range);
            y = Random.Range(-range, range);
            pos = new Vector2(x, y);
            if (isExist(r, pos))
                continue;

            GameObject item = Instantiate(example);
            item.transform.localScale = new Vector2(r, r);
            item.transform.position = pos;
            item.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);

            n--;
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

    void Check()
    {
        Collider2D[] colliders = Physics2D.OverlapAreaAll(new Vector2(-range,-range), new Vector2(range, range));
        int len = colliders.Length;
        
        for(int i = 0; i < len; i++)
        {
            if (colliders[i].gameObject.tag == "food"
                || colliders[i].gameObject.tag == "Player")
                len--;
        }
        if (len < minNum)
            Creat(minNum - len);
    }

    void CalSocre()
    {
        if (level == Level.Hard && record >= transform.localScale.x)
        {
            GameManagement.instance.CellEat(gameObject);
        }

        score += Mathf.FloorToInt(transform.localScale.x);
        GameManagement.instance.Display(score);
    }
}
