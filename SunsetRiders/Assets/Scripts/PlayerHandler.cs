using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    Vector2 vel;
    bool is_touching_ground = false;
    Vector3 stageMax;
    Vector3 stageMin;
    Dictionary<string, int> ANIM_STATES = new Dictionary<string, int>
    {
        { "idle", 0 },
        { "walk", 1 },
        { "jump", 2 }
    };

    public float movement_speed;
    public float jump_speed;
    public GameObject sprite_top;
    public GameObject sprite_bot;

    void Start()
    {
        StageHandler stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageHandler>();
        stageMax = stage.max.transform.position;
        stageMin = stage.min.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            move_horizontally(-movement_speed);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            move_horizontally(movement_speed);
        }

        if ((Input.GetKeyUp(KeyCode.A) && vel.x < 0) || (Input.GetKeyUp(KeyCode.D) && vel.x > 0))
        {
            vel.x = 0;
            if (is_touching_ground)
            {
                GetComponent<Animator>().SetInteger("state", ANIM_STATES["idle"]);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }
    }

    private void FixedUpdate()
    {
        if (!is_touching_ground)
        {
            vel.y += Physics2D.gravity.y * Time.deltaTime;
        }

        GetComponent<Rigidbody2D>().position += vel * Time.deltaTime;
        checkLimits();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            land();
        };
    }

    private void move_horizontally(float velX)
    {
        vel.x = velX;
        flipSprites(vel);
        if (is_touching_ground)
        {
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["walk"]);
        }
    }

    private void jump()
    {
        if (is_touching_ground)
        {
            vel.y += jump_speed;
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["jump"]);
            is_touching_ground = false;
        }
    }

    private void land()
    {
        vel.y = 0;
        if (vel.x != 0)
        {
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["walk"]);
        }
        else
        {
            GetComponent<Animator>().SetInteger("state", ANIM_STATES["idle"]);
        }
        is_touching_ground = true;
    }

    private void flipSprites(Vector2 vel)
    {
        gameObject.transform.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
    }

    private void checkLimits()
    {
        if (GetComponent<Rigidbody2D>().position.x > stageMax.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(stageMax.x, GetComponent<Rigidbody2D>().position.y);
        }
        else if (GetComponent<Rigidbody2D>().position.x < stageMin.x)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(stageMin.x, GetComponent<Rigidbody2D>().position.y);
        }
    }
}
