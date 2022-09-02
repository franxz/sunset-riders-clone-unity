using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    Vector2 vel;
    bool is_touching_ground = false;

    Dictionary<string, int> ANIM_STATES = new Dictionary<string, int>
    {
        { "idle", 0 },
        { "walk", 1 }
    };

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
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["walk"]);
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            vel.x = movement_speed;
            flipSprites(vel);
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["walk"]);
        }

        if ((Input.GetKeyUp(KeyCode.A) && vel.x < 0) || (Input.GetKeyUp(KeyCode.D) && vel.x > 0))
        {
            vel.x = 0;
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["idle"]);
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
        gameObject.transform.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
    }
}
