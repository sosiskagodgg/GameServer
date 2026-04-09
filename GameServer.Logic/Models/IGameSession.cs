using GameServer.Core.Models;
using GameServer.Logic.Models;

namespace GameServer.Logic.Game
{
    /// <summary>
    /// Игровая сессия для конкретной игры (камень-ножницы-бумага)
    /// </summary>
    public interface IGameSession
    {
        /// <summary>
        /// ID комнаты, в которой проходит игра
        /// </summary>
        int RoomId { get; }

        /// <summary>
        /// Список игроков в игре
        /// </summary>
        IReadOnlyList<Player> Players { get; }

        /// <summary>
        /// Статус игры (ожидание, активна, завершена)
        /// </summary>
        GameStatus Status { get; }

        /// <summary>
        /// Обработать ход игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <param name="data">Данные хода</param>
        Task HandleInput(int playerId, byte[] data);

        /// <summary>
        /// Получить ID всех игроков в игре
        /// </summary>
        int[] GetPlayerIds();

        /// <summary>
        /// Начать игру
        /// </summary>
        Task Start();

        /// <summary>
        /// Завершить игру досрочно
        /// </summary>
        Task ForceEnd();

        /// <summary>
        /// Событие при завершении игры
        /// </summary>
        event EventHandler<int>? GameFinished;

        /// <summary>
        /// Событие когда игрок сделал ход
        /// </summary>
        event EventHandler<Motion>? PlayerMadeMove;

        /// <summary>
        /// Событие когда определен победитель
        /// </summary>
        event EventHandler<Motion?>? GameResultReady;
    }

    /// <summary>
    /// Статус игры
    /// </summary>
    public enum GameStatus
    {
        WaitingForPlayers,  // Ожидание игроков
        InProgress,         // В процессе (ждутся ходы)
        Finished            // Завершена
    }
}