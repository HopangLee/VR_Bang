using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;

// 전체적인 게임 진행 관리
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject playerPrefab;
    public Text msgList; // 필요 x
    public InputField ifSendMsg; // 필요 x
    public Text playerCount;

    // 외부에서 싱글톤 오브젝트를 가져올 때 사용할 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreatePlayer();

        PhotonNetwork.IsMessageQueueRunning = true;
        Invoke("CheckPlayerCount", 0.5f);
    }

    void CreatePlayer(){
        float x = 0;
        float z = 0;        

        // 6명 -> 360도 나누기 6 = 60도
        int radius = 3;

        int r = 0;

        Vector3 pos;

        /*
        while(true){
            x = radius * Mathf.Cos(r * Mathf.PI * 1 / 3);
            z = radius * Mathf.Sin(r * Mathf.PI * 1 / 3);

            pos = new Vector3(x, 0.5f, z);

            Collider[] hitColliders = Physics.OverlapSphere(pos, 2, 1 << 8);

            if(hitColliders.Length == 0 || r == 6){
                Debug.Log("r: " + r);
                break;
            }
            r++;
        }
        */

        r = PhotonNetwork.PlayerList.Length;

        x = radius * Mathf.Cos(r * Mathf.PI * 1 / 3);
        z = radius * Mathf.Sin(r * Mathf.PI * 1 / 3);
        
        pos = new Vector3(x, 1f, z);

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
    }

    void Update()
    {
        
    }

    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {

        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨

        }
    }

    //msg를 RPC의 버퍼에 보내준다.
    public void OnSendChatMsg()
    {
        string msg = string.Format("[{0}] {1}"
                                  , PhotonNetwork.LocalPlayer.NickName
                                  , ifSendMsg.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);

        //    ifSendMsg.text = "";
        //    ifSendMsg.ActivateInputField();
    }

    //RPC의 버퍼에 있는 msg를 가져와서 
    [PunRPC]
    void ReceiveMsg(string msg)
    {
        msgList.text += "\n" + msg;
    }

    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("\n<color=#00ff00>[{0}]님이 입장했습니다.</color>"
                                    , newPlayer.NickName);

        photonView.RPC("ReceiveMsg", RpcTarget.Others, msg);
        //ReceiveMsg(msg);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckPlayerCount();

        string msg = string.Format("\n<color=#ff0000>[{0}]님이 퇴장했습니다.</color>"
                                    , otherPlayer.NickName);

        photonView.RPC("ReceiveMsg", RpcTarget.Others, msg);
        //ReceiveMsg(msg);
    }

    
    void CheckPlayerCount()
    {
        photonView.RPC("ChangePlayerCount", RpcTarget.All);
    }

    [PunRPC]
    void ChangePlayerCount(){
        int currPlayer = PhotonNetwork.PlayerList.Length;
        int maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCount.text = string.Format("[{0}/{1}]", currPlayer, maxPlayer);
    }
    
}
