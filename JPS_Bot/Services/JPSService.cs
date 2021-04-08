using JPS_Bot.Models;
using JPS_Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace JPS_Bot.Services
{
    public class JPSService : IJPSService
    {
        private const string BaseURL = "https://integrations.jps.go.cr/api/Sorteos";
        private readonly IHttpClientFactory _httpClientFactory;

        public JPSService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<LotteryRewardInfo> CalculateRewards(LotteryState lotteryState)
        {
            var url = $"{BaseURL}/GetSorteoDevuelvePremios?TipoLoteria=N&" +
                $"NumeroSorteo={lotteryState.LotteryNumber}&" +
                $"Serie={lotteryState.FractionSerie}&" +
                $"Numero={lotteryState.FractionNumber}";

            using var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync(requestUri: url);

                // Check if the request was success
                if (!response.IsSuccessStatusCode) return null;

                var standarResponse = JsonSerializer.Deserialize<JpsStandarResponse>(
                    json: await response.Content.ReadAsStringAsync(),
                    options: new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                    }
                );

                // Calculate the amount earned based on each "MontoUnitario"
                var amountEarned = standarResponse.Result.Sum(result => result.MontoUnitario);

                return new LotteryRewardInfo()
                {
                    LotteryInfo = lotteryState,
                    AmountEarned = (long)amountEarned
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
