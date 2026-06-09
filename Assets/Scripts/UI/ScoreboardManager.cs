using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;

    [SerializeField] private TextMeshProUGUI halfText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Update()
    {
        if (matchManager == null)
        {
            return;
        }

        UpdateHalfText();
        UpdateScoreText();
        UpdateTimeText();
    }

    private void UpdateHalfText()
    {
        int currentHalf = matchManager.GetCurrentHalf();

        if (!matchManager.CanUsePlayerActions() && matchManager.GetTimer() <= 0f)
        {
            halfText.text = "FULL TIME";
            return;
        }

        if (currentHalf == 1)
        {
            halfText.text = "FIRST HALF";
        }
        else if (currentHalf == 2)
        {
            halfText.text = "SECOND HALF";
        }
        else
        {
            halfText.text = "HALF " + currentHalf;
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = matchManager.GetPlayer1Score() + " - " + matchManager.GetPlayer2Score();
    }

    private void UpdateTimeText()
    {
        float timer = matchManager.GetTimer();

        if (timer < 0f)
        {
            timer = 0f;
        }

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}