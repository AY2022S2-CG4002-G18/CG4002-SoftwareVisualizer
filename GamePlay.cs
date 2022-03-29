using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public string playerID;

    public GameData gameData;
    public DataSync dataSync;
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

    public bool canShield = true;

    public int deathNum = 0;

    public static bool enemyInSight = false;

    public GameObject gameover;
    private void Start()
    {
        playerID = Config.PLAYER_ID;
        webClient.queueName = Config.PLAYER_ID;
        webClient.pubRoutingKey = "visualizer" + Config.PLAYER_ID;
    }

    private void Update()
    {
        CheckForShoot();
    }

    public void HandleMessageFromServer(string message)
    {
        if (!message.Contains("|"))
        {
            return;
        }
        else
        {
            string name = message.Substring(0, message.IndexOf("|")).Trim();
            Debug.Log(name);
            string value = message.Substring(message.IndexOf("|") + 1).Trim();
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
                    case "Life":
                        {
                            Invoker.InvokeInMainThread(Life);
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
                    case "Respawn":
                        {
                            Invoker.InvokeInMainThread(Respawn);
                            break;
                        }
                    case "Sync":
                        {
                            //Invoker.InvokeInMainThread(SyncData);
                            dataSync.StartSync();
                            break;
                        }
                    case "Logout":
                        {
                            Invoker.InvokeInMainThread(Logout);
                            break;
                        }
                    default:
                        {
                            if (!value.Contains("-"))
                            {
                                return;
                            }
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
                                case "Sync":
                                    {
                                        dataSync.SyncDataFromServer(tValue);
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
        gameData.SetValue("ShieldNum", 3);
        gameData.SetValue("Ammo", 6);
        gameData.SetValue("Grenade", 2);
        //gameData.SetValue("OHP", 100);
        //gameData.SetValue("OAmmo", 6);
        foreach (SimpleCountdown countdown in shieldCountdown)
        {
            countdown.resetCountDown();
        }
    }

    private void Respawn()
    {
        Init();
    }

    
    public float bulletCheckTime;

    private void CheckForShoot()
    {
        if (beginShoot && hitOpponent)
        {
            beginShoot = false;
            hitOpponent = false;
            CancelInvoke("ResetBeginShoot");
            CancelInvoke("ResetHitOpponent");
            if (gameData.AddValue("Ammo", -1))
            {
                ValidShoot();
            }
            else
            {
                NoAmmo();
            }
        }
/*        else if (beginShoot)
        {
            beginShoot = false;
            CancelInvoke("ResetBeginShoot");

            if (gameData.AddValue("Ammo", -1))
            {
                NoHit();
            }
            else
            {
                NoAmmo();
            }
        }*/
    }
    private bool beginShoot = false;
    private void Shoot()
    {
        beginShoot = true;
        Invoke("ResetBeginShoot", bulletCheckTime);
    }
    private void ResetBeginShoot()
    {
        beginShoot = false;
        //Shoot but didn't hit. Missed.
        NoHit();
    }
    private bool hitOpponent = false;
    private void Life()
    {
        hitOpponent = true;
        Invoke("ResetHitOpponent", bulletCheckTime);
    }
    private void ResetHitOpponent()
    {
        hitOpponent = false;
        //Hit but didn't shoot? Shouldn't happen..
        HitButNoShoot();
    }

    private void ValidShoot()
    {
        shootAudioSource.Play();

        webClient.SendClientMessage(playerID + "|ValidShoot");
    }
    private void NoAmmo()
    {
        reloadAnim.SetTrigger("Hint");
        webClient.SendClientMessage(playerID + "|InvalidShoot");
    }
    private void NoHit()
    {
        //Really no hit or have async hit, update ammo anyway
        //Just send invalidshoot
        if (gameData.AddValue("Ammo", -1))
        {
            shootAudioSource.Play();
            webClient.SendClientMessage(playerID + "|InvalidShoot");
        }
        else
        {
            NoAmmo();
        }

    }

    private void HitButNoShoot()
    {
        //There must be an async shoot, so no need to update ammo
        //Just send validshoot

        if (gameData.GetValue("Ammo") > 0) //This is buggy but...
        {
            webClient.SendClientMessage(playerID + "|ValidShoot");
        }
        else
        {
            NoAmmo();
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
                webClient.SendClientMessage(playerID + "|ValidGrenade");
            }
            else
            {
                //Failed grenade
                webClient.SendClientMessage(playerID + "|InvalidGrenade");
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
                Invoke("ResetShieldActivation", 10);

                int shieldIndex = gameData.GetValue("ShieldNum");
                shieldCountdown[shieldIndex].beginCountdown();

                webClient.SendClientMessage(playerID + "|ShieldOn");
                //Invoke("SendShieldOn", 0.5f);
            }
        }
    }

    private void ResetShield()
    {
        gameData.SetValue("Shield", 0);
        shieldObject.SetActive(false);

        int shieldIndex = gameData.GetValue("ShieldNum");
        shieldCountdown[shieldIndex].forceToZero();

        webClient.SendClientMessage(playerID + "|ShieldOff");
        //Invoke("SendShieldOff", 0.5f);
    }

/*    private void SendShieldOn()
    {
        webClient.SendClientMessage(playerID + "|ShieldOn");
    }
        private void SendShieldOff()
    {
        webClient.SendClientMessage(playerID + "|ShieldOff");
    }*/

    private void ResetShieldActivation()
    {
        canShield = true;
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

        deathNum++;
        webClient.SendClientMessage(playerID + "|Die");
    }

    public void Logout()
    {
        gameover.SetActive(true);
    }
}
