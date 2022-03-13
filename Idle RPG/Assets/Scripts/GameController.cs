using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameController : MonoBehaviour
{
    public double coins;
    public double power;
    public double playerHealth;
    public double maxPlayerHealth;
    public double enemyHealth;
    public double enemyPower;

    public float kills;
    public float killsMax;

    public int stage;
    public int stageMax;
    public int killsTotal;
    public int bossMultiplier;

    public Text coinsText;
    public Text powerText;
    public Text stageText;
    public Text healthText;
    public Text killsTotalText;
    public Text enemyHealthText;
    public Text enemyPowerText;

    public Image healthBar;
    public Image stageBar;

    public Animator coinExplode;

    // Offline
    public DateTime currentDate;
    public DateTime oldTime;
    public int offlineProgressCheck = 1;
    public float idleTime;
    public Text offlineTimeText;
    public float saveTime;
    public GameObject offlineBox;

    // Start is called before the first frame update
    void Start()
    {
        offlineBox.gameObject.SetActive(false);
        Load();
        killsMax = 10;
        maxPlayerHealth = 100;
        bossMultiplier = 1;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        coinsText.text = coins.ToString("F0") + " Coins";
        powerText.text = power.ToString("F0") + " Power";
        stageText.text = "Stage " + stage.ToString("F0");
        healthText.text = (playerHealth / maxPlayerHealth * 100) + "%";
        killsTotalText.text = killsTotal.ToString("F0") + " Kills";
        enemyHealthText.text = enemyHealth.ToString("F0") + " HP";
        enemyPowerText.text = enemyPower.ToString("F0") + " Power";

        // Update the player health bar
        healthBar.fillAmount = (float)(playerHealth / maxPlayerHealth);
        stageBar.fillAmount = kills / killsMax;

        // Save the game every 5s
        saveTime += Time.deltaTime;
        if (saveTime >= 5)
        {
            saveTime = 0;
            Save();
        }
    }

    public void StartGame()
    {
        // Hit the enemy automatically after 1s, every 0.3s
        InvokeRepeating("Hit", 1.0f, 0.5f);
        Debug.Log("Starting Game");
    }

    public void Save()
    {
        PlayerPrefs.SetString("coins", coins.ToString());
        PlayerPrefs.SetString("power", power.ToString());
        PlayerPrefs.SetString("playerHealth", playerHealth.ToString());
        PlayerPrefs.SetString("enemyHealth", enemyHealth.ToString());
        PlayerPrefs.SetString("enemyPower", enemyPower.ToString());
        PlayerPrefs.SetString("kills", kills.ToString());
        PlayerPrefs.SetInt("killsTotal", killsTotal);
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);

        PlayerPrefs.SetString("offlineTime", DateTime.Now.ToBinary().ToString());

        // offlineProgressCheck = 1;
    }

    public void Load()
    {
        coins = double.Parse(PlayerPrefs.GetString("coins", "0"));
        power = double.Parse(PlayerPrefs.GetString("power", "100"));
        playerHealth = double.Parse(PlayerPrefs.GetString("playerHealth", "100"));
        enemyHealth = double.Parse(PlayerPrefs.GetString("enemyHealth", "10"));
        enemyPower = double.Parse(PlayerPrefs.GetString("enemyPower", "1"));
        kills = float.Parse(PlayerPrefs.GetString("kills", "0"));
        killsTotal = PlayerPrefs.GetInt("killsTotal", 0);
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        offlineProgressCheck = PlayerPrefs.GetInt("offlineProgressCheck", 0);

        LoadOfflineProduction();
    }

    public void LoadOfflineProduction()
    {
        if (offlineProgressCheck == 1)
        {
            offlineBox.gameObject.SetActive(true);

            long previousTime = Convert.ToInt64(PlayerPrefs.GetString("offlineTime"));
            oldTime = DateTime.FromBinary(previousTime);
            currentDate = DateTime.Now;
            TimeSpan difference = currentDate.Subtract(oldTime);
            idleTime = (float)difference.TotalSeconds;

            // Offline formula
            // Get max stage enemy health
            double temp = 10 * System.Math.Pow(2, stageMax);
            var coinsToEarn = System.Math.Ceiling(temp / 14) * idleTime;
            coins += coinsToEarn;

            TimeSpan timer = TimeSpan.FromSeconds(idleTime);

            offlineTimeText.text = "You were gone for: \n" + timer.ToString(@"hh\:mm\:ss")
            + "\n\nYou earned: \n" + coinsToEarn.ToString("F0") + " coins";
        }
    }

    public void CloseOfflineBox()
    {
        offlineBox.gameObject.SetActive(false);
    }

    public void isBossChecker()
    {
        // Boss health multiplier
        if (stage % 5 == 0)
        {
            bossMultiplier = 10;
            stageText.text = "(BOSS) Stage - " + stage;
        } else 
        {
            bossMultiplier = 1;
            stageText.text = "Stage - " + stage;
        }
    }

    public void Hit()
    {
        // If player can kill enemy and how effectively
        if (power > enemyHealth * 100)            // 100x power
        {
            // increase game time by 2x
            // no hp loss
        }
        else if (power > enemyHealth * 10)       // 10x power
        {
            playerHealth -= enemyPower / 2;
        }
        else if (power >= enemyHealth)          // power > enemyHealth
        {
            playerHealth -= enemyPower;
        }
        else                                    // Not enough power
        {
            // kill player and reset to stage 1
            Debug.Log("Not enough power -- Power: " + power + " | Enemy HP: " + enemyHealth);
            playerHealth = 0;
            CancelInvoke("Hit");
            StartCoroutine(PlayerDied());
            return;
        }

        coins += System.Math.Ceiling(enemyHealth / 14);
        kills += 1;
        killsTotal += 1;

        //! Coin animation Doesn't work right
        // coinExplode.Play("coinExplode", 0, 0);

        // If you die, don't proceed
        if (playerHealth <= 0)
        {
            Debug.Log("Player ran out of health");
            playerHealth = 0;
            CancelInvoke("Hit");
            StartCoroutine(PlayerDied());
            return;
        }

        if (kills >= killsMax)
        {
            kills = 0;
            stage += 1;

            if (stage > stageMax)
                stageMax = stage;
        }

        isBossChecker();

        // Increase health and power depending on stage
        enemyHealth = 10 * System.Math.Pow(2, stage-1) * bossMultiplier;
        enemyPower = 1 * System.Math.Pow(2, stage-1) * bossMultiplier;
    }

    IEnumerator PlayerDied()
    {
        Debug.Log("The player has died....");
        Debug.Log("Restarting Game. Wait 3 seconds.");
        yield return new WaitForSeconds(3.0f);

        stage = 1;
        kills = 0;
        killsMax = 10;
        playerHealth = 100;
        enemyHealth = 10;
        enemyPower = 1;
        bossMultiplier = 1;

        StartGame();

        
    }
}
