/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:51 PM
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess
{
	/// <summary>
	/// Description of SelectionForm.
	/// </summary>
	public partial class SelectionForm : Form
	{
		public SelectionForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void StartButtonClick(object sender, EventArgs e)
		{
			int option = 0;
			if (pvaiButton.Checked) option = 1;
			else if (aivaiButton.Checked) option = 2;
			else if (demogameButton.Checked) option = 3;
			MainForm m = new MainForm(option);
			m.Closed += (s, args) => this.Close();
			this.Hide();
			m.Show();
		}
	}
}
