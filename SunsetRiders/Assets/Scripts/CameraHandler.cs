using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public GameObject player;
    public GameObject playerLimit;

    Vector3 stageMax;
    Vector3 stageMin;

    void Start()
    {
        StageHandler stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageHandler>();
        stageMax = stage.max.transform.position;
        stageMin = stage.min.transform.position;
    }

    void Update()
    {
        if ((player.transform.position.x > playerLimit.transform.position.x) && (playerLimit.transform.position.x < stageMax.x))
        {
            transform.position += new Vector3(0.02f, 0, 0);
        }
    }
}
