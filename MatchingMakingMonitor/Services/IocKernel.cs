using MatchingMakingMonitor.SocketIO;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.Services
{
	public static class IoCKernel
	{
		private static IKernel kernel;

		public static void Init()
		{
			if (kernel == null)
			{
				kernel = new StandardKernel();

				kernel.Bind<LoggingService>().To<LoggingService>().InSingletonScope();
				kernel.Bind<SocketIOService>().To<SocketIOService>().InSingletonScope();
				kernel.Bind<Settings>().To<Settings>().InSingletonScope();
			}
		}

		public static T Get<T>()
		{
			return kernel.Get<T>();
		}
	}
}
