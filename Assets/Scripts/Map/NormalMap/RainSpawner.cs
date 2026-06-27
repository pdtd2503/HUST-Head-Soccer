using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    public GameObject obstacle;

    [Header("Stun Effect")]
    public GameObject starPrefab; // kéo Star Prefab vào đây

    public float leftX = -8;
    public float rightX = 8;
    public float spawnY = 6;
    public float interval = 2f;

    void Start()
    {
        InvokeRepeating(
            nameof(Spawn),
            1,
            interval
        );
    }

    void Spawn()
    {
        float x = Random.Range(leftX, rightX);

        GameObject spawnedObstacle = Instantiate(
            obstacle,
            new Vector2(x, spawnY),
            Quaternion.identity
        );

        // Truyền starPrefab vào FallingObstacle vừa spawn
        FallingObstacle fo = spawnedObstacle.GetComponent<FallingObstacle>();
        if (fo != null)
        {
            fo.starPrefab = starPrefab;
        }
    }
}