using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallMovement : NetworkBehaviour
{
    [Tooltip("Speed at which the ball moves.")]
    public float speed = 5f;

    [Tooltip("Speed increase multiplier on player collision.")]
    public float speedMultiplier = 1.1f;

    [Tooltip("Reference to the Rigidbody2D component.")]
    public Rigidbody2D rb;

    // The starting position is saved so we can reset the ball after a score.
    private Vector2 startPosition;

    public override void OnNetworkSpawn()
    {
        // Ensure we have a reference to the Rigidbody2D.
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;

        // Only the server controls the ball's movement.
        if (IsSessionOwner)
        {
            Launch();
        }
    }

    /// <summary>
    /// Launches the ball in a random diagonal direction.
    /// </summary>
    void Launch()
    {
        float xDir = Random.Range(0, 2) == 0 ? -1f : 1f;
        float yDir = Random.Range(0, 2) == 0 ? -1f : 1f;
        Vector2 velocity = new Vector2(xDir, yDir).normalized * speed;
        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Resets the ball's position and velocity, then launches it again.
    /// </summary>
    void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        Launch();
    }
    public Vector2 GetVelocity()
    {
        return rb.linearVelocity;
    }

    /// <summary>
    /// Detect collisions with goal objects.
    /// This function runs only on the server.
    /// </summary>
    private async void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsSessionOwner) return; // Only the server handles scoring

        // When the ball collides with the left goal, assume the right player scores.
        if (collision.gameObject.CompareTag("Goal1"))
        {
            Debug.Log("Collide" + collision.gameObject.name);
            ScoreManager.Instance.AddScore(false);

            ResetBall();
        }
        // When the ball collides with the right goal, assume the left player scores.
        else if (collision.gameObject.CompareTag("Goal2"))
        {
            Debug.Log("Collide" + collision.gameObject.name);
            ScoreManager.Instance.AddScore(true);
            ResetBall();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity *= speedMultiplier; // Increase speed
        }
    }
}
