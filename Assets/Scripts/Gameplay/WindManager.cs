using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static float CurrentWind = 0f;

    [Header("Wind Settings")]
    public float windForce = 2f;
    public float switchTime = 5f;

    private float timer;

    private void Start()
    {
        CurrentWind = windForce;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchTime)
        {
            timer = 0f;

            windForce *= -1f;

            CurrentWind = windForce;
        }
    }

    private void OnDestroy()
    {
        CurrentWind = 0f;
    }
}