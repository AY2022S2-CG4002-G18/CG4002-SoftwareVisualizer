using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfigManager : MonoBehaviour
{
    public InputField hostInput;
    public InputField portInput;

    public Dropdown playerID;

    public Toggle godMode;

    private void Start()
    {
        ;
    }
    public void UpdateConfig()
    {
        if (hostInput.IsActive()) Config.HOST = hostInput.text;
        if (portInput.IsActive()) Config.PORT = int.Parse(portInput.text);
        Config.PLAYER_ID = playerID.options[playerID.value].text;
        Config.GOD_MODE = godMode.isOn;
        EnterNextScene();
    }

    void EnterNextScene()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
