using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 300f; // 5 minutes
    private float timeRemaining;
    private bool gameEnded = false;

    public TextMeshProUGUI timerText;
    public GameObject leaderboardPanel; // to show at end
    public GameObject gameplayUI; // to hide at end

    void Start()
    {
        timeRemaining = gameDuration;
        gameEnded = false;

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);
    }

    void Update()
    {
        if (gameEnded) return;

        timeRemaining -= Time.deltaTime;

        if (timerText != null)
            timerText.text = FormatTime(timeRemaining);

        if (timeRemaining <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
