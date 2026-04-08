using GameServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Server.PlayerManagement
{
    public class PlayerManager : IPlayerManager
    {
        private readonly Dictionary<int, Player> _players = new();
        private readonly object _lock = new object();
        private int nextId = 1;
        public int PlayerCount
        {
            get
            {
                lock (_lock)
                {
                    return _players.Count;
                }
            }
        }

        public event EventHandler<Player>? PlayerAdded;
        public event EventHandler<int>? PlayerRemoved;

        public Player AddPlayer(int connectionId)
        {
            lock (_lock) 
            {
                if (_players.Values.Any(p => p.ConnectionId == connectionId))
                    throw new InvalidOperationException("Игрок уже существует");

                Player player = new(nextId, connectionId);
                _players.Add(player.Id, player);
                nextId++;
                PlayerAdded?.Invoke(this, player);
                return player;
            }
        }

        public bool RemovePlayer(int playerId)
        {
            lock (_lock)
            {
                if (!_players.ContainsKey(playerId))
                    return false;

                _players.Remove(playerId);
                PlayerRemoved?.Invoke(this, playerId);
                return true;
            }
        }

        public Player? GetPlayer(int playerId)
        {
            lock (_lock)
            {
                return _players.GetValueOrDefault(playerId);
            }
        }
        public Player? GetPlayerByConnectionId(int connectionId)
        {
            lock (_lock) 
            {
                return _players.Values.FirstOrDefault(p=>p.ConnectionId == connectionId);
            }
        }
        public IReadOnlyList<Player> GetAllPlayers()
        {
            lock (_lock)
            {
                return _players.Values.ToList();
            }
        }

        public bool ContainsPlayer(int playerId)
        {
            lock (_lock)
            {
                return _players.ContainsKey(playerId);
            }
        }

        public void ClearAllPlayers()
        {
            lock (_lock)
            {
                _players.Clear();
            }
        }


    }
}