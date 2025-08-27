using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject player;

    public Text ammoText;
    public Text scoreText;
    public Text enemyWaveText;

    public GameObject gameOverUi;

    private void Start()
    {
        player = GameObject.FindWithTag(Tag.Player);
        var gun = player.GetComponent<PlayerShooter>().gun;
        gun.AmmoCountChanged += UpdateAmmoText;
        UpdateAmmoText(gun.magAmmo, gun.ammoRemain);

        player.GetComponent<PlayerHealth>().DieEvent += GameOver;        
    }

    public void GameReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateAmmoText(int magAmmo, int ammoRemain)
    {
        ammoText.text = $"{magAmmo}/{ammoRemain}";
    }

    public void GameOver()
    {
        gameOverUi.SetActive(true);
    }
}
