using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Dots
{
	/// <summary>
	/// Диалог выбора сохраненной игры для загрузки. Результат выбора,
	/// строка с полным путем до сохраненной игры, помещается
	/// в поле SavedGamePath, при этом результат вызова диалога == DialogResult.OK. 
	/// Если юзер отменил загрузку результат DialogResultCancel
	/// </summary>
	public class frmLoadGame : System.Windows.Forms.Form
	{
		
		/// <summary>
		/// Результат вызова этого диалога, файл с сохраненем, который выбрал юзер
		/// </summary>
		public string SavedGamePath;

		/*string[] DaysOfWeek = {	"Понедельник", 
								"Вторник",
								"Среда",
								"Четверг",
								"Пятница",
								"Суббота",
								"Воскресенье"};*/

		string[] Games;


		#region Controls

		private System.Windows.Forms.ColumnHeader colNo;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colSaveDate;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.ListView lstGames;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		public frmLoadGame()
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
			this.lstGames = new System.Windows.Forms.ListView();
			this.colNo = new System.Windows.Forms.ColumnHeader();
			this.colName = new System.Windows.Forms.ColumnHeader();
			this.colSaveDate = new System.Windows.Forms.ColumnHeader();
			this.btnLoad = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstGames
			// 
			this.lstGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.colNo,
																					   this.colName,
																					   this.colSaveDate});
			this.lstGames.FullRowSelect = true;
			this.lstGames.GridLines = true;
			this.lstGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lstGames.Location = new System.Drawing.Point(16, 16);
			this.lstGames.MultiSelect = false;
			this.lstGames.Name = "lstGames";
			this.lstGames.Size = new System.Drawing.Size(368, 192);
			this.lstGames.TabIndex = 0;
			this.lstGames.View = System.Windows.Forms.View.Details;
			this.lstGames.ItemActivate += new System.EventHandler(this.lstGames_ItemActivate);
			// 
			// colNo
			// 
			this.colNo.Text = "№";
			this.colNo.Width = 34;
			// 
			// colName
			// 
			this.colName.Text = "Имя файла";
			this.colName.Width = 175;
			// 
			// colSaveDate
			// 
			this.colSaveDate.Text = "Время сохранения";
			this.colSaveDate.Width = 155;
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(296, 216);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(88, 23);
			this.btnLoad.TabIndex = 1;
			this.btnLoad.Text = "Загрузить";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// frmLoadGame
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(400, 254);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.lstGames);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmLoadGame";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "frmLoadGame";
			this.Load += new System.EventHandler(this.frmLoadGame_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmLoadGame_Load(object sender, System.EventArgs e) {
			string GamesFolder = Directory.GetCurrentDirectory() + 
				"\\" + Dot.SAVED_GAMES_FOLDER;
			string SearchPattern = "*." + Dot.SAVED_GAMES_EXTENSION;
			DateTime LastWriteTime;
			int Count = 0;
			ListViewItem CurrItem = null;

			Games = Directory.GetFiles(GamesFolder, SearchPattern);
			lstGames.BeginUpdate();
			foreach(string i in Games) {
				Count++;
				LastWriteTime = File.GetLastWriteTime(i);
				CurrItem = new ListViewItem();
				CurrItem.Text = Count.ToString();
				CurrItem.SubItems.Add(Path.GetFileNameWithoutExtension(i));
				CurrItem.SubItems.Add(LastWriteTime.ToString("f"));
				lstGames.Items.Add(CurrItem);
			}
			lstGames.EndUpdate();
		}

		private void lstGames_ItemActivate(object sender, System.EventArgs e) {
			SavedGamePath = Games[lstGames.SelectedIndices[0]];	
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnLoad_Click(object sender, System.EventArgs e) {
			if(lstGames.SelectedIndices.Count > 0)
				lstGames_ItemActivate(null, null);
			else 
				MessageBox.Show("Сначала выберите одну из сохраненных игр!", "!",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
