using TowerDefence3d.Scripts.Enemies;
using UnityEngine;
using UnityEngine.UI;
using static TowerDefence3d.Scripts.Enemies.Enemy;

public class HealthVizual : MonoBehaviour
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
        if (healthRemained.HealthRemainedInProcent < 0.5)
        {
            _image.color = Color.red;
        }
    }
}
