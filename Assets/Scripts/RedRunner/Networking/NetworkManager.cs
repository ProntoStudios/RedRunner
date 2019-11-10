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
		[SerializeField]
		private string hostAddress = "localhost";

		private static bool isHosting = false;

#if DEBUG
		private static bool shouldHost = false;
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
				return IsConnected && isHosting;
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
				Connect(true);
			}
		}

#if DEBUG
		public void OnGUI()
		{
			if (!Mirror.NetworkClient.isConnected && !Mirror.NetworkServer.active && !Mirror.NetworkClient.active)
			{
				shouldHost = GUILayout.Toggle(shouldHost, "Host");
				if (!shouldHost)
				{
					hostAddress = GUILayout.TextField(hostAddress);
				}
			}
		}
#endif

		public void Connect(bool host = false)
		{
			isHosting = host
#if DEBUG
				|| shouldHost
#endif
				;

			if (isHosting)
			{
				StartHost();
			} else
			{
				networkAddress = hostAddress;
				StartClient();
			}

			OnConnected();
		}

		public override void OnServerAddPlayer(Mirror.NetworkConnection conn)
		{
			if (!(Application.isBatchMode && conn.connectionId == 0))
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
