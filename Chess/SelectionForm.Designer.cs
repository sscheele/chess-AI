/*
 * User: samsc
 * Date: 9/22/2015
 * Time: 12:51 PM
 */
namespace Chess
{
	partial class SelectionForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RadioButton pvpButton;
		private System.Windows.Forms.RadioButton pvaiButton;
		private System.Windows.Forms.RadioButton demogameButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button settingsButton;
		private System.Windows.Forms.RadioButton aivaiButton;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.pvpButton = new System.Windows.Forms.RadioButton();
			this.pvaiButton = new System.Windows.Forms.RadioButton();
			this.demogameButton = new System.Windows.Forms.RadioButton();
			this.startButton = new System.Windows.Forms.Button();
			this.settingsButton = new System.Windows.Forms.Button();
			this.aivaiButton = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// pvpButton
			// 
			this.pvpButton.Checked = true;
			this.pvpButton.Location = new System.Drawing.Point(55, 24);
			this.pvpButton.Name = "pvpButton";
			this.pvpButton.Size = new System.Drawing.Size(104, 24);
			this.pvpButton.TabIndex = 0;
			this.pvpButton.TabStop = true;
			this.pvpButton.Text = "Player vs Player";
			this.pvpButton.UseVisualStyleBackColor = true;
			// 
			// pvaiButton
			// 
			this.pvaiButton.Location = new System.Drawing.Point(55, 68);
			this.pvaiButton.Name = "pvaiButton";
			this.pvaiButton.Size = new System.Drawing.Size(104, 24);
			this.pvaiButton.TabIndex = 1;
			this.pvaiButton.Text = "Player vs AI";
			this.pvaiButton.UseVisualStyleBackColor = true;
			// 
			// demogameButton
			// 
			this.demogameButton.Location = new System.Drawing.Point(55, 150);
			this.demogameButton.Name = "demogameButton";
			this.demogameButton.Size = new System.Drawing.Size(167, 24);
			this.demogameButton.TabIndex = 2;
			this.demogameButton.Text = "Demo Game (from text file)";
			this.demogameButton.UseVisualStyleBackColor = true;
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(171, 210);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(101, 39);
			this.startButton.TabIndex = 3;
			this.startButton.Text = "Start!";
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.StartButtonClick);
			// 
			// settingsButton
			// 
			this.settingsButton.Location = new System.Drawing.Point(12, 210);
			this.settingsButton.Name = "settingsButton";
			this.settingsButton.Size = new System.Drawing.Size(85, 39);
			this.settingsButton.TabIndex = 4;
			this.settingsButton.Text = "Settings";
			this.settingsButton.UseVisualStyleBackColor = true;
			// 
			// aivaiButton
			// 
			this.aivaiButton.Location = new System.Drawing.Point(55, 110);
			this.aivaiButton.Name = "aivaiButton";
			this.aivaiButton.Size = new System.Drawing.Size(167, 24);
			this.aivaiButton.TabIndex = 5;
			this.aivaiButton.Text = "AI vs AI (experimental)";
			this.aivaiButton.UseVisualStyleBackColor = true;
			// 
			// SelectionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.aivaiButton);
			this.Controls.Add(this.settingsButton);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.demogameButton);
			this.Controls.Add(this.pvaiButton);
			this.Controls.Add(this.pvpButton);
			this.Name = "SelectionForm";
			this.Text = "SelectionForm";
			this.ResumeLayout(false);

		}
	}
}
