using System;
using Core.Game;
using TowerDefence3d.Scripts.Setup;
using TowerDefense.Game;
using TowerDefense.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Threading.Tasks;

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
		[SerializeField]
		private AudioClip _victorySound;

		/// <summary>
		/// AudioClip to play when failed
		/// </summary>
        [SerializeField]
        private AudioClip _defeatSound;

		/// <summary>
		/// AudioSource that plays the sound
		/// </summary>
        [SerializeField]
		private AudioSource _audioSource;

		/// <summary>
		/// The containing panel of the End Game UI
		/// </summary>
        [SerializeField]
        private Canvas _endGameCanvas;

		/// <summary>
		/// Reference to the Text object that displays the result message
		/// </summary>
        [SerializeField]
        private Text _endGameMessageText;

		/// <summary>
		/// Panel that shows final star rating
		/// </summary>
        [SerializeField]
		private ScorePanel _scorePanel;

		/// <summary>
		/// Name of level select screen
		/// </summary>
        private const string _menuSceneName = "MainMenu";

        [SerializeField]
        private string _onRestartAdUnitName;

        [SerializeField]
        private string _onMenuAdUnitName;

		/// <summary>
		/// Text to be displayed on popup
		/// </summary>
		private const string _levelCompleteText = "LEVEL COMPLETE!";

        private const string _levelFailedText = "LEVEL FAILED!";

        [SerializeField] 
        private int _probabilityOfAds = 100;

		/// <summary>
		/// Background image
		/// </summary>
		[SerializeField]
		private Image _background;

		/// <summary>
		/// Color to set _background
		/// </summary>
        [SerializeField]
		private Color _winBackgroundColor;

        [SerializeField]
		private Color _loseBackgroundColor;

		/// <summary>
		/// The Canvas that holds the button to go to the next level
		/// if the player has beaten the level
		/// </summary>
        [SerializeField]
		private Canvas _nextLevelButton;

		/// <summary>
		/// Safely unsubscribes from <see cref="LevelManager" /> events.
		/// Go back to the main menu scene
		/// </summary>
		public void GoToMainMenu()
		{
			Debug.Log("Main menu");

			if (Advertisement.IsReady() &&
                GetRandomProbability())
            {
				LoadAd(_onMenuAdUnitName);
			}
			LoadMenu();

		}

		private void LoadMenu()
        {
			Time.timeScale = 1f;
			SceneManager.LoadScene(_menuSceneName);
			SafelyUnsubscribe();
		}

        private void LoadAd(string adName, Action<ShowResult> adCallBackAction = null)
        {
			var options = new ShowOptions();
			if (adCallBackAction == null)
			{
				options.resultCallback = DefaultAdCallBackHandler;
			}
			else
			{
				options.resultCallback = adCallBackAction;
			}

			Debug.Log("Load ads " + adName);
			Advertisement.Show(adName);
		}

        private bool GetRandomProbability()
        {
            var gen = new System.Random();
            int prob = gen.Next(100);
			Debug.Log(prob.ToString() + (prob <= _probabilityOfAds).ToString());
            return prob <= _probabilityOfAds;
		}

        /// <summary>
        /// Safely unsubscribes from <see cref="LevelManager" /> events.
        /// Reloads the active scene
        /// </summary>
        public void RestartLevel()
		{
            if (Advertisement.IsReady()
            && GetRandomProbability())
            {
				LoadAd(_onRestartAdUnitName);
            }

			Game._instance.StopGame();
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
			Debug.Log("Unity Ads initialized: " + Advertisement.isInitialized);
			Debug.Log("Unity Ads is supported: " + Advertisement.isSupported);
			Debug.Log("Unity Ads test mode enabled: " + Advertisement.debugMode);

			Advertisement.Load(_onRestartAdUnitName);
			Advertisement.Load(_onMenuAdUnitName);

			LazyLoad();
			_endGameCanvas.enabled = false;
			_nextLevelButton.enabled = false;
			_nextLevelButton.gameObject.SetActive(false);
            Time.timeScale = 1f;

			Game._instance.LevelComplete += Victory;
			Game._instance.LevelDefeat += Defeat;
		}

		/// <summary>
		/// Shows the end game screen
		/// </summary>
		protected void OpenEndGameScreen(string endResultText, bool isWin)
		{
			if (_endGameCanvas.enabled)
                return;
            
            _endGameCanvas.enabled = true;

			int score = CalculateFinalScore();
			_scorePanel.SetStars(score);
            
			if (isWin)
            {
                _endGameMessageText.text = _levelCompleteText;
				LevelItem level = GameManager.instance.GetLevelForCurrentScene();
                GameManager.instance.CompleteLevel(level.id, score);
			}
            else
            {
                _endGameMessageText.text = _levelFailedText;
			}
		}

		/// <summary>
		/// Occurs when the level is sucessfully completed
		/// </summary>
		protected void Victory(object sender, EventArgs e)
		{
            OpenEndGameScreen(_levelCompleteText, true);
			if ((_victorySound != null) && (_audioSource != null))
			{
				_audioSource.PlayOneShot(_victorySound);
			}
			_background.color = _winBackgroundColor;
			_nextLevelButton.enabled = true;
			_nextLevelButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// Occurs when level is failed
		/// </summary>
		protected void Defeat(object sender, EventArgs e)
		{
			OpenEndGameScreen(_levelFailedText, false);
			if (_nextLevelButton != null)
			{
				_nextLevelButton.enabled = false;
				_nextLevelButton.gameObject.SetActive(false);
			}
			if ((_defeatSound != null) && (_audioSource != null))
			{
				_audioSource.PlayOneShot(_defeatSound);
			}
			_background.color = _loseBackgroundColor;
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

		private void DefaultAdCallBackHandler(ShowResult result)
		{
			switch (result)
			{
				case ShowResult.Finished:
					Time.timeScale = 1f;
					break;

				case ShowResult.Failed:
					Time.timeScale = 1f;
					break;

				case ShowResult.Skipped:
					Time.timeScale = 1f;
					break;
			}
		}
	}
}