using MatchingMakingMonitor.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public class ApiService
	{
		private Settings settings;
		private LoggingService loggingService;

		private List<ShipModel> fallbackShips;
		public IReadOnlyList<ShipModel> FallbackShips => fallbackShips.AsReadOnly();

		private HttpClient httpClient;
		public ApiService(LoggingService loggingService, Settings settings)
		{
			this.settings = settings;
			this.loggingService = loggingService;
			Ships();
		}

		private string applicationId
		{
			get
			{
				return settings.Get<string>("AppId" + settings.Get<string>("Region"));
			}
		}

		private async Task Ships()
		{
			try
			{
				var client = new HttpClient() { BaseAddress = new Uri("https://wowreplays.com") };

				var result = await client.PostAsync("/Home/GetShipsForMatchmakingMonitor", null);
				if (result.StatusCode == HttpStatusCode.OK)
				{
					var shipsJson = await result.Content.ReadAsStringAsync();
					fallbackShips = await Task.Run(() => JsonConvert.DeserializeObject<List<ShipModel>>(shipsJson));
				}
			} //end try
			catch (Exception ex)
			{
				loggingService.Log("Exception Occurred While Retrieving Ships:  " + ex.Message);
				fallbackShips = new List<ShipModel>();
			} //end catch
		}

		public async Task<IEnumerable<PlayerStatsByShip>> Stats(ReplayModel replay)
		{
			try
			{
				var baseUrl = settings.Get<string>("BaseUrl" + settings.Get<string>("Region"));
				httpClient = new HttpClient();
				httpClient.BaseAddress = new Uri(baseUrl);

				var tasks = replay.vehicles.Select(v => getAsync(v)).ToList();
				var list = await Task.WhenAll(tasks);
				return list.Where(s => s != null && s.data != null && s.data.Ships != null && s.data.Ships.Any());
			}
			catch (Exception e)
			{
				return new List<PlayerStatsByShip>();
			}
		}

		private async Task<PlayerStatsByShip> getAsync(Vehicle vehicle)
		{
			var playerResponse = await httpClient.GetAsync($"wows/account/list/?application_id={applicationId}&search={vehicle.name}");
			if (playerResponse.StatusCode == HttpStatusCode.OK)
			{
				var playerJson = await playerResponse.Content.ReadAsStringAsync();
				var players = await Task.Run(() => JsonConvert.DeserializeObject<WargamingSearch>(playerJson));

				var player = players.data.SingleOrDefault(p => p.nickname == vehicle.name);
				if (player != null)
				{
					var shipResponse = await httpClient.GetAsync($"wows/ships/stats/?application_id={applicationId}&account_id={player.account_id}&ship_id={vehicle.shipId}");
					if (shipResponse.StatusCode == HttpStatusCode.OK)
					{
						var shipJson = await shipResponse.Content.ReadAsStringAsync();
						shipJson = shipJson.Replace("\"" + player.account_id + "\":", "\"Ships\":");
						return await Task.Run(() => JsonConvert.DeserializeObject<PlayerStatsByShip>(shipJson));
					}
				}
			}
			return new PlayerStatsByShip();
		}
	}
}
