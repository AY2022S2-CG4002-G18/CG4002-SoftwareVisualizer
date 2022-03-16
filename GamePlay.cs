using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public string playerID;

    public GameData gameData;
    public WebClient webClient;
    public GrenadeControl grenadeControl;
    public GameObject shieldObject;

    public SimpleCountdown[] shieldCountdown;
    
    public GameObject opponentShieldObject;
    //public GameObject grenadeObject;
    //public Transform grenadeRespawn;
    public AudioSource shootAudioSource;
    public AudioSource throwAudioSource;

    public Animator reloadAnim;

    public GameObject effectRespawn;
    public GameObject hitEffect;
    public GameObject hitEffectWithShield;

    public GameObject explosion;

    bool canShield = true;

    public static bool enemyInSight = false;

    private void Start()
    {
        playerID = Config.PLAYER_ID;
    }

    public void HandleMessageFromServer(string message)
    {
        if (!message.Contains(":"))
        {
            return;
        }
        else
        {
            string name = message.Substring(0, message.IndexOf(":")).Trim();
            Debug.Log(name);
            string value = message.Substring(message.IndexOf(":") + 1).Trim();
            Debug.Log(value);
            if (name == playerID)
            {
                switch (value)
                {
                    case "Shoot":
                        {
                            Invoker.InvokeInMainThread(Shoot);
                            break;
                        }
                    case "Shield":
                        {
                            Invoker.InvokeInMainThread(Shield);
                            break;
                        }
                    case "Grenade":
                        {
                            Invoker.InvokeInMainThread(Grenade);
                            break;
                        }
                    case "HitByBullet":
                        {
                            Invoker.InvokeInMainThread(HitByBullet);
                            break;
                        }
                    case "HitByGrenade":
                        {
                            Invoker.InvokeInMainThread(HitByGrenade);
                            break;
                        }
                    case "Reload":
                        {
                            Invoker.InvokeInMainThread(Reload);
                            break;
                        }
                    default:
                        {
                            //Handle non-action message
                            string tName = value.Substring(0, value.IndexOf("-")).Trim();
                            string tValue = value.Substring(value.IndexOf("-") + 1).Trim();
                            switch (tName)
                            {
                                case "Score":
                                    {
                                        Invoker.InvokeInMainThread(() => ChangeScore(int.Parse(tValue)));
                                        break;
                                    }
                                case "OScore":
                                    {
                                        Invoker.InvokeInMainThread(() => ChangeOpponentScore(int.Parse(tValue)));
                                        break;
                                    }
                                case "OHP":
                                    {
                                        Invoker.InvokeInMainThread(() => ChangeOpponentHP(int.Parse(tValue)));
                                        break;
                                    }
                                case "OAmmo":
                                    {
                                        Invoker.InvokeInMainThread(() => ChangeOpponentAmmo(int.Parse(tValue)));
                                        break;
                                    }
                                case "OShield":
                                    {
                                        Invoker.InvokeInMainThread(() => ChangeOpponentShield(int.Parse(tValue)));
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }
    }

    public void Init()
    {
        gameData.SetValue("HP", 100);
        gameData.SetValue("Shield", 0);
        gameData.SetValue("Ammo", 6);
        gameData.SetValue("Grenade", 2);
        gameData.SetValue("OHP", 100);
        gameData.SetValue("OAmmo", 6);
    }

    private void Shoot()
    {
        if (gameData.AddValue("Ammo", -1))
        {
            //Successful shot
            shootAudioSource.Play();

            webClient.SendClientMessage(playerID + ":ValidShoot");
        }
        else
        {
            //No ammo
            reloadAnim.SetTrigger("Hint");
        }
    }

    private void HitByBullet()
    {
        Hit(10);
    }

    private void HitByGrenade()
    {
        Hit(30);

        Instantiate(explosion, grenadeControl.transform);
    }

    private void Hit(int damage)
    {
        int damageAfterShield;
        if (canShield)
        {
            //shield off
            damageAfterShield = damage;

            Instantiate(hitEffect, effectRespawn.transform);
        }
        else
        {
            //shield on
            int currentShield = gameData.GetValue("Shield");
            
            if (currentShield >= damage)
            {
                //Can take all damage
                damageAfterShield = 0;
                gameData.AddValue("Shield", -1 * damage);

                Instantiate(hitEffectWithShield, effectRespawn.transform);
            }
            else
            {
                damageAfterShield = damage - currentShield;
                ResetShield();

                Instantiate(hitEffect, effectRespawn.transform);
            }
        }
        if (!gameData.AddValue("HP", -1 * damageAfterShield))
        {
            //HP < 0
            gameData.SetValue("HP", 0);
            GameOver();
        }
    }

    private void Grenade()
    {
        if (enemyInSight)
        {
            if (gameData.AddValue("Grenade", -1))
            {
                //Successful grenade
                throwAudioSource.Play();
                //Instantiate(grenadeObject, grenadeRespawn.position, grenadeRespawn.rotation);
                grenadeControl.ThrowGrenade();
                webClient.SendClientMessage(playerID + ":ValidGrenade");
            }
        }
    }

    private void Shield()
    {
        if (canShield)
        {
            if (gameData.AddValue("ShieldNum", -1))
            {
                gameData.SetValue("Shield", 30);
                shieldObject.SetActive(true);
                canShield = false;
                Invoke("ResetShield", 10);

                int shieldIndex = gameData.GetValue("ShieldNum");
                shieldCountdown[shieldIndex].beginCountdown();

                webClient.SendClientMessage(playerID + ":ShieldOn");
            }
        }
    }

    private void ResetShield()
    {
        gameData.SetValue("Shield", 0);
        shieldObject.SetActive(false);
        canShield = true;

        int shieldIndex = gameData.GetValue("ShieldNum");
        shieldCountdown[shieldIndex].forceToZero();

        webClient.SendClientMessage(playerID + ":ShieldOff");
    }

    private void Reload()
    {
        gameData.SetValue("Ammo", 6);

        reloadAnim.SetTrigger("Reload");
    }

    private void ChangeScore(int score)
    {
        gameData.SetValue(playerID, score);
    }

    private void ChangeOpponentScore(int score)
    {
        if (playerID == "P1")
        {
            gameData.SetValue("P2", score);
        }
        else
        {
            gameData.SetValue("P1", score);
        }
    }

    private void ChangeOpponentHP(int hp)
    {
        gameData.SetValue("OHP", hp);
    }

    private void ChangeOpponentAmmo(int ammo)
    {
        gameData.SetValue("OAmmo", ammo);
    }

    private void ChangeOpponentShield(int state)
    {
        if (state == 1)
        {
            opponentShieldObject.SetActive(true);
        }
        else
        {
            opponentShieldObject.SetActive(false);
        }
    }
    public void EnemyEnterSight()
    {
        enemyInSight = true;
    }

    public void EnemyLeaveSight()
    {
        enemyInSight = false;
    }
    public void GameOver()
    {
        //???
        webClient.SendClientMessage(playerID + ":Die");
    }
}