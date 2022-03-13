using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    public double coins;
    public double power;
    public double defense;
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
    public Text defenseText;
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

    // Upgrades
    public double powerUpgradeCost;
    public double defenseUpgradeCost;
    public double goldUpgradeCost;
    public double trainingUpgradeCost;

    public int powerUpgradeLevel;
    public int defenseUpgradeLevel;
    public int goldUpgradeLevel;
    public int trainingUpgradeLevel;

    public double powerUpgradePower;
    public double defenseUpgradePower;
    public double goldUpgradePower;
    public double trainingUpgradePower;

    public Text powerUpgradeCostText;
    public Text defenseUpgradeCostText;
    public Text goldUpgradeCostText;
    public Text trainingUpgradeCostText;
    public Text powerUpgradeLevelText;
    public Text defenseUpgradeLevelText;
    public Text goldUpgradeLevelText;
    public Text trainingUpgradeLevelText;
    public Text powerUpgradePowerText;
    public Text defenseUpgradePowerText;
    public Text goldUpgradePowerText;
    public Text trainingUpgradePowerText;

    public Button powerUpgradeButton;
    public Button defenseUpgradeButton;
    public Button goldUpgradeButton;
    public Button trainingUpgradeButton;


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
        // Update Text
        coinsText.text = Formatter(coins, "F2") + " Coins";
        powerText.text = Formatter(((power + powerUpgradePower) * trainingUpgradePower), "F2") + " Pow";
        defenseText.text = Formatter((defense + defenseUpgradePower) * trainingUpgradePower, "F2") + " Def";
        stageText.text = "Stage " + stage.ToString("F0");
        healthText.text = (playerHealth / maxPlayerHealth * 100).ToString("F0") + "%";
        killsTotalText.text = killsTotal.ToString("F0") + " Kills";
        enemyHealthText.text = enemyHealth.ToString("F0") + " HP";
        enemyPowerText.text = enemyPower.ToString("F0") + " Power";

        // Update the player health bar and stage bar
        healthBar.fillAmount = (float)(playerHealth / maxPlayerHealth);
        stageBar.fillAmount = kills / killsMax;

        // Save the game every 5s
        saveTime += Time.deltaTime;
        if (saveTime >= 5)
        {
            saveTime = 0;
            Save();
        }

        // Upgrade Updates
        UpgradesUpdate();

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
        PlayerPrefs.SetString("defense", defense.ToString());
        PlayerPrefs.SetString("playerHealth", playerHealth.ToString());
        PlayerPrefs.SetString("enemyHealth", enemyHealth.ToString());
        PlayerPrefs.SetString("enemyPower", enemyPower.ToString());
        PlayerPrefs.SetString("kills", kills.ToString());
        PlayerPrefs.SetString("powerUpgradeCost", powerUpgradeCost.ToString());
        PlayerPrefs.SetString("defenseUpgradeCost", defenseUpgradeCost.ToString());
        PlayerPrefs.SetString("goldUpgradeCost", goldUpgradeCost.ToString());
        PlayerPrefs.SetString("trainingUpgradeCost", trainingUpgradeCost.ToString());
        PlayerPrefs.SetInt("killsTotal", killsTotal);
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("powerUpgradeLevel", powerUpgradeLevel);
        PlayerPrefs.SetInt("defenseUpgradeLevel", defenseUpgradeLevel);
        PlayerPrefs.SetInt("goldUpgradeLevel", goldUpgradeLevel);
        PlayerPrefs.SetInt("trainingUpgradeLevel", trainingUpgradeLevel);
        PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);

        PlayerPrefs.SetString("offlineTime", DateTime.Now.ToBinary().ToString());

        offlineProgressCheck = 1;
    }

    public void Load()
    {
        coins = double.Parse(PlayerPrefs.GetString("coins", "0"));
        power = double.Parse(PlayerPrefs.GetString("power", "100"));
        defense = double.Parse(PlayerPrefs.GetString("defense", "10"));
        playerHealth = double.Parse(PlayerPrefs.GetString("playerHealth", "100"));
        enemyHealth = double.Parse(PlayerPrefs.GetString("enemyHealth", "10"));
        enemyPower = double.Parse(PlayerPrefs.GetString("enemyPower", "1"));
        powerUpgradeCost = double.Parse(PlayerPrefs.GetString("powerUpgradeCost", "10"));
        defenseUpgradeCost = double.Parse(PlayerPrefs.GetString("defenseUpgradeCost", "10"));
        goldUpgradeCost = double.Parse(PlayerPrefs.GetString("goldUpgradeCost", "10"));
        trainingUpgradeCost = double.Parse(PlayerPrefs.GetString("trainingUpgradeCost", "10"));
        kills = float.Parse(PlayerPrefs.GetString("kills", "0"));
        killsTotal = PlayerPrefs.GetInt("killsTotal", 0);
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        powerUpgradeLevel = PlayerPrefs.GetInt("powerUpgradeLevel", 1);
        defenseUpgradeLevel = PlayerPrefs.GetInt("defenseUpgradeLevel", 1);
        goldUpgradeLevel = PlayerPrefs.GetInt("goldUpgradeLevel", 1);
        trainingUpgradeLevel = PlayerPrefs.GetInt("trainingUpgradeLevel", 1);
        offlineProgressCheck = PlayerPrefs.GetInt("offlineProgressCheck", 0);

        LoadOfflineProduction();
    }

    // Number Formatter
    public string Formatter(double number, string digits, string type = "letter")
    {
        if (type == "letter")
        {
            double digitsTemp = Math.Floor(Math.Log10(number));
            IDictionary<double, string> prefixes = new Dictionary<double, string>()
            {
                {3, "K"},
                {6, "M"},
                {9, "B"},
                {12, "T"},
                {15, "Qu"},
                {18, "Qt"},
                {21, "Se"},
                {24, "Sep"}
            };

            double digitsEvery3 = 3 * Math.Floor(digitsTemp / 3);
            if (number >= 1000)
                return (number / Math.Pow(10, digitsEvery3)).ToString(digits) + prefixes[digitsEvery3];
            return number.ToString("F0");
        }
        return "Failed to format";
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
            var coinsToEarn = System.Math.Ceiling(temp / 14) * idleTime * (goldUpgradeLevel * 1.2);
            coins += coinsToEarn;

            TimeSpan timer = TimeSpan.FromSeconds(idleTime);

            offlineTimeText.text = "You were gone for: \n" + timer.ToString(@"hh\:mm\:ss")
            + "\n\nYou earned: \n" + Formatter(coinsToEarn, "F0") + " coins";
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
            bossMultiplier = 2;
            killsMax = 1;
        } else 
        {
            bossMultiplier = 1;
            killsMax = 10;
        }
    }

    public void Hit()
    {
        double fullPower = (power + powerUpgradePower) * trainingUpgradePower;
        double fullDefense = (defense + defenseUpgradePower) * trainingUpgradePower;

        if (!PowerCheck(fullPower))
            return;
        
        if (!DefenseCheck(fullDefense))
            return;



        coins += System.Math.Ceiling(enemyHealth / 10) * goldUpgradePower;
        kills += 1;
        killsTotal += 1;

        //! Coin animation Doesn't work right
        // coinExplode.Play("coinExplode", 0, 0);

        if (kills >= killsMax)
        {
            kills = 0;
            stage += 1;

            if (stage > stageMax)
                stageMax = stage;
        }

        isBossChecker();

        // Increase health and power depending on stage
        enemyHealth = 10 * System.Math.Pow(1.5, stage-1) * bossMultiplier;
        enemyPower = 1 * System.Math.Pow(2, stage-1) * bossMultiplier;
    }

    // If player can kill enemy and how effectively
    public bool PowerCheck(double fullPower)
    {
        if (fullPower > enemyHealth * 100)            // 100x power
        {
            // increase game time by 2x
        }
        else if (fullPower > enemyHealth * 10)       // 10x power
        {
            // increase game time by 1.5x
        }
        else if (fullPower >= enemyHealth)          // power > enemyHealth
        {
            // Set game time to 1.0x
        }
        else                                    // Not enough power
        {
            // kill player and reset to stage 1
            Debug.Log("Not enough power -- Power: " + power + " | Enemy HP: " + enemyHealth);
            playerHealth = 0;
            CancelInvoke("Hit");
            StartCoroutine(PlayerDied());
            return false;
        }

        return true;
    }

    // Check if the player takes damage and how much
    public bool DefenseCheck(double fullDefense)
    {
        if (fullDefense > enemyPower * 20)            // 20x def - Take no damage
        {
            // Take no damage
        }
        else if (fullDefense > enemyPower * 5)       // 5x defense - Take 0-3 dmg
        {
            // playerHealth -= enemyPower / 2;
            playerHealth -= UnityEngine.Random.Range(0, 4);
            Debug.Log("small hit");
        }
        else if (fullDefense >= enemyPower)          // defense > enemyPower - Take 3-7 dmg
        {
            playerHealth -= UnityEngine.Random.Range(3, 8);
            Debug.Log("med hit");
        }
        else if (fullDefense >= enemyPower / 2)     // defense > enemyPower / 2 - Take 8-15 dmg
        {
            playerHealth -= UnityEngine.Random.Range(8, 16);
            Debug.Log("big hit");
        }
        else                                    // Not enough defense
        {
            // kill player and reset to stage 1
            Debug.Log("Not enough defense -- Def: " + defense + " | Enemy Power: " + enemyPower);
            playerHealth = 0;
        }

        if (playerHealth >= 0)
        {
            return true;
        } else
        {
            Debug.Log("player died...rut roh");
            playerHealth = 0;
            CancelInvoke("Hit");
            StartCoroutine(PlayerDied());
            return false;
        }
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

        public void BuyUpgrade(string id)
    {
        switch (id)
        {
            case "powerUpgrade1":
                if (coins >= powerUpgradeCost)
                {
                    coins -= powerUpgradeCost;
                    powerUpgradeLevel++;
                    powerUpgradeCost = System.Math.Ceiling(powerUpgradeCost * System.Math.Pow(1.07, powerUpgradeLevel));
                }
                break;
            case "defenseUpgrade1":
                if (coins >= defenseUpgradeCost)
                {
                    coins -= defenseUpgradeCost;
                    defenseUpgradeLevel++;
                    defenseUpgradeCost = System.Math.Ceiling(defenseUpgradeCost * System.Math.Pow(1.09, defenseUpgradeLevel));
                }
                break;
            case "goldUpgrade1":
                if (coins >= goldUpgradeCost)
                {
                    coins -= goldUpgradeCost;
                    goldUpgradeLevel++;
                    goldUpgradeCost = System.Math.Ceiling(goldUpgradeCost * System.Math.Pow(1.15, goldUpgradeLevel));
                }
                break;
            case "trainingUpgrade1":
                if (stageMax >= trainingUpgradeCost)
                {
                    trainingUpgradeLevel++;
                    trainingUpgradeCost += 10;
                }
                break;
            default:
                Debug.Log("Upgrade doesn't exist.");
                break;
        }
    }

    public void UpgradesUpdate()
    {
        // Text
        powerUpgradeCostText.text = Formatter(powerUpgradeCost, "F2") + " coins";
        powerUpgradeLevelText.text = "Level : " + powerUpgradeLevel.ToString("F0");
        powerUpgradePowerText.text = "+" + Formatter(powerUpgradePower, "F2") + " Pow";

        defenseUpgradeCostText.text = Formatter(defenseUpgradeCost, "F2") + " coins";
        defenseUpgradeLevelText.text = "Level : " + defenseUpgradeLevel.ToString("F0");
        defenseUpgradePowerText.text = "+" + Formatter(defenseUpgradePower, "F2") + " Def";

        goldUpgradeCostText.text = Formatter(goldUpgradeCost, "F2") + " coins";
        goldUpgradeLevelText.text = "Level : " + goldUpgradeLevel.ToString("F0");
        goldUpgradePowerText.text = "x" + goldUpgradePower.ToString("F2") + " Coins";

        trainingUpgradeCostText.text = trainingUpgradeCost.ToString("F0") + " Stages";
        trainingUpgradeLevelText.text = "Level : " + trainingUpgradeLevel.ToString("F0");
        trainingUpgradePowerText.text = "x" + trainingUpgradePower.ToString("F2") + " Pow + Def";

        // Buttons
        if (coins >= powerUpgradeCost)
            powerUpgradeButton.interactable = true;
        else
            powerUpgradeButton.interactable = false;
        
        if (coins >= defenseUpgradeCost)
            defenseUpgradeButton.interactable = true;
        else
            defenseUpgradeButton.interactable = false;

        if (coins >= goldUpgradeCost)
            goldUpgradeButton.interactable = true;
        else
            goldUpgradeButton.interactable = false;

        if (stageMax >= trainingUpgradeCost)
            trainingUpgradeButton.interactable = true;
        else
            trainingUpgradeButton.interactable = false;


        // Power
        if (powerUpgradeLevel <= 1)
            powerUpgradePower = 0;
        else
            powerUpgradePower = (powerUpgradeLevel-1) * 80;

        if (defenseUpgradeLevel <= 1)
            defenseUpgradePower = 0;
        else
            defenseUpgradePower = (defenseUpgradeLevel-1) * 160;

        if (goldUpgradeLevel <= 1)
            goldUpgradePower = 1.00;
        else
            goldUpgradePower = (goldUpgradeLevel-1) * 1.2;

        if (trainingUpgradeLevel <= 1)
            trainingUpgradePower = 1.00;
        else
            trainingUpgradePower = (trainingUpgradeLevel-1) * 2;


    }
}
