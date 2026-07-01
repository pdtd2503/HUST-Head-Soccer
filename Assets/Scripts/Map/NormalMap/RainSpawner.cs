using System.Collections.Generic;
using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    public GameObject obstacle;

    [Header("Stun Effect")]
    public GameObject starPrefab;

    public float leftX = -8;
    public float rightX = 8;
    public float spawnY = 6;
    public float interval = 2f;

    private readonly List<GameObject> spawnedObstacles = new();
    private bool spawningEnabled;

    private void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (obstacle == null)
        {
            Debug.LogWarning("RainSpawner: obstacle prefab is null.");
            return;
        }

        spawningEnabled = true;

        CancelInvoke(nameof(Spawn));

        InvokeRepeating(
            nameof(Spawn),
            1f,
            interval
        );

        Debug.Log("RainSpawner: Start spawning books.");
    }

    public void StopSpawningAndClearObstacles()
    {
        spawningEnabled = false;

        CancelInvoke(nameof(Spawn));

        ClearObstacles();

        Debug.Log("RainSpawner: Stop spawning and clear books.");
    }

    public void ClearObstacles()
    {
        for (int i = spawnedObstacles.Count - 1; i >= 0; i--)
        {
            if (spawnedObstacles[i] != null)
            {
                Destroy(spawnedObstacles[i]);
            }
        }

        spawnedObstacles.Clear();
    }

    private void Spawn()
    {
        if (!spawningEnabled)
        {
            return;
        }

        if (obstacle == null)
        {
            Debug.LogWarning("RainSpawner: obstacle prefab is null.");
            return;
        }

        float x = Random.Range(leftX, rightX);

        GameObject spawnedObstacle = Instantiate(
            obstacle,
            new Vector2(x, spawnY),
            Quaternion.identity
        );

        spawnedObstacles.Add(spawnedObstacle);

        FallingObstacle fallingObstacle =
            spawnedObstacle.GetComponent<FallingObstacle>();

        if (fallingObstacle != null)
        {
            fallingObstacle.starPrefab = starPrefab;
        }
    }
}