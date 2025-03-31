using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class getTest : MonoBehaviour
{
    Sprite oldSprite;
    GameObject otherCell;
    Vector2 aim;
    float isEat;

    void Start()
    {
        oldSprite = GetComponent<SpriteRenderer>().sprite;
        aim = new Vector2(0, 0);
        isEat = 0;
        
    }

    void Update()
    {
        if (isEat > 0 && isEat < 3 && otherCell != null)
        {
            Debug.Log(isEat);
            SpriteRenderer sr = otherCell.GetComponent<SpriteRenderer>();
            Sprite newSprite = Instantiate(sr.sprite);
            Vector2[] points = new Vector2[sr.sprite.vertices.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 pos = otherCell.transform.TransformPoint(sr.sprite.vertices[i]); // 转换到世界坐标
                Vector2 dir = pos + (aim - pos).normalized * Vector2.Distance(aim, pos) * isEat / 1000;
                Debug.DrawLine(dir, aim, Color.blue);
                pos = otherCell.transform.InverseTransformPoint(dir);
                points[i] = (pos * oldSprite.pixelsPerUnit) + oldSprite.pivot;
                points[i].x = Mathf.Clamp(points[i].x, 0.0f, newSprite.rect.width);
                points[i].y = Mathf.Clamp(points[i].y, 0.0f, newSprite.rect.height);
            }
            newSprite.OverrideGeometry(points, sr.sprite.triangles);
            sr.sprite = newSprite;
            isEat += Time.deltaTime;
        }
        else if (isEat >= 3)
        {

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("in");
        if(collision.transform.localScale.x<transform.localScale.x)
        {
            //找到接触点
            aim = collision.transform.position;
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            Vector2 point = transform.position;
            foreach (ContactPoint2D p in contacts)
            {
                if (Vector2.Distance(aim, p.point) < Vector2.Distance(aim, point))
                    point = p.point;
            }
            aim = point;
            otherCell = collision.gameObject;
            isEat = 1;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("out");
        isEat = 0;
        GetComponent<SpriteRenderer>().sprite = oldSprite;
    }
}
