using GameServer.Core.Models;
using GameServer.Logic.Game;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic.Matchmaking
{
    public class MatchmakingManager : IMatchmakingManager
    {
        IGameManager _gameManager;
        ILogger<MatchmakingManager> _logger;

        public MatchmakingManager(IGameManager gameManager,ILogger<MatchmakingManager> logger)
        {
            _gameManager = gameManager;
            _logger = logger;
        }




        private List<Player> _players = [];
        private object _lock = new object();

        public int QueueSize => _players.Count;

        public event EventHandler<(Player Player1, Player Player2)>? MatchFound;
        public event EventHandler<int>? PlayerLeftQueue;

        public bool AddToQueue(Player player)
        {
            lock (_lock)
            {
                if(_players.Any(p=>p.Id==player.Id))return false;
                _players.Add(player);
                
            }
            CheckMatch();
            return true;
        }
        private async Task CheckMatch()
        {
            lock (_lock)
            {
                if (_players.Count >= 2)
                {
                    var firstPlayer = _players[0];
                    _players.RemoveAt(0);
                    var secondPlayer = _players[0];
                    _players.RemoveAt(0);

                    _ = _gameManager.CreateGame([firstPlayer.Id, secondPlayer.Id]);
                    MatchFound?.Invoke(this, (firstPlayer, secondPlayer));
                }
            }
        }


        public void ClearQueue()
        {
            lock (_lock)
            {
                var playerToRemove = _players.ToList();
                foreach (var p in playerToRemove)
                {
                    PlayerLeftQueue?.Invoke(this, p.Id);;
                }
                _players.Clear();
            }
        }

        public IReadOnlyList<Player> GetQueuePlayers()
        {
            lock (_lock)
            {
                return _players.AsReadOnly();
            }
        }

        public bool IsInQueue(int playerId)
        {
            lock (_lock)
            {
                return _players.Any(p => p.Id == playerId);
            }
        }

        public bool RemoveFromQueue(int playerId)
        {
            lock (_lock)
            {
                var player = _players.Find(p => p.Id == playerId);
                if (player != null) 
                {

                    _players.Remove(player);
                    PlayerLeftQueue?.Invoke(this, player.Id);
                    return true;
                }
                return false;
            }
        }
    }
}
