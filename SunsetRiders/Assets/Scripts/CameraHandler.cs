using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public GameObject player;
    public GameObject rightLimit;
    public GameObject leftLimit;
    public GameObject cameraRightLimit;
    public GameObject cameraLeftLimit;

    Vector3 stageMax;
    Vector3 stageMin;

    void Start()
    {
        rightLimit = gameObject.transform.Find("right_limit").gameObject;
        leftLimit = gameObject.transform.Find("left_limit").gameObject;
        cameraRightLimit = gameObject.transform.Find("camera_right_limit").gameObject;
        cameraLeftLimit = gameObject.transform.Find("camera_left_limit").gameObject;

        StageHandler stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageHandler>();
        stageMax = stage.max.transform.position;
        stageMin = stage.min.transform.position;
    }

    void Update()
    {
        int playerDir = player.GetComponent<CharacterHandler>().getHorizontalDirection();

        if ((playerDir > 0 && (player.transform.position.x > rightLimit.transform.position.x) && (cameraRightLimit.transform.position.x < stageMax.x))
            || (playerDir < 0 && (player.transform.position.x < leftLimit.transform.position.x) && (cameraLeftLimit.transform.position.x > stageMin.x)))
        {
            transform.position += new Vector3(-transform.position.x + GameObject.FindGameObjectWithTag("Player").transform.position.x, 0, 0);
        }
    }
}
