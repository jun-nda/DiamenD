﻿using Proto;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    //角色
    public class Player : Actor
    {
        //当前角色的客户端连接
        public Connection conn;

        public PlayerInput Input;
        
        public Player(int id, Vector3Int position, Vector3Int direction) : base(id, position, direction)
        {

        }    
    }
}