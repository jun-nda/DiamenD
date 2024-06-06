using System.Diagnostics;
using Proto;
using Summer.Network;

namespace GameServer.Model;

public class LockStep
{
    public Connection Conn;
    
    private float _remainTime = 0f;
    private const float DeltaTime = 0.03f;
    private float _accmulatedTime = 0f;
    private int _curTick;

    public Dictionary<int, PlayerInput[]> Tick2Inputs = new Dictionary<int, PlayerInput[]>();

    
    public void Start()
    {

    }

    public void CheckInput()
    {
        if (Tick2Inputs.TryGetValue(_curTick, out var inputs))
        {
            if (inputs != null) {
                bool isFullInput = true;
                for (int i = 0; i < inputs.Length; i++) {
                    if (inputs[i] == null) {
                        isFullInput = false;
                        break;
                    }
                }

                if (isFullInput) {
                    BoardInputMsg(_curTick, inputs);
                    Tick2Inputs.Remove(_curTick);
                    _curTick++;
                }
            }
        }
    }

    public void BoardInputMsg(int curTick, PlayerInput[] inputs)
    {
        FramePlayerInputs playerInputForward = new FramePlayerInputs();
        playerInputForward.Tick = curTick;
        for (int i = 0; i < inputs.Length; i++)
        {
            PlayerInputInfo playerInputInfo = new PlayerInputInfo();
            playerInputInfo.InputX = (int)(inputs[i].InputX * 1000);
            playerInputInfo.InputZ = (int)(inputs[i].InputZ * 1000);
            playerInputForward.PlayerInputInfos.Add(playerInputInfo);

        }
        
        Conn.Send(playerInputForward);
    }

    
    public void Destroy()
    {
        _remainTime = 0f;
        _accmulatedTime = 0f;
    } 
}