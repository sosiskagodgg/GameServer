using GameServer.Core.Interfaces;
using GameServer.Server;
using GameServer.Server.Lifecycle;
using GameServer.Server.Network;
using GameServer.Server.PlayerManagement;
using GameServer.Server.RoomManagement;
using GameServer.Server.Serialization;
using GameServer.Server.State;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.DI
{
    /// <summary>
    /// Скрипт регистрации серверных сервисов в DI контейнере
    /// </summary>
    public static class GameServerRegistry
    {
        public static IServiceCollection AddGameServer(this IServiceCollection services)
        {
            // State
            services.AddSingleton<IServerState, ServerState>();

            // Network
            services.AddSingleton<ITcpServer, TcpServer>();

            // Player management
            services.AddSingleton<IPlayerManager, PlayerManager>();

            // Room management
            services.AddSingleton<IRoomManager, RoomManager>();

            // Lifecycle
            services.AddSingleton<IServerLifecycle, ServerLifecycle>();

            services.AddScoped<IMessageConverter, MessageConverter>();

            services.AddSingleton<IGameServer, GameServer.Server.GameServer>();
            return services;
        }
    }
}