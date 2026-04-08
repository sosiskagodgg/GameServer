using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GameServer.Core.Models
{
    public class Room
    {
        public int Id { get; }
        private readonly List<Player> _players = new();
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        private object _lock = new object();

        public event EventHandler<Player>? PlayerJoined;
        public event EventHandler<Player>? PlayerLeft;

        public Room(int id)
        {
            Id = id;
        }
        public void Join(Player player)
        {
            lock (_lock)
            {
                if (_players.Contains(player))
                    throw new InvalidOperationException("Игрок уже находиться в этой комнате");
                if (_players.Count >= 2)
                    throw new InvalidOperationException("Комната переполненна");

                _players.Add(player);
            }
            PlayerJoined?.Invoke(this, player);
        }
        public void Leave(Player player)
        {
            lock (_lock)
            {
                if (!_players.Contains(player))
                    throw new InvalidOperationException("Игрока нету в этой комнате");
                _players.Remove(player);
            }
            PlayerLeft?.Invoke(this, player);
        }
    }
}
