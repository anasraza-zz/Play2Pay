using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;



public class NetworkManager : MonoBehaviourPunCallbacks
{

   
    public GameObject roomUI;
    public GameObject playbutton;
    public TMP_InputField playerNickName;
    public TextMeshProUGUI networkUpdate;

    private bool isKeyPressed = false;


    void Update()
    {


        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Q))
        {
            isKeyPressed = true;
            InitializeRoom();
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.W))
        {
            isKeyPressed = true;
            ConnectToServer();
        }
    }


    public void ConnectToServer()
    {
        playbutton.SetActive(false);


        




        PhotonNetwork.ConnectUsingSettings();
        networkUpdate.text = "Trying To Connect to server...";
        
        Debug.Log("Trying To Connect to server...");
    }



    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server.");
        networkUpdate.text = "Connected to Server.";

 

        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("we Joined the Lobby");
        networkUpdate.text = "Joined the Lobby";
        

        roomUI.SetActive(true);
    }

    public void InitializeRoom()
    {
        if (!string.IsNullOrEmpty(playerNickName.text))
        {
            PhotonNetwork.NickName = playerNickName.text;
        }
        else if(isKeyPressed)
        {
            PhotonNetwork.NickName = "admin39";
        }
        else
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1, 9999);
        }

        string roomName = "MyRoom";
        byte maxPlayers = 10;
        int sceneIndex = isKeyPressed ? 4 : 1; // Use sceneIndex 4 if key was pressed, else 1

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            IsVisible = true,
            IsOpen = true,
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel(sceneIndex);

        // Reset the flag
        isKeyPressed = false;
    }






    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        base.OnJoinedRoom();





    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("new Player Joined");
        base.OnPlayerEnteredRoom(newPlayer);
    }




   



}
