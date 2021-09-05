using System;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefence3d.Scripts.UIObjects
{
	/// <summary>
	/// UI to display the game over screen
	/// </summary>
	public class EndGameScreen : MonoBehaviour
	{
		/// <summary>
		/// AudioClip to play when victorious
		/// </summary>
		public AudioClip victorySound;

		/// <summary>
		/// AudioClip to play when failed
		/// </summary>
		public AudioClip defeatSound;

		/// <summary>
		/// AudioSource that plays the sound
		/// </summary>
		public AudioSource audioSource;

		/// <summary>
		/// The containing panel of the End Game UI
		/// </summary>
		public Canvas endGameCanvas;

		/// <summary>
		/// Reference to the Text object that displays the result message
		/// </summary>
		public Text endGameMessageText;

		/// <summary>
		/// Panel that shows final star rating
		/// </summary>
		public ScorePanel scorePanel;

		/// <summary>
		/// Name of level select screen
		/// </summary>
		public string menuSceneName = "MainMenu";

		/// <summary>
		/// Text to be displayed on popup
		/// </summary>
		public string levelCompleteText = "{0} COMPLETE!";
		
		public string levelFailedText = "{0} FAILED!";

		/// <summary>
		/// Background image
		/// </summary>
		public Image background;

		/// <summary>
		/// Color to set background
		/// </summary>
		public Color winBackgroundColor;
		
		public Color loseBackgroundColor;

		/// <summary>
		/// The Canvas that holds the button to go to the next level
		/// if the player has beaten the level
		/// </summary>
		public Canvas nextLevelButton;

		/// <summary>
		/// Reference to the <see cref="LevelManager" />
		/// </summary>
		private Game _gameInstance;

		/// <summary>
		/// Safely unsubscribes from <see cref="LevelManager" /> events.
		/// Go back to the main menu scene
		/// </summary>
		public void GoToMainMenu()
		{
			SceneManager.LoadScene(menuSceneName);
			SafelyUnsubscribe();
		}

		/// <summary>
		/// Safely unsubscribes from <see cref="LevelManager" /> events.
		/// Reloads the active scene
		/// </summary>
		public void RestartLevel()
		{
			Game._instance.BeginNewGame();
			SafelyUnsubscribe();
			Start();
		}

		/// <summary>
		/// Safely unsubscribes from <see cref="LevelManager" /> events.
		/// Goes to the next scene if valid
		/// </summary>
		public void GoToNextLevel()
		{
			GoToMainMenu();
		}

		/// <summary>
		/// Hide the panel if it is active at the start.
		/// Subscribe to the <see cref="LevelManager" /> completed/failed events.
		/// </summary>
		protected void Start()
		{
			LazyLoad();
			endGameCanvas.enabled = false;
			nextLevelButton.enabled = false;
			nextLevelButton.gameObject.SetActive(false);

			Game._instance.LevelComplete += Victory;
			Game._instance.LevelDefeat += Defeat;
		}

		/// <summary>
		/// Shows the end game screen
		/// </summary>
		protected void OpenEndGameScreen(string endResultText)
		{
			endGameCanvas.enabled = true;

			int score = CalculateFinalScore();
			scorePanel.SetStars(score);
		}

		/// <summary>
		/// Occurs when the level is sucessfully completed
		/// </summary>
		protected void Victory(object sender, EventArgs e)
		{
			OpenEndGameScreen(levelCompleteText);
			if ((victorySound != null) && (audioSource != null))
			{
				audioSource.PlayOneShot(victorySound);
			}
			background.color = winBackgroundColor;
			nextLevelButton.enabled = true;
			nextLevelButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// Occurs when level is failed
		/// </summary>
		protected void Defeat(object sender, EventArgs e)
		{
			OpenEndGameScreen(levelFailedText);
			if (nextLevelButton != null)
			{
				nextLevelButton.enabled = false;
				nextLevelButton.gameObject.SetActive(false);
			}
			if ((defeatSound != null) && (audioSource != null))
			{
				audioSource.PlayOneShot(defeatSound);
			}
			background.color = loseBackgroundColor;
		}

		/// <summary>
		/// Safely unsubscribes from <see cref="LevelManager" /> events.
		/// </summary>
		protected void OnDestroy()
		{
			SafelyUnsubscribe();
		}

		/// <summary>
		/// Ensure that <see cref="LevelManager" /> events are unsubscribed from when necessary
		/// </summary>
		protected void SafelyUnsubscribe()
		{
			LazyLoad();
			Game._instance.LevelComplete -= Victory;
			Game._instance.LevelDefeat -= Defeat;
		}

		/// <summary>
		/// Ensure <see cref="m_LevelManager" /> is not null
		/// </summary>
		protected void LazyLoad()
		{
		}

		/// <summary>
		/// Add up the health of all the Home Bases and return a score
		/// </summary>
		/// <returns>Final score</returns>
		protected int CalculateFinalScore()
		{
			int totalRemainingHealth = Game.CurrentHealth;
			int totalBaseHealth = 10;
			int score = CalculateScore(totalRemainingHealth, totalBaseHealth);
			return score;
		}

		/// <summary>
		/// Take the final remaining health of all bases and rates them
		/// </summary>
		/// <param name="remainingHealth">the total remaining health of all home bases</param>
		/// <param name="maxHealth">the total maximum health of all home bases</param>
		/// <returns>0 to 3 depending on how much health is remaining</returns>
		protected int CalculateScore(float remainingHealth, float maxHealth)
		{
			float normalizedHealth = remainingHealth / maxHealth;
			if (Mathf.Approximately(normalizedHealth, 1f))
			{
				return 3;
			}
			if ((normalizedHealth <= 0.9f) && (normalizedHealth >= 0.5f))
			{
				return 2;
			}
			if ((normalizedHealth < 0.5f) && (normalizedHealth > 0f))
			{
				return 1;
			}
			return 0;
		}
	}
}