using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using Summer;

namespace GameServer.Mgr
{
    /// <summary>
    /// Entity管理器
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        private Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();

        public Entity CreateEntity()
        {
            lock (this)
            {
                var entity = new Entity(index++, Vector3Int.zero, Vector3Int.zero);
                AllEntities[entity.entityId] = entity;
                return entity;
            }
        }

        public int NewEntityId
        {
            get {
                lock(this){
                    return index++; 
                }
            }
        }

    }
}