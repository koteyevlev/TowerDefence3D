using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence3d.Scripts.Enemies;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;
using UnityEngine.UI;

public class WaveVizual : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    [SerializeField]
    private Text _text;

    private int cachedCurrentWave = 0;

    private int cachedTotalWave = 0;

    internal void GameUpdate(GameScenario.State activeScenario)
    {
        _image.fillAmount = activeScenario.WaveProgress;
        if (activeScenario.CurrentWave != cachedCurrentWave)
        {
            cachedCurrentWave = activeScenario.CurrentWave;
            cachedTotalWave = activeScenario.WavesCount.Value;
            _text.text = activeScenario.CurrentWave.ToString() + "/" + activeScenario.WavesCount.ToString();
        }
    }

    public void ResetWaves()
    {
        _image.fillAmount = 0;
        if (cachedCurrentWave != 0)
        {
            cachedCurrentWave = 1;
            _text.text = cachedCurrentWave.ToString() + "/" + cachedTotalWave.ToString();
        }
    }
}