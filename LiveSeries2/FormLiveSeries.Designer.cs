
namespace LiveSeries2
{
    partial class FormLiveSeries
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLiveSeries));
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblTop = new System.Windows.Forms.Label();
            this.txtSearchBar = new System.Windows.Forms.TextBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.flpShowsList = new System.Windows.Forms.FlowLayoutPanel();
            this.lblNumSubscriptions = new System.Windows.Forms.Label();
            this.prgFileSave = new System.Windows.Forms.ProgressBar();
            this.prgDownload = new System.Windows.Forms.ProgressBar();
            this.lblDownloadStatus = new System.Windows.Forms.Label();
            this.lblDownloadInfo = new System.Windows.Forms.Label();
            this.ctxMenuTaskBar = new System.Windows.Forms.ContextMenu();
            this.itmRestore = new System.Windows.Forms.MenuItem();
            this.itmSeparator1 = new System.Windows.Forms.MenuItem();
            this.itmHome = new System.Windows.Forms.MenuItem();
            this.itmSettings = new System.Windows.Forms.MenuItem();
            this.itmSubscriptions = new System.Windows.Forms.MenuItem();
            this.itmSeparator2 = new System.Windows.Forms.MenuItem();
            this.itmExit = new System.Windows.Forms.MenuItem();
            this.icnTaskBar = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblCentre = new System.Windows.Forms.Label();
            this.tlpNewEpisodes = new System.Windows.Forms.TableLayoutPanel();
            this.flpNewEpisodes = new System.Windows.Forms.FlowLayoutPanel();
            this.flpFutureEpisodes = new System.Windows.Forms.FlowLayoutPanel();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnMostPopular = new System.Windows.Forms.Button();
            this.tlpNewEpisodes.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSearch.Location = new System.Drawing.Point(467, 95);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(49, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // lblTop
            // 
            this.lblTop.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblTop.AutoSize = true;
            this.lblTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTop.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblTop.Location = new System.Drawing.Point(161, 10);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(80, 13);
            this.lblTop.TabIndex = 3;
            this.lblTop.Text = "Sample Results";
            // 
            // txtSearchBar
            // 
            this.txtSearchBar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtSearchBar.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtSearchBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtSearchBar.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtSearchBar.Location = new System.Drawing.Point(254, 98);
            this.txtSearchBar.Name = "txtSearchBar";
            this.txtSearchBar.Size = new System.Drawing.Size(207, 20);
            this.txtSearchBar.TabIndex = 0;
            this.txtSearchBar.Text = "Sample Search Query";
            // 
            // btnHome
            // 
            this.btnHome.AutoSize = true;
            this.btnHome.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHome.Location = new System.Drawing.Point(1, 0);
            this.btnHome.MinimumSize = new System.Drawing.Size(70, 23);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(70, 23);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.BtnHome_Click);
            this.btnHome.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BtnHome_KeyDown);
            // 
            // flpShowsList
            // 
            this.flpShowsList.AutoScroll = true;
            this.flpShowsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpShowsList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpShowsList.Location = new System.Drawing.Point(0, 0);
            this.flpShowsList.Margin = new System.Windows.Forms.Padding(0);
            this.flpShowsList.Name = "flpShowsList";
            this.flpShowsList.Size = new System.Drawing.Size(914, 441);
            this.flpShowsList.TabIndex = 4;
            this.flpShowsList.WrapContents = false;
            // 
            // lblNumSubscriptions
            // 
            this.lblNumSubscriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNumSubscriptions.AutoSize = true;
            this.lblNumSubscriptions.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblNumSubscriptions.Location = new System.Drawing.Point(750, 3);
            this.lblNumSubscriptions.Name = "lblNumSubscriptions";
            this.lblNumSubscriptions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblNumSubscriptions.Size = new System.Drawing.Size(88, 13);
            this.lblNumSubscriptions.TabIndex = 0;
            this.lblNumSubscriptions.Text = "Subscriptions: 99";
            this.lblNumSubscriptions.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblNumSubscriptions.Click += new System.EventHandler(this.LblNumSubscriptions_Click);
            // 
            // prgFileSave
            // 
            this.prgFileSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prgFileSave.Location = new System.Drawing.Point(3, 395);
            this.prgFileSave.Name = "prgFileSave";
            this.prgFileSave.Size = new System.Drawing.Size(115, 23);
            this.prgFileSave.TabIndex = 0;
            this.prgFileSave.Visible = false;
            // 
            // prgDownload
            // 
            this.prgDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.prgDownload.Location = new System.Drawing.Point(710, 395);
            this.prgDownload.Name = "prgDownload";
            this.prgDownload.Size = new System.Drawing.Size(125, 23);
            this.prgDownload.TabIndex = 0;
            this.prgDownload.Visible = false;
            // 
            // lblDownloadStatus
            // 
            this.lblDownloadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDownloadStatus.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblDownloadStatus.Location = new System.Drawing.Point(590, 400);
            this.lblDownloadStatus.Name = "lblDownloadStatus";
            this.lblDownloadStatus.Size = new System.Drawing.Size(116, 13);
            this.lblDownloadStatus.TabIndex = 0;
            this.lblDownloadStatus.Text = "Currently Downloading:";
            this.lblDownloadStatus.Visible = false;
            // 
            // lblDownloadInfo
            // 
            this.lblDownloadInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDownloadInfo.AutoSize = true;
            this.lblDownloadInfo.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblDownloadInfo.Location = new System.Drawing.Point(710, 365);
            this.lblDownloadInfo.MaximumSize = new System.Drawing.Size(125, 26);
            this.lblDownloadInfo.Name = "lblDownloadInfo";
            this.lblDownloadInfo.Size = new System.Drawing.Size(116, 26);
            this.lblDownloadInfo.TabIndex = 5;
            this.lblDownloadInfo.Text = "Sample Show Episode With Long Name S01E01";
            this.lblDownloadInfo.Visible = false;
            // 
            // ctxMenuTaskBar
            // 
            this.ctxMenuTaskBar.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.itmRestore,
            this.itmSeparator1,
            this.itmHome,
            this.itmSettings,
            this.itmSubscriptions,
            this.itmSeparator2,
            this.itmExit});
            // 
            // itmRestore
            // 
            this.itmRestore.Index = 0;
            this.itmRestore.Text = "&Restore";
            this.itmRestore.Click += new System.EventHandler(this.ItmRestore_Click);
            // 
            // itmSeparator1
            // 
            this.itmSeparator1.Index = 1;
            this.itmSeparator1.Text = "-";
            // 
            // itmHome
            // 
            this.itmHome.Index = 2;
            this.itmHome.Text = "&Home";
            this.itmHome.Click += new System.EventHandler(this.ItmHome_Click);
            // 
            // itmSettings
            // 
            this.itmSettings.Index = 3;
            this.itmSettings.Text = "&Settings";
            this.itmSettings.Click += new System.EventHandler(this.ItmSettings_Click);
            // 
            // itmSubscriptions
            // 
            this.itmSubscriptions.Index = 4;
            this.itmSubscriptions.Text = "Su&bscriptions";
            this.itmSubscriptions.Click += new System.EventHandler(this.ItmSubscriptions_Click);
            // 
            // itmSeparator2
            // 
            this.itmSeparator2.Index = 5;
            this.itmSeparator2.Text = "-";
            // 
            // itmExit
            // 
            this.itmExit.Index = 6;
            this.itmExit.Text = "E&xit";
            this.itmExit.Click += new System.EventHandler(this.ItmExit_Click);
            // 
            // icnTaskBar
            // 
            this.icnTaskBar.ContextMenu = this.ctxMenuTaskBar;
            this.icnTaskBar.Icon = ((System.Drawing.Icon)(resources.GetObject("icnTaskBar.Icon")));
            this.icnTaskBar.Text = "LiveSeries";
            this.icnTaskBar.Visible = true;
            this.icnTaskBar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.IcnTaskBar_MouseClick);
            // 
            // lblCentre
            // 
            this.lblCentre.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblCentre.AutoSize = true;
            this.lblCentre.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblCentre.Location = new System.Drawing.Point(359, 140);
            this.lblCentre.Name = "lblCentre";
            this.lblCentre.Size = new System.Drawing.Size(136, 13);
            this.lblCentre.TabIndex = 0;
            this.lblCentre.Text = "No new episodes to watch.";
            // 
            // tlpNewEpisodes
            // 
            this.tlpNewEpisodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpNewEpisodes.AutoScroll = true;
            this.tlpNewEpisodes.AutoSize = true;
            this.tlpNewEpisodes.BackColor = System.Drawing.Color.Gray;
            this.tlpNewEpisodes.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpNewEpisodes.ColumnCount = 1;
            this.tlpNewEpisodes.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpNewEpisodes.Controls.Add(this.flpNewEpisodes, 0, 0);
            this.tlpNewEpisodes.Controls.Add(this.flpFutureEpisodes, 0, 1);
            this.tlpNewEpisodes.Location = new System.Drawing.Point(0, 166);
            this.tlpNewEpisodes.Margin = new System.Windows.Forms.Padding(0, 166, 0, 0);
            this.tlpNewEpisodes.MinimumSize = new System.Drawing.Size(914, 283);
            this.tlpNewEpisodes.Name = "tlpNewEpisodes";
            this.tlpNewEpisodes.RowCount = 2;
            this.tlpNewEpisodes.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpNewEpisodes.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpNewEpisodes.Size = new System.Drawing.Size(914, 283);
            this.tlpNewEpisodes.TabIndex = 0;
            // 
            // flpNewEpisodes
            // 
            this.flpNewEpisodes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.flpNewEpisodes.AutoScroll = true;
            this.flpNewEpisodes.AutoSize = true;
            this.flpNewEpisodes.BackColor = System.Drawing.SystemColors.ControlDark;
            this.flpNewEpisodes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flpNewEpisodes.Location = new System.Drawing.Point(354, 1);
            this.flpNewEpisodes.Margin = new System.Windows.Forms.Padding(0);
            this.flpNewEpisodes.MinimumSize = new System.Drawing.Size(205, 145);
            this.flpNewEpisodes.Name = "flpNewEpisodes";
            this.flpNewEpisodes.Size = new System.Drawing.Size(205, 145);
            this.flpNewEpisodes.TabIndex = 0;
            this.flpNewEpisodes.WrapContents = false;
            // 
            // flpFutureEpisodes
            // 
            this.flpFutureEpisodes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flpFutureEpisodes.AutoScroll = true;
            this.flpFutureEpisodes.AutoSize = true;
            this.flpFutureEpisodes.BackColor = System.Drawing.SystemColors.ControlDark;
            this.flpFutureEpisodes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flpFutureEpisodes.Location = new System.Drawing.Point(354, 148);
            this.flpFutureEpisodes.Margin = new System.Windows.Forms.Padding(0);
            this.flpFutureEpisodes.MaximumSize = new System.Drawing.Size(860, 400);
            this.flpFutureEpisodes.MinimumSize = new System.Drawing.Size(205, 134);
            this.flpFutureEpisodes.Name = "flpFutureEpisodes";
            this.flpFutureEpisodes.Padding = new System.Windows.Forms.Padding(40, 15, 40, 0);
            this.flpFutureEpisodes.Size = new System.Drawing.Size(205, 134);
            this.flpFutureEpisodes.TabIndex = 6;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(768, 20);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 6;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Visible = false;
            this.btnReload.Click += new System.EventHandler(this.BtnReload_Click);
            // 
            // btnMostPopular
            // 
            this.btnMostPopular.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnMostPopular.AutoSize = true;
            this.btnMostPopular.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMostPopular.Location = new System.Drawing.Point(375, 0);
            this.btnMostPopular.MinimumSize = new System.Drawing.Size(70, 23);
            this.btnMostPopular.Name = "btnMostPopular";
            this.btnMostPopular.Size = new System.Drawing.Size(114, 23);
            this.btnMostPopular.TabIndex = 7;
            this.btnMostPopular.Text = "Most Popular Shows";
            this.btnMostPopular.UseVisualStyleBackColor = true;
            this.btnMostPopular.Click += new System.EventHandler(this.BtnMostPopular_Click);
            // 
            // FormLiveSeries
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(914, 441);
            this.Controls.Add(this.btnMostPopular);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.lblDownloadInfo);
            this.Controls.Add(this.lblDownloadStatus);
            this.Controls.Add(this.lblNumSubscriptions);
            this.Controls.Add(this.lblTop);
            this.Controls.Add(this.lblCentre);
            this.Controls.Add(this.txtSearchBar);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.prgFileSave);
            this.Controls.Add(this.prgDownload);
            this.Controls.Add(this.tlpNewEpisodes);
            this.Controls.Add(this.flpShowsList);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MinimumSize = new System.Drawing.Size(860, 400);
            this.Name = "FormLiveSeries";
            this.Text = "LiveSeries";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLiveSeries_Closing);
            this.tlpNewEpisodes.ResumeLayout(false);
            this.tlpNewEpisodes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.TextBox txtSearchBar;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.FlowLayoutPanel flpShowsList;
        private System.Windows.Forms.Label lblNumSubscriptions;
        private System.Windows.Forms.ProgressBar prgFileSave;
        private System.Windows.Forms.ProgressBar prgDownload;
        private System.Windows.Forms.Label lblDownloadStatus;
        private System.Windows.Forms.Label lblDownloadInfo;
        private System.Windows.Forms.ContextMenu ctxMenuTaskBar;
        private System.Windows.Forms.MenuItem itmExit;
        private System.Windows.Forms.NotifyIcon icnTaskBar;
        private System.Windows.Forms.MenuItem itmRestore;
        private System.Windows.Forms.MenuItem itmHome;
        private System.Windows.Forms.MenuItem itmSettings;
        private System.Windows.Forms.MenuItem itmSubscriptions;
        private System.Windows.Forms.MenuItem itmSeparator1;
        private System.Windows.Forms.MenuItem itmSeparator2;
        private System.Windows.Forms.Label lblCentre;
        private System.Windows.Forms.TableLayoutPanel tlpNewEpisodes;
        private System.Windows.Forms.FlowLayoutPanel flpNewEpisodes;
        private System.Windows.Forms.FlowLayoutPanel flpFutureEpisodes;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnMostPopular;
    }
}

