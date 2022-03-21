using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameData : MonoBehaviour
{
    public GamePlay gamePlay;
    public WebClient webClient;
    public string[] names;
    public int[] values;
    public Vector2[] valuesRange;
    public Text[] textUI;

    private void Update()
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (textUI[i] != null) textUI[i].text = values[i].ToString();
        }
    }

    public bool SetValue(string name, int value)
    {
        if (Config.GOD_MODE) return true;
        int index = Array.IndexOf(names, name);
        if (index != -1)
        {
            if ((value >= valuesRange[index].x) && (value <= valuesRange[index].y))
            {
                values[index] = value;

                //SyncHPAndAmmo(name, values[index]);

                return true;
            }
        }
        return false;
    }

    public bool AddValue(string name, int value)
    {
        if (Config.GOD_MODE) return true;
        int index = Array.IndexOf(names, name);
        if (index != -1)
        {
            if ((values[index] + value >= valuesRange[index].x) && (values[index] + value <= valuesRange[index].y))
            {
                values[index] += value;

                //SyncHPAndAmmo(name, values[index]);
                //SyncData();

                return true;
            }

            
        }
        return false;
    }

    public void SyncData()
    {
        webClient.SendClientMessage(gamePlay.playerID +
            String.Format("|Data|{{\"hp\": {0},\"bullets\": {1},\"grenades\": {2},\"shield_time\": {3},\"shield_health\": {4},\"num_deaths\": {5},\"num_shield\": {6}}}",
            GetValue("HP"),
            GetValue("Ammo"),
            GetValue("Grenade"),
            gamePlay.canShield ? 0 : (10 - gamePlay.shieldCountdown[GetValue("ShieldNum")].count),
            GetValue("Shield"),
            gamePlay.deathNum,
            GetValue("ShieldNum")
            ));
    }
    
    public void SyncData(int hp, int bullet, int grenade, int num_death, int num_shield)
    {
        SetValue("HP", hp);
        SetValue("Ammo", bullet);
        SetValue("Grenade", grenade);

        gamePlay.deathNum = num_death;
        if (SetValue("ShieldNum", num_shield))
        {
            for (int shieldIndex = 0; shieldIndex < gamePlay.shieldCountdown.Length; shieldIndex++)
            {
                if (shieldIndex < num_shield)
                {
                    gamePlay.shieldCountdown[shieldIndex].resetCountDown();
                }
                else
                {
                    gamePlay.shieldCountdown[shieldIndex].forceToZero();
                }
            }
        }

        //Change score
        SetValue((gamePlay.playerID == "P1") ? "P2" : "P1", num_death);
    }
    public void SyncData(int hp, int bullet, int num_death)
    {
        //For opponent
        SetValue("OHP", hp);
        SetValue("OAmmo", bullet);

        SetValue((gamePlay.playerID == "P1") ? "P1" : "P2", num_death);
    }
    /*    private void SyncHPAndAmmo(string name, int value)
        {
            if (name == "HP")
            {
                webClient.SendClientMessage(gamePlay.playerID + ":HP-" + value.ToString());
            }
            if (name == "Ammo")
            {
                webClient.SendClientMessage(gamePlay.playerID + ":Ammo-" + value.ToString());
            }
        }*/

    public int GetValue(string name)
    {
        int index = Array.IndexOf(names, name);
        if (index != -1)
        {
            return values[index];
        }
        return -1;
    }
}
