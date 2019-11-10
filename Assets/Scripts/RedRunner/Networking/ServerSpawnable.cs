using UnityEngine;

namespace RedRunner.Networking
{
	/// <summary>
	/// Helper component for game object spawned by the server which should synchronize to clients.
	/// </summary>
	[RequireComponent(typeof(Mirror.NetworkIdentity))]
	public class ServerSpawnable : Mirror.NetworkBehaviour
	{
		public bool ping = false;

		public virtual void Awake()
		{
			if (NetworkManager.IsServer && !ping)
			{
				NetworkManager.Spawn(gameObject);
			}
		}

		[Mirror.ClientRpc]
		void RpcPing2() {
			Debug.Log("Ping2!");
		}

		public void Update() {
			if (NetworkManager.IsServer && ping) {
				RpcPing2();
			}
		}
	}
}
