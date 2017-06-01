using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models
{
	public class Form
	{
		public FormMode Mode { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
	}

	public enum FormMode
	{
		Bug = 0,
		Feature = 1,
	}
}
