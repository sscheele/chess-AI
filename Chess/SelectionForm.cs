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
        int option = 0;
        //option is a  number that tells us which colors are AIs
        //white is LSB, black is MSB
		public SelectionForm()
		{
			InitializeComponent();
		}
		void StartButtonClick(object sender, EventArgs e)
		{
			MainForm m = new MainForm(option);
			m.Closed += (s, args) => this.Close();
			this.Hide();
			m.Show();
		}

        private void settingsButton_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) option &= 2;
            if (comboBox1.SelectedIndex == 1) option |= 1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) option &= 1;
            if (comboBox2.SelectedIndex == 1) option |= 2;
        }
        
    }
}
