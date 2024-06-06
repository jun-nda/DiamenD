using GameServer.Model;
using Proto;
using Summer;
using Summer.Network;
using PlayerInput = Proto.PlayerInputInfo;

namespace GameServer.Service;

/// <summary>
/// 房间服务
/// </summary>
public class RoomService : Singleton<RoomService>
{
    private Dictionary<int,Room> roomDict = new Dictionary<int, Room>();
    
    public void Start()
    {
        //位置同步请求
        //MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(_SpaceEntitySyncRequest);

        MessageRouter.Instance.Subscribe<PlayerInputSingle>(_OnPlayerInput);
        
        //测试房间
        var room = new Room
        {
            Name = "新手村",
            Id = 6 //新手村id
        };
        roomDict[room.Id] = room;

    }
    
    public Room GetRoom(int roomId)
    {
        return roomDict[roomId];
    }

    private static void _OnPlayerInput(Connection conn, PlayerInputSingle input)
    {
        Player player = conn.Get<Player>();
        Room room = conn.Get<Room>();
        LockStep lockStep = room.LockStep;
        player.Input.InputX = input.PlayerInputInfo.InputX;
        player.Input.InputZ = input.PlayerInputInfo.InputZ;
        var tick = input.Tick;

        // 一个Tick对应一个全体输入
        if (!lockStep.Tick2Inputs.ContainsKey(tick))
        {
            lockStep.Tick2Inputs.Add(tick, new Model.PlayerInput[Room.maxPlayerCount]);
        }
        lockStep.Tick2Inputs[tick][player.entityId] = player.Input;
        lockStep.CheckInput();
    }
    
    private static void _SpaceEntitySyncRequest(Connection conn, SpaceEntitySyncRequest msg)
    {
        //获取当前角色
        var sp = conn.Get<Room>();
        sp.UpdateEntity(msg.EntitySync);
    }
    
}