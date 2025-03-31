using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int lifeTime;
    float delta;
    Color c;

    void Start()
    {
        delta = 1.0f / lifeTime;
        c = GetComponent<SpriteRenderer>().color;
        InvokeRepeating("Change", 0.5f, 0.5f);
    }

    void Change()
    {
        if (lifeTime == 0)
            Destroy(gameObject);
        c.a -= delta;
        GetComponent<SpriteRenderer>().color = c;
        transform.localScale -= transform.localScale * delta;
        lifeTime--;
    }
}
