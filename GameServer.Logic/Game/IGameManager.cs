using GameServer.Core.Models;
using GameServer.Logic.Models;

namespace GameServer.Logic.Game
{
    /// <summary>
    /// Управляет активными игровыми сессиями и маршрутизирует сообщения игроков
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Создать новую игру для двух игроков
        /// </summary>
        /// <param name="playerIds">ID двух игроков</param>
        /// <returns>true - игра создана, false - ошибка</returns>
        Task<bool> CreateGame(int[] playerIds);

        /// <summary>
        /// Завершить игру в комнате
        /// </summary>
        /// <param name="roomId">ID комнаты</param>
        void EndGame(int roomId);

        /// <summary>
        /// Обработать входящее сообщение от игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="data">Данные сообщения</param>
        void HandlePlayerInput(int playerId, byte[] data);

        /// <summary>
        /// Получить игру по ID игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <returns>Игровая сессия или null</returns>
        IGameSession? GetGameByPlayer(int playerId);

        /// <summary>
        /// Получить игру по ID комнаты
        /// </summary>
        /// <param name="roomId">ID комнаты</param>
        /// <returns>Игровая сессия или null</returns>
        IGameSession? GetGameByRoom(int roomId);

        /// <summary>
        /// Находится ли игрок в активной игре
        /// </summary>
        bool IsPlayerInGame(int playerId);

        /// <summary>
        /// Получить все активные игры
        /// </summary>
        IReadOnlyList<IGameSession> GetAllActiveGames();

        /// <summary>
        /// Количество активных игр
        /// </summary>
        int ActiveGamesCount { get; }

        /// <summary>
        /// Обработка отключения игрока
        /// </summary>
        /// <param name="playerId">ID отключившегося игрока</param>
        void HandlePlayerDisconnected(int playerId);

        /// <summary>
        /// Событие при создании игры
        /// </summary>
        event EventHandler<int>? GameCreated;

        /// <summary>
        /// Событие при завершении игры
        /// </summary>
        public event EventHandler<(int RoomId, List<int> PlayerIds)>? GameEnded;
    }
}