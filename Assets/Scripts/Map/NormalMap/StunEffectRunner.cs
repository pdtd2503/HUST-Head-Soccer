using UnityEngine;
using System.Collections;

public class StunEffectRunner : MonoBehaviour
{
    private Coroutine currentRoutine;

    public void PlayStunEffect(Transform playerTransform, float duration,
        GameObject starPrefab, int starCount, float orbitRadius,
        float orbitSpeed, float starHeight)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(
            StunRoutine(playerTransform, duration, starPrefab,
                starCount, orbitRadius, orbitSpeed, starHeight)
        );
    }

    private IEnumerator StunRoutine(Transform playerTransform, float duration,
        GameObject starPrefab, int starCount, float orbitRadius,
        float orbitSpeed, float starHeight)
    {
        // Spawn sao xoay
        GameObject[] stars = new GameObject[starCount];
        float angleStep = 360f / starCount;

        for (int i = 0; i < starCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * orbitRadius,
                starHeight,
                0f
            );
            stars[i] = Instantiate(
                starPrefab,
                playerTransform.position + offset,
                Quaternion.identity
            );
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (playerTransform == null) break;

            elapsed += Time.deltaTime;

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null) continue;

                float angle = (elapsed * orbitSpeed + angleStep * i) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * orbitRadius,
                    starHeight,
                    0f
                );

                stars[i].transform.position = playerTransform.position + offset;
                stars[i].transform.Rotate(0f, 0f, orbitSpeed * Time.deltaTime);
            }

            yield return null;
        }

        foreach (GameObject star in stars)
        {
            if (star != null) Destroy(star);
        }

        currentRoutine = null;
    }
}