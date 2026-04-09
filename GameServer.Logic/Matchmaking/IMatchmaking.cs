using GameServer.Core.Models;

namespace GameServer.Logic
{
    public interface IMatchmakingManager
    {
        /// <summary>
        /// Добавить игрока в очередь поиска
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <returns>true - добавлен, false - уже в очереди</returns>
        bool AddToQueue(Player player);

        /// <summary>
        /// Удалить игрока из очереди поиска
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <returns>true - удален, false - не найден</returns>
        bool RemoveFromQueue(int playerId);

        /// <summary>
        /// Проверить, находится ли игрок в очереди
        /// </summary>
        bool IsInQueue(int playerId);

        /// <summary>
        /// Получить количество игроков в очереди
        /// </summary>
        int QueueSize { get; }

        /// <summary>
        /// Получить всех игроков в очереди
        /// </summary>
        IReadOnlyList<Player> GetQueuePlayers();

        /// <summary>
        /// Очистить очередь
        /// </summary>
        void ClearQueue();

        /// <summary>
        /// Событие при успешном создании игры (найдена пара)
        /// </summary>
        event EventHandler<(Player Player1, Player Player2)>? MatchFound;

        /// <summary>
        /// Событие при выходе игрока из очереди
        /// </summary>
        event EventHandler<int>? PlayerLeftQueue;
    }
}