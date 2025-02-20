using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    // Player scores
    public NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(0);

    // Flag to indicate if the game is over
    public NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds a point based on which goal was hit.
    /// If isGoalOnPlayer1Side is true, the ball entered Player 1's goal, so Player 2 scores.
    /// If false, Player 1 scores.
    /// </summary>
    public void AddScore(bool isGoalOnPlayer1Side)
    {
        if (!IsSessionOwner) return;
        if (gameOver.Value) return; // Do nothing if the game is over

        if (isGoalOnPlayer1Side)
        {
            // Ball entered Player 1's goal ? Player 2 scores.
            player2Score.Value++;
        }
        else
        {
            // Ball entered Player 2's goal ? Player 1 scores.
            player1Score.Value++;
        }

        Debug.Log($"Score Updated: Player 1: {player1Score.Value} - Player 2: {player2Score.Value}");
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (player1Score.Value >= 5 || player2Score.Value >= 5)
        {
            gameOver.Value = true;
            string winner = (player1Score.Value >= 5) ? "Player 1" : "Player 2";
            Debug.Log($"Game Over! {winner} wins!");
            // Optionally, you could trigger additional UI/game over events here.
        }
    }
}
