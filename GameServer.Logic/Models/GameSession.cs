using GameServer.Core.Interfaces;
using GameServer.Core.Models;
using GameServer.Logic.Enums;
using GameServer.Logic.Game;
using GameServer.Server.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GameServer.Logic.Models
{
    public class GameSession : IGameSession
    {

        IGameServer _gameServer;
        IMessageConverter _messageConverter;
        ILogger<GameSession> _logger;

        List<Motion> motions = new()
        {
            new Motion(),
            new Motion()
        };
        



        public Room Room { get; }

        public int RoomId => Room.Id;

        public IReadOnlyList<Player> Players => Room.Players;

        public GameStatus Status { get; private set; }

        public GameSession(Room room,IGameServer gameServer, IMessageConverter messageConverter,ILogger<GameSession> logger) 
        {  
            Room = room;
            motions[0].PlayerId = room.Players[0].Id;
            motions[1].PlayerId = room.Players[1].Id;
            _gameServer = gameServer;
            _messageConverter = messageConverter;
            _logger = logger;
        }

        public event EventHandler<int>? GameFinished;
        public event EventHandler<Motion>? PlayerMadeMove;
        public event EventHandler<Motion?>? GameResultReady;

        public async Task HandleInput(int playerId, byte[] data)
        {
            _logger.LogInformation("HandlePlayerInput: игрок {PlayerId}", playerId);
            if (!Players.Any(p=>p.Id == playerId)) 
            {
                _logger.LogError("Игрок {PlayerId} не состоит в комнате {RoomId}", playerId, RoomId);
            }


            string? request = Encoding.UTF8.GetString(data);
            _logger.LogInformation("Получен запрос: {Request}", request);
            if (request == null)
            {
                _logger.LogError("Запрос игрока не был извлечен");
                return;
                
            }
            

            if(Status == GameStatus.InProgress)
            {
                if(request == "1"|| request == "2" || request == "3")
                {
                    Motion? motion = motions.Find(m=>m.PlayerId==playerId);
                    if (motion == null) return;
                    motion.RSP = (RSP)Convert.ToInt32(request);
                    PlayerMadeMove?.Invoke(this, motion);
                    await SendData($"Вы выбрали {GameModul.GetRSPName(motion.RSP)}", playerId);
                }

                if (motions.All(m => m.RSP != 0))
                {
                    await SendData($"Игрок 1 - {GameModul.GetRSPName(motions[0].RSP)}\nИгрок 2 - {GameModul.GetRSPName(motions[1].RSP)}");

                    Motion? winner = GameModul.MakeAMove(motions[0], motions[1]);
                    if (winner == null) { await SendData("Ничья"); await ForceEnd(); GameResultReady?.Invoke(this, null); return; }
                    Status = GameStatus.Finished;
                    await SendData("Вы победили", winner.PlayerId);
                    await SendData("Вы проиграли",motions.First(m=>m.PlayerId!=winner.PlayerId).PlayerId);
                    GameResultReady?.Invoke(this,winner);
                    await ForceEnd();
                }
            }
            
        }
        


        private async Task SendData( string request,int playerId = 0)
        {
            _logger.LogInformation("Отправка: {Request} игроку {PlayerId}", request, playerId == 0 ? "ВСЕМ" : playerId);
            byte[] data = _messageConverter.ToBytes(request);
            if(playerId == 0)await _gameServer.SendToRoomAsync(RoomId, data);
            else await _gameServer.SendToPlayerAsync(playerId, data); 
        }




        public int[] GetPlayerIds()
        {
            return Players.Select(p=>p.Id).ToArray();
        }

        public async Task Start()
        {
            Status = GameStatus.InProgress;
            await SendData("Игра началась\n" +
                "1 - Камень\n" +
                "2 - Ножиницы\n" +
                "3 - Бумага\n");
        }

        public async Task ForceEnd()
        {
            Status = GameStatus.Finished;
            await SendData("Игра завершена");
            GameFinished?.Invoke(this, RoomId);
        }
    }
}
