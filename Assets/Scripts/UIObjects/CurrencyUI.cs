using System;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;
using UnityEngine.UI;
using static TowerDefence3d.Scripts.Setup.Currency;

namespace TowerDefence3d.Scripts.UIObjects
{
	/// <summary>
	/// A class for controlling the displaying the currency
	/// </summary>
	public class CurrencyUI : MonoBehaviour
	{
		/// <summary>
		/// The text element to display information on
		/// </summary>
		public Text display;

		/// <summary>
		/// The currency prefix to display next to the amount
		/// </summary>
		public string currencySymbol = "$";

		protected Currency m_Currency;

		/// <summary>
		/// Assign the correct currency value
		/// </summary>
		protected virtual void Start()
		{
			m_Currency = Game._instance.Currency;

			UpdateDisplay(null, new CurrencyEventArgs { NewValue = 0f });
			m_Currency.CurrencyChanged += UpdateDisplay;
		}

		/// <summary>
		/// Unsubscribe from events
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (m_Currency != null)
			{
				m_Currency.CurrencyChanged -= UpdateDisplay;
			}
		}

		/// <summary>
		/// A method for updating the display based on the current currency
		/// </summary>
		protected void UpdateDisplay(object sender, CurrencyEventArgs e)
		{
			int current = (int)e.NewValue;
			display.text = current.ToString();
		}
	}
}