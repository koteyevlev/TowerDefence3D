using Core.UI;
using System.Collections.Generic;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// Simple options menu for setting volumes 
	/// </summary>
	public class OptionsMenu : SimpleMainMenuPage
	{
		public Slider masterSlider;

		public Slider sfxSlider;

		public Slider musicSlider;

		[SerializeField]
		private Slider _levelDifficultSlider;

		[SerializeField]
		private Text _difficultLabel;

		private readonly Dictionary<int, string> _difficultNames = 
			new Dictionary<int, string>
			{
				{ 1, "SUPEREASY"},
				{ 2, "EASY STREET"},
				{ 3, "NORMAL"},
				{ 4, "MIDDLE ROAD"},
				{ 5, "HARD WAY"},
				{ 6, "SUPERHARD"},
			};


		/// <summary>
		/// Event fired when sliders change
		/// </summary>
		public void UpdateVolumes()
		{
			float masterVolume, sfxVolume, musicVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, false);
			}
		}

		public void UpdateDifficult()
		{
			var difficult = _levelDifficultSlider != null ? _levelDifficultSlider.value : 1;

			_difficultNames.TryGetValue((int)difficult, out var label) ;
			_difficultLabel.text = label;
			if (GameManager.instanceExists)
			{
				GameManager.instance.SetDifficults(difficult / _levelDifficultSlider.maxValue * 2);
			}
		}

		/// <summary>
		/// Set initial slider values
		/// </summary>
		public override void Show()
		{
			if (GameManager.instanceExists)
			{
				float master, sfx, music;
				GameManager.instance.GetVolumes(out master, out sfx, out music);

				if (masterSlider != null)
				{
					masterSlider.value = master;
				}
				if (sfxSlider != null)
				{
					sfxSlider.value = sfx;
				}
				if (musicSlider != null)
				{
					musicSlider.value = music;
				}
			}

			base.Show();
		}

		/// <summary>
		/// Persist volumes to data store
		/// </summary>
		public override void Hide()
		{
			float masterVolume, sfxVolume, musicVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, true);
			}

			base.Hide();
		}

		/// <summary>
		/// Retrieve values from sliders
		/// </summary>
		void GetSliderVolumes(out float masterVolume, out float sfxVolume, out float musicVolume)
		{
			masterVolume = masterSlider != null ? masterSlider.value : 1;
			sfxVolume = sfxSlider != null ? sfxSlider.value : 1;
			musicVolume = musicSlider != null ? musicSlider.value : 1;
		}
	}
}