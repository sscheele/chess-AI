/*
 * User: samsc
 * Date: 8/13/2015
 * Time: 1:55 PM
 */
using System;
using System.Windows.Forms;

namespace Chess
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SelectionForm());
		}
		
	}
}
