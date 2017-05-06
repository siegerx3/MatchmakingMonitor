using MatchingMakingMonitor.SocketIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MatchingMakingMonitor
{
	/// <summary>
	/// Interaction logic for QrCode.xaml
	/// </summary>
	public partial class QrCodeWindow : Window
	{
		private SocketIOService socketIOService;
		public QrCodeWindow(SocketIOService socketIOService)
		{
			this.socketIOService = socketIOService;
			InitializeComponent();
		}
	}
}
