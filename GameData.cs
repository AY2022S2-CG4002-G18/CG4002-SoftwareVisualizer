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

                SyncHPAndAmmo(name, values[index]);

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
                SyncData();

                return true;
            }

            
        }
        return false;
    }

    private void SyncData()
    {
        webClient.SendClientMessage(gamePlay.playerID + "Game data");
    }
    
    private void SyncHPAndAmmo(string name, int value)
    {
        if (name == "HP")
        {
            webClient.SendClientMessage(gamePlay.playerID + ":HP-" + value.ToString());
        }
        if (name == "Ammo")
        {
            webClient.SendClientMessage(gamePlay.playerID + ":Ammo-" + value.ToString());
        }
    }

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
