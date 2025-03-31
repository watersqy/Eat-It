using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//����ж�����
public class Player : MonoBehaviour
{
    public GameObject food;
    public float R;//�뾶

    Rigidbody2D rb;
    GameObject bubble;

    Sprite oldSprite;
    GameObject otherCell;
    Vector2 aim;
    float isEating;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.localScale = new Vector2(R, R);//���ó�ʼ�ߴ�
        GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);//�����ɫ

        bubble = (Resources.Load("Material/bubble")) as GameObject;
        bubble.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        oldSprite = GetComponent<SpriteRenderer>().sprite;
        aim = new Vector2(0, 0);
        otherCell = null;
        isEating = 0;
    }


    void Update()
    {
        R = transform.localScale.x;//��������
        if(Input.GetMouseButtonDown(0))
            Move();//�ƶ�
        if (Input.GetMouseButtonDown(1))
            Throw();//�³�ʳ��
        Eat();
    }

    void Move()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//������λ��
        Vector2 dir = (mousePos - rb.position).normalized;//�õ���������
        rb.AddForce(dir * 2, ForceMode2D.Impulse);//ʩ����
        Track(-dir);
    }

    void Track(Vector3 dir)
    {
        int num = Random.Range(10, 15);
        float r = R / 50;
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(-30, 30);
            Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
            GameObject item = Instantiate(bubble);
            item.transform.position = gameObject.transform.position + R * Random.Range(0.2f, 1) * newDir;
            item.transform.localScale = new Vector2(r, r);

            item.GetComponent<Rigidbody2D>().AddForce(dir * Random.Range(0.5f, 2), ForceMode2D.Impulse);
        }
    }

    void Throw()
    {
        float r = 1;
        if (R > 1)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = (mousePos - rb.position).normalized;
            
            //����������
            GameObject item = Instantiate(food);
            item.transform.position = gameObject.transform.position + (R + r) * dir;
            item.GetComponent<SpriteRenderer>().color = gameObject.GetComponent<SpriteRenderer>().color;
            item.transform.localScale = new Vector2(r, r);

            item.GetComponent<Rigidbody2D>().AddForce(dir * 5, ForceMode2D.Impulse);

            //��������
            R = R - r;
            transform.localScale = new Vector2(R, R);
        }
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

            Debug.Log("pEat");
            SpriteRenderer sr = otherCell.GetComponent<SpriteRenderer>();
            GetAimPoint(sr.sprite.vertices);
            Sprite newSprite = Instantiate(sr.sprite);
            Vector2[] points = new Vector2[sr.sprite.vertices.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 pos = otherCell.transform.TransformPoint(sr.sprite.vertices[i]); // ת������������
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
        else if (isEating >= 3)
        {
            R = R + otherCell.transform.localScale.x / 5;
            transform.localScale = new Vector2(R, R);
            isEating = 0;
            GameManagement.instance.CellEat(otherCell);
            otherCell = null;
        }
    }

    void GetAimPoint(Vector2[] vertices)
    {
        //�ҵ��Ӵ���
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
        if (rb == null)
            return;

        GameObject cell = collision.gameObject;
        if (cell.tag == "wall" || otherCell == cell)
            return;

        R = transform.localScale.x;
        float r = cell.transform.localScale.x;
        if(cell.tag=="food")
        {
            Debug.Log("p-food");

            Vector2 dir = transform.position - cell.transform.position;
            cell.GetComponent<Rigidbody2D>().AddForce(dir * 2, ForceMode2D.Impulse);

            R = R + r;
            transform.localScale = new Vector2(R, R);
            Destroy(cell.gameObject);
        }
        else if (R > r)//�ж��Ƿ�ȶԷ���
        {
            Debug.Log("p-eat");

            Vector2 dir = transform.position - cell.transform.position;
            cell.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
            otherCell = cell;
            isEating = 1;
        }
        else if (GameManagement.instance.mode == Mode.ColorWar
            && GameManagement.instance.level != Level.Hard
            && cell.tag == "friend")
        {
            Debug.Log("p-f");

            Vector2 dir = transform.position - cell.transform.position;
            cell.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
            otherCell = cell;
            isEating = 1;
        }
    }
}
