using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static Vector3 sessionOwnerSpawnPoint = new Vector3(-8, 0.17f, 0);
    public static Vector3 clientSpawnPoint = new Vector3(8, 0.17f, 0);
    public GameObject ballPrefab; // Assign the Ball prefab in the Inspector
    private static int playerCount = 0; // Tracks the number of players

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (NetworkManager.Singleton.ConnectedClients.Count == 1) // First player (session owner)
            {
                transform.position = sessionOwnerSpawnPoint;
            }
            else // Second player
            {
                transform.position = clientSpawnPoint;
            }
        }
    }
}
