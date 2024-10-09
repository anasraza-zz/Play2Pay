using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameOverSceneLoader : MonoBehaviour
{

    void Start()
    {
        DisconnectFromPhoton();
    }

    void DisconnectFromPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void LoadSceneByName()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("1_home");
    }
}


