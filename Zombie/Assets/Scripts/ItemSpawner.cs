using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public UiManager uiManager;

    public Item[] itemPrefabs;

    public float minX;
    public float maxX;
    public float y = 0.5f;
    public float minZ;
    public float maxZ;

    public float SpawnInterval = 2f;
    private float lastSpawnTime;

    private void Update()
    {
        if(lastSpawnTime + SpawnInterval < Time.time)
        {
            lastSpawnTime = Time.time;
            while (true)
            {
                var randomPoint = new Vector3(0, y, 0);
                randomPoint.x = Random.Range(minX, maxX);
                randomPoint.z = Random.Range(minZ, maxZ);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.5f, NavMesh.AllAreas))
                {
                   Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)],
                       hit.position, Quaternion.identity, gameObject.transform);
                    break;
                }
            }
        }
    }
}
