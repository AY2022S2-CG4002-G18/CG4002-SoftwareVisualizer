using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DataSync : MonoBehaviour
{
    public Slider slider;
    public GameData gameData;
    public GamePlay gamePlay;
    float timeout;
    float lastTime;
    bool startSync = false;
    private void Start()
    {
        timeout = Config.TIMEOUT;

        slider.maxValue = timeout;
    }

    private void Update()
    {
        if (!startSync) return;

        float currentTime = Time.realtimeSinceStartup;

        slider.value = currentTime - lastTime;

        if (currentTime - lastTime >= timeout)
        {
            Sync();
        }
    }

    public void StartSync()
    {
        startSync = true;
        lastTime = Time.realtimeSinceStartup;

        slider.gameObject.SetActive(true);
    }

    void Sync()
    {
        gameData.SyncData();
        startSync = false;

        slider.gameObject.SetActive(false);
    }


    public void SyncDataFromServer(string json)
    {
        if (json == null) return;
        Debug.Log(json);
        json = json.Replace("\'", "");
        JSONNode jSONNode = JSON.Parse(json);
        JSONNode playerNode = jSONNode[(gamePlay.playerID == "P1") ? "p1" : "p2"];
        Debug.Log(playerNode);

        gameData.SyncData(playerNode["hp"].AsInt,
            playerNode["bullets"].AsInt,
            playerNode["grenades"].AsInt,
            playerNode["num_deaths"].AsInt,
            playerNode["num_shield"].AsInt);
        JSONNode opponentNode = jSONNode[(gamePlay.playerID == "P1") ? "p2" : "p1"];
        gameData.SyncData(opponentNode["hp"].AsInt,
            opponentNode["bullets"].AsInt,
            opponentNode["num_deaths"].AsInt);
    }
}
