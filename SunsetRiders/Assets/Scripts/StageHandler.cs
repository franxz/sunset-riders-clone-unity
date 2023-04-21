using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHandler : MonoBehaviour
{
    public GameObject min;
    public GameObject max;
    public GameObject reaperPreferedHeight;
    public GameObject reaper;
    public GameObject[] enemySpawns;

    void Start()
    {
        reaperPreferedHeight = gameObject.transform.Find("reaper_prefered_height").gameObject;
        enemySpawns = new GameObject[3] {
            GameObject.Find("stage/enemy_spawns/spawn1"),
            GameObject.Find("stage/enemy_spawns/spawn2"),
            GameObject.Find("stage/enemy_spawns/spawn3"),
        };
        spawnEnemies();
    }

    void Update()
    {

    }

    void spawnEnemies()
    {
        foreach (GameObject spawn in enemySpawns)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                Instantiate(reaper, spawn.transform.position, Quaternion.identity);
            }
        }
        Invoke("spawnEnemies", Random.Range(3f, 5f));
    }
}
