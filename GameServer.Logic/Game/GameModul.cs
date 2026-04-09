using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Logic.Enums;
using GameServer.Logic.Models;
namespace GameServer.Logic.Game
{
    public static class GameModul
    {


        public static Motion? MakeAMove(Motion firstMotion,Motion secondMotion)
        {
            if (firstMotion.RSP == secondMotion.RSP) return null;
            
            bool firstWins = (firstMotion.RSP == RSP.rock && secondMotion.RSP == RSP.scissors) ||
                     (firstMotion.RSP == RSP.scissors && secondMotion.RSP == RSP.paper) ||
                     (firstMotion.RSP == RSP.paper && secondMotion.RSP == RSP.rock);

            return firstWins ? firstMotion : secondMotion;
        }
        public static string GetRSPName(RSP rsp)
        {
            return rsp switch
            {
                RSP.rock => "Камень",
                RSP.scissors => "Ножницы",
                RSP.paper => "Бумага",
                _ => "Неизвестно"
            };
        }

    }
}
