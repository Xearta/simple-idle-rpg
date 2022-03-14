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
    public float bossMultiplier;

    public int stage;
    public int stageMax;

    public Text coinsText;
    public Text powerText;
    public Text defenseText;
    public Text stageText;
    public Text healthText;
    public Text enemyHealthText;
    public Text enemyPowerText;

    // Stats
    public int killsTotal;
    public int totalDeaths;

    public Text killsTotalText;
    public Text totalDeathsText;


    public Image healthBar;
    public Image stageBar;
    public Image worldBar;

    public Animator coinExplode;

    // Menus
    public CanvasGroup upgradesGroup;
    public CanvasGroup statsGroup;
    public CanvasGroup settingsGroup;

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
        // Set FPS to 60 (VSync)
        Application.targetFrameRate = 60;

        // Menus
        MenuChanger(upgradesGroup, true);
        MenuChanger(statsGroup, false);
        MenuChanger(settingsGroup, false);

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
        enemyHealthText.text = Formatter(enemyHealth, "F2") + " HP";
        enemyPowerText.text = Formatter(enemyPower, "F2") + " Pow";

        // Stats
        killsTotalText.text = "Total Kills: " + Formatter(killsTotal, "F2");
        totalDeathsText.text = "Total Deaths: " + Formatter(totalDeaths, "F2");


        // Update the bars
        healthBar.fillAmount = (float)(playerHealth / maxPlayerHealth);
        stageBar.fillAmount = kills / killsMax;
        worldBar.fillAmount = (float)(stage) / 100f;

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

    // Turn menus on and off depending on what is passed in
    public void MenuChanger(CanvasGroup x, bool y)
    {
        if (y)
        {
            x.alpha = 1;
            x.interactable = true;
            x.blocksRaycasts = true;
            return;
        }

        x.alpha = 0;
        x.interactable = false;
        x.blocksRaycasts = false;
    }

    // Allow the user to change tabs depending on the id entered
    public void ChangeTabs(string id)
    {
        switch (id)
        {
            case "upgrades":
                MenuChanger(upgradesGroup, true);
                MenuChanger(statsGroup, false);
                MenuChanger(settingsGroup, false);
                break;
            case "stats":
                MenuChanger(upgradesGroup, false);
                MenuChanger(statsGroup, true);
                MenuChanger(settingsGroup, false);
                break;
            case "settings":
                MenuChanger(upgradesGroup, false);
                MenuChanger(statsGroup, false);
                MenuChanger(settingsGroup, true);
                break;
        }
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
        PlayerPrefs.SetInt("stage", stage);
        PlayerPrefs.SetInt("stageMax", stageMax);
        PlayerPrefs.SetInt("powerUpgradeLevel", powerUpgradeLevel);
        PlayerPrefs.SetInt("defenseUpgradeLevel", defenseUpgradeLevel);
        PlayerPrefs.SetInt("goldUpgradeLevel", goldUpgradeLevel);
        PlayerPrefs.SetInt("trainingUpgradeLevel", trainingUpgradeLevel);
        PlayerPrefs.SetInt("offlineProgressCheck", offlineProgressCheck);

        // Stats
        PlayerPrefs.SetInt("killsTotal", killsTotal);
        PlayerPrefs.SetInt("totalDeaths", totalDeaths);

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
        stage = PlayerPrefs.GetInt("stage", 1);
        stageMax = PlayerPrefs.GetInt("stageMax", 1);
        powerUpgradeLevel = PlayerPrefs.GetInt("powerUpgradeLevel", 1);
        defenseUpgradeLevel = PlayerPrefs.GetInt("defenseUpgradeLevel", 1);
        goldUpgradeLevel = PlayerPrefs.GetInt("goldUpgradeLevel", 1);
        trainingUpgradeLevel = PlayerPrefs.GetInt("trainingUpgradeLevel", 1);
        offlineProgressCheck = PlayerPrefs.GetInt("offlineProgressCheck", 0);

        // Stats
        killsTotal = PlayerPrefs.GetInt("killsTotal", 0);
        totalDeaths = PlayerPrefs.GetInt("totalDeaths", 0);

        LoadOfflineProduction();
    }

    public void ResetWholeGame()
    {
        coins = 0;
        power = 100;
        defense = 10;
        playerHealth = 100;
        maxPlayerHealth = 100;
        enemyHealth = 10;
        enemyPower = 1;
        powerUpgradeCost = 10;
        defenseUpgradeCost = 10;
        goldUpgradeCost = 10;
        trainingUpgradeCost = 10;
        kills = 0;
        killsMax = 10;
        killsTotal = 0;
        totalDeaths = 0;
        stage = 1;
        stageMax = 1;
        powerUpgradeLevel = 1;
        defenseUpgradeLevel = 1;
        goldUpgradeLevel = 1;
        trainingUpgradeLevel = 1;
        offlineProgressCheck = 0;
        bossMultiplier = 1;
        Save();
        StartGame();
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
            double temp = 10 * System.Math.Pow(1.5, stageMax-5);
            var coinsToEarn = System.Math.Ceiling(temp / 250) * idleTime;
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
            bossMultiplier = 2f;
            killsMax = 1;
            
            // Slow down the boss fights near max stage for suspense
            if (stage >= stageMax - 5)
                Time.timeScale = 0.5f;
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



        coins += System.Math.Ceiling(enemyHealth / 50) * goldUpgradePower;
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
        enemyHealth = 10 * System.Math.Pow(1.35, stage-1) * bossMultiplier;
        enemyPower = 1 * System.Math.Pow(1.35, stage-1) * bossMultiplier;
    }

    // If player can kill enemy and how effectively
    public bool PowerCheck(double fullPower)
    {
        if (fullPower > enemyHealth * 100)            // 100x power - 10x game speed
        {
            Time.timeScale = 50;
        }
        else if (fullPower > enemyHealth * 10)       // 10x power - 5x game speed
        {
            Time.timeScale = 20;
        }
        else if (fullPower > enemyHealth * 5)       // 5x power - 5x game speed
        {
            Time.timeScale = 5;  
        }
        else if (fullPower > enemyHealth * 2)       // 5x power - 3x game speed
        {
            Time.timeScale = 3;  
        }
        else if (fullPower >= enemyHealth)          // power > enemyHealth - 1x game speed
        {
            // Set game time to 1.0x
            Time.timeScale = 1;
        } else if (fullPower >= enemyHealth / 2)    // power > enemyHealth / 2 - 0.5x game speed
        {
            playerHealth -= 33;
            Debug.Log("MEGA HIT -- Power: " + fullPower + " | Enemy HP: " + enemyHealth);
            //! Add this after animations are in to slow down the attack animation
            // Time.timeScale = 0.5;                   
        }
        else                                    // Not enough power
        {
            // kill player and reset to stage 1
            Debug.Log("Not enough power -- Power: " + Formatter(fullPower, "F2") + " | Enemy HP: " + Formatter(enemyHealth, "F2"));
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
        else if (fullDefense > enemyPower * 5)       // 5x defense - Take 0-2 dmg
        {
            // playerHealth -= enemyPower / 2;
            playerHealth -= UnityEngine.Random.Range(0, 3);
        }
        else if (fullDefense >= enemyPower)          // defense > enemyPower - Take 3-5 dmg
        {
            playerHealth -= UnityEngine.Random.Range(3, 6);
            Debug.Log("med hit");
        }
        else if (fullDefense <= enemyPower)     // defense > enemyPower / 2 - Take 8-15 dmg
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
            playerHealth = 0;
            CancelInvoke("Hit");
            StartCoroutine(PlayerDied());
            return false;
        }
    }

    IEnumerator PlayerDied()
    {
        Debug.Log("Restarting Game. Wait 3 seconds.");
        Time.timeScale = 1;
        totalDeaths++;
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
        var b = 10;
        var c = coins;

        switch (id)
        {
            case "powerUpgrade1":
                var r = 1.45;
                var k = powerUpgradeLevel;
                var n = System.Math.Floor(System.Math.Log(c * (r - 1) / (b * System.Math.Pow(r, k)) + 1, r));

                var cost = b * (System.Math.Pow(r, k) * (System.Math.Pow(r, n) - 1) / (r - 1));

                if (coins >= cost)
                {
                    coins -= cost;
                    powerUpgradeLevel += (int)n;
                    powerUpgradeCost = System.Math.Ceiling(10 * System.Math.Pow(1.45, powerUpgradeLevel));
                }
                break;
            case "defenseUpgrade1":
                var r2 = 1.5;
                var k2 = defenseUpgradeLevel;
                var n2 = System.Math.Floor(System.Math.Log(c * (r2 - 1) / (b * System.Math.Pow(r2, k2)) + 1, r2));

                var cost2 = b * (System.Math.Pow(r2, k2) * (System.Math.Pow(r2, n2) - 1) / (r2 - 1));

                if (coins >= cost2)
                {
                    coins -= cost2;
                    defenseUpgradeLevel += (int)n2;
                    defenseUpgradeCost = System.Math.Ceiling(10 * System.Math.Pow(1.5, defenseUpgradeLevel));
                }
                break;
            case "goldUpgrade1":
                var r3 = 2.4;
                var k3 = goldUpgradeLevel;
                var n3 = System.Math.Floor(System.Math.Log(c * (r3 - 1) / (b * System.Math.Pow(r3, k3)) + 1, r3));

                var cost3 = b * (System.Math.Pow(r3, k3) * (System.Math.Pow(r3, n3) - 1) / (r3 - 1));

                if (coins >= cost3)
                {
                    coins -= cost3;
                    goldUpgradeLevel += (int)n3;
                    goldUpgradeCost = System.Math.Ceiling(10 * System.Math.Pow(2.4, goldUpgradeLevel));
                }
                break;
            case "trainingUpgrade1":
                if (stageMax-1 >= trainingUpgradeCost)
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
        powerUpgradeLevelText.text = "Level : " + powerUpgradeLevel.ToString("F0");
        powerUpgradePowerText.text = Formatter(((power + powerUpgradePower) * trainingUpgradePower), "F2") + " Pow";

        defenseUpgradeLevelText.text = "Level : " + defenseUpgradeLevel.ToString("F0");
        defenseUpgradePowerText.text = Formatter(((defense + defenseUpgradePower) * trainingUpgradePower), "F2") + " Def";

        goldUpgradeLevelText.text = "Level : " + goldUpgradeLevel.ToString("F0");
        goldUpgradePowerText.text = "x" + goldUpgradePower.ToString("F2") + " Coins";

        trainingUpgradeLevelText.text = "Level : " + trainingUpgradeLevel.ToString("F0");
        trainingUpgradePowerText.text = "x" + trainingUpgradePower.ToString("F2") + " Pow + Def";

        // Buttons
        if (coins >= powerUpgradeCost)
        {
            powerUpgradeButton.interactable = true;
            powerUpgradeButton.GetComponentInChildren<Text>().text = "Buy Max (" + (GetMaxUpgradeCount(10, powerUpgradeLevel, 1.45)).ToString() + ")\n"
                            + Formatter(GetMaxUpgradeCost(10, powerUpgradeLevel, 1.45), "F2") + " coins";
        } else
        {
            powerUpgradeButton.interactable = false;
            powerUpgradeButton.GetComponentInChildren<Text>().text = "Cannot afford\n" + Formatter(powerUpgradeCost, "F2") + " coins";
        }
        
        if (coins >= defenseUpgradeCost)
        {
            defenseUpgradeButton.interactable = true;
            defenseUpgradeButton.GetComponentInChildren<Text>().text = "Buy Max (" + (GetMaxUpgradeCount(10, defenseUpgradeLevel, 1.5)).ToString() + ")\n"
                            + Formatter(GetMaxUpgradeCost(10, defenseUpgradeLevel, 1.5), "F2") + " coins";
        } else
        {
            defenseUpgradeButton.interactable = false;
            defenseUpgradeButton.GetComponentInChildren<Text>().text = "Cannot afford\n" + Formatter(defenseUpgradeCost, "F2") + " coins";
        }

        if (coins >= goldUpgradeCost)
        {
            goldUpgradeButton.interactable = true;
            goldUpgradeButton.GetComponentInChildren<Text>().text = "Buy Max (" + (GetMaxUpgradeCount(10, goldUpgradeLevel, 2.4)).ToString() + ")\n"
                            + Formatter(GetMaxUpgradeCost(10, goldUpgradeLevel, 2.4), "F2") + " coins";
        } else
        {
            goldUpgradeButton.interactable = false;
            goldUpgradeButton.GetComponentInChildren<Text>().text = "Cannot afford\n" + Formatter(goldUpgradeCost, "F2") + " coins";
        }

        if (stageMax - 1 >= trainingUpgradeCost)
        {
            trainingUpgradeButton.interactable = true;
            trainingUpgradeButton.GetComponentInChildren<Text>().text = "Buy\n" + Formatter(trainingUpgradeCost, "F2") + " Stages";
        } else {
            trainingUpgradeButton.interactable = false;
            trainingUpgradeButton.GetComponentInChildren<Text>().text = "Cannot afford\n" + Formatter(trainingUpgradeCost, "F2") + " Stages";
        }


        // Power
        if (powerUpgradeLevel <= 1)
            powerUpgradePower = 0;
        else
            powerUpgradePower = 50 * System.Math.Pow(1.25, powerUpgradeLevel - 1);

        if (defenseUpgradeLevel <= 1)
            defenseUpgradePower = 0;
        else
            defenseUpgradePower = 50 * System.Math.Pow(1.25, defenseUpgradeLevel - 1);

        if (goldUpgradeLevel <= 1)
            goldUpgradePower = 1.00;
        else
            goldUpgradePower = (goldUpgradeLevel-1) * 1.6;

        if (trainingUpgradeLevel <= 1)
            trainingUpgradePower = 1.00;
        else
            trainingUpgradePower = System.Math.Pow(2, trainingUpgradeLevel - 1);
    }

    public double GetMaxUpgradeCount(int baseNum, int level, double rate)
    {
        var b = baseNum;
        var c = coins;
        var r = rate;
        var k = level;
        var n = System.Math.Floor(System.Math.Log(c * (r - 1) / (b * System.Math.Pow(r, k)) + 1, r));
        return n;
    }

    public double GetMaxUpgradeCost(int baseNum, int level, double rate)
    {
        var b = baseNum;
        var c = coins;
        var r = rate;
        var k = level;
        var n = System.Math.Floor(System.Math.Log(c * (r - 1) / (b * System.Math.Pow(r, k)) + 1, r));
        var cost = b * (System.Math.Pow(r, k) * (System.Math.Pow(r, n) - 1) / (r - 1));
        return cost;
    }
}
