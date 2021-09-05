using System;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefence3d.Scripts.UIObjects
{
	/// <summary>
	/// A simple implementation of UI for player base health
	/// </summary>
	public class PlayerBaseHealth : MonoBehaviour
	{
		/// <summary>
		/// The text element to display information on
		/// </summary>
		public Text display;

		/// <summary>
		/// The highest health that the base can go to
		/// </summary>
		protected float m_MaxHealth;

		private int _currentHealth;
		protected virtual void Start()
		{
			_currentHealth = Game.CurrentHealth;
			Game._instance.EnemyReachedBase += OnBaseDamaged;
			UpdateDisplay();
		}

		/// <summary>
		/// Subscribes to the player base health died event
		/// </summary>
		/// <param name="info">
		/// The associated health change information
		/// </param>
		protected virtual void OnBaseDamaged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		/// <summary>
		/// Get the current health of the home base and display it on m_Display
		/// </summary>
		protected void UpdateDisplay()
		{
			display.text = _currentHealth.ToString();
			_currentHealth -= 1;
		}
	}
}