using UnityEngine;
using RedRunner.Characters;
using RedRunner.Utilities;

namespace RedRunner.Networking
{
	public class NetworkManager : Mirror.NetworkManager
	{
		[SerializeField]
		private Transform spawnPoint;
		[SerializeField]
		private CameraController cameraController;

#if !SERVER
		private static string host = "localhost";
#endif

		public delegate void NetworkEvent();

		public static event NetworkEvent OnConnected;

		public static bool IsConnected
		{
			get
			{
				return Mirror.NetworkClient.isConnected;
			}
		}

		public static bool IsServer {
			get
			{
				return Application.isBatchMode && IsConnected;
			}
		}

		public static RedCharacter LocalCharacter { get; private set; }

		public static NetworkManager Instance { get; private set; }

		public override void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("Only a single NetworkManager should be active at a time");
				return;
			}
			Instance = GetComponent<NetworkManager>();
			base.Awake();

			RedCharacter.LocalPlayerSpawned += () =>
			{
				cameraController.Follow(RedCharacter.Local.transform);
			};
		}

		public override void Start()
		{
			if (Application.isBatchMode)
			{
				StartHost();

				// TODO(shane) see if we can do this properly.
				OnConnected();
			}
		}

#if !SERVER
		public void OnGUI()
		{
			if (!Mirror.NetworkClient.isConnected && !Mirror.NetworkServer.active && !Mirror.NetworkClient.active)
			{
				host = GUILayout.TextField(host);
			}
		}

		public void Connect()
		{
			networkAddress = host;
			StartClient();

			// TODO(shane) see if we can do this properly.
			OnConnected();
		}
#endif

		public override void OnServerAddPlayer(Mirror.NetworkConnection conn)
		{
			// Instantiate characters for clients only.
			if (conn.connectionId != 0)
			{
				GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
				Mirror.NetworkServer.AddPlayerForConnection(conn, player);
			}
		}

		public static void RegisterSpawnablePrefab(GameObject prefab)
		{
			Mirror.ClientScene.RegisterPrefab(prefab);
		}

		public static void Spawn(GameObject gameObject)
		{
			Mirror.NetworkServer.Spawn(gameObject);
		}
	}
}
