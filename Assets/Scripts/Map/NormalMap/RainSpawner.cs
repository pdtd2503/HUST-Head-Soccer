using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    public GameObject obstacle;

    public float leftX=-8;

    public float rightX=8;

    public float spawnY=6;

    public float interval=2f;

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
        float x=
            Random.Range(
                leftX,
                rightX
            );

        Instantiate(
            obstacle,
            new Vector2(
                x,
                spawnY
            ),
            Quaternion.identity
        );
    }
}