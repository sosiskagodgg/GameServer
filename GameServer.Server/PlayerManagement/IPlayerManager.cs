using GameServer.Core.Models;

namespace GameServer.Server.PlayerManagement
{
    public interface IPlayerManager
    {
        /// <summary>
        /// Добавить нового игрока
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <returns>True - успешно добавлен, False - игрок с таким Id уже существует</returns>
        bool AddPlayer(Player player);

        /// <summary>
        /// Удалить игрока
        /// </summary>
        /// <param name="playerId">ID игрока</param>
        /// <returns>True - успешно удалён, False - игрок не найден</returns>
        bool RemovePlayer(int playerId);

        /// <summary>
        /// Получить игрока по ID
        /// </summary>
        Player? GetPlayer(int playerId);

        /// <summary>
        /// Получить всех игроков
        /// </summary>
        IReadOnlyList<Player> GetAllPlayers();

        /// <summary>
        /// Проверить, существует ли игрок
        /// </summary>
        bool ContainsPlayer(int playerId);

        /// <summary>
        /// Получить количество онлайн игроков
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Очистить всех игроков
        /// </summary>
        void ClearAllPlayers();

        /// <summary>
        /// Событие при добавлении игрока
        /// </summary>
        event EventHandler<Player>? PlayerAdded;

        /// <summary>
        /// Событие при удалении игрока
        /// </summary>
        event EventHandler<int>? PlayerRemoved;
    }
}