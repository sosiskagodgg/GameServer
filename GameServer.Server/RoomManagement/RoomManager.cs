using GameServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Server.RoomManagement
{
    public class RoomManager : IRoomManager
    {
        private List<Room> _rooms = new();
        private object _lock = new object();
        private int _nextId = 1;

        public event EventHandler<(int PlayerId, int RoomId)>? PlayerJoinedRoom;
        public event EventHandler<(int PlayerId, int RoomId)>? PlayerLeftRoom;
        public event EventHandler<int>? RoomCreated;
        public event EventHandler<int>? RoomDeleted;

        public void ClearAllRooms()
        {
            lock (_lock)
            {
                _rooms.Clear();
            }
        }

        public int CreateRoom()
        {
            lock( _lock)
            {
                var room = new Room(_nextId);
                _rooms.Add(room);
                _nextId++;
                RoomCreated?.Invoke(this, room.Id);
                return room.Id;
            }
        }

        public bool DeleteRoom(int roomId)
        {
            try
            {
                lock(_lock)
                {
                    _rooms.Remove(_rooms.First(r => r.Id == roomId));
                    RoomDeleted?.Invoke(this, roomId);
                    return true;
                }
            }
            catch
            {
                return false;
            }
             
        }

        public IReadOnlyList<Room>? GetAllRooms()
        {
            lock (_lock) 
            { 
                return _rooms.AsReadOnly();
            }
            
        }

        public Room? GetPlayerRoom(int playerId)
        {
            lock (_lock)
            {
                return _rooms.FirstOrDefault(r => r.Players.FirstOrDefault(i => i.Id == playerId) != null);
            }
        }
        public IReadOnlyList<Player>? GetPlayersInRoom(int roomId)
        {
            lock (_lock) 
            {
                return _rooms.FirstOrDefault(r=>r.Id==roomId)?.Players;
            }
        }

        public Room? GetRoom(int roomId)
        {
            lock(_lock)
            {
                return _rooms.FirstOrDefault(r=>r.Id==roomId);
            }
        }

        public bool IsPlayerInAnyRoom(int playerId)
        {
            if(GetPlayerRoom(playerId)!=null)
                return true;
            else
                return false;
        }

        public bool JoinRoom(Player player, int roomId)
        {
            lock (_lock)
            {
                try
                {
                    var room = _rooms.FirstOrDefault(r => r.Id == roomId);
                    room?.Join(player);
                    PlayerJoinedRoom?.Invoke(this,(player.Id, roomId));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool LeaveRoom(Player player)
        {
            lock (_lock)
            {
                try
                {
                    var room = GetPlayerRoom(player.Id);
                    if(room == null)return false;
                    room.Leave(player);
                    PlayerLeftRoom?.Invoke(this, (player.Id, room.Id));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
