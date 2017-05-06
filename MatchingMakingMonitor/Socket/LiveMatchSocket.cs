using System;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WowsAio.Models;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

namespace WowsAio.SocketIO
{
	public class LiveMatchSocket
	{
		private Socket socket;

		private ReplaySubject<object> onTokenChanged;
		private ReplaySubject<object> onStatsRequested;
		private ReplaySubject<Match> onStatsReceived;

		public LiveMatchSocket(Socket socket)
		{
			this.socket = socket;
			this.onTokenChanged = new ReplaySubject<object>();
			this.socket.On("tokenChanged", () =>
			{
				this.onTokenChanged.OnNext(null);
			});
			this.onStatsRequested = new ReplaySubject<object>();
			this.socket.On("statsRequested", () =>
			{
				this.onStatsRequested.OnNext(null);
			});
			this.onStatsReceived = new ReplaySubject<Match>();
			this.socket.On("statsReceived", (stats) =>
			{
				this.onStatsReceived.OnNext(JsonConvert.DeserializeObject<Match>((string)stats));
			});
		}

		public void SetToken(string token)
		{
			socket.Emit("setToken", token);
		}

		public void RequestStats()
		{
			socket.Emit("requestStats");
		}

		public void SendStats(Match stats)
		{
			socket.Emit("sendStats", JsonConvert.SerializeObject(stats));
		}

		public IObservable<object> OnTokenChanged
		{
			get
			{
				return this.onTokenChanged.AsObservable();
			}
		}
		public IObservable<object> OnStatsRequested
		{
			get
			{
				return this.onStatsRequested.AsObservable();
			}
		}
		public IObservable<Match> OnStatsReceived
		{
			get
			{
				return this.onStatsReceived.AsObservable();
			}
		}
	}
}
