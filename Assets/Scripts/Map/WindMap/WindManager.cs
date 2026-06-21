using UnityEngine;
using System.Collections;

public class WindManager : MonoBehaviour
{
    [Header("Wind Cycle (gió thường)")]
    [SerializeField] private float cycleDuration = 12f;
    [SerializeField] private float normalWindMinForce = 0.5f;
    [SerializeField] private float normalWindMaxForce = 2f;
    [SerializeField] private float maxVerticalAngle = 20f; // độ xiên tối đa (độ)

    [Header("Wind Gust (gió giật, ngẫu nhiên)")]
    [SerializeField] private float gustMinInterval = 6f;
    [SerializeField] private float gustMaxInterval = 18f;
    [SerializeField] private float gustForce = 6f;
    [SerializeField] private float gustDuration = 0.6f;
    [SerializeField] private float gustMaxVerticalAngle = 35f; // gust xiên nhiều hơn

    [Header("Ball Reference")]
    [SerializeField] private string ballTag = "Ball";

    [Header("Leaf Particle Effect")]
    [SerializeField] private Sprite leafSprite; // kéo sprite lá thật vào đây
    [SerializeField] private float leafSpawnInterval = 0.3f;
    [SerializeField] private int leavesPerSpawn = 2;

    private float currentWindAngle; // góc gió (độ), 0 = ngang phải, dương = chếch lên, âm = chếch xuống
    private float currentWindDirection = 1f;
    private float currentWindForce;

    private float gustAngle;
    private float gustDirection = 1f;
    private bool isGusting;

    private Rigidbody2D ballRb;
    private float leafSpawnTimer;

    private void Start()
    {
        StartCoroutine(WindCycleRoutine());
        StartCoroutine(GustRoutine());
        FindBall();
    }

    private void FindBall()
    {
        GameObject ballObj = GameObject.FindGameObjectWithTag(ballTag);
        if (ballObj != null)
        {
            ballRb = ballObj.GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        if (ballRb == null)
        {
            FindBall();
            return;
        }

        float direction = isGusting ? gustDirection : currentWindDirection;
        float angle = isGusting ? gustAngle : currentWindAngle;
        float force = isGusting ? gustForce : currentWindForce;

        // Tính vector lực theo góc xiên
        float rad = angle * Mathf.Deg2Rad;
        Vector2 windVector = new Vector2(
            direction * Mathf.Cos(rad),
            Mathf.Sin(rad)
        ) * force;

        ballRb.AddForce(windVector);
    }

    private void Update()
    {
        leafSpawnTimer -= Time.deltaTime;
        if (leafSpawnTimer <= 0f)
        {
            SpawnLeafParticles();
            leafSpawnTimer = leafSpawnInterval;
        }
    }

    private IEnumerator WindCycleRoutine()
    {
        while (true)
        {
            currentWindDirection = Random.value > 0.5f ? 1f : -1f;
            currentWindForce = Random.Range(normalWindMinForce, normalWindMaxForce);
            currentWindAngle = Random.Range(-maxVerticalAngle, maxVerticalAngle);

            yield return new WaitForSeconds(cycleDuration);
        }
    }

    private IEnumerator GustRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(gustMinInterval, gustMaxInterval);
            yield return new WaitForSeconds(waitTime);

            gustDirection = Random.value > 0.5f ? 1f : -1f;
            gustAngle = Random.Range(-gustMaxVerticalAngle, gustMaxVerticalAngle);

            isGusting = true;
            yield return new WaitForSeconds(gustDuration);
            isGusting = false;
        }
    }

    private void SpawnLeafParticles()
    {
        if (leafSprite == null) return;

        float direction = isGusting ? gustDirection : currentWindDirection;
        float angle = isGusting ? gustAngle : currentWindAngle;
        float force = isGusting ? gustForce : currentWindForce;

        int count = isGusting ? leavesPerSpawn * 3 : leavesPerSpawn;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(
                Camera.main.transform.position.x - direction * 8f,
                Random.Range(-2f, 6f),
                0f
            );

            GameObject leaf = new GameObject("WindLeaf");
            leaf.transform.position = spawnPos;
            leaf.transform.localScale = Vector3.one * Random.Range(0.06f, 0.1f);

            SpriteRenderer sr = leaf.AddComponent<SpriteRenderer>();
            sr.sprite = leafSprite;
            sr.sortingOrder = 5;

            StartCoroutine(LeafFlyRoutine(leaf, direction, angle, force));
        }
    }

    private IEnumerator LeafFlyRoutine(GameObject leaf, float direction, float angle, float force)
    {
        float lifetime = Random.Range(2f, 3.5f);
        float elapsed = 0f;

        float rad = angle * Mathf.Deg2Rad;
        float speedX = direction * Mathf.Cos(rad) * (1.5f + force * 0.4f);
        float speedY = Mathf.Sin(rad) * (1.5f + force * 0.4f);

        Vector3 startPos = leaf.transform.position;
        float sineOffset = Random.Range(0f, Mathf.PI * 2f);

        while (elapsed < lifetime)
        {
            if (leaf == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            float x = startPos.x + speedX * elapsed;
            float y = startPos.y + speedY * elapsed + Mathf.Sin(elapsed * 3f + sineOffset) * 0.25f;

            leaf.transform.position = new Vector3(x, y, 0f);
            leaf.transform.Rotate(0f, 0f, 250f * Time.deltaTime * direction);

            SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(leaf);
    }

    private void OnDrawGizmos()
    {
        float direction = isGusting ? gustDirection : currentWindDirection;
        float angle = isGusting ? gustAngle : currentWindAngle;

        Gizmos.color = isGusting ? Color.red : Color.cyan;
        Vector3 pos = transform.position + Vector3.up * 5f;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(direction * Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        Gizmos.DrawLine(pos, pos + dir * 2f);
    }
}