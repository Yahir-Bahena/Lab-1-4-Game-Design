using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI balloonsText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {GameManager.Instance.GetScore()}";
            }
            
            if (livesText != null)
            {
                livesText.text = $"Lives: {GameManager.Instance.GetLives()}";
            }
            
            if (balloonsText != null)
            {
                balloonsText.text = $"Balloons: {GameManager.Instance.GetBalloonsPopped()}";
            }
        }
    }
}