using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DimaSoft;
using XPExplorerBar;

namespace Dots
{

	/// <summary>
	/// Игровой экран. В качестве результата работы возвращается поле Result
	/// </summary>
	public class frmGameScreen : System.Windows.Forms.Form
	{

		/// <summary>Режим игры (наприм. одиночный, multiplayer)</summary>
		[Serializable]
		public enum GameType {
			/// <summary>Режим игры против компьютера</summary>
			Single,
			/// <summary>Режим игры двоих игроков за одим компом.</summary>
			HotSeat
		}

		/// <summary>Возвращаемый в вызывающий код результат работы формы</summary>
		public enum DlgResult {
			/// <summary>Юзер прервал игру, т.е. закрыл приложение</summary>
			Abort,
			/// <summary>Юзер вышел в главное меню</summary>
			ExitToMainMenu,
			/// <summary>Игры закончена</summary>
			GameOver
		}
		
		/// <summary>Возвращаемый в вызывающий код результат работы формы</summary>
		public DlgResult Result;

		// ====================================================================

		/// <summary>
		/// Расстояние между соседними точками при отрисовке
		/// </summary>
		private const int CELL_SIZE = 20;

		/// <summary>Размеры игрового поля в клетках</summary>
		private Size FieldSize;

		/// <summary>Текущий тип игры</summary>
		private GameType gameType;

		/// <summary>
		/// Здесь храниться информация об точках. 
		/// Игровое поле состоит из матрицы точек. Если точка в клетке x, y не стоит
		/// Dots[x,y] == null.
		/// </summary>
		private Dot[,] Dots;

		private int _ActivePlayer = 0;

		/// <summary>
		/// Заштрихованные поля красного и синего игроков.
		/// Красный индекс 0, синий - 1. 
		/// Поле  - это объект класса CapturedField
		/// </summary>
		private ArrayList[] Fields = new ArrayList[2];

		/// <summary>
		/// Заштрихованные поля красного и синего игроков, 
		/// внутри которых нет вражеских точек. Не отрисовываются.
		/// Красный индекс 0, синий - 1. 
		/// Поле  - это объект класса CapturedField
		/// </summary>
		private ArrayList[] EmptyFileds = new ArrayList[2];

		/// <summary>
		/// Содержит все возможные контуры, для данной цепочки точек с
		/// вражескими точками внутри
		/// Ключ - это контур типа стэк из точек, 
		/// значение - это площадь данного контура (float). 
		/// Используется для того, чтобы из всех контуров выбрать самый большой
		/// по площади.
		/// </summary>
		private ListDictionary Contours;

		/// <summary>
		/// Содержит все возможные контуры, для данной цепочки точек,
		/// внутри которых нет вражеских точек.
		/// Ключ - это контур типа стэк из точек, 
		/// значение - это площадь данного контура (float). 
		/// </summary>
		private ListDictionary EmptyContours = new ListDictionary();

		bool LastContourIsEmpty;

		/// <summary>
		/// Статистика игроков
		/// </summary>
		private Statistics[] PlayerStat = new Statistics[2];

		#region Controls
		private System.Windows.Forms.Panel panGame;
		private DimaSoft.DrawBox ctrlDrawBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private XPExplorerBar.TaskPane GamePanel;
		private XPExplorerBar.Expando expActivePlayer;
		private System.Windows.Forms.Label lblActivePlayer;
		private System.Windows.Forms.RadioButton radRedPlayer;
		private System.Windows.Forms.RadioButton radBluePlayer;
		private XPExplorerBar.TaskItem tskSaveGame;
		private XPExplorerBar.TaskItem tskLoadGame;
		private XPExplorerBar.TaskItem tskNewGame;
		private XPExplorerBar.TaskItem tskExit;
		private XPExplorerBar.TaskItem tskReset;
		private XPExplorerBar.Expando expStatistics;
		private System.Windows.Forms.ColumnHeader colKey;
		private System.Windows.Forms.ColumnHeader colValue;
		private System.Windows.Forms.RadioButton radRedStat;
		private System.Windows.Forms.RadioButton radBlueStat;
		private System.Windows.Forms.ListView lstStat;
		private XPExplorerBar.TaskItem tskGiveUp;
		private XPExplorerBar.Expando expMenu;
		#endregion

		
		// ====================================================================

		/// <summary>
		/// Номер игрока который сейчас ходит.
		/// </summary>
		/// <remarks>
		/// В игре 2 игрока: 
		/// красный (№0) и синий (№1). Игра начинается с хода красного игрока
		/// </remarks>
		private int ActivePlayer {
			get {return _ActivePlayer;}
			set {
				switch(value) {
					case 0:
						radRedPlayer.Checked = true;
						radBluePlayer.Checked = false;
						radRedStat.Checked = true;
						break;
					case 1:
						radBluePlayer.Checked = true;
						radRedPlayer.Checked = false;
						radBlueStat.Checked = true;
						break;
				}
				_ActivePlayer = value;
			}
		}

		// ====================================================================

		public frmGameScreen(Size fieldSize, GameType game)
		{
			InitializeComponent();

			FieldSize = fieldSize;
			gameType = game;

			Dots = new Dot[FieldSize.Width, FieldSize.Height];
			Fields[0] = new ArrayList();
			Fields[1] = new ArrayList();
			EmptyFileds[0] = new ArrayList();
			EmptyFileds[1] = new ArrayList();
			ctrlDrawBox.Size = new Size((FieldSize.Width + 1) * CELL_SIZE,
				(FieldSize.Height + 1) * CELL_SIZE);
			ctrlDrawBox.DoubleBuffer = true;
			expStatistics_Resize(null, null);
			frmGameScreen_Resize(this, null);
			ActivePlayer = 0;
		}

		// --------------------------------------------------------------------

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

		// --------------------------------------------------------------------

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmGameScreen));
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "Территория",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "Полей",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "Пустых п-ей",
																													 ""}, -1);
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
																													 "Точек",
																													 ""}, -1);
			this.panGame = new System.Windows.Forms.Panel();
			this.ctrlDrawBox = new DimaSoft.DrawBox();
			this.GamePanel = new XPExplorerBar.TaskPane();
			this.expActivePlayer = new XPExplorerBar.Expando();
			this.lblActivePlayer = new System.Windows.Forms.Label();
			this.radRedPlayer = new System.Windows.Forms.RadioButton();
			this.radBluePlayer = new System.Windows.Forms.RadioButton();
			this.expMenu = new XPExplorerBar.Expando();
			this.tskSaveGame = new XPExplorerBar.TaskItem();
			this.tskLoadGame = new XPExplorerBar.TaskItem();
			this.tskNewGame = new XPExplorerBar.TaskItem();
			this.tskExit = new XPExplorerBar.TaskItem();
			this.tskReset = new XPExplorerBar.TaskItem();
			this.tskGiveUp = new XPExplorerBar.TaskItem();
			this.expStatistics = new XPExplorerBar.Expando();
			this.radRedStat = new System.Windows.Forms.RadioButton();
			this.radBlueStat = new System.Windows.Forms.RadioButton();
			this.lstStat = new System.Windows.Forms.ListView();
			this.colKey = new System.Windows.Forms.ColumnHeader();
			this.colValue = new System.Windows.Forms.ColumnHeader();
			this.panGame.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.GamePanel)).BeginInit();
			this.GamePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expActivePlayer)).BeginInit();
			this.expActivePlayer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expMenu)).BeginInit();
			this.expMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expStatistics)).BeginInit();
			this.expStatistics.SuspendLayout();
			this.SuspendLayout();
			// 
			// panGame
			// 
			this.panGame.AutoScroll = true;
			this.panGame.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.panGame.Controls.Add(this.ctrlDrawBox);
			this.panGame.Location = new System.Drawing.Point(208, 0);
			this.panGame.Name = "panGame";
			this.panGame.Size = new System.Drawing.Size(424, 432);
			this.panGame.TabIndex = 0;
			// 
			// ctrlDrawBox
			// 
			this.ctrlDrawBox.BackColor = System.Drawing.SystemColors.Window;
			this.ctrlDrawBox.Location = new System.Drawing.Point(0, 0);
			this.ctrlDrawBox.Name = "ctrlDrawBox";
			this.ctrlDrawBox.Size = new System.Drawing.Size(424, 432);
			this.ctrlDrawBox.TabIndex = 0;
			this.ctrlDrawBox.Text = "control1";
			this.ctrlDrawBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ctrlDrawBox_Paint);
			this.ctrlDrawBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ctrlDrawBox_MouseDown);
			// 
			// GamePanel
			// 
			this.GamePanel.AutoScroll = true;
			this.GamePanel.AutoScrollMargin = new System.Drawing.Size(12, 12);
			this.GamePanel.CustomSettings.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
			this.GamePanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.GamePanel.Expandos.AddRange(new XPExplorerBar.Expando[] {
																			 this.expActivePlayer,
																			 this.expMenu,
																			 this.expStatistics});
			this.GamePanel.Location = new System.Drawing.Point(0, 0);
			this.GamePanel.Name = "GamePanel";
			this.GamePanel.Size = new System.Drawing.Size(208, 464);
			this.GamePanel.TabIndex = 1;
			// 
			// expActivePlayer
			// 
			this.expActivePlayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.expActivePlayer.Animate = true;
			this.expActivePlayer.ExpandedHeight = 92;
			this.expActivePlayer.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.expActivePlayer.Items.AddRange(new System.Windows.Forms.Control[] {
																					   this.lblActivePlayer,
																					   this.radRedPlayer,
																					   this.radBluePlayer});
			this.expActivePlayer.Location = new System.Drawing.Point(12, 12);
			this.expActivePlayer.Name = "expActivePlayer";
			this.expActivePlayer.Size = new System.Drawing.Size(184, 92);
			this.expActivePlayer.TabIndex = 0;
			this.expActivePlayer.Text = "Активный игрок";
			// 
			// lblActivePlayer
			// 
			this.lblActivePlayer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.lblActivePlayer.Location = new System.Drawing.Point(16, 32);
			this.lblActivePlayer.Name = "lblActivePlayer";
			this.lblActivePlayer.Size = new System.Drawing.Size(120, 16);
			this.lblActivePlayer.TabIndex = 0;
			this.lblActivePlayer.Text = "Активный игрок:";
			// 
			// radRedPlayer
			// 
			this.radRedPlayer.AutoCheck = false;
			this.radRedPlayer.Checked = true;
			this.radRedPlayer.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.radRedPlayer.Location = new System.Drawing.Point(32, 48);
			this.radRedPlayer.Name = "radRedPlayer";
			this.radRedPlayer.Size = new System.Drawing.Size(104, 16);
			this.radRedPlayer.TabIndex = 1;
			this.radRedPlayer.Text = "Красный игрок";
			// 
			// radBluePlayer
			// 
			this.radBluePlayer.AutoCheck = false;
			this.radBluePlayer.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.radBluePlayer.Location = new System.Drawing.Point(32, 72);
			this.radBluePlayer.Name = "radBluePlayer";
			this.radBluePlayer.Size = new System.Drawing.Size(104, 16);
			this.radBluePlayer.TabIndex = 2;
			this.radBluePlayer.Text = "Синий игрок";
			// 
			// expMenu
			// 
			this.expMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.expMenu.Animate = true;
			this.expMenu.ExpandedHeight = 180;
			this.expMenu.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.expMenu.Items.AddRange(new System.Windows.Forms.Control[] {
																			   this.tskSaveGame,
																			   this.tskLoadGame,
																			   this.tskNewGame,
																			   this.tskExit,
																			   this.tskReset,
																			   this.tskGiveUp});
			this.expMenu.Location = new System.Drawing.Point(12, 116);
			this.expMenu.Name = "expMenu";
			this.expMenu.Size = new System.Drawing.Size(184, 180);
			this.expMenu.TabIndex = 1;
			this.expMenu.Text = "Меню";
			// 
			// tskSaveGame
			// 
			this.tskSaveGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskSaveGame.BackColor = System.Drawing.Color.Transparent;
			this.tskSaveGame.Image = ((System.Drawing.Image)(resources.GetObject("tskSaveGame.Image")));
			this.tskSaveGame.Location = new System.Drawing.Point(8, 104);
			this.tskSaveGame.Name = "tskSaveGame";
			this.tskSaveGame.Size = new System.Drawing.Size(120, 24);
			this.tskSaveGame.TabIndex = 0;
			this.tskSaveGame.Text = "Сохранить игру";
			this.tskSaveGame.Click += new System.EventHandler(this.tskSaveGame_Click);
			// 
			// tskLoadGame
			// 
			this.tskLoadGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskLoadGame.BackColor = System.Drawing.Color.Transparent;
			this.tskLoadGame.Image = ((System.Drawing.Image)(resources.GetObject("tskLoadGame.Image")));
			this.tskLoadGame.Location = new System.Drawing.Point(8, 128);
			this.tskLoadGame.Name = "tskLoadGame";
			this.tskLoadGame.Size = new System.Drawing.Size(120, 24);
			this.tskLoadGame.TabIndex = 1;
			this.tskLoadGame.Text = "Загрузить игру";
			this.tskLoadGame.Click += new System.EventHandler(this.tskLoadGame_Click);
			// 
			// tskNewGame
			// 
			this.tskNewGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskNewGame.BackColor = System.Drawing.Color.Transparent;
			this.tskNewGame.Image = ((System.Drawing.Image)(resources.GetObject("tskNewGame.Image")));
			this.tskNewGame.Location = new System.Drawing.Point(8, 32);
			this.tskNewGame.Name = "tskNewGame";
			this.tskNewGame.Size = new System.Drawing.Size(120, 24);
			this.tskNewGame.TabIndex = 2;
			this.tskNewGame.Text = "Новая игра";
			// 
			// tskExit
			// 
			this.tskExit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskExit.BackColor = System.Drawing.Color.Transparent;
			this.tskExit.Image = ((System.Drawing.Image)(resources.GetObject("tskExit.Image")));
			this.tskExit.Location = new System.Drawing.Point(8, 152);
			this.tskExit.Name = "tskExit";
			this.tskExit.Size = new System.Drawing.Size(120, 24);
			this.tskExit.TabIndex = 3;
			this.tskExit.Text = "Выход";
			this.tskExit.Click += new System.EventHandler(this.tskExit_Click);
			// 
			// tskReset
			// 
			this.tskReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskReset.BackColor = System.Drawing.Color.Transparent;
			this.tskReset.Image = ((System.Drawing.Image)(resources.GetObject("tskReset.Image")));
			this.tskReset.Location = new System.Drawing.Point(8, 80);
			this.tskReset.Name = "tskReset";
			this.tskReset.Size = new System.Drawing.Size(120, 24);
			this.tskReset.TabIndex = 4;
			this.tskReset.Text = "Перезапуск игры";
			this.tskReset.Click += new System.EventHandler(this.tskReset_Click);
			// 
			// tskGiveUp
			// 
			this.tskGiveUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tskGiveUp.BackColor = System.Drawing.Color.Transparent;
			this.tskGiveUp.Image = ((System.Drawing.Image)(resources.GetObject("tskGiveUp.Image")));
			this.tskGiveUp.Location = new System.Drawing.Point(8, 56);
			this.tskGiveUp.Name = "tskGiveUp";
			this.tskGiveUp.Size = new System.Drawing.Size(120, 24);
			this.tskGiveUp.TabIndex = 5;
			this.tskGiveUp.Text = "Сдаться";
			// 
			// expStatistics
			// 
			this.expStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.expStatistics.Animate = true;
			this.expStatistics.ExpandedHeight = 120;
			this.expStatistics.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.expStatistics.Items.AddRange(new System.Windows.Forms.Control[] {
																					 this.radRedStat,
																					 this.radBlueStat,
																					 this.lstStat});
			this.expStatistics.Location = new System.Drawing.Point(12, 308);
			this.expStatistics.Name = "expStatistics";
			this.expStatistics.Size = new System.Drawing.Size(184, 120);
			this.expStatistics.TabIndex = 3;
			this.expStatistics.Text = "Статистика";
			this.expStatistics.Resize += new System.EventHandler(this.expStatistics_Resize);
			// 
			// radRedStat
			// 
			this.radRedStat.Appearance = System.Windows.Forms.Appearance.Button;
			this.radRedStat.BackColor = System.Drawing.SystemColors.Control;
			this.radRedStat.ForeColor = System.Drawing.Color.Red;
			this.radRedStat.Location = new System.Drawing.Point(0, 24);
			this.radRedStat.Name = "radRedStat";
			this.radRedStat.Size = new System.Drawing.Size(80, 24);
			this.radRedStat.TabIndex = 0;
			this.radRedStat.Text = "Красный";
			this.radRedStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.radRedStat.CheckedChanged += new System.EventHandler(this.radRedStat_CheckedChanged);
			// 
			// radBlueStat
			// 
			this.radBlueStat.Appearance = System.Windows.Forms.Appearance.Button;
			this.radBlueStat.BackColor = System.Drawing.SystemColors.Control;
			this.radBlueStat.ForeColor = System.Drawing.Color.Blue;
			this.radBlueStat.Location = new System.Drawing.Point(81, 24);
			this.radBlueStat.Name = "radBlueStat";
			this.radBlueStat.Size = new System.Drawing.Size(85, 24);
			this.radBlueStat.TabIndex = 1;
			this.radBlueStat.Text = "Синий";
			this.radBlueStat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.radBlueStat.CheckedChanged += new System.EventHandler(this.radBlueStat_CheckedChanged);
			// 
			// lstStat
			// 
			this.lstStat.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstStat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					  this.colKey,
																					  this.colValue});
			this.lstStat.Enabled = false;
			this.lstStat.FullRowSelect = true;
			this.lstStat.GridLines = true;
			this.lstStat.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstStat.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
																					listViewItem1,
																					listViewItem2,
																					listViewItem3,
																					listViewItem4});
			this.lstStat.Location = new System.Drawing.Point(0, 48);
			this.lstStat.MultiSelect = false;
			this.lstStat.Name = "lstStat";
			this.lstStat.Scrollable = false;
			this.lstStat.Size = new System.Drawing.Size(168, 72);
			this.lstStat.TabIndex = 2;
			this.lstStat.View = System.Windows.Forms.View.Details;
			// 
			// colKey
			// 
			this.colKey.Width = 70;
			// 
			// colValue
			// 
			this.colValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.colValue.Width = 100;
			// 
			// frmGameScreen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(674, 464);
			this.Controls.Add(this.GamePanel);
			this.Controls.Add(this.panGame);
			this.Name = "frmGameScreen";
			this.Text = "frmGameScreen";
			this.Resize += new System.EventHandler(this.frmGameScreen_Resize);
			this.panGame.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.GamePanel)).EndInit();
			this.GamePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.expActivePlayer)).EndInit();
			this.expActivePlayer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.expMenu)).EndInit();
			this.expMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.expStatistics)).EndInit();
			this.expStatistics.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// --------------------------------------------------------------------

		/// <summary>
		/// Возвращает квадрат, в котором отрисовывается 
		/// точка с заданным индексом (не координатами!)
		/// </summary>
		static Rectangle GetDotRect(int XIndex, int YIndex) {
			Rectangle res = new Rectangle();
			int DotX, DotY;

			DotX = CELL_SIZE + XIndex * CELL_SIZE;
			DotY = CELL_SIZE + YIndex * CELL_SIZE;
			res.X = DotX - CELL_SIZE / 4;
			res.Y = DotY - CELL_SIZE / 4;
			res.Width = res.Height = CELL_SIZE / 4 * 2;
			return res;
		}

		// --------------------------------------------------------------------
		
		/// <summary>
		/// Устанавливает точку в заданной позиции
		/// </summary>
		/// <param name="Player">Игрок, которому принадлежит точка</param>
		void SetDot(int XIndex, int YIndex, int Player) {
			int Enemy = Player == 0 ? 1 : 0;

			Cursor.Current = Cursors.WaitCursor;
			SuspendLayout();
			Dots[XIndex, YIndex] = new Dot(XIndex, YIndex);
			PlayerStat[Player].numDots++;
			Dots[XIndex, YIndex].Owner = Player;
			Dots[XIndex, YIndex].Coord.X = CELL_SIZE + XIndex * CELL_SIZE;
			Dots[XIndex, YIndex].Coord.Y = CELL_SIZE + YIndex * CELL_SIZE;
			GetTerritory(ActivePlayer, Dots[XIndex, YIndex]);
			ctrlDrawBox.Invalidate();
			ActivePlayer = ActivePlayer == 0 ? 1 : 0;
			ResumeLayout();
			Cursor.Current = Cursors.Default;
		}

		// --------------------------------------------------------------------

		/// <summary>
		/// Проверяет для текущей точки: образует ли она с другими 
		/// точками контур и если образует - создает заштрихованное поле
		/// </summary>
		void GetTerritory(int Player, Dot StartSearchDot) {
			Stack Contour = new Stack();
			float Record = 0f, EmptyRecord = 0f;
			Stack RecCont = null, EmptyRecCont = null;
			int Enemy = Player == 0 ? 1 : 0;
			
			Contour.Push(StartSearchDot);
			NextDot(StartSearchDot, null, StartSearchDot, Contour, Player);

			// просмотрим все контуры и выберем контур с максимальной площадью
			if(Contours != null) {
				foreach(DictionaryEntry i in Contours) {
					if((float) i.Value > Record) {
						Record = (float) i.Value;
						RecCont = (Stack) i.Key;
					}
				}
			}
			if(RecCont != null) {
				// добавляем заштрихованное поле к остальным
				CapturedField NewField = ContourToFiled(RecCont);
				NewField.Size = Record;
				SetFiled(NewField, Player);
				Contours = null;
			}
			/* контуров нет, т.е. ничего нельзя заштриховать, проверим не стоит 
			ли только что поставленная точка в пустом контуре */
			else {
				bool PtInPoly = false;

				for(int i = 0; i < EmptyFileds[Enemy].Count; i++) {
					PtInPoly = CapturedField.PointInPolygon(StartSearchDot.Coord, 
					((CapturedField) EmptyFileds[Enemy][i]).Pts);
					if(!PtInPoly) continue;

					CapturedField EmpFiled = (CapturedField) EmptyFileds[Enemy][i];

					for(int i2 = 0; i2 < Dots.GetLength(0); i2++) {
						for(int i3 = 0; i3 < Dots.GetLength(1); i3++) {
							if(Dots[i2, i3] != null &&
								Dots[i2, i3].Owner == Enemy &&
								Dots[i2, i3].IsCatched == false &&
								CapturedField.PointInPolygon(Dots[i2, i3].Coord, EmpFiled.Pts))
									Dots[i2, i3].IsCatched = true;
						}
					}
					StartSearchDot.IsCatched = true;
					SetFiled(EmpFiled, Enemy);
					PlayerStat[Enemy].numEmptyFields--;
				}
			}

			// просмотрим пустые контуры и выберем самый большой
			if(EmptyContours != null) {
				foreach(DictionaryEntry j in EmptyContours) {
					if((float) j.Value > EmptyRecord) {
						EmptyRecord = (float) j.Value;
						EmptyRecCont = (Stack) j.Key;
					}
				}	
			}
			if(EmptyRecCont != null) {
				// добавляем пустое поле к остальным
				CapturedField NewEmpField = ContourToFiled(EmptyRecCont);
				NewEmpField.Size = EmptyRecord;
				EmptyFileds[Player].Add(NewEmpField);
				EmptyContours.Clear();
				PlayerStat[Player].numEmptyFields++;
				ShowStatistics(Player);
			}
		}

		// --------------------------------------------------------------------

		/// <remarks>
		/// Рекурсивный метод, который находит рядом с точкой CurrDot соседние точки
		/// добавляет эти точки в образующийся контур (Stack Contour),
		/// и вызывает себя для каждой соседней точки, таким образом получая все возможные
		/// контуры для данной группы точек. Все полученные контуры помещаются
		/// в ListDictionary Contours и EmptyContours в зависимости от того, есть ли
		/// внутри контура вражеские точки или нет.
		/// </remarks>
		void NextDot(Dot CurrDot, Dot PrevDot, Dot FirstDot, Stack Contour, int Player) {
			Dot[] Neighbour = new Dot[8];
			int x = CurrDot.Index.X, y = CurrDot.Index.Y;
			int i;
			bool f = false;
			ArrayList CatchedPlayerDots = new ArrayList();
			ListDictionary SearchContour;
			
			if(PrevDot != null && CurrDot == FirstDot) { // контур замкнулся
				Contour.Pop();
				if(Contour.Count > 3) {
					int Enemy = Player == 0 ? 1 : 0;
					// проверяем есть ли в контуре точки
					for(i = 0; i < FieldSize.Width; i++) {
						for(int i2 = 0; i2 < FieldSize.Height; i2++) {
							if(Dots[i, i2] == null) continue;
							if(CapturedField.PointInPolygon(
								Dots[i, i2].Coord, Contour)) 
							{
								if(!Contour.Contains(Dots[i, i2])) 
									Dots[i, i2].IsCatched = true;
								if(Dots[i, i2].Owner == Player) 
									CatchedPlayerDots.Add(Dots[i, i2]);
								else
									f = true;
							}
						}
					}
					if(f == false) {
						if(!EmptyContours.Contains(Contour)) {
							EmptyContours.Add(Contour.Clone(), 
								CapturedField.CalcContourSquare(Contour));
							LastContourIsEmpty = true;
						}
						return;
					}
					for(i = 0; i < CatchedPlayerDots.Count; i++)
						((Dot)CatchedPlayerDots[i]).IsCatched = true;
					if(Contours == null) Contours = new ListDictionary();
					if(!Contours.Contains(Contour)) {
						Contours.Add(Contour.Clone(), CapturedField.CalcContourSquare(Contour));
						LastContourIsEmpty = false;
					}
				}
				return;
			}

			// обходим по часовой стрелке
			// точка слева сверху
			 Neighbour[0] = (x > 0 && y > 0) ? Dots[x - 1, y - 1] : null;
			// точка сверху
			Neighbour[1] = (y > 0) ? Dots[x, y - 1] : null;
			// точка справа сверху
			Neighbour[2] = (x < FieldSize.Width - 1 && y > 0) ? Dots[x + 1, y - 1] : null;
			// точка справа
			Neighbour[3] = (x < FieldSize.Width - 1) ? Dots[x + 1, y] : null;
			// точка справа снизу
			Neighbour[4] = (x < FieldSize.Width - 1 && y < FieldSize.Height - 1) ?
				Dots[x + 1, y + 1] : null;
			// точка снизу
			Neighbour[5] = (y < FieldSize.Height - 1) ? Dots[x, y + 1] : null;
			// точка снизу слева
			Neighbour[6] = (x > 0 && y < FieldSize.Height - 1) ? Dots[x - 1, y + 1] : null;
			// точка слева
			Neighbour[7] = (x > 0) ? Dots[x - 1, y] : null;

			for(i = 0; i < Neighbour.Length; i++) {
				if( Neighbour[i] == null			|| 
					Neighbour[i] == PrevDot			||
					Neighbour[i].Owner != Player	||
					Neighbour[i].IsCatched == true	||
					(Contour.Contains(Neighbour[i])) && Neighbour[i] != FirstDot)
						continue;

				/* для того чтоб не обходить контур 2 раза (т.к. по нему можно идти в 2
				стороны) проверим не является ли вторая точка 
				текщуего контура частью прошлого контура */
				SearchContour = LastContourIsEmpty ? EmptyContours : Contours;
				if(PrevDot == FirstDot && SearchContour != null && 
					SearchContour.Count > 0) 
				{
					int Count = 0;
					foreach(DictionaryEntry j in SearchContour) {
						if(Count == SearchContour.Count - 1) {
							Stack s = (Stack) j.Key;
							if(s.Contains(CurrDot)) goto NextIter;
						}
						Count++;
					}
				}

				Contour.Push(Neighbour[i]);
				NextDot(Neighbour[i], CurrDot, FirstDot, Contour, Player);
			NextIter:;
			}
			if(Contour.Count > 0) Contour.Pop();
		}

		// --------------------------------------------------------------------

		void SetFiled(CapturedField field, int Player) {
			for(int i = 0; i < field.Dots.Length; i++)
				field.Dots[i].IsBounding = true;
			Fields[Player].Add(field);
			PlayerStat[Player].numFileds++;
			PlayerStat[Player].TerritorySize += field.Size;
		}

		// --------------------------------------------------------------------

		/// <summary>
		/// Преобразует контур с точками в полигон с экранными координатами
		/// </summary>
		CapturedField ContourToFiled(Stack Contour) {
			CapturedField res = new CapturedField();
			Point[] Pts = new Point[Contour.Count];
			Dot[] Dots = new Dot[Contour.Count];
			int i = 0;
			
			while(Contour.Count > 0) {
				Dots[i] = (Dot) Contour.Pop();
				Pts[i].X = CELL_SIZE + Dots[i].Index.X * CELL_SIZE;
				Pts[i].Y = CELL_SIZE + Dots[i].Index.Y * CELL_SIZE;
				i++;
			}
			res.Pts = Pts;
			res.Dots = Dots;
			return res;
		}

		// --------------------------------------------------------------------

		/// <summary>
		/// Показывает статичтику для заданного игрока в ListView'е lstStat
		/// </summary>
		void ShowStatistics(int PlayerID) {
			lstStat.Items[0].SubItems[1].Text = PlayerStat[PlayerID].TerritorySize.ToString();
			lstStat.Items[1].SubItems[1].Text = PlayerStat[PlayerID].numFileds.ToString();
			lstStat.Items[2].SubItems[1].Text = PlayerStat[PlayerID].numEmptyFields.ToString();
			lstStat.Items[3].SubItems[1].Text = PlayerStat[PlayerID].numDots.ToString();
		}

		// --------------------------------------------------------------------

		void RestartGame() {
			Dots = new Dot[FieldSize.Width, FieldSize.Height];
			PlayerStat = new Dots.Statistics[2];
			if(Contours != null) Contours.Clear();
			EmptyContours.Clear();
			LastContourIsEmpty = false;
			Fields[0].Clear();
			Fields[1].Clear();
			ActivePlayer = 0;
			ShowStatistics(ActivePlayer);
			Refresh();
		}

		// --------------------------------------------------------------------

		void SaveGame() {
			SaveFileDialog dlg = new SaveFileDialog();

			dlg.Filter = "Dots save (*.sav)|*.sav";
			dlg.RestoreDirectory = true;
			dlg.InitialDirectory = Directory.GetCurrentDirectory() + 
				"\\" + Dot.SAVED_GAMES_FOLDER;
			if(dlg.ShowDialog() == DialogResult.Cancel || dlg.FileName == "") return;
			
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(dlg.FileName, FileMode.OpenOrCreate, 
				FileAccess.Write, FileShare.None);

			formatter.Serialize(stream, FieldSize);
			formatter.Serialize(stream, gameType);
			formatter.Serialize(stream, Dots);
			formatter.Serialize(stream, _ActivePlayer);
			formatter.Serialize(stream, Fields);
			formatter.Serialize(stream, EmptyFileds);
			formatter.Serialize(stream, LastContourIsEmpty);
			formatter.Serialize(stream, PlayerStat);
			stream.Close();
		}

		// --------------------------------------------------------------------
		
		void LoadGame(string FilePath) {
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read,
				FileShare.None);
			
			SuspendLayout();
			FieldSize = (Size) formatter.Deserialize(stream);
			gameType = (GameType) formatter.Deserialize(stream);
			Dots = (Dot[,]) formatter.Deserialize(stream);
			_ActivePlayer = (int) formatter.Deserialize(stream);
			ActivePlayer = _ActivePlayer;
			Fields = (ArrayList[]) formatter.Deserialize(stream);
			EmptyFileds = (ArrayList[]) formatter.Deserialize(stream);
			LastContourIsEmpty = (bool) formatter.Deserialize(stream);
			PlayerStat = (Statistics[]) formatter.Deserialize(stream);
			stream.Close();
			Invalidate();
			ResumeLayout();
		}

		// ====================================================================

		private void ctrlDrawBox_Paint(object sender, 
			System.Windows.Forms.PaintEventArgs e) 
		{
			Point p1 = new Point(), p2 = new Point();
			Control ctrlDraw = (Control) sender;
			int i, i2;
			Pen CellPen;
			SolidBrush brush;
			HatchBrush[] FiledBrush = new HatchBrush[2];
			CapturedField CurrField;

			// рисуем горизонтальные линии
			p1.X = CELL_SIZE;
			p1.Y = 0;
			p2.X = CELL_SIZE;
			p2.Y = ctrlDraw.Height;
			CellPen = new Pen(Color.Pink);
			for(i = 0; i < FieldSize.Width; i++) {
				e.Graphics.DrawLine(CellPen, p1, p2);
				p1.X += CELL_SIZE;
				p2.X += CELL_SIZE;
			}

			// рисуем вертикальные линии
			p1.X = 0;
			p1.Y = CELL_SIZE;
			p2.X = ctrlDraw.Width;
			p2.Y = CELL_SIZE;
			for(i = 0; i < FieldSize.Height; i++) {
				e.Graphics.DrawLine(CellPen, p1, p2);
				p1.Y += CELL_SIZE;
				p2.Y += CELL_SIZE;
			}

			// рисуем точки
			for(i = 0; i < FieldSize.Width; i++) {
				for(i2 = 0; i2 < FieldSize.Height; i2++) {
					if(Dots[i, i2] == null) continue;
					brush = Dot.DotsBrushes[Dots[i, i2].Owner];
					e.Graphics.FillEllipse(brush, GetDotRect(i, i2));
				}
			}

			// рисуем заштрихованные области
			FiledBrush[0] = new HatchBrush(HatchStyle.DiagonalCross, 
				Dot.PlayerColors[0], Color.Transparent);
			FiledBrush[1] = new HatchBrush(HatchStyle.DiagonalCross, 
				Dot.PlayerColors[1], Color.Transparent);
			for(i = 0; i < 2; i++) {
				for(i2 = 0; i2 < Fields[i].Count; i2++) {
					CurrField = (CapturedField) Fields[i][i2];
					e.Graphics.FillPolygon(FiledBrush[i], CurrField.Pts);
				}
			}
		}

		// ------------------------------------------------------------------

		private void ctrlDrawBox_MouseDown(object sender, MouseEventArgs e) {
			Control ctrl = (Control) sender;

			// проверяем не находится ли курсор за отступами
			if(e.X < CELL_SIZE || e.X > (ctrl.Width - CELL_SIZE)) return;
			if(e.Y < CELL_SIZE || e.Y > (ctrl.Height - CELL_SIZE)) return;
			
			double XIndex = Math.Round(((double)e.X) / ((double)CELL_SIZE)) - 1,
				   YIndex = Math.Round(((double)e.Y) / ((double)CELL_SIZE)) - 1;
			if(Dots[(int)XIndex, (int)YIndex] != null) return;
			SetDot((int)XIndex, (int)YIndex, ActivePlayer);
		}

		// ------------------------------------------------------------------

		private void frmGameScreen_Resize(object sender, System.EventArgs e) {
			panGame.Size = new Size(this.ClientSize.Width - GamePanel.Width, 
									this.ClientSize.Height);
		}

		// ------------------------------------------------------------------

		private void tskExit_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show("Вы действительно хотите выйти?", "", 
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				Application.Exit();
		}

		// ------------------------------------------------------------------

		private void tskReset_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show("Вы действительно хотите запустить игру сначала?", "?",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					RestartGame();
		}

		// ------------------------------------------------------------------

		private void radRedStat_CheckedChanged(object sender, System.EventArgs e) {
			ShowStatistics(0);
		}

		// ------------------------------------------------------------------

		private void radBlueStat_CheckedChanged(object sender, System.EventArgs e) {
			ShowStatistics(1);
		}

		// ------------------------------------------------------------------

		private void expStatistics_Resize(object sender, System.EventArgs e) {
			int HalfWidth = expStatistics.ClientSize.Width / 2;

			lstStat.Width = expStatistics.ClientSize.Width;
			lstStat.Columns[1].Width = lstStat.ClientSize.Width - lstStat.Columns[0].Width;
			radRedStat.Width = HalfWidth;
			radBlueStat.Location = new Point(HalfWidth, radBlueStat.Location.Y);
			radBlueStat.Width = HalfWidth;
		}

		// ------------------------------------------------------------------

		private void tskSaveGame_Click(object sender, System.EventArgs e) {
			SaveGame();
		}

		// ------------------------------------------------------------------

		private void tskLoadGame_Click(object sender, System.EventArgs e) {
			frmLoadGame dlg = new frmLoadGame();

			dlg.ShowDialog();
		}
	}
}
