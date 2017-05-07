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

		private List<ShipInfo> shipInfos;
		public IReadOnlyList<ShipInfo> ShipInfos => shipInfos.AsReadOnly();

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
					shipInfos = await Task.Run(() => JsonConvert.DeserializeObject<List<ShipInfo>>(shipsJson));
				}
			} //end try
			catch (Exception e)
			{
				loggingService.Error("Exception Occurred While Retrieving Ships", e);
				shipInfos = new List<ShipInfo>();
			} //end catch
		}

		public async Task<IEnumerable<PlayerShip>> Players(Replay replay)
		{
			try
			{
				while (shipInfos == null)
				{
					await Task.Delay(1000);
				}
				var baseUrl = settings.Get<string>("BaseUrl" + settings.Get<string>("Region"));
				httpClient = new HttpClient();
				httpClient.BaseAddress = new Uri(baseUrl);

				var tasks = replay.vehicles.Select(v => getAsync(v)).ToList();
				var list = await Task.WhenAll(tasks);
				return list.Where(p => p != null);
			}
			catch (Exception e)
			{
				loggingService.Error("Error occured while fetching players", e);
				return new List<PlayerShip>();
			}
		}

		private async Task<PlayerShip> getAsync(Vehicle vehicle)
		{
			var shipInfo = ShipInfos.SingleOrDefault(s => s.ShipId == vehicle.shipId);

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
						var stats = await Task.Run(() => JsonConvert.DeserializeObject<PlayerStats>(shipJson));
						if (stats != null && stats.data != null && stats.data.Ships != null && stats.data.Ships.Any())
						{
							var ship = stats.data.Ships.First();
							return new PlayerShip(ship, player, shipInfo, vehicle.relation);
						}
					}
				}
			}
			return new PlayerShip(shipInfo)
			{
				Nickname = vehicle.name,
				Relation = vehicle.relation,
				IsPrivateOrHidden = true
			};
		}
	}
}
