using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using Serilog;
using Summer.Network;

namespace GameServer.Model
{
    public class Room
    {
        public const int maxPlayerCount = 2;
        public int Id { get; set; }
        public string Name { get; set; }

        private Dictionary<int, Character> CharacterDict = new Dictionary<int, Character>();

        private Dictionary<Connection, Character> ConnCharacter = new Dictionary<Connection, Character>();
        
        private LockStep _lockStep = new LockStep();
        
        public void CharacterJoin(Connection conn,Character character)
        {
            Log.Information("角色进入场景:" + character.entityId);
            conn.Set<Character>(character); //把角色存入连接
            conn.Set<Room>(this);          //把场景存入连接
            character.SpaceId = this.Id;
            
            CharacterDict[character.entityId] = character;
            character.conn = conn;
            if (!ConnCharacter.ContainsKey(conn))
            {
                ConnCharacter[conn] = character;
            }
            //把新进入的角色广播给其他玩家
            var resp = new SpaceCharactersEnterResponse();
            resp.SpaceId = this.Id; //场景ID
            resp.EntityList.Add(character.GetData());
            foreach(var kv in CharacterDict)
            {
                if (kv.Value.conn != conn)
                {
                    kv.Value.conn.Send(resp);
                }
            }
            //新上线的角色需要获取全部角色
            foreach (var kv in CharacterDict)
            {
                if (kv.Value.conn == conn) continue;
                resp.EntityList.Clear();
                resp.EntityList.Add(kv.Value.GetData());
                conn.Send(resp);
            }
            
            // lockStep
            if (CharacterDict.Count >= maxPlayerCount)
            {
                _lockStep.Start();
            }
        }
        
        /// <summary>
        /// 角色离开地图
        /// 客户端离线、切换地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterLeave(Connection conn,Character character)
        {
            Log.Information("角色离开场景:" + character.entityId);
            conn.Set<Room>(null);          //取消conn的场景记录
            CharacterDict.Remove(character.entityId);
            SpaceCharacterLeaveResponse resp = new SpaceCharacterLeaveResponse();
            resp.EntityId = character.entityId;
            foreach (var kv in CharacterDict)
            {
                kv.Value.conn.Send(resp);
            }
        }
        
        /// <summary>
        /// 广播更新Entity信息
        /// </summary>
        /// <param name="entitySync"></param>
        public void UpdateEntity(NEntitySync entitySync)
        {
            Log.Information("UpdateEntity {0}", entitySync);
            foreach (var kv in CharacterDict)
            {
                if(kv.Value.entityId == entitySync.Entity.Id)
                {
                    kv.Value.SetEntityData(entitySync.Entity);
                }
                else
                {
                    SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                    resp.EntitySync = entitySync;
                    kv.Value.conn.Send(resp);
                }
            }
        }
        
    }
}
