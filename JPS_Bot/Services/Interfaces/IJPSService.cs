using JPS_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPS_Bot.Services.Interfaces
{
    public interface IJPSService
    {
        /// <summary>
        /// Consults and calculates the rewards obtained according to the lotterState argument.
        /// </summary>
        /// <param name="lotteryState">The lotteryState with the data to consult</param>
        /// <returns>A LotterRewardInfo object with the data. If an error occurs, returns null.</returns>
        Task<LotteryRewardInfo> CalculateRewards(LotteryState lotteryState);
    }
}
