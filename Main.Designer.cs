namespace VEMO_PlanetCNC
{
	partial class Main
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this._result = new System.Windows.Forms.ListBox();
			this.button1 = new System.Windows.Forms.Button();
			this._buttonsPanel = new System.Windows.Forms.Panel();
			this._dummyLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _result
			// 
			this._result.FormattingEnabled = true;
			this._result.IntegralHeight = false;
			this._result.ItemHeight = 25;
			this._result.Location = new System.Drawing.Point(55, 599);
			this._result.Name = "_result";
			this._result.Size = new System.Drawing.Size(1821, 504);
			this._result.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(0, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			// 
			// _buttonsPanel
			// 
			this._buttonsPanel.AutoScroll = true;
			this._buttonsPanel.Location = new System.Drawing.Point(55, 157);
			this._buttonsPanel.Name = "_buttonsPanel";
			this._buttonsPanel.Size = new System.Drawing.Size(1821, 242);
			this._buttonsPanel.TabIndex = 2;
			// 
			// _dummyLabel
			// 
			this._dummyLabel.AutoSize = true;
			this._dummyLabel.Location = new System.Drawing.Point(129, 477);
			this._dummyLabel.Name = "_dummyLabel";
			this._dummyLabel.Size = new System.Drawing.Size(146, 25);
			this._dummyLabel.TabIndex = 3;
			this._dummyLabel.Text = "_dummyLabel";
			this._dummyLabel.Visible = false;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(774, 529);
			this.Controls.Add(this._dummyLabel);
			this.Controls.Add(this._buttonsPanel);
			this.Controls.Add(this._result);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Main";
			this.Text = "PlanetCNC interaction test";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ListBox _result;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel _buttonsPanel;
		private System.Windows.Forms.Label _dummyLabel;
	}
}

