using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float movement_speed;

    Vector2 vel;
    bool isBoom = false;

    void Start()
    {
        vel = transform.rotation * new Vector2(movement_speed, 0);
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
        if (isBoom)
        {
            return;
        }

        if (col.gameObject.tag == "Enemy")
        {
            isBoom = true;
            col.gameObject.GetComponent<reaper>().getHit();
            Destroy(gameObject);
        }
    }
}