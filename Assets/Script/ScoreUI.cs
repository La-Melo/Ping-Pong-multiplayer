using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreUI : NetworkBehaviour
{
    [Tooltip("TextMeshProUGUI component for displaying scores (format 0:0) and game over message.")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI GameOverText;

    private void Start()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance is not found in the scene.");
            return;
        }

        // Subscribe to changes in score and game over state
        ScoreManager.Instance.player1Score.OnValueChanged += (prev, curr) => RefreshUI();
        ScoreManager.Instance.player2Score.OnValueChanged += (prev, curr) => RefreshUI();
        ScoreManager.Instance.gameOver.OnValueChanged += (prev, curr) => RefreshUI();

        RefreshUI();
    }

    /// <summary>
    /// Updates the TextMeshProUGUI text with the current score in the format "Player1Score:Player2Score".
    /// If the game is over, also displays a game-over message with the winner.
    /// </summary>
    private void RefreshUI()
    {
        int p1 = ScoreManager.Instance.player1Score.Value;
        int p2 = ScoreManager.Instance.player2Score.Value;

        if (ScoreManager.Instance.gameOver.Value)
        {
            string winner = (p1 >= 5) ? "Player 1" : "Player 2";
            scoreText.text = $"{p1}:{p2}";
            GameOverText.text = $"Game Over! { winner} wins!";
            Time.timeScale = 0f;
        }
        else
        {
            scoreText.text = $"{p1}:{p2}";
        }
    }
}
