using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.Interfaces;
using GameServer.Core.Models;
using GameServer.Logic.Game;
namespace GameServer.Logic.Models
{
    public class GameSession : IGameSession
    {

        IGameServer _gameServer;



        public Room Room { get; }

        public int RoomId => Room.Id;

        public IReadOnlyList<Player> Players => Room.Players;

        public GameStatus Status { get; private set; }

        public GameSession(Room room,IGameServer gameServer) 
        {  
            Room = room;
            _gameServer = gameServer;
        }

        public event EventHandler<int>? GameFinished;
        public event EventHandler<(int PlayerId, string Move)>? PlayerMadeMove;
        public event EventHandler<(int? WinnerId, string Result)>? GameResultReady;

        public void HandleInput(int playerId, byte[] data)
        {
            throw new NotImplementedException();
        }

        public int[] GetPlayerIds()
        {
            return Players.Select(p=>p.Id).ToArray();
        }

        public void Start()
        {
            Status = GameStatus.InProgress;
        }

        public void ForceEnd()
        {
            Status = GameStatus.Finished;
            GameFinished?.Invoke(this, RoomId);
        }
    }
}
