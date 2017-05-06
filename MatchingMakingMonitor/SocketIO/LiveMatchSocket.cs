using System;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using MatchingMakingMonitor.Models;
using System.Collections.Generic;

namespace MatchingMakingMonitor.SocketIO
{
	public class LiveMatchSocket
	{
		private Socket socket;

		private ReplaySubject<object> onTokenChanged;
		private ReplaySubject<object> onStatsRequested;

		public LiveMatchSocket(Socket socket)
		{
			this.socket = socket;

			this.onStatsRequested = new ReplaySubject<object>();
			this.socket.On("statsRequested", () =>
			{
				this.onStatsRequested.OnNext(null);
			});
		}

		public void SetToken(string token)
		{
			socket.Emit("setToken", token);
		}

		public void SendStats(List<DisplayPlayer> stats)
		{
			socket.Emit("sendStats", JsonConvert.SerializeObject(stats));
		}

		public IObservable<object> OnStatsRequested
		{
			get
			{
				return this.onStatsRequested.AsObservable();
			}
		}
	}
}
