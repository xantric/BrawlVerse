using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI playerScoreText;

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = "Score: " + score;
    }
}
