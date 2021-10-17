using System;
using UnityEngine;

namespace Core.Game
{
	/// <summary>
	/// Element describing a level
	/// </summary>
	[Serializable]
	public class LevelItem
	{
		/// <summary>
		/// The id - used in persistence
		/// </summary>
		public string id;

		/// <summary>
		/// The human readable level name
		/// </summary>
		public string name;

		/// <summary>
		/// The description of the level - flavour text
		/// </summary>
		public string description;

		/// <summary>
		/// The name of the scene to load
		/// </summary>
		public string SceneName;

        public Vector2Int BoardSize;

        public Vector2Int[] StartPoints;

        public Vector2Int[] EndPoints;

        public Vector2Int[] RocksPoints;
	}
}