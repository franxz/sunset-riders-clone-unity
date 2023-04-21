using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reaper : MonoBehaviour
{
    public GameObject bullet;
    public float movementSpeed;
    public float cooldown;

    GameObject player;
    GameObject sprite;
    Animator animator;
    Vector3 stageMax;
    Vector3 stageMin;
    Vector3 preferedHeight;
    Vector2 vel = new Vector2(0, 0);
    bool isGettingClose = true;
    bool isCooldown = false;
    bool isFireAnimationActive = false;
    bool isDead = false;

    void Start()
    {
        StageHandler stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageHandler>();
        stageMax = stage.max.transform.position;
        stageMin = stage.min.transform.position;
        preferedHeight = stage.reaperPreferedHeight.transform.position;

        player = GameObject.Find("gunner");
        sprite = gameObject.transform.Find("reaper_sprite").gameObject;
        animator = sprite.GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            vel.x *= 0.8f;
            return;
        }

        float optimalDistance = isGettingClose ? 3.8f : 4.8f;

        Vector2 distance = player.gameObject.transform.position - gameObject.transform.position;
        vel = distance.normalized * movementSpeed;

        if (distance.sqrMagnitude > optimalDistance)
        {
            // we need to get close
            isGettingClose = true;
        }
        else
        {
            // we're close enough -> let's fire
            isGettingClose = false;
            vel = -1 * vel / 4f;
            if (!isCooldown)
            {
                isCooldown = true;
                isFireAnimationActive = true;
                animator.Play("reaper_fire");
                Invoke("resetFireAnimation", cooldown / 3f);
                Invoke("resetCooldown", cooldown);
                GameObject newBullet = Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
                newBullet.GetComponent<reaper_bullet>().setDirection(distance.normalized);
            }
        }

        // we want the reaper flying at the preferedHeight, so we move it there
        vel.y += Mathf.Sign(preferedHeight.y - transform.position.y) * Mathf.Sqrt(Mathf.Abs(preferedHeight.y - transform.position.y));

        if (!isFireAnimationActive)
        {
            if (Mathf.Abs(vel.x) > 0.2)
            {
                animator.Play("reaper_moving");
                sprite.GetComponent<SpriteRenderer>().flipX = vel.x > 0;
            }
            else
            {
                animator.Play("reaper_idle");
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            vel.y += Physics2D.gravity.y * Time.deltaTime;
        }
        GetComponent<Rigidbody2D>().position += vel * Time.deltaTime;
        checkLimits();
    }

    void checkLimits()
    {
        //float yPadTop = 0.075f;
        float yPadBot = 0.075f;

        /*if (GetComponent<Rigidbody2D>().position.y > stageMax.y - yPadTop)
        {
            GetComponent<Rigidbody2D>().position = new Vector2(GetComponent<Rigidbody2D>().position.x, stageMax.y - yPadTop);
        }
        else*/
        if (GetComponent<Rigidbody2D>().position.y < stageMin.y + yPadBot)
        {
            if (isDead)
            {
                vel.y *= -0.35f;
                GetComponent<Rigidbody2D>().position = new Vector2(GetComponent<Rigidbody2D>().position.x, stageMin.y + yPadBot);
            } else
            {
                GetComponent<Rigidbody2D>().position = new Vector2(GetComponent<Rigidbody2D>().position.x, stageMin.y + yPadBot);
            }
        }
    }

    void resetFireAnimation()
    {
        isFireAnimationActive = false;
    }

    void resetCooldown()
    {
        isCooldown = false;
    }

    public void getHit()
    {
        isDead = true;
        animator.Play("reaper_dead");
        Destroy(gameObject, 1f);
    }
}
