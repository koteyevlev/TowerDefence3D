using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence3d.Scripts.Enemies;
using TowerDefence3d.Scripts.Setup;
using UnityEngine;
using UnityEngine.UI;
using static TowerDefence3d.Scripts.Enemies.Enemy;

public class WaveVizual : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Enemy>().GetDamage += HealthChanged;
    }

    private void HealthChanged(object sender, GetDamageEventArgs healthRemained)
    {
        _image.fillAmount = healthRemained.HealthRemainedInProcent;
    }
}
