using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    Vector2 vel;
    bool is_touching_ground = false;

    public float movement_speed;
    public GameObject sprite_top;
    public GameObject sprite_bot;

    void Start()
    {}

    void Update()
    {
        // horizontal movement
        if (Input.GetKeyDown(KeyCode.A))
        {
            vel.x = -movement_speed;
            flipSprites(vel);
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            vel.x = movement_speed;
            flipSprites(vel);
        }

        if ((Input.GetKeyUp(KeyCode.A) && vel.x < 0) || (Input.GetKeyUp(KeyCode.D) && vel.x > 0))
        {
            vel.x = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!is_touching_ground)
        {
            vel.y += Physics2D.gravity.y * Time.deltaTime;
        }
        
        GetComponent<Rigidbody2D>().position += vel * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            is_touching_ground = true;
            vel.y = 0;
        };
    }

    private void flipSprites(Vector2 vel)
    {
        bool flipValue = vel.x < 0;
        sprite_top.GetComponent<SpriteRenderer>().flipX = flipValue;
        sprite_bot.GetComponent<SpriteRenderer>().flipX = flipValue;
    }
}
