using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reaper_bullet : MonoBehaviour
{
    public float movement_speed;

    Vector2 vel;
    Vector2 dir;

    void Start()
    {
        vel = dir * movement_speed;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().position += vel * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }

    public void setDirection(Vector2 direction)
    {
        dir = direction;
    }
}