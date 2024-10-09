using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviourPunCallbacks

{
    public TextMeshProUGUI networkUpdate;
    public TextMeshProUGUI networkUpdate2;
    public TextMeshProUGUI networkUpdate3;
  

    public TextMeshProUGUI networkUpdate5;

    [SerializeField] private Transform parentObject; // Assign this to the desired parent object in the inspecto


    string collectabelName;

    public AudioSource soundSource;
    public AudioClip coinSound;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();

    }

    // Update is called once per frame
    void Update()
    {


        networkUpdate.text = PhotonNetwork.CurrentRoom.Name;


        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit Hit;
            if (Physics.Raycast(ray, out Hit))
            {
                collectabelName = Hit.transform.name;


                Transform childObjTransform = transform.Find(collectabelName);
                PhotonView childPhotonView = childObjTransform.GetComponent<PhotonView>();

                soundSource.PlayOneShot(coinSound);



                PlayerInventory playerInventory = GetComponent<PlayerInventory>();
                playerInventory.ItemsCollected();
                networkUpdate2.text = "Locally detected : " + childPhotonView.ViewID;
                // childObjTransform.gameObject.SetActive(false);



                // Call the RPC and pass the PhotonView ID as a parameter
                photonView.RPC("DeactivateCollectibleRPC", RpcTarget.AllBuffered, childPhotonView.ViewID, collectabelName);
                networkUpdate3.text = "RPC is sent out " + childPhotonView.ViewID + "," + collectabelName;




            }





        }



    }

    public override void OnJoinedRoom()
    {





        GameObject newObject = PhotonNetwork.Instantiate("c4", transform.position, transform.rotation);





        newObject.transform.SetParent(parentObject, false);
       
        


        newObject.transform.position = new Vector3(-2.36999989f, -0.785000026f, -2.38000011f);
        




        string positionString = newObject.transform.localScale.ToString();





        networkUpdate5.text = "Cap3 Position: " + positionString;

        // Set the parent of the instantiated object
        // Make sure that this does not conflict with any network synchronization





    }


    [PunRPC]
    void DeactivateCollectibleRPC(int viewID, string colName)
    {
        networkUpdate3.text = "RPC Received : " + colName;

        Transform childObjTransform = transform.Find(colName);

        childObjTransform.gameObject.SetActive(false);

        /*PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            networkUpdate3.text = "Remote Item found:  " + viewID;
            targetView.gameObject.SetActive(false);


        }
        else
        {
            networkUpdate3.text = "NO ID found: ";
        }*/
    }

    internal static void LoadScene(string v)
    {
        throw new NotImplementedException();
    }
}