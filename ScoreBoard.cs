using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;


using TMPro;

using UnityEngine.Splines;
using ExitGames.Client.Photon;



public class ScoreBoard : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI coinRemainingNum;
    public TextMeshProUGUI onlinePlayer;
    public TextMeshProUGUI freebieNum;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerScoreText;



    public TextMeshProUGUI playerListText;

    public TextMeshProUGUI winner;

    
    public TextMeshProUGUI coinCollectedNum;
    
  
   
  

    
    public TextMeshProUGUI WinnerScreenNickName;
    public TextMeshProUGUI WinnerScreenTotalCoin;
   



   





    private PhotonView myphotonView;

   

    private int coinCollected;
    public int coinRemaining;

    public int totalCoins = 30;
    public int freedrinks = 1;



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        UpdatePlayerListUI();

        // Initialize the remaining coins count in the room properties
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RemainingCoins"))
        {
            Hashtable props = new Hashtable { { "RemainingCoins", totalCoins } }; // totalCoins should reflect the actual total
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }


    void Start()
    {
        
        myphotonView = GetComponent<PhotonView>();
        freebieNum.text = freedrinks.ToString();
       


     

    }












    void UpdatePlayerListUI()
    {
        // Define a list to hold player names and scores
        List<(string Name, int Score)> playersAndScores = new List<(string Name, int Score)>();

        // Populate the list
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Exclude "admin" from the list
            if (player.NickName.Equals("admin39", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int score = player.CustomProperties.ContainsKey("score") ? (int)player.CustomProperties["score"] : 0;
            playersAndScores.Add((player.NickName, score));
        }

        // Sort the list by score in descending order
        playersAndScores.Sort((a, b) => b.Score.CompareTo(a.Score));

        // Build the display strings
        string playerNames = "";
        string playerScores = "";
        foreach (var playerScore in playersAndScores)
        {
            playerNames += playerScore.Name + "\n";
            playerScores += playerScore.Score.ToString() + "\n";
        }

        // Update the UI
        playerNameText.text = playerNames;
        playerScoreText.text = playerScores;
        onlinePlayer.text =  playersAndScores.Count.ToString();
    }





    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerListUI();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey("score"))
        {
            UpdatePlayerListUI();
        }
    }








    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey("RemainingCoins"))
        {
            coinRemaining = (int)propertiesThatChanged["RemainingCoins"];
            coinRemainingNum.text = coinRemaining.ToString();



        }
    }








    


  





    void ShowWinner(string winnerName, int coinsCollected)
    {
        // Display the winner's name and coin count
        Debug.Log($"Winner: {winnerName} with {coinsCollected} coins collected!");
        // Update the UI or show a message to all players
        winner.text = winnerName;
    }









    [PunRPC]
    void ShowWinnerRPC(string winnerName, int coinsCollected)
    {
        // Display the winner's name and coin count
        Debug.Log($"Winner: {winnerName} with {coinsCollected} coins collected!");
        // Update the UI or show a message to all players
        winner.text = $"Winner: {winnerName} with {coinsCollected} coins!";

       
        


    }

   





}


