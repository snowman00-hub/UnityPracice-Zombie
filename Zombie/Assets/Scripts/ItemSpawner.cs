using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    public UiManager uiManager;
    public GameManager gameManager;

    public Item[] itemPrefabs;

    public float radius = 30f;

    public float SpawnInterval = 2f;
    private float lastSpawnTime;

    private void Update()
    {
        if (lastSpawnTime + SpawnInterval < Time.time)
        {
            lastSpawnTime = Time.time;
            TryCreateItem();
        }
    }

    private bool TryCreateItem()
    {
        for (int i = 0; i < 30; i++)
        {
            var randomPoint = transform.position + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
            {
                var item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)],
                    hit.position, Quaternion.identity, gameObject.transform);

                if (item.itemType == Item.Types.Coin)
                    item.OnUse += () => gameManager.AddScore(item.value);

                return true;
            }
        }

        return false;
    }
}
