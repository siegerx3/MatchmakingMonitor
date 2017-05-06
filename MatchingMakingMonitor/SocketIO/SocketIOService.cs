using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.SocketIO
{
	public class SocketIOService
	{
		public LiveMatchSocket Hub;

		public IObservable<ConnectionState> StateChanged
		{
			get
			{
				return stateChanged.AsObservable();
			}
		}

		private ReplaySubject<ConnectionState> stateChanged;


		public ConnectionState ConnectionState { get { return this.connectionState; } }

		private ConnectionState connectionState;

		private Socket socketConnection;

		public SocketIOService()
		{
			socketConnection = IO.Socket("http://localhost:4000", new IO.Options() { AutoConnect = false });
			Hub = new LiveMatchSocket(socketConnection);

			stateChanged = new ReplaySubject<ConnectionState>();

			socketConnection.On(Socket.EVENT_CONNECT, () =>
			{
				connectionState = ConnectionState.Connected;
				stateChanged.OnNext(connectionState);
			});

			socketConnection.On(Socket.EVENT_DISCONNECT, () =>
			{
				connectionState = ConnectionState.Disconnected;
				stateChanged.OnNext(connectionState);
			});

			socketConnection.On(Socket.EVENT_RECONNECTING, () =>
			{
				connectionState = ConnectionState.Reconnecting;
				stateChanged.OnNext(connectionState);
			});

			socketConnection.On(Socket.EVENT_RECONNECT, () =>
			{
				connectionState = ConnectionState.Connected;
				stateChanged.OnNext(connectionState);
			});
		}

		public void Connect()
		{
			connectionState = ConnectionState.Connecting;
			stateChanged.OnNext(connectionState);
			socketConnection.Connect();
		}

		public void Disconnect()
		{
			socketConnection.Disconnect();
		}
		
	}

	public enum ConnectionState
	{
		Connecting,
		Reconnecting,
		Disconnected,
		Connected
	}
}
