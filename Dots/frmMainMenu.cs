using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DimaSoft;

namespace Dots
{
	/// <summary>Главное меню</summary>
	public class frmMainMenu : System.Windows.Forms.Form
	{

		/// <summary>Результат работы диалога</summary>
		public enum DlgResult {
			NewGame, LoadGame, ExitGame
		}

		public DlgResult Result;

		#region Controls

		private DimaSoft.ImageButton ibtnNewGame;
		private DimaSoft.ImageButton ibtnLoadGame;
		private DimaSoft.ImageButton ibtnExit;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		public static void Main() {
			frmMainMenu		DlgMainMenu = null;
			frmNewGame		DlgNewGame = null;
			frmLoadGame		DlgLoadGame = null;
			frmGameScreen	DlgGameScreen = null;

		MainMenuShow:
			DlgMainMenu = new frmMainMenu();
			DlgMainMenu.ShowDialog();
			switch(DlgMainMenu.Result) {
				case frmMainMenu.DlgResult.NewGame:
					goto NewGameShow;
				case frmMainMenu.DlgResult.LoadGame:
					goto LoadGameShow;
				case frmMainMenu.DlgResult.ExitGame:
					Application.Exit();
					break;
			}
			return;

		NewGameShow:
			DlgNewGame = new frmNewGame();
			switch(DlgNewGame.ShowDialog()) {
				case DialogResult.OK:
					goto GameScreenShow;
				case DialogResult.Cancel:
					goto MainMenuShow;
			}
			return;
			
		LoadGameShow:
			DlgLoadGame = new frmLoadGame();
			switch(DlgLoadGame.ShowDialog()) {
				case DialogResult.OK:
					goto GameScreenShow;
				case DialogResult.Cancel:
					goto MainMenuShow;
			}
			return;

		GameScreenShow:
			DlgGameScreen = new frmGameScreen(DlgNewGame.FieldSize, DlgNewGame.GameType);
			DlgGameScreen.ShowDialog();
			switch(DlgGameScreen.Result) {
				case frmGameScreen.DlgResult.GameOver:
					MessageBox.Show("Game Over");
					break;
				case frmGameScreen.DlgResult.ExitToMainMenu:
					goto MainMenuShow;
				case frmGameScreen.DlgResult.Abort:
					Application.Exit();
					break;
			}
		}

		public frmMainMenu()
		{
			InitializeComponent();
		}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMainMenu));
			this.ibtnNewGame = new DimaSoft.ImageButton();
			this.ibtnLoadGame = new DimaSoft.ImageButton();
			this.ibtnExit = new DimaSoft.ImageButton();
			this.SuspendLayout();
			// 
			// ibtnNewGame
			// 
			this.ibtnNewGame.BackColor = System.Drawing.Color.Transparent;
			this.ibtnNewGame.Location = new System.Drawing.Point(120, 104);
			this.ibtnNewGame.MouseHoverImg = ((System.Drawing.Image)(resources.GetObject("ibtnNewGame.MouseHoverImg")));
			this.ibtnNewGame.Name = "ibtnNewGame";
			this.ibtnNewGame.NormalImg = ((System.Drawing.Image)(resources.GetObject("ibtnNewGame.NormalImg")));
			this.ibtnNewGame.Size = new System.Drawing.Size(169, 24);
			this.ibtnNewGame.TabIndex = 0;
			this.ibtnNewGame.Text = "imageButton1";
			this.ibtnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
			// 
			// ibtnLoadGame
			// 
			this.ibtnLoadGame.BackColor = System.Drawing.Color.Transparent;
			this.ibtnLoadGame.Enabled = false;
			this.ibtnLoadGame.Location = new System.Drawing.Point(120, 152);
			this.ibtnLoadGame.MouseHoverImg = ((System.Drawing.Image)(resources.GetObject("ibtnLoadGame.MouseHoverImg")));
			this.ibtnLoadGame.Name = "ibtnLoadGame";
			this.ibtnLoadGame.NormalImg = ((System.Drawing.Image)(resources.GetObject("ibtnLoadGame.NormalImg")));
			this.ibtnLoadGame.Size = new System.Drawing.Size(169, 24);
			this.ibtnLoadGame.TabIndex = 1;
			this.ibtnLoadGame.Text = "imageButton2";
			this.ibtnLoadGame.Click += new System.EventHandler(this.ibtnLoadGame_Click);
			// 
			// ibtnExit
			// 
			this.ibtnExit.BackColor = System.Drawing.Color.Transparent;
			this.ibtnExit.Location = new System.Drawing.Point(120, 200);
			this.ibtnExit.MouseHoverImg = ((System.Drawing.Image)(resources.GetObject("ibtnExit.MouseHoverImg")));
			this.ibtnExit.Name = "ibtnExit";
			this.ibtnExit.NormalImg = ((System.Drawing.Image)(resources.GetObject("ibtnExit.NormalImg")));
			this.ibtnExit.Size = new System.Drawing.Size(169, 24);
			this.ibtnExit.TabIndex = 2;
			this.ibtnExit.Text = "imageButton3";
			this.ibtnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// frmMainMenu
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(402, 304);
			this.Controls.Add(this.ibtnExit);
			this.Controls.Add(this.ibtnLoadGame);
			this.Controls.Add(this.ibtnNewGame);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmMainMenu";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "frmMainMenu";
			this.TransparencyKey = System.Drawing.Color.Black;
			this.Load += new System.EventHandler(this.frmMainMenu_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnExit_Click(object sender, System.EventArgs e) {
			Result = DlgResult.ExitGame;
			Close();
		}

		private void btnNewGame_Click(object sender, System.EventArgs e) {
			Result = DlgResult.NewGame;
			Close();
		}

		private void ibtnLoadGame_Click(object sender, System.EventArgs e) {
			Result = DlgResult.LoadGame;
			Close();
		}

		private void frmMainMenu_Load(object sender, System.EventArgs e) {
			string[] files = Directory.GetFiles(Dot.SAVED_GAMES_FOLDER);
			ibtnLoadGame.Enabled = files.Length == 0 ? false : true;
		}
	}
}
