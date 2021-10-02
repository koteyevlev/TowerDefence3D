using System;
using TowerDefence3d.Scripts.MapObject;
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
        private Camera _camera;

		[SerializeField]
		private GameTileContentFactory _factory;

        [SerializeField]
        private GameBoard _board;

		private Image towerIcon;
        private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);

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
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(TouchRay, out var position))
            {
                _draggedTower.transform.position = TouchRay.GetPoint(position);
            }
			//_draggedTower.transform.localPosition = Input.mousePosition;
			//if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
			//{
			//}
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

			//var offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
			//Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
			//Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

			var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(TouchRay, out var position))
            {
                _draggedTower.transform.position = TouchRay.GetPoint(position);
            }
            //new Vector3(-Input.mousePosition.x, -Input.mousePosition.y, 0);  // Input.mousePosition;

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
            GameTile tile = _board.GetTile(TouchRay);
            if (tile != null)
            {
                _board.ToggleTower(tile, _draggedTower.TowerType);
            }

            _factory.Reclaim(_draggedTower);
			_draggedTower = null;

        }
    }
}