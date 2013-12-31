using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Dots
{
	/// <summary>
	/// Показывает диалог с настройкой параметров новой игры 
	/// (тип игры, размер поля и т.д.)
	/// Возвращает DialogResult.OK если юзер запускает игру
	/// и DialogResult.Cancel если юзер отменяет игру
	/// </summary>
	public class frmNewGame : System.Windows.Forms.Form
	{

		/// <summary>Какой тип игры выбрал пользователь.</summary>
		public frmGameScreen.GameType GameType;

		/// <summary>Ширина поля выбранная пользователем</summary>
		public Size FieldSize;

		#region Contorls
		private System.Windows.Forms.RadioButton radSinglePlayer;
		private System.Windows.Forms.RadioButton radHotSeat;
		private System.Windows.Forms.Button btnStartGame;
		private System.Windows.Forms.GroupBox grpGameMode;
		private System.Windows.Forms.Label lblWidth;
		private System.Windows.Forms.Label lblHeight;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.GroupBox grpFieldSize;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		public frmNewGame()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.radSinglePlayer = new System.Windows.Forms.RadioButton();
			this.radHotSeat = new System.Windows.Forms.RadioButton();
			this.btnStartGame = new System.Windows.Forms.Button();
			this.grpGameMode = new System.Windows.Forms.GroupBox();
			this.lblWidth = new System.Windows.Forms.Label();
			this.lblHeight = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.grpFieldSize = new System.Windows.Forms.GroupBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpGameMode.SuspendLayout();
			this.grpFieldSize.SuspendLayout();
			this.SuspendLayout();
			// 
			// radSinglePlayer
			// 
			this.radSinglePlayer.Checked = true;
			this.radSinglePlayer.Location = new System.Drawing.Point(24, 24);
			this.radSinglePlayer.Name = "radSinglePlayer";
			this.radSinglePlayer.Size = new System.Drawing.Size(160, 24);
			this.radSinglePlayer.TabIndex = 0;
			this.radSinglePlayer.TabStop = true;
			this.radSinglePlayer.Text = "Одиночная игра";
			// 
			// radHotSeat
			// 
			this.radHotSeat.Location = new System.Drawing.Point(24, 64);
			this.radHotSeat.Name = "radHotSeat";
			this.radHotSeat.Size = new System.Drawing.Size(160, 24);
			this.radHotSeat.TabIndex = 1;
			this.radHotSeat.Text = "2 игрока";
			// 
			// btnStartGame
			// 
			this.btnStartGame.Location = new System.Drawing.Point(52, 128);
			this.btnStartGame.Name = "btnStartGame";
			this.btnStartGame.Size = new System.Drawing.Size(160, 32);
			this.btnStartGame.TabIndex = 2;
			this.btnStartGame.Text = "Играть";
			this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
			// 
			// grpGameMode
			// 
			this.grpGameMode.Controls.Add(this.radSinglePlayer);
			this.grpGameMode.Controls.Add(this.radHotSeat);
			this.grpGameMode.Location = new System.Drawing.Point(16, 8);
			this.grpGameMode.Name = "grpGameMode";
			this.grpGameMode.Size = new System.Drawing.Size(200, 104);
			this.grpGameMode.TabIndex = 3;
			this.grpGameMode.TabStop = false;
			this.grpGameMode.Text = "Режим игры";
			// 
			// lblWidth
			// 
			this.lblWidth.Location = new System.Drawing.Point(8, 32);
			this.lblWidth.Name = "lblWidth";
			this.lblWidth.Size = new System.Drawing.Size(80, 20);
			this.lblWidth.TabIndex = 4;
			this.lblWidth.Text = "Ширина поля";
			this.lblWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblHeight
			// 
			this.lblHeight.Location = new System.Drawing.Point(16, 64);
			this.lblHeight.Name = "lblHeight";
			this.lblHeight.Size = new System.Drawing.Size(72, 20);
			this.lblHeight.TabIndex = 5;
			this.lblHeight.Text = "Высота поля";
			this.lblHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(96, 32);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(88, 20);
			this.txtWidth.TabIndex = 6;
			this.txtWidth.Text = "20";
			// 
			// txtHeight
			// 
			this.txtHeight.Location = new System.Drawing.Point(96, 64);
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(88, 20);
			this.txtHeight.TabIndex = 7;
			this.txtHeight.Text = "20";
			// 
			// grpFieldSize
			// 
			this.grpFieldSize.Controls.Add(this.lblWidth);
			this.grpFieldSize.Controls.Add(this.txtWidth);
			this.grpFieldSize.Controls.Add(this.lblHeight);
			this.grpFieldSize.Controls.Add(this.txtHeight);
			this.grpFieldSize.Location = new System.Drawing.Point(232, 8);
			this.grpFieldSize.Name = "grpFieldSize";
			this.grpFieldSize.Size = new System.Drawing.Size(192, 104);
			this.grpFieldSize.TabIndex = 8;
			this.grpFieldSize.TabStop = false;
			this.grpFieldSize.Text = "Размеры поля";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(228, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(160, 32);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmNewGame
			// 
			this.AcceptButton = this.btnStartGame;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(440, 174);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.grpFieldSize);
			this.Controls.Add(this.grpGameMode);
			this.Controls.Add(this.btnStartGame);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frmNewGame";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Новая игра";
			this.grpGameMode.ResumeLayout(false);
			this.grpFieldSize.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnStartGame_Click(object sender, System.EventArgs e) {
			const string ERR_MSG_TXT = "Неверный формат строки";

			try {
				FieldSize.Width = int.Parse(txtWidth.Text);
			}
			catch {
				MessageBox.Show(ERR_MSG_TXT);
			}

			try {
				FieldSize.Height = int.Parse(txtHeight.Text);
			}
			catch {
				MessageBox.Show(ERR_MSG_TXT);
			}
			
			GameType = radSinglePlayer.Checked ? frmGameScreen.GameType.Single :
				frmGameScreen.GameType.HotSeat;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
