using GameServer.Model;
using Proto;
using Summer;
using Summer.Network;

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
        MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(_SpaceEntitySyncRequest);

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
    
    private static void _SpaceEntitySyncRequest(Connection conn, SpaceEntitySyncRequest msg)
    {
        //获取当前角色
        var sp = conn.Get<Room>();
        sp.UpdateEntity(msg.EntitySync);
    }
    
}