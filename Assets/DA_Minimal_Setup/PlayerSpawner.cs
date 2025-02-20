using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public GameObject playerPrefab; // Assign your player prefab in the Inspector
    public Transform spawnPoint1;    // Assign your first spawn point in the Inspector
    public Transform spawnPoint2;    // Assign your second spawn point in the Inspector

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Only the server should handle player spawning
        if (IsServer)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        // Decide where to spawn the player based on the connection count
        Transform spawnPoint = NetworkManager.Singleton.ConnectedClients.Count % 2 == 0 ? spawnPoint1 : spawnPoint2;

        // Instantiate the player prefab at the chosen spawn point
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Spawn the player object as a network object
        player.GetComponent<NetworkObject>().Spawn();
    }
}