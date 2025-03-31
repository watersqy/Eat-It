using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Friend : MonoBehaviour
{
    public float R;//半径

    Rigidbody2D rb;

    Sprite oldSprite;
    GameObject otherCell;
    Vector2 aim;
    float isEating;

    void Start()
    {
        R = transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        oldSprite = GetComponent<SpriteRenderer>().sprite;
        otherCell = null;
        aim = new Vector2(0, 0);
        isEating = 0;
        InvokeRepeating("Move", 3, 3);
    }

    void Update()
    {
        R = transform.localScale.x;
        Eat();
    }

    void Move()
    {
        R = transform.localScale.x;
        Level L = GameManagement.instance.level;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, R * 2);
        float catchLen = float.MaxValue, runLen = float.MaxValue;
        Vector2 catchDir = Random.insideUnitCircle;
        Vector2 runDir = Random.insideUnitCircle;
        switch (L)
        {
            case Level.Easy:
                break;
            case Level.Normal:
                foreach (Collider2D c in colliders)
                {
                    if (c.transform.localScale.x < R && c.tag == "enemy")
                    {
                        catchDir = c.transform.position - transform.position;
                        catchLen = Mathf.Min(catchLen, catchDir.magnitude);
                    }
                }
                break;
            case Level.Hard:
                foreach (Collider2D c in colliders)
                {
                    if (c.transform.localScale.x < R && c.tag == "enemy")
                    {
                        catchDir = c.transform.position - transform.position;
                        catchLen = Mathf.Min(catchLen, catchDir.magnitude);
                    }
                    if (c.transform.localScale.x > R && c.tag == "enemy")//逃跑
                    {
                        runDir = transform.position - c.transform.position;
                        runLen = Mathf.Min(catchLen, catchDir.magnitude);
                    }
                }
                catchDir = (catchLen < runLen) ? catchDir : runDir;
                break;
            default:
                break;
        }
        rb.AddForce(catchDir / 2, ForceMode2D.Impulse);
    }

    void Eat()
    {
        if (isEating > 0 && isEating < 3 && otherCell != null)
        {
            if (Vector2.Distance(transform.position, otherCell.transform.position) > (R + otherCell.transform.localScale.x) / 2 + 0.5)
            {
                isEating = 0;
                otherCell.GetComponent<SpriteRenderer>().sprite = oldSprite;
                otherCell = null;
                return;
            }

            Debug.Log("fEat");
            SpriteRenderer sr = otherCell.GetComponent<SpriteRenderer>();
            GetAimPoint(sr.sprite.vertices);
            Sprite newSprite = Instantiate(sr.sprite);
            Vector2[] points = new Vector2[sr.sprite.vertices.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 pos = otherCell.transform.TransformPoint(sr.sprite.vertices[i]); // 转换到世界坐标
                Vector2 dir = pos + (aim - pos).normalized * Vector2.Distance(aim, pos) * isEating / 3000;
                Debug.DrawLine(dir, aim, Color.blue);
                pos = otherCell.transform.InverseTransformPoint(dir);
                points[i] = (pos * oldSprite.pixelsPerUnit) + oldSprite.pivot;
                points[i].x = Mathf.Clamp(points[i].x, 0.0f, newSprite.rect.width);
                points[i].y = Mathf.Clamp(points[i].y, 0.0f, newSprite.rect.height);
            }
            newSprite.OverrideGeometry(points, sr.sprite.triangles);
            sr.sprite = newSprite;
            isEating += Time.deltaTime;
        }
        else if (isEating >= 2.5)
        {
            R = R + otherCell.transform.localScale.x / 5;
            transform.localScale = new Vector2(R, R);
            isEating = 0;
            GameManagement.instance.CellEat(otherCell);
        }
    }

    void GetAimPoint(Vector2[] vertices)
    {
        //找到接触点
        aim = vertices[0];
        Vector2 point = transform.position;
        foreach (Vector2 v in vertices)
        {
            Vector3 p = otherCell.transform.TransformPoint(v);
            if (Vector2.Distance(aim, point) > Vector2.Distance(p, point))
                aim = p;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb == null || collision.gameObject.tag == "wall")
            return;

        GameObject cell = collision.gameObject;
        R = transform.localScale.x;
        float r = cell.transform.localScale.x;
        if (cell.tag == "food")
        {
            Debug.Log("p-food");

            Vector2 dir = transform.position - cell.transform.position;
            cell.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);

            R = R + r;
            transform.localScale = new Vector2(R, R);
            Destroy(cell.gameObject);
        }
        else if (R > r && cell.tag != "Player")//判断是否比对方大
        {
            Debug.Log("f-eat");

            Vector2 dir = transform.position - cell.transform.position;
            cell.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
            rb.AddForce(-dir, ForceMode2D.Impulse);
            otherCell = cell;
            isEating = 1;
        }
    }
}
