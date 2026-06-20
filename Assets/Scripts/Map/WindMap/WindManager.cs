using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static float CurrentWind;

    [Header("Base Wind")]
    public float maxWindStrength = 3f;
    public float noiseSpeed = 0.1f;

    [Header("Gust Wind")]
    public float gustChance = 0.15f;
    public float gustStrength = 2f;
    public float gustDuration = 1f;

    private float gustTimer;
    private float currentGust;

    private void Update()
    {
        // Base wind từ Perlin Noise
        float noise = Mathf.PerlinNoise(Time.time * noiseSpeed, 0f);

        float baseWind = Mathf.Lerp(
            -maxWindStrength,
             maxWindStrength,
             noise
        );

        // Gust logic
        if (gustTimer <= 0)
        {
            if (Random.value < gustChance * Time.deltaTime)
            {
                currentGust = Random.Range(
                    -gustStrength,
                     gustStrength
                );

                gustTimer = gustDuration;
            }
        }
        else
        {
            gustTimer -= Time.deltaTime;

            if (gustTimer <= 0)
            {
                currentGust = 0f;
            }
        }

        CurrentWind = baseWind + currentGust;
    }
}