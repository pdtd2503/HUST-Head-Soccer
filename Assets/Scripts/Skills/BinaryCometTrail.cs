using UnityEngine;
using System.Collections.Generic;

public class BinaryCometTrail : MonoBehaviour
{
    // ---- Cấu hình hình dạng đuôi kim cương (7 hàng, kéo dài) ----
    private static readonly int[] ROW_LENGTHS = { 10, 20, 30, 40, 30, 20, 10  };
    private const int ROW_COUNT = 7;

    private const float ROW_SPACING = 0.08f;
    private const float CHAR_SPACING = 0.1f;
    private const float CHAR_SIZE = 0.03f;

    private const float FLICKER_INTERVAL = 0.1f;

    private readonly Color brightColor = new Color(0.7f, 1f, 0.8f);
    private readonly Color baseColor = new Color(0f, 0.85f, 0.3f);

    private Rigidbody2D rb;
    private GameObject clusterRoot;
    private List<TextMesh> activeSymbols = new List<TextMesh>();

    private float flickerTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        flickerTimer = 0f;
        BuildTail();
    }

    private void OnDisable()
    {
        if (clusterRoot != null) Destroy(clusterRoot);
        activeSymbols.Clear();
    }

    private void BuildTail()
    {
        if (clusterRoot != null) Destroy(clusterRoot);

        clusterRoot = new GameObject("BinaryTailRoot");
        clusterRoot.transform.SetParent(transform);
        clusterRoot.transform.localPosition = Vector3.zero;

        activeSymbols.Clear();

        int maxLength = ROW_LENGTHS[ROW_COUNT / 2];

        for (int row = 0; row < ROW_COUNT; row++)
        {
            int length = ROW_LENGTHS[row];
            int rowOffsetIndex = row - (ROW_COUNT / 2);

            for (int i = 0; i < length; i++)
            {
                GameObject symbolObj = new GameObject("BinaryChar");
                symbolObj.transform.SetParent(clusterRoot.transform);

                symbolObj.transform.localPosition = new Vector3(
                    -i * CHAR_SPACING,
                    rowOffsetIndex * ROW_SPACING,
                    0f
                );

                TextMesh tm = symbolObj.AddComponent<TextMesh>();
                tm.text = Random.value > 0.5f ? "1" : "0";
                tm.fontSize = 48;
                tm.characterSize = CHAR_SIZE;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;

                float distanceRatio = (float)i / maxLength;
                float alpha = Mathf.Lerp(1f, 0.6f, distanceRatio);

                Color baseCol = i == 0 ? brightColor : baseColor;
                tm.color = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);

                MeshRenderer mr = symbolObj.GetComponent<MeshRenderer>();
                mr.sortingOrder = 100;

                activeSymbols.Add(tm);
            }
        }
    }

    private void Update()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f && clusterRoot != null)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            clusterRoot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            foreach (TextMesh tm in activeSymbols)
            {
                if (tm != null) tm.text = Random.value > 0.5f ? "1" : "0";
            }
            flickerTimer = FLICKER_INTERVAL;
        }
    }
}