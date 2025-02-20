using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PowerUpSpawner : NetworkBehaviour
{
    [Tooltip("List of power-up prefabs to spawn.")]
    public GameObject[] powerUpPrefabs;

    [Tooltip("Spawn area for the power-ups.")]
    public Vector2 spawnAreaMin = new Vector2(-3, -2);
    public Vector2 spawnAreaMax = new Vector2(3, 2);

    private GameObject currentPowerUp;

    public override void OnNetworkSpawn()
    {
        if (IsSessionOwner) // Only the server handles power-up spawning
        {
            StartCoroutine(SpawnPowerUpRoutine());
        }
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            if (currentPowerUp == null) // Only spawn if no power-up exists
            {
                float waitTime = Random.Range(5f, 10f);
                yield return new WaitForSeconds(waitTime);

                SpawnPowerUpServerRpc();
            }
            yield return null;
        }
    }

    [ServerRpc]
    private void SpawnPowerUpServerRpc()
    {
        if (currentPowerUp != null || powerUpPrefabs.Length == 0) return;

        // Pick a random power-up
        GameObject powerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        // Pick a random spawn position
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // Instantiate and spawn the power-up
        currentPowerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        currentPowerUp.GetComponent<NetworkObject>().Spawn();
    }

    public void OnPowerUpCollected(GameObject player, string powerUpTag)
    {
        if (!IsSessionOwner) return;

        switch (powerUpTag)
        {
            case "BallMultiplier":
                StartCoroutine(SpawnExtraBalls());
                break;
            case "PaddleEnlarger":
                StartCoroutine(ScalePlayer(player));
                break;
        }

        // Destroy the power-up after collection
        if (currentPowerUp != null)
        {
            NetworkObject netObj = currentPowerUp.GetComponent<NetworkObject>();
            if (netObj != null) netObj.Despawn();
            Destroy(currentPowerUp);
            currentPowerUp = null;
        }
    }

    private IEnumerator SpawnExtraBalls()
    {
        GameObject mainBall = GameObject.FindGameObjectWithTag("Ball");
        if (mainBall == null) yield break;

        BallMovement mainBallScript = mainBall.GetComponent<BallMovement>();
        if (mainBallScript == null) yield break;

        GameObject extraBall1 = SpawnExtraBall(mainBallScript);
        GameObject extraBall2 = SpawnExtraBall(mainBallScript);

        yield return new WaitForSeconds(5f);

        if (extraBall1 != null) Destroy(extraBall1);
        if (extraBall2 != null) Destroy(extraBall2);
    }

    private GameObject SpawnExtraBall(BallMovement originalBall)
    {
        GameObject extraBall = Instantiate(originalBall.gameObject, originalBall.transform.position, Quaternion.identity);
        NetworkObject netObj = extraBall.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn();
            extraBall.GetComponent<Rigidbody2D>().linearVelocity = originalBall.GetVelocity();
        }

        return extraBall;
    }

    private IEnumerator ScalePlayer(GameObject player)
    {
        Vector3 originalScale = player.transform.localScale;

        player.transform.localScale = new Vector3(originalScale.x, originalScale.y * 3, originalScale.z);
        yield return new WaitForSeconds(6f);

        player.transform.localScale = originalScale;
    }
}
