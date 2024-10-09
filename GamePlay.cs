using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

using TMPro;

using UnityEngine.Splines;
using ExitGames.Client.Photon;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class GamePlay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI winner;

    public TextMeshProUGUI coinCollectedNumTop;
    public TextMeshProUGUI coinCollectedNum;
    public TextMeshProUGUI coinRemainingNum;
    public TextMeshProUGUI onlinePlayer;
    public TextMeshProUGUI freebieNum;
    public TextMeshProUGUI myNickname;

    public GameObject WinnerScreen; 
    public TextMeshProUGUI WinnerScreenNickName;
    public TextMeshProUGUI WinnerScreenTotalCoin;
    public Button WinnerScreenButton;


    public GameObject GameOverScreen;
    public TextMeshProUGUI GameOverScreenNickName;
    public TextMeshProUGUI GameOverScreenPlayerScore;    
    public TextMeshProUGUI GameOverScreenWinnerName;
    public TextMeshProUGUI GameOverScreenWinnerScore;
    public Button GameOverScreenButton;


  
    public AudioClip winnerSound;
    public AudioClip loserSound;



    [SerializeField] private Transform parentObject;
    string collectabelName;

    public AudioSource soundSource;
    public AudioClip coinSound;
    private PhotonView myphotonView;

    public SplineContainer spline;
    private List<GameObject> instantiatedCoin = new List<GameObject>();

    private bool canCollectCoin = true;



    private int coinCollected;
    private int coinRemaining;

    public int totalCoins;
    public int freedrinks = 1;

   

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        CreateSplineAndCoins();



        // Initialize the remaining coins count in the room properties
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RemainingCoins"))
        {
            Hashtable props = new Hashtable { { "RemainingCoins", totalCoins } }; // totalCoins should reflect the actual total
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        // Immediately update the UI with the current remaining coins when joining the room
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RemainingCoins", out object remainingCoins))
        {
            coinRemaining = (int)remainingCoins;
            coinRemainingNum.text = coinRemaining.ToString();
        }
    }

   




    void Start()
    {
        soundSource = GetComponent<AudioSource>();
        myphotonView = GetComponent<PhotonView>();
        freebieNum.text = freedrinks.ToString();
        myNickname.text = PhotonNetwork.NickName;


        WinnerScreenButton.onClick.AddListener(() => LoadScene("3_RedeemCoins"));

        GameOverScreenButton.onClick.AddListener(() => LoadScene("4_GameOver"));
        

    }

    void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }



    void Update()
    {
        onlinePlayer.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();

        // Modify the condition to check if canCollectCoin is true
        if (canCollectCoin && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit Hit;
            if (Physics.Raycast(ray, out Hit))
            {
                PhotonView childPhotonView = Hit.transform.GetComponent<PhotonView>();
                collectabelName = Hit.transform.name;

                if (childPhotonView != null)
                {
                    StartCoroutine(CollectCoinCooldown()); // Start the cooldown coroutine
                    OnCoinTapped(collectabelName);
                }
            }
        }
    }

    private IEnumerator CollectCoinCooldown()
    {
        canCollectCoin = false; // Disable coin collection
        yield return new WaitForSeconds(1); // Wait for 1 seconds
        canCollectCoin = true; // Re-enable coin collection
    }



    private void OnCoinTapped(string collectableName)
    {
        soundSource.PlayOneShot(coinSound);
        coinCollected++;

        coinCollectedNum.text = coinCollected.ToString();
        coinCollectedNumTop.text = coinCollected.ToString();

        // Update this player's custom properties with the new total
        Hashtable playerProps = new Hashtable
    {
        { "CoinsCollected", coinCollected }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        // Store this player's score in custom properties
        Hashtable score = new Hashtable();
        score["score"] = coinCollected;
        PhotonNetwork.LocalPlayer.SetCustomProperties(score);


        // Decrement the remaining coins in the room properties
        DecrementRemainingCoins();

        myphotonView.RPC("DeactivateCollectibleRPC", RpcTarget.AllBuffered, collectableName);

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








    void DecrementRemainingCoins()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RemainingCoins", out object remainingCoins))
        {
            int newCount = (int)remainingCoins - 1;
            Hashtable props = new Hashtable { { "RemainingCoins", newCount } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // Check if all coins have been collected
            if (newCount <= 0)
            {
                DetermineWinner();
            }
        }
    }


    void DetermineWinner()
    {
        Player winnerPlayer = null;
        int maxCoinsCollected = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("CoinsCollected", out object coinsCollected))
            {
                int coins = (int)coinsCollected;
                if (coins > maxCoinsCollected)
                {
                    winnerPlayer = player;
                    maxCoinsCollected = coins;
                }
            }
        }

        if (winnerPlayer != null)
        {
            // Call the RPC to display the winner on all clients
            myphotonView.RPC("ShowWinnerRPC", RpcTarget.AllBuffered, winnerPlayer.NickName, maxCoinsCollected);
        }
    }





  




    private void CreateSplineAndCoins()
    {
        
        float totalcoinsFactor = 1f / totalCoins;

        for (float t = 0; t <= 1; t += totalcoinsFactor)
        {
            Vector3 pointOnSpline = spline.EvaluatePosition(t);
            // Use standard Unity Instantiate method for local instantiation
            GameObject newCoin = Instantiate(Resources.Load("c3") as GameObject, pointOnSpline, Quaternion.identity);
            newCoin.name = "Spline_" + t;
            newCoin.transform.SetParent(parentObject, false);
            instantiatedCoin.Add(newCoin);
        }
    }

   



    [PunRPC]
    void ShowWinnerRPC(string winnerName, int coinsCollected)
    {
        // Display the winner's name and coin count
        Debug.Log($"Winner: {winnerName} with {coinsCollected} coins collected!");
        // Update the UI or show a message to all players
        winner.text = $"Winner: {winnerName} with {coinsCollected} coins!";

        if(winnerName== PhotonNetwork.NickName)
        {

            WinnerScreenNickName.text = PhotonNetwork.NickName;
            WinnerScreenTotalCoin.text = coinsCollected.ToString();
            soundSource.PlayOneShot(winnerSound);

            
            WinnerScreen.gameObject.SetActive(true);


            Camera.main.gameObject.SetActive(false);

        }
        else
        {
            GameOverScreenNickName.text = PhotonNetwork.NickName;
            GameOverScreenPlayerScore.text = coinCollectedNum.text;

            GameOverScreenWinnerName.text = winnerName; 
            GameOverScreenWinnerScore.text = coinsCollected.ToString();
            soundSource.PlayOneShot(loserSound);
            GameOverScreen.gameObject.SetActive(true);
            Camera.main.gameObject.SetActive(false);

        }


    }

    [PunRPC]
    void DeactivateCollectibleRPC(string colName)
    {

        Transform childObjTransform = transform.Find(colName);

        if (childObjTransform != null)
        {
            childObjTransform.gameObject.SetActive(false);
        }

    }


   


}


