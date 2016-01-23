namespace Chess
{
    partial class promotionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.knightButton = new System.Windows.Forms.Button();
            this.bishopButton = new System.Windows.Forms.Button();
            this.rookButton = new System.Windows.Forms.Button();
            this.queenButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // knightButton
            // 
            this.knightButton.Location = new System.Drawing.Point(21, 49);
            this.knightButton.Name = "knightButton";
            this.knightButton.Size = new System.Drawing.Size(113, 113);
            this.knightButton.TabIndex = 0;
            this.knightButton.UseVisualStyleBackColor = true;
            this.knightButton.Click += new System.EventHandler(this.knightButton_Click);
            // 
            // bishopButton
            // 
            this.bishopButton.Location = new System.Drawing.Point(169, 49);
            this.bishopButton.Name = "bishopButton";
            this.bishopButton.Size = new System.Drawing.Size(113, 113);
            this.bishopButton.TabIndex = 1;
            this.bishopButton.UseVisualStyleBackColor = true;
            this.bishopButton.Click += new System.EventHandler(this.bishopButton_Click);
            // 
            // rookButton
            // 
            this.rookButton.Location = new System.Drawing.Point(317, 49);
            this.rookButton.Name = "rookButton";
            this.rookButton.Size = new System.Drawing.Size(113, 113);
            this.rookButton.TabIndex = 2;
            this.rookButton.UseVisualStyleBackColor = true;
            this.rookButton.Click += new System.EventHandler(this.rookButton_Click);
            // 
            // queenButton
            // 
            this.queenButton.Location = new System.Drawing.Point(472, 49);
            this.queenButton.Name = "queenButton";
            this.queenButton.Size = new System.Drawing.Size(113, 113);
            this.queenButton.TabIndex = 3;
            this.queenButton.UseVisualStyleBackColor = true;
            this.queenButton.Click += new System.EventHandler(this.queenButton_Click);
            // 
            // promotionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 230);
            this.Controls.Add(this.queenButton);
            this.Controls.Add(this.rookButton);
            this.Controls.Add(this.bishopButton);
            this.Controls.Add(this.knightButton);
            this.Name = "promotionForm";
            this.Text = "promotionForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button knightButton;
        private System.Windows.Forms.Button bishopButton;
        private System.Windows.Forms.Button rookButton;
        private System.Windows.Forms.Button queenButton;
    }
}