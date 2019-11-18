using UnityEngine;
using RedRunner.Characters;
using RedRunner.Utilities;

namespace RedRunner.Networking
{
    public class NetworkManager : Mirror.NetworkManager
    {
        [SerializeField]
        private Transform m_SpawnPoint;
        [SerializeField]
        private string m_HostAddress = "localhost";
        private static int m_ClientCount = 0;

				public static int ClientCount {
					get {
						return m_ClientCount;
					}
				}

		private static bool m_IsHosting = false;

#if DEBUG
		private static bool m_ShouldHost = false;
#endif

		public delegate void NetworkEvent();

		public static event NetworkEvent OnConnected;

        public static int connectionId
        {
            get
            {
                return Mirror.NetworkClient.connection.connectionId;
            }
        }
		public static bool IsConnected
		{
			get
			{
				return Mirror.NetworkClient.isConnected;
			}
		}

		public static bool IsServer
		{
			get
			{
				return IsConnected && m_IsHosting;
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
		}

		public override void Start()
		{
			if (Application.isBatchMode)
			{
				--m_ClientCount;
				Connect(true);
			}
		}

#if DEBUG
		public void OnGUI()
		{
			if (!Mirror.NetworkClient.isConnected && !Mirror.NetworkServer.active && !Mirror.NetworkClient.active)
			{
				m_ShouldHost = GUILayout.Toggle(m_ShouldHost, "Host");
				if (!m_ShouldHost)
				{
					m_HostAddress = GUILayout.TextField(m_HostAddress);
				}
			}
		}
#endif

		public void Connect(bool host = false)
		{
			m_IsHosting = host
#if DEBUG
				|| m_ShouldHost
#endif
				;

			if (m_IsHosting)
			{
				StartHost();
			}
			else
			{
				networkAddress = m_HostAddress;
				StartClient();
			}

            OnConnected?.Invoke();
        }

		public override void OnServerAddPlayer(Mirror.NetworkConnection conn)
		{
			if (!(Application.isBatchMode && conn.connectionId == 0))
			{
				GameObject player = Instantiate(playerPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);
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

        public override void OnServerConnect(Mirror.NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            m_ClientCount++;
        }

        public override void OnServerDisconnect(Mirror.NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            m_ClientCount--;
            ServerRounds.Instance.DecrementPlayer();
        }
    }
}
