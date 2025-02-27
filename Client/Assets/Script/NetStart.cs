using System;
using System.Collections;
using System.Collections.Generic;
using Proto;
using Summer.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetStart : MonoBehaviour {
    
    [Header("服务器信息")]
    public string host;
    public int port;

    [Header("登陆参数")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    
    // Start is called before the first frame update
    void Start()
    {
        NetClient.Instance.ConnectToServer(host, port);
        MessageRouter.Instance.Start(4);
        MessageRouter.Instance.Subscribe<GameEnterResponse>(_GameEnterResponse);
        MessageRouter.Instance.Subscribe<SpaceCharactersEnterResponse>(_SpaceCharactersEnterResponse);
        
    }

    // 加入游戏的响应(自己）
    private void _GameEnterResponse (Connection conn, 
        GameEnterResponse msg) {
        Debug.Log("加入游戏响应结果: " + msg.Success);
        if (msg.Success) {
            Debug.Log("角色信息: " + msg.Entity);
            // 加载预制体
            var e = msg.Entity;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameObject.Find("EnterGame").SetActive(false);
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Jammo");
                GameObject hero = spawnPlayer(prefab);
                hero.name = "Character(Player)";
                hero.GetComponent<Player>().isMine = true;
            });
            
        }
    }
    
    // 当有角色进入地图时候的通知，别人
    private void _SpaceCharactersEnterResponse(Connection conn, 
        SpaceCharactersEnterResponse msg) {
        Debug.Log("角色加入：地图=" + msg.SpaceId + ",entityId=" + msg.CharacterList[0].Id);
        var e = msg.CharacterList[0];
        
        // 加载预制体
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Jammo");
            GameObject hero = spawnPlayer(prefab);
            hero.name = "Character-" + e.Id;
            hero.GetComponent<Player>().isMine = false;
        });
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterGame()
    {
        GameEnterRequest request = new GameEnterRequest();
        request.CharacterId = 0;
        NetClient.Send(request);
    }
    

    void OnApplicationQuit () {
        NetClient.Close();
    }

    private GameObject spawnPlayer( GameObject prefab)
    {
        GameObject hero = Instantiate(prefab);

        return hero;
    }
}
