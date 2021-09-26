using System;
using TowerDefence3d.Scripts.Setup;
using TowerDefence3d.Scripts.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefence3d.Scripts.UIObjects
{
	/// <summary>
	/// A button controller for spawning towers
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class TowerSpawnButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
	{
		/// <summary>
		/// The text attached to the button
		/// </summary>
		
		[SerializeField]
		private Text buttonText;


		/// <summary>
		/// The tower controller that defines the button
		/// </summary>
		
		[SerializeField]
		private Tower m_Tower;

		[SerializeField]
		private GameTileContentFactory _factory;

		private Image towerIcon;

		[SerializeField]
		private Button buyButton;
		[SerializeField]
		private Image energyIcon;

		private Color energyDefaultColor;

		private Color energyInvalidColor;

		private Tower _draggedTower = null;


		/// <summary>
		/// Cached reference to level currency
		/// </summary>
		Currency m_Currency;

		/// <summary>
		/// The attached rect transform
		/// </summary>
		RectTransform m_RectTransform;

		/// <summary>
		/// Checks if the pointer is out of bounds
		/// and then fires the draggedOff event
		/// </summary>
		public virtual void OnDrag(PointerEventData eventData)
		{
			Debug.Log("OnDrag");
			_draggedTower.transform.localPosition = Input.mousePosition;
			if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
			{
			}
		}

		/// <summary>
		/// Define the button information for the tower
		/// </summary>
		/// <param name="towerData">
		/// The tower to initialize the button with
		/// </param>
		void Start()
		{
			//if (towerData.levels.Length > 0)
			//{
			//	TowerLevel firstTower = towerData.levels[0];
			buttonText.text = m_Tower.PurchaseCost.ToString();
								//	towerIcon.sprite = firstTower.levelData.icon;
								//}
								//else
								//{
								//	Debug.LogWarning("[Tower Spawn Button] No level data for tower");
								//}

			m_Currency = Game._instance.Currency;
			m_Currency.CurrencyChanged += UpdateButton;
			UpdateButton(null, null);
		}

		/// <summary>
		/// Cache the rect transform
		/// </summary>
		protected virtual void Awake()
		{
			m_RectTransform = (RectTransform) transform;
		}

		/// <summary>
		/// Unsubscribe from events
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (m_Currency != null)
			{
				m_Currency.CurrencyChanged -= UpdateButton;
			}
		}

		/// <summary>
		/// The click for when the button is tapped
		/// </summary>
		public void OnBeginDrag(PointerEventData eventData)
		{
			Debug.LogWarning("OnBeginDrag");
			_draggedTower = _factory.Get(m_Tower.TowerType);
			_draggedTower.transform.localPosition = Input.mousePosition;

		}

		/// <summary>
		/// Update the button's button state based on cost
		/// </summary>
		void UpdateButton(object sender, EventArgs e)
		{
			if (m_Currency == null)
			{
				return;
			}

			// Enable button
			if (m_Currency.CanAfford(m_Tower.PurchaseCost) && !buyButton.interactable)
			{
				buyButton.interactable = true;
				energyIcon.color = energyDefaultColor;
			}
			else if (!m_Currency.CanAfford(m_Tower.PurchaseCost) && buyButton.interactable)
			{
				buyButton.interactable = false;
				energyIcon.color = energyInvalidColor;
			}
		}

        public void OnEndDrag(PointerEventData eventData)
        {
			Debug.Log("OnEndDrag");
			_factory.Reclaim(_draggedTower);
			_draggedTower = null;
        }
    }
}