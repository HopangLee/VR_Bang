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

    public GameObject currentTurn;// 현재 차례의 플레이어
    public List<GameObject> playerList; // 현재 플레이어 모음 리스트 (살아있는 플레이어)

    public enum Phase {Phase0, Phase1, Phase2, Phase3};
    /* 
     * phase 0: 드로우 전에 사용되는 효과 (ex. 다이너마이트)
     * phase 1: 덱에서 카드 2장 가져옴 (캐릭터에 카드관련 특수능력 있을 시 사용)
     * phase 2: 패의 카드 사용
     * phase 3: 현재 라이프와 패의 카드 수가 같도록 함
     */

    public Phase currentPhase;

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

        r = PhotonNetwork.PlayerList.Length;

        x = radius * Mathf.Cos(r * Mathf.PI * 1 / 3);
        z = radius * Mathf.Sin(r * Mathf.PI * 1 / 3);
        
        pos = new Vector3(x, 1f, z);

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        GameObject o = PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);

        photonView.RPC("Addplayer", RpcTarget.All, o);
    }

    [PunRPC]
    void AddPlayer(GameObject o) // 리스트에 플레이어 추가하기
    {
        if (!playerList.Contains(o))
        {
            playerList.Add(o);
        }
        
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
            stream.SendNext(playerList);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨
            List<GameObject> o = (List<GameObject>)stream.ReceiveNext();

            if(playerList.Count < o.Count)
            {
                playerList = o;
            }
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
    
    [PunRPC]
    public void GameStart()
    {
        currentTurn = playerList[0]; // 현재 순서 = 첫번째 플레이어
        currentPhase = Phase.Phase1; // 현재 페이즈 = 페이즈1 (드로우)

        // 게임 시작후 실행될 함수
    }

    [PunRPC]
    public void startPhase0()
    {

    }

    [PunRPC]
    public void endPhase0()
    {

    }

    [PunRPC]
    public void startPhase1()
    {

    }

    [PunRPC]
    public void endPhase1()
    {

    }

    [PunRPC]
    public void startPhase2()
    {

    }

    [PunRPC]
    public void endPhase2()
    {

    }

    [PunRPC]
    public void startPhase3()
    {

    }

    [PunRPC]
    public void endPhase3()
    {

    }
}
