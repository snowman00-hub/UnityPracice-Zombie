﻿using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public Zombie prefab;

    public ZombieData[] zombieDatas;
    public Transform[] spawnPoints;

    private List<Zombie> zombies = new List<Zombie>();

    private int wave;

    private void Update()
    {
        if(zombies.Count == 0)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        wave++;
        int count = Mathf.RoundToInt(wave * 1.5f);
        for (int i = 0; i < count; i++)
        {
            CreateZombie();
        }
    }

    public void CreateZombie()
    {
        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var zombie = Instantiate(prefab, point.position, point.rotation);
        zombie.Setup(zombieDatas[Random.Range(0, zombieDatas.Length)]);
        zombies.Add(zombie);

        zombie.OnDeath += () => zombies.Remove(zombie);
        zombie.OnDeath += () => Destroy(zombie.gameObject, 5f);
    }
}
