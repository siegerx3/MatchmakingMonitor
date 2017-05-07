using MatchMakingMonitor.Services;
using MatchMakingMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MatchMakingMonitor.ViewModels
{
	public class ViewModelLocator
	{
		private DependencyObject dummy = new DependencyObject();
		public MainWindowViewModel MainWindowViewModel
		{
			get
			{
				if (IsInDesignMode())
				{
					return new MainWindowViewModel();
				}
				return IoCKernel.Get<MainWindowViewModel>();
			}
		}

		public HeaderViewModel HeaderViewModel
		{
			get
			{
				if (IsInDesignMode())
				{
					return new HeaderViewModel();
				}
				return IoCKernel.Get<HeaderViewModel>();
			}
		}

		public SubHeaderViewModel SubHeaderViewModel
		{
			get
			{
				if (IsInDesignMode())
				{
					return new SubHeaderViewModel();
				}
				return IoCKernel.Get<SubHeaderViewModel>();
			}
		}

		public SettingsWindowViewModel SettingsViewModel
		{
			get
			{
				if (IsInDesignMode())
				{
					return new SettingsWindowViewModel();
				}
				return IoCKernel.Get<SettingsWindowViewModel>();
			}
		}

		public StatsViewModel StatsViewModel
		{
			get
			{
				if (IsInDesignMode())
				{
					return new StatsViewModel();
				}
				return IoCKernel.Get<StatsViewModel>();
			}
		}

		private bool IsInDesignMode()
		{
			return DesignerProperties.GetIsInDesignMode(dummy);
		}
	}
}
