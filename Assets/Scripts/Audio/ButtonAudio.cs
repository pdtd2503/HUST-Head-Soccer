using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        StartCoroutine(RegisterAllButtons());
    }

    private IEnumerator RegisterAllButtons()
    {
        // Chờ 2 frame để đảm bảo tất cả object đã được khởi tạo xong
        yield return null;
        yield return null;

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        Debug.Log($"ButtonAudio tìm thấy {allButtons.Length} buttons");

        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.scene.isLoaded)
            {
                btn.onClick.AddListener(() =>
                {
                    if (buttonClickSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(buttonClickSound);
                    }
                });
                Debug.Log($"Đã gắn audio cho: {btn.gameObject.name}");
            }
        }
    }
}