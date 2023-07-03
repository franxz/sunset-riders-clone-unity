using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterHandler : MonoBehaviour
{
    static Dictionary<string, int> BOT_ANIM_STATES = new Dictionary<string, int>
    {
        { "idle", 0 },
        { "walk", 1 },
        { "jump", 2 },
    };

    static Dictionary<string, int> TOP_ANIM_STATES = new Dictionary<string, int>
    {
        { "idle", 0 },
        { "fire", 3 },
    };

    static Dictionary<string, int> TOP_ANIM_DIRECTIONS = new Dictionary<string, int>
    {
        { "S", 0 },
        { "SE", 1 },
        { "E", 2 },
        { "NE", 3 },
        { "N", 4 },
    };

    static Dictionary<int, int> FIRE_ROTATION_DEGREES = new Dictionary<int, int>
    {
        { TOP_ANIM_DIRECTIONS["S"], -90 },
        { TOP_ANIM_DIRECTIONS["SE"], -45 },
        { TOP_ANIM_DIRECTIONS["E"], 0 },
        { TOP_ANIM_DIRECTIONS["NE"], 45 },
        { TOP_ANIM_DIRECTIONS["N"], 90 },
    };

    static Dictionary<string, KeyCode> KEYS = new Dictionary<string, KeyCode>
    {
        { "UP", KeyCode.W },
        { "RIGHT", KeyCode.D },
        { "DOWN", KeyCode.S },
        { "LEFT", KeyCode.A },
        { "JUMP", KeyCode.Space },
        { "FIRE", KeyCode.M },
    };
    // TODO: make constant??? ^^^

    Vector2 vel;
    Dictionary<string, bool> myInput = new Dictionary<string, bool>
    {
        { "UP", false },
        { "RIGHT", false },
        { "DOWN", false },
        { "LEFT", false },
        { "JUMP", false},
        { "FIRE", false },
    };
    Dictionary<string, bool> keyboardInput = new Dictionary<string, bool>
    {
        { "UP", false },
        { "RIGHT", false },
        { "DOWN", false },
        { "LEFT", false },
        { "JUMP", false},
        { "FIRE", false },
    };
    Dictionary<string, bool> mobileInput = new Dictionary<string, bool>
    {
        { "UP", false },
        { "RIGHT", false },
        { "DOWN", false },
        { "LEFT", false },
        { "JUMP", false},
        { "FIRE", false },
    };
    Dictionary<int, GameObject> bulletSpawns;

    Vector3 stageMax;
    Vector3 stageMin;

    public float movement_speed;
    public float jump_speed;
    public GameObject bullet;

    bool is_cooldown = false;
    bool isFacingRight = true;
    bool is_touching_ground = false;

    int direction = TOP_ANIM_DIRECTIONS["E"];

    float cooldown = 0.11f;

    GameObject spritesContainer;
    GameObject sprite_top;
    GameObject sprite_bot;

    void Start()
    {
        StageHandler stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageHandler>();
        stageMax = stage.max.transform.position;
        stageMin = stage.min.transform.position;

        spritesContainer = GameObject.Find("gunner/sprites");
        sprite_top = GameObject.Find("gunner/sprites/spr_top");
        sprite_bot = GameObject.Find("gunner/sprites/spr_bot");

        bulletSpawns = new Dictionary<int, GameObject>
        {
            //{ TOP_ANIM_DIRECTIONS["S"], GameObject.Find("gunner/sprites/bullet_s") },
            //{ TOP_ANIM_DIRECTIONS["SE"], GameObject.Find("gunner/sprites/bullet_se") },
            { TOP_ANIM_DIRECTIONS["E"], GameObject.Find("gunner/sprites/bullet_spawn") },
            { TOP_ANIM_DIRECTIONS["NE"], GameObject.Find("gunner/sprites/bullet_spawn_ne") },
            { TOP_ANIM_DIRECTIONS["N"], GameObject.Find("gunner/sprites/bullet_spawn_n") },
        };
    }

    void checkInput()
    {
        // TODO: improve this!
        foreach (KeyValuePair<string, KeyCode> entry in KEYS)
        {
            if (Input.GetKeyDown(entry.Value))
            {
                keyboardInput[entry.Key] = true;
            }
            else if (Input.GetKeyUp(entry.Value))
            {
                keyboardInput[entry.Key] = false;
            }
        }

#if UNITY_ANDROID
        // TODO: improve this!
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0.3)
            mobileInput["RIGHT"] = true;
        else
            mobileInput["RIGHT"] = false;
        if (CrossPlatformInputManager.GetAxis("Horizontal") < -0.3)
            mobileInput["LEFT"] = true;
        else
            mobileInput["LEFT"] = false;

        if (CrossPlatformInputManager.GetAxis("Vertical") > 0.3)
            mobileInput["UP"] = true;
        else
            mobileInput["UP"] = false;
        if (CrossPlatformInputManager.GetAxis("Vertical") < -0.3)
            mobileInput["DOWN"] = true;
        else
            mobileInput["DOWN"] = false;
        
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            mobileInput["JUMP"] = true;
        }
        else if (CrossPlatformInputManager.GetButtonUp("Jump"))
        {
            mobileInput["JUMP"] = false;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire"))
        {
            mobileInput["FIRE"] = true;
        }
        else if (CrossPlatformInputManager.GetButtonUp("Fire"))
        {
            mobileInput["FIRE"] = false;
        }
#endif

        foreach (KeyValuePair<string, KeyCode> entry in KEYS)
        {
            myInput[entry.Key] = keyboardInput[entry.Key] || mobileInput[entry.Key];
        }
    }

    void Update()
    {
        checkInput();

        // HORIZONTAL MOVE
        if (myInput["RIGHT"])
        {
            move_horizontally(movement_speed);
        }
        else if (myInput["LEFT"])
        {
            move_horizontally(-movement_speed);
        }

        if (vel.x != 0 && (!myInput["RIGHT"] && isFacingRight) || (!myInput["LEFT"] && !isFacingRight))
        {
            vel.x = 0;
            if (is_touching_ground)
            {
                setBotAnimState(BOT_ANIM_STATES["idle"]);
            }
        }

        // TOP-PART DIRECTION
        setTopAnimDirection(TOP_ANIM_DIRECTIONS["E"]);
        if (myInput["DOWN"])
        {
            if (!myInput["RIGHT"] && !myInput["LEFT"])
            {
                setTopAnimDirection(TOP_ANIM_DIRECTIONS["S"]);
            }
            else
            {
                setTopAnimDirection(TOP_ANIM_DIRECTIONS["SE"]);
            }
        }
        else if (myInput["UP"])
        {
            if (!myInput["RIGHT"] && !myInput["LEFT"])
            {
                setTopAnimDirection(TOP_ANIM_DIRECTIONS["N"]);
            }
            else
            {
                setTopAnimDirection(TOP_ANIM_DIRECTIONS["NE"]);
            }
        }

        // OTHER ACTIONS
        if (myInput["JUMP"])
        {
            jump();
        }

        if (myInput["FIRE"])
        {
            fire();
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
    private void flipSprites(Vector2 vel)
    {
        spritesContainer.transform.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
        isFacingRight = Mathf.Sign(vel.x) > 0;
    }

    private void move_horizontally(float velX)
    {
        vel.x = velX;
        flipSprites(vel);
        if (is_touching_ground)
        {
            setBotAnimState(BOT_ANIM_STATES["walk"]);
        }
    }

    private void fire()
    {
        if (!is_cooldown)
        {
            is_cooldown = true;
            Invoke("resetCooldown", cooldown);
            int angle = FIRE_ROTATION_DEGREES[direction];
            GameObject bulletSpawn = bulletSpawns[direction];
            GameObject newBullet = Instantiate(bullet, bulletSpawn.transform.position, Quaternion.Euler(0, 0, isFacingRight ? angle : 180 - angle));
            setTopAnimationState(TOP_ANIM_STATES["fire"]);
        }
    }

    private void jump()
    {
        if (is_touching_ground)
        {
            vel.y += jump_speed;
            setBotAnimState(BOT_ANIM_STATES["jump"]); // TODO: add walking animation mid-air!
            is_touching_ground = false;
        }
    }

    private void land()
    {
        vel.y = 0;
        if (vel.x != 0)
        {
            setBotAnimState(BOT_ANIM_STATES["walk"]);
        }
        else
        {
            setBotAnimState(BOT_ANIM_STATES["idle"]);
        }
        is_touching_ground = true;
    }


    private void resetCooldown()
    {
        setTopAnimationState(TOP_ANIM_STATES["idle"]); // TODO: improve this!
        is_cooldown = false;
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

    private void setBotAnimState(int state)
    {
        sprite_bot.GetComponent<Animator>().SetInteger("state", state);
    }

    private void setTopAnimationState(int state)
    {
        sprite_top.GetComponent<Animator>().SetInteger("state", state);
    }

    private void setTopAnimDirection(int dir)
    {
        if (dir != direction)
        {
            direction = dir;
            sprite_top.GetComponent<Animator>().SetInteger("direction", dir);
        }
    }

    public int getHorizontalDirection()
    {
        return isFacingRight ? 1 : -1;
    }
}
