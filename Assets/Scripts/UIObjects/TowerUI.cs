using System;
using System.Collections.Generic;
using TowerDefence3d.Scripts.Setup;
using TowerDefence3d.Scripts.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TowerDefence3d.Scripts.Setup.Currency;

namespace TowerDefence3d.Scripts.UIObjects
{
	/// <summary>
	/// Controls the UI objects that draw the tower data
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class TowerUI : MonoBehaviour
	{
		/// <summary>
		/// The text object for the name
		/// </summary>
		public Text towerName;

		/// <summary>
		/// The text object for the description
		/// </summary>
		public Text description;

		[SerializeField]private Text _currentDPS;

		[SerializeField] private Text _upgradeCost;

		[SerializeField] private Text _sellCost;

		public Text upgradeDescription;

		/// <summary>
		/// The attached sell button
		/// </summary>
		public Button sellButton;

		/// <summary>
		/// The attached upgrade button
		/// </summary>
		public Button upgradeButton;

		public RectTransform panelRectTransform;

		public Button confirmationButton;

		/// <summary>
		/// The main game camera
		/// </summary>
		protected Camera m_GameCamera;

		/// <summary>
		/// The current tower to draw
		/// </summary>
		protected Tower m_Tower;

		/// <summary>
		/// The canvas attached to the gameObject
		/// </summary>
		protected Canvas m_Canvas;

		private GraphicRaycaster _raycaster;

		/// <summary>
		/// Draws the tower data on to the canvas
		/// </summary>
		/// <param name="towerToShow">
		/// The tower to gain info from
		/// </param>
		/// 
		private bool _isActive = false;
		public virtual void Show(Tower towerToShow)
		{
			if (towerToShow == null)
			{
				return;
			}
			m_Tower = towerToShow;
			AdjustPosition();

			m_Canvas.enabled = true;

			int sellValue = m_Tower.GetSellLevel();
			if (sellButton != null)
			{
				sellButton.gameObject.SetActive(sellValue > 0);
			}
			if (upgradeButton != null)
			{
				upgradeButton.interactable = 
					Game._instance.Currency.CanAfford(m_Tower.GetCostForNextLevel());
				bool maxLevel = m_Tower.IsAtMaxLevel;
				upgradeButton.gameObject.SetActive(!maxLevel);
				//if (!maxLevel)
				//{
				//	upgradeDescription.text =
				//		m_Tower.levels[m_Tower.CurrentLevel + 1].upgradeDescription.ToUpper();
				//}
			}
			Game._instance.Currency.CurrencyChanged += OnCurrencyChanged;
			// towerInfoDisplay.Show(towerToShow);
			AddRelevantDisplayInfo();
			confirmationButton.gameObject.SetActive(false);
		}

        private void AddRelevantDisplayInfo()
        {
			towerName.text = m_Tower.TitleName;
			description.text = m_Tower.Description;
			upgradeDescription.text = m_Tower.UpgradeDescription;
			_currentDPS.text = m_Tower.CurrentDPS;
			_upgradeCost.text = m_Tower.GetCostForNextLevel().ToString();
			_sellCost.text = m_Tower.GetSellLevel().ToString();
		}

        /// <summary>
        /// Hides the tower info UI and the radius visualizer
        /// </summary>
        public virtual void Hide()
		{
			m_Tower = null;
			_isActive = false;
			m_Canvas.enabled = false;
			Game._instance.Currency.CurrencyChanged -= OnCurrencyChanged;
			Game._instance.CleanSelectedTower();
		}

		/// <summary>
		/// Upgrades the tower through <see cref="GameUI"/>
		/// </summary>
		public void UpgradeButtonClick()
		{
			Game._instance.UpgradeSelectedTower();
			Hide();
		}

		/// <summary>
		/// Sells the tower through <see cref="GameUI"/>
		/// </summary>
		public void SellButtonClick()
		{
			Game._instance.SellSelectedTower();
			Hide();
		}

		/// <summary>
		/// Get the text attached to the buttons
		/// </summary>
		protected virtual void Awake()
		{
			m_Canvas = GetComponent<Canvas>();
			_raycaster = GetComponent<GraphicRaycaster>();
		}

		/// <summary>
		/// Fires when tower is selected/deselected
		/// </summary>
		/// <param name="newTower"></param>
		public virtual void OnUISelectionChanged(Tower newTower)
		{
			if (!_isActive)
			{
				Show(newTower);
				_isActive = true;
			}
			else if (IsClickOnCanvas())
			{
				Debug.Log("Canvas clicked");
			}
			else
			{
				Hide();
				_isActive = false;
			}
        }

        public bool IsClickOnCanvas()
        {
			if (_raycaster == null)
            {
				Debug.Log("No raycaster");
				return false;
            }
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			List<RaycastResult> results = new List<RaycastResult>();

			//Raycast using the Graphics Raycaster and mouse click position
			pointerData.position = Input.mousePosition;
			_raycaster.Raycast(pointerData, results);
			foreach (RaycastResult result in results)
			{
				Debug.Log("Hit " + result.gameObject.name);

				// костыль
				if (result.gameObject == m_Canvas
					|| result.gameObject.name == upgradeButton.name
					|| result.gameObject.name == sellButton.name
					|| result.gameObject.name == description.name
					|| result.gameObject.name == confirmationButton.name)
                {
					return true;
                }
			}

			Debug.Log("Close canvas");
			return false;
		}

		/// <summary>
		/// Subscribe to mouse button action
		/// </summary>
		protected virtual void Start()
		{
			m_GameCamera = Camera.main;
			Debug.Log(Camera.main);
			m_Canvas.enabled = false;
			//if (GameUI.instanceExists)
			//{
			//	GameUI.instance.selectionChanged += OnUISelectionChanged;
			//	GameUI.instance.stateChanged += OnGameUIStateChanged;
			//}
		}

		/// <summary>
		/// Adjust position when the camera moves
		/// </summary>
		protected virtual void Update()
		{
			AdjustPosition();
		}

		/// <summary>
		/// Unsubscribe from currencyChanged
		/// </summary>
		protected virtual void OnDisable()
		{
			//if (LevelManager.instanceExists)
			//{
			//	LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
			//}
		}

		/// <summary>
		/// Adjust the position of the UI
		/// </summary>
		protected void AdjustPosition()
		{
			if (m_Tower == null)
			{
				return;
			}
			Vector3 point = m_GameCamera.WorldToScreenPoint(m_Tower.Position);
			point.z = 0;
			panelRectTransform.transform.position = point;
		}

		/// <summary>
		/// Check if player can afford upgrade on currency changed
		/// </summary>
		void OnCurrencyChanged(object sender, CurrencyEventArgs e)
		{
			if (m_Tower != null && upgradeButton != null)
			{
				upgradeButton.interactable = 
					Game._instance.Currency.CanAfford(m_Tower.GetCostForNextLevel());
			}
		}

		/// <summary>
		/// Unsubscribe from GameUI selectionChanged and stateChanged
		/// </summary>
		void OnDestroy()
		{
		}
	}
}