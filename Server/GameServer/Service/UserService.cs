using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer.Network;
using Proto;
using Serilog;
using GameServer.Model;
using GameServer.Mgr;
using GameServer.Service;
using Summer;

namespace GameServer.Network
{
    /// <summary>
    /// 玩家服务
    /// 注册，登录，创建角色，进入游戏
    /// </summary>
    public class UserService : Singleton<UserService>
    {
        public void Start()
        {
            MessageRouter.Instance.Subscribe<GameEnterRequest>(_GameEnterRequest);
        }
        
        private void _GameEnterRequest(Connection conn, GameEnterRequest msg)
        {
            Log.Information("有玩家进入游戏");
            int entityId = EntityManager.Instance.NewEntityId;
            Random rand = new Random();
            Vector3Int pos = new Vector3Int(rand.Next(-5,5), 0, rand.Next(-5, 5));
            pos *= 1000;
            Character character = new Character(entityId, pos, Vector3Int.zero);
            //通知玩家登录成功
            GameEnterResponse resp = new GameEnterResponse();
            resp.Success = true;
            resp.Entity = character.GetData().Entity;
            conn.Send(resp);
            //将新角色加入到地图
            var room = RoomService.Instance.GetRoom(6); //新手村
            room.CharacterJoin(conn, character);
        }
    }
}
