using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;

using RedRunner.Characters;
using RedRunner.Collectables;
using RedRunner.TerrainGeneration;
using RedRunner.Networking;
using RedRunner.Target;
using RedRunner.Utilities;

namespace RedRunner
{
	public sealed class GameManager : MonoBehaviour
	{
		public delegate void AudioEnabledHandler(bool active);

		public delegate void ScoreHandler(float newScore, float highScore, float lastScore);

		public delegate void ResetHandler();

		public static event ResetHandler OnReset;
		public static event ScoreHandler OnScoreChanged;
		public static event AudioEnabledHandler OnAudioEnabled;

		private static GameManager m_Singleton;

		public static GameManager Singleton
		{
			get
			{
				return m_Singleton;
			}
		}

		[SerializeField]
		[TextArea(3, 30)]
		private string m_ShareText;
		[SerializeField]
		private string m_ShareUrl;
		private float m_StartScoreX = 0f;
		private float m_HighScore = 0f;
		private float m_LastScore = 0f;
		private float m_Score = 0f;

		private bool m_GameStarted = false;
		private bool m_GameRunning = false;
		private bool m_AudioEnabled = true;

        [SerializeField]
        private GameEvent leftEvent;
        [SerializeField]
        private GameEvent rightEvent;

		[SerializeField]
		private CameraController m_CameraController;
        [SerializeField]
        private SpawnLock spawnLockPrefab = default;
        private SpawnLock spawnLock;

		/// <summary>
		/// This is my developed callbacks compoents, because callbacks are so dangerous to use we need something that automate the sub/unsub to functions
		/// with this in-house developed callbacks feature, we garantee that the callback will be removed when we don't need it.
		/// </summary>
		public Property<int> m_Coin = new Property<int>(0);


		#region Getters
		public bool gameStarted
		{
			get
			{
				return m_GameStarted;
			}
		}

		public bool gameRunning
		{
			get
			{
				return m_GameRunning;
			}
		}

		public bool audioEnabled
		{
			get
			{
				return m_AudioEnabled;
			}
		}
		#endregion

		void Awake()
		{
			if (m_Singleton != null)
			{
				Destroy(gameObject);
				return;
			}
			SaveGame.Serializer = new SaveGameBinarySerializer();
			m_Singleton = this;
			m_Score = 0f;

			if (SaveGame.Exists("coin"))
			{
				m_Coin.Value = SaveGame.Load<int>("coin");
			}
			else
			{
				m_Coin.Value = 0;
			}
			if (SaveGame.Exists("audioEnabled"))
			{
				SetAudioEnabled(SaveGame.Load<bool>("audioEnabled"));
			}
			else
			{
				SetAudioEnabled(true);
			}
			if (SaveGame.Exists("lastScore"))
			{
				m_LastScore = SaveGame.Load<float>("lastScore");
			}
			else
			{
				m_LastScore = 0f;
			}
			if (SaveGame.Exists("highScore"))
			{
				m_HighScore = SaveGame.Load<float>("highScore");
			}
			else
			{
				m_HighScore = 0f;
			}

			RedCharacter.LocalPlayerSpawned += () =>
			{
				RedCharacter.Local.IsDead.AddEventAndFire(UpdateDeathEvent, this);

				m_CameraController?.Follow(RedCharacter.Local.transform);
			};

			RedCharacter.OnTargetChanged += () =>
			{
				StartCoroutine("UpdateTracking");
			};
		}

		void UpdateDeathEvent(bool isDead)
		{
			if (isDead)
			{
				StartCoroutine(DeathCrt());
			}
			else
			{
				StopCoroutine("DeathCrt");
			}
		}

		IEnumerator DeathCrt()
		{
			m_LastScore = m_Score;
			if (m_Score > m_HighScore)
			{
				m_HighScore = m_Score;
			}
			if (OnScoreChanged != null)
			{
				OnScoreChanged(m_Score, m_HighScore, m_LastScore);
			}

			yield return new WaitForSecondsRealtime(1.5f);

			EndGame();
		}

		IEnumerator UpdateTracking()
		{
			// Wait bit if we are switching away from the local player.
			if (RedCharacter.Target != RedCharacter.Local) {
				yield return new WaitForSecondsRealtime(1.5f);
			}

			m_CameraController?.Follow(RedCharacter.Target.transform);
		}

		private void Start()
		{
			Init();
		}

		public void Init()
		{
			EndGame();
			UIManager.Singleton.Init();
			StartCoroutine(Load());
		}

		void Update()
		{
			if (m_GameRunning && RedCharacter.Local != null)
			{
				if (RedCharacter.Local.transform.position.x > m_StartScoreX && RedCharacter.Local.transform.position.x > m_Score)
				{
					m_Score = RedCharacter.Local.transform.position.x;
					if (OnScoreChanged != null)
					{
						OnScoreChanged(m_Score, m_HighScore, m_LastScore);
					}
				}
			}

            #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                leftEvent.Raise();
            } 
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                rightEvent.Raise();
            }
            #endif
        }

        IEnumerator Load()
		{
			var startScreen = UIManager.Singleton.UISCREENS.Find(el => el.ScreenInfo == UIScreenInfo.START_SCREEN);
			yield return new WaitForSecondsRealtime(3f);
			UIManager.Singleton.OpenScreen(startScreen);
		}

		void OnApplicationQuit()
		{
			if (m_Score > m_HighScore)
			{
				m_HighScore = m_Score;
			}
			SaveGame.Save<int>("coin", m_Coin.Value);
			SaveGame.Save<float>("lastScore", m_Score);
			SaveGame.Save<float>("highScore", m_HighScore);
		}

		public void ExitGame()
		{
			Application.Quit();
		}

		public void ToggleAudioEnabled()
		{
			SetAudioEnabled(!m_AudioEnabled);
		}

		public void SetAudioEnabled(bool active)
		{
			m_AudioEnabled = active;
			AudioListener.volume = active ? 1f : 0f;
			if (OnAudioEnabled != null)
			{
				OnAudioEnabled(active);
			}
		}

		public void StartGame()
		{
			m_GameStarted = true;
            ResumeGame();
		}

		public void StopGame()
		{
			m_GameRunning = false;
			Time.timeScale = 0f;
		}

		public void ResumeGame()
		{
			m_GameRunning = true;
			Time.timeScale = 1f;
		}

		public void EndGame()
		{
			m_GameStarted = false;
			//StopGame();
		}

		public void RespawnMainCharacter()
		{
				RespawnCharacter(RedCharacter.Local);
		}

		public void RespawnCharacter(Character character)
        {
            PutCharacterOnStart(character);
            m_StartScoreX = character.transform.position.x;
            character.Reset();
        }

        public void LockCharacterToStart()
        {
            GameObject respawn = SpawnSingleton.instance;
            Vector3 position = respawn.transform.position;
            Debug.Log("Text: " + position);
            position.y += 2.56f;
            float width = respawn.GetComponent<SpriteRenderer>().bounds.size.x;
            spawnLock = Instantiate(spawnLockPrefab, position, Quaternion.identity);
            spawnLock.SetWidth(width);
        }

        public void UnlockCharacterFromStart()
        {
            if (null != spawnLock)
            {
                spawnLock.Unlock();
            }
        }

        private void PutCharacterOnStart(Character character)
        {
            GameObject respawn = SpawnSingleton.instance;
            if (respawn != null)
            {
                Vector3 position = respawn.transform.position;
                Debug.Log("Text: " + position);
                position.y += 2.56f;
                float width = respawn.GetComponent<SpriteRenderer>().bounds.size.x;
                position.x -= width / 2;
                position.x += UnityEngine.Random.Range(0, width);
                character.transform.position = position;
            }
        }

        public void Reset()
		{
			m_Score = 0f;
			if (OnReset != null)
			{
				OnReset();
			}
		}

		public void ShareOnTwitter()
		{
			Share("https://twitter.com/intent/tweet?text={0}&url={1}");
		}

		public void ShareOnGooglePlus()
		{
			Share("https://plus.google.com/share?text={0}&href={1}");
		}

		public void ShareOnFacebook()
		{
			Share("https://www.facebook.com/sharer/sharer.php?u={1}");
		}

		public void Share(string url)
		{
			Application.OpenURL(string.Format(url, m_ShareText, m_ShareUrl));
		}

		[System.Serializable]
		public class LoadEvent : UnityEvent
		{

		}

	}

}
