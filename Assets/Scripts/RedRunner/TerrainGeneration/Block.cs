﻿using UnityEngine;

using RedRunner.Networking;

namespace RedRunner.TerrainGeneration
{
	public abstract class Block : ServerSpawnable
	{
		[SerializeField]
		protected float m_Width;
		[SerializeField]
		protected float m_Probability = 1f;

		public virtual float Width {
			get {
				return m_Width;
			}
			set {
				m_Width = value;
			}
		}

		public virtual float Probability {
			get {
				return m_Probability;
			}
		}

		public virtual void OnRemove (TerrainGenerator generator)
		{
			
		}

		public virtual void PreGenerate (TerrainGenerator generator)
		{
			
		}

		public virtual void PostGenerate (TerrainGenerator generator)
		{
			
		}
	}
}
