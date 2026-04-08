using GameServer.Core.Models;

namespace GameServer.Server.RoomManagement
{
    public interface IRoomManager
    {
        /// <summary>
        /// Создать новую комнату
        /// </summary>
        int CreateRoom();

        /// <summary>
        /// Удалить комнату
        /// </summary>
        bool DeleteRoom(int roomId);

        /// <summary>
        /// Добавить игрока в комнату
        /// </summary>
        bool JoinRoom(Player player, int roomId);

        /// <summary>
        /// Удалить игрока из комнаты
        /// </summary>
        bool LeaveRoom(Player player);

        /// <summary>
        /// Получить комнату игрока
        /// </summary>
        Room? GetPlayerRoom(int playerId);

        /// <summary>
        /// Получить комнату по ID
        /// </summary>
        Room? GetRoom(int roomId);

        /// <summary>
        /// Получить все комнаты
        /// </summary>
        IReadOnlyList<Room>? GetAllRooms();

        /// <summary>
        /// Получить список игроков в комнате
        /// </summary>
        IReadOnlyList<Player>? GetPlayersInRoom(int roomId);

        /// <summary>
        /// Проверить, находится ли игрок в какой-либо комнате
        /// </summary>
        bool IsPlayerInAnyRoom(int playerId);

        /// <summary>
        /// Очистить все комнаты
        /// </summary>
        void ClearAllRooms();

        /// <summary>
        /// Событие при входе игрока в комнату
        /// </summary>
        event EventHandler<(int PlayerId, int RoomId)>? PlayerJoinedRoom;

        /// <summary>
        /// Событие при выходе игрока из комнаты
        /// </summary>
        event EventHandler<(int PlayerId, int RoomId)>? PlayerLeftRoom;

        /// <summary>
        /// Событие при создании комнаты
        /// </summary>
        event EventHandler<int>? RoomCreated;

        /// <summary>
        /// Событие при удалении комнаты
        /// </summary>
        event EventHandler<int>? RoomDeleted;
    }
}