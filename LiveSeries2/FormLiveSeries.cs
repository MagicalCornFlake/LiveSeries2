using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LiveSeries2
{
    public partial class FormLiveSeries : Form
    {
        private const string episodateURL = "episodate.com/api";
        private const string apibayURL = "apibay.org";
        private const string shortDateFormat = "dd MMM";
        private const string longDateFormat = "dd MMM yyyy";
        private const string shortTimeFormat = "h:mm tt";
        private readonly ProgramSettings Settings;
        private readonly NewEpisodesManager mainChecker;
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private bool CurrentlyViewingSubscriptions = false;
        private static DateTime currentTime = DateTime.MinValue;

        public FormLiveSeries()
        {
            using (StreamReader r = new StreamReader("data.json"))
            {
                string json = r.ReadToEnd();
                Settings = JsonConvert.DeserializeObject<ProgramSettings>(json);
            }
            InitializeComponent();
            if (OptimiseSettings())
                SaveDataFile();
            Log("Application initialising...", true);
            txtSearchBar.Text = "What are you looking for?";
            flpShowsList.Size = new Size(844, 440);
            UpdateSubscriptions();
            mainChecker = new NewEpisodesManager(this);
            GoHome();
            Task.Run(async () => await mainChecker.RunChecksForNewEpisodes());
        }

        private bool ToggleShowSubscription(TvShow Show, Button btnThatWasClicked)
        {
            ShowHistory savedShow;
            if (Settings.ShowLibrary.Any(SavedShow => SavedShow.ID == Show.ID))
            {
                savedShow = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == Show.ID);
                savedShow.Subscribed = !savedShow.Subscribed;
            }
            else
            {
                savedShow = new ShowHistory { ID = Show.ID, Subscribed = true };
                Settings.ShowLibrary.Add(savedShow);
            }
            SaveDataFile();
            UpdateSubscribeButtonText(btnThatWasClicked, Show);
            return savedShow.Subscribed;
        }

        private void DisplayShows(ProgramMenu lastMenu, string sourceQuery, int pageNumber)
        {
            flpShowsList.Controls.Clear();
            flpShowsList.Visible = true;
            flpNewEpisodes.Visible = true;
            lblTop.Visible = true;
            btnHome.Focus();
            string sourcePageName;
            switch (lastMenu)
            {
                case ProgramMenu.Search:
                    sourcePageName = "search";
                    break;
                case ProgramMenu.MostPopular:
                    sourcePageName = "most-popular";
                    break;
                default:
                    throw new ArgumentException($"{lastMenu} is an invalid value for the 'lastMenu' argument.", "lastMenu");
            }
            string rawResult = GetJsonStringFromURL(episodateURL, sourcePageName, sourceQuery, pageNumber);
            ShowList shows = JsonConvert.DeserializeObject<ShowList>(rawResult);

            if (DisplayShowsList(shows, lastMenu, sourceQuery, pageNumber))
            {
                lblTop.Text = $"Total results: {shows.TotalShows} | Page {shows.CurrentPage} of {shows.TotalPages}";
                Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
                Font font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point);
                SizeF leftSize = graphics.MeasureString(lblTop.Text.Split('|')[0], font);
                SizeF rightSize = graphics.MeasureString(lblTop.Text.Split('|')[1], font);
                (int leftLength, int rightLength) = ((int)Math.Floor(leftSize.Width), (int)Math.Floor(rightSize.Width));
                lblTop.Location = new Point(205 * 2 - leftLength + 5, 10);

                Panel pnlHeaderContainer = new Panel() { AutoSize = true };
                Panel pnlFooterContainer = new Panel() { AutoSize = true };

                void btnFrstPage_Click(object sender, EventArgs e)
                {
                    if (shows.CurrentPage != 1)
                    {
                        DisplayShows(lastMenu, sourceQuery, pageNumber: 1);
                    }
                }
                void btnPrevPage_Click(object sender, EventArgs e)
                {
                    if (shows.CurrentPage > 1)
                    {
                        DisplayShows(lastMenu, sourceQuery, pageNumber: shows.CurrentPage - 1);
                    }
                }
                void btnNextPage_Click(object sender, EventArgs e)
                {
                    if (shows.CurrentPage < shows.TotalPages)
                    {
                        DisplayShows(lastMenu, sourceQuery, pageNumber: shows.CurrentPage + 1);
                    }
                }
                void btnLastPage_Click(object sender, EventArgs e)
                {
                    if (shows.CurrentPage != shows.TotalPages)
                    {
                        DisplayShows(lastMenu, sourceQuery, pageNumber: shows.TotalPages);
                    }
                }

                Label lblBottom = new Label
                {
                    Text = lblTop.Text,
                    Size = lblTop.Size,
                    Location = new Point(lblTop.Location.X - pnlFooterContainer.Location.X, 5),
                    Font = lblTop.Font,
                    ForeColor = lblTop.ForeColor
                };
                pnlFooterContainer.Controls.Add(lblBottom);

                const int btnDims = 23;
                foreach (Control c in new Control[] { pnlHeaderContainer, pnlFooterContainer })
                {
                    Button btnFrstPage = new Button
                    {
                        Name = "delete_me_on_menu_change",
                        Size = new Size(btnDims + 5, btnDims),
                        Text = "|<",
                        Location = new Point(205 * 2 - leftLength - btnDims * 2 - 15, 0),
                        UseVisualStyleBackColor = true,
                        Enabled = shows.CurrentPage != 1
                    };
                    Button btnPrevPage = new Button
                    {
                        Name = "delete_me_on_menu_change",
                        Size = new Size(btnDims, btnDims),
                        Text = "<",
                        Location = new Point(205 * 2 - leftLength - btnDims * 1 - 5, 0),
                        UseVisualStyleBackColor = true,
                        Enabled = shows.CurrentPage > 1
                    };
                    Button btnNextPage = new Button
                    {
                        Name = "delete_me_on_menu_change",
                        Size = new Size(btnDims, btnDims),
                        Text = ">",
                        Location = new Point(205 * 2 + rightLength + btnDims * 0 + 5, 0),
                        UseVisualStyleBackColor = true,
                        Enabled = shows.CurrentPage < shows.TotalPages
                    };
                    Button btnLastPage = new Button
                    {
                        Name = "delete_me_on_menu_change",
                        Size = new Size(btnDims + 5, btnDims),
                        Text = ">|",
                        Location = new Point(205 * 2 + rightLength + btnDims * 1 + 10, 0),
                        UseVisualStyleBackColor = true,
                        Enabled = shows.CurrentPage != shows.TotalPages
                    };
                    btnFrstPage.Click += new EventHandler(btnFrstPage_Click);
                    btnPrevPage.Click += new EventHandler(btnPrevPage_Click);
                    btnNextPage.Click += new EventHandler(btnNextPage_Click);
                    btnLastPage.Click += new EventHandler(btnLastPage_Click);
                    Control[] btns = new Control[] { btnFrstPage, btnPrevPage, btnNextPage, btnLastPage };
                    c.Controls.AddRange(btns);
                }

                Control[] cc = flpShowsList.Controls.Cast<Control>().ToArray();
                flpShowsList.Controls.Clear();
                flpShowsList.Controls.Add(pnlHeaderContainer);
                flpShowsList.Controls.AddRange(cc);
                flpShowsList.Controls.Add(pnlFooterContainer);
            }
            else
            {
                lblTop.Text = "No results for '" + sourceQuery + "'.";
            }

        }

        private bool DisplayShowsList(ShowList shows, ProgramMenu lastMenu, string rawSearchQuery = null, int pageNumber = 0)
        {
            if (shows.TotalShows == "0")
                return false;
            foreach (TvShow Show in shows.TvShows)
            {
                Panel pnlShowDetailsContainer = new Panel
                {
                    Name = "pnl_show_" + Show.ID.ToString(),
                    Visible = true,
                    Width = 205 * 4,
                    Height = 274 + 50
                };
                if (Show.StartDate is null)
                {
                    string startDateType = Show.StartDateString is null ? "null" : Show.StartDateString.GetType().ToString();
                    Log($"{Show.Name} start date: '{Show.StartDateString}' ({startDateType})");
                }
                string startDate = Show.StartDate is null ? Show.StartDateString ?? "Unknown" : ((DateTime)Show.StartDate).ToString(longDateFormat);
                string endDate = Show.EndDate is null ? Show.EndDateString ?? "Present" : ((DateTime)Show.EndDate).ToString(longDateFormat);
                string txt = $"{Show.Name}\n\n{Show.Network} ({Show.Country})\n{startDate} — {endDate}";
                Label showInfoLabel = new Label
                {
                    Name = "lbl_show_" + Show.ID.ToString(),
                    Text = txt,
                    Font = lblTop.Font,
                    ForeColor = lblTop.ForeColor,
                    Visible = true,
                    Width = 205,
                    Height = 13 * 4,
                    Location = new Point(205 * 2 + 10, 50)
                };
                PictureBox imgShowThumbnail = new PictureBox
                {
                    Name = "img_show_" + Show.ID.ToString(),
                    Width = 205,
                    Height = 274,
                    Location = new Point(205, 50),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    ImageLocation = Show.ThumbnailPath,
                    Anchor = AnchorStyles.Right
                };
                Button btnShowDetails = new Button
                {
                    Name = "btn_show_" + Show.ID.ToString(),
                    Location = new Point(205 * 2 + 10, 50 + showInfoLabel.Height + 10),
                    Text = "View Show Details...",
                    Width = 195,
                    UseVisualStyleBackColor = true
                };
                Button btnToggleShowSubscription = new Button
                {
                    Name = "btn_sub_" + Show.ID.ToString(),
                    Location = new Point(205 * 2 + 10, 50 + showInfoLabel.Height + 10 + btnShowDetails.Height + 10),
                    Width = 195,
                    UseVisualStyleBackColor = true
                };
                UpdateSubscribeButtonText(btnToggleShowSubscription, Show);
                void btnShowDetails_Click(object sender, EventArgs e)
                {
                    GoShowDetails(Show.ID, lastMenu, rawSearchQuery, pageNumber);
                }
                void btnToggleShowSubscription_Click(object sender, EventArgs e)
                {
                    if (!ToggleShowSubscription(Show, btnToggleShowSubscription))
                        if (CurrentlyViewingSubscriptions)
                            flpShowsList.Controls.Remove(pnlShowDetailsContainer);
                }
                showInfoLabel.Click += btnShowDetails_Click;
                imgShowThumbnail.Click += btnShowDetails_Click;
                btnShowDetails.Click += btnShowDetails_Click;
                btnToggleShowSubscription.Click += btnToggleShowSubscription_Click;
                pnlShowDetailsContainer.Controls.Add(showInfoLabel);
                pnlShowDetailsContainer.Controls.Add(imgShowThumbnail);
                pnlShowDetailsContainer.Controls.Add(btnShowDetails);
                pnlShowDetailsContainer.Controls.Add(btnToggleShowSubscription);
                flpShowsList.Controls.Add(pnlShowDetailsContainer);
            }
            return true;
        }

        private void DisplayNewEpisodes(Dictionary<int, SortedSet<string>> newEpisodes)
        {
            foreach (int showID in newEpisodes.Keys)
            {
                Label lblShowName = new Label
                {
                    Name = "lbl_shw_" + showID.ToString(),
                    Text = Settings.ShowNames[showID],
                    Font = lblTop.Font,
                    ForeColor = lblTop.ForeColor,
                    Width = 200,
                    Height = 13 * 2
                };
                lblShowName.Click += new EventHandler(LblShowName_Click);
                void LblShowName_Click(object sender, EventArgs e)
                {
                    GoShowDetails(showID, ProgramMenu.Home);
                }
                FlowLayoutPanel flpNewEpisodesInShow = new FlowLayoutPanel
                {
                    Name = "flp_shw_" + showID.ToString(),
                    Width = 235,
                    Height = newEpisodes[showID].Count * (23 + 6) + lblShowName.Height,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };
                flpNewEpisodesInShow.Controls.Add(lblShowName);
                for (int i = 0; i < newEpisodes[showID].Count; i++)
                {
                    string episodeSerialised = newEpisodes[showID].ElementAt(i);
                    Panel pnlEpisode = new Panel
                    {
                        Name = "pnl_epi_" + episodeSerialised,
                        Width = 235,
                        Height = 23,
                        BorderStyle = BorderStyle.Fixed3D
                    };
                    Label lblEpisode = new Label
                    {
                        Name = "lbl_epi_" + episodeSerialised,
                        Text = $"{i + 1}. {episodeSerialised}",
                        Font = lblTop.Font,
                        ForeColor = lblTop.ForeColor,
                        Width = 100,
                        Height = 13
                    };
                    Label lblStatus = new Label
                    {
                        Name = "lbl_sts_" + episodeSerialised,
                        Text = "In queue...",
                        Font = btnSearch.Font,
                        ForeColor = lblTop.ForeColor,
                        Width = 100,
                        Height = 13,
                        Location = new Point(lblEpisode.Width, 0)
                    };
                    lblEpisode.Click += new EventHandler(episodeClickEvent);
                    lblStatus.Click += new EventHandler(episodeClickEvent);
                    void episodeClickEvent(object sender, EventArgs e)
                    {
                        if (lblStatus.Text != "Downloaded")
                            return;
                        string torrentRelativePath = mainChecker.GetEpisodeTorrentPath(showID, episodeSerialised);
                        string torrentDirectory = AppDomain.CurrentDomain.BaseDirectory + torrentRelativePath;
                        ProcessStartInfo info = new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $@"""{torrentDirectory}""",
                            ErrorDialog = true
                        };
                        // Get the path to the magnet link file if it exists
                        foreach (string filename in Directory.GetFiles(torrentDirectory))
                        {
                            if (!filename.EndsWith(".url"))
                                continue;
                            // Add the process start argument to make the explorer window select the file
                            info.Arguments = $@"/select, ""{filename}""";
                            break;
                        }

                        Process.Start(info);
                    }
                    Episode episode = new Episode();
                    if (mainChecker.EpisodeHasBeenDownloaded(showID, episodeSerialised))
                    {
                        lblStatus.Text = "Downloaded";
                    }
                    else if (lblDownloadInfo.Visible && lblDownloadInfo.Text == Settings.ShowNames[showID] + " " + episodeSerialised)
                    {
                        lblStatus.Text = "Downloading...";
                    }
                    CheckBox chkEpisode = new CheckBox
                    {
                        Name = "chk_epi_" + episodeSerialised,
                        Location = new Point(lblStatus.Location.X + lblStatus.Width + 5, 0)
                    };
                    chkEpisode.Click += new EventHandler(chkWatchedEpisode_Click);
                    void chkWatchedEpisode_Click(object sender, EventArgs e)
                    {
                        if (!Settings.ShowLibrary.Any(SavedShow => SavedShow.ID == showID))
                            Settings.ShowLibrary.Add(new ShowHistory { ID = showID });
                        ShowHistory showToBeToggled = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == showID);
                        string seasonNumber = int.Parse(episodeSerialised.Remove(0, 1).Split('E')[0]).ToString();
                        int episodeNumber = int.Parse(episodeSerialised.Split('E')[1]);
                        if (showToBeToggled.WatchedEpisodes is null || !showToBeToggled.WatchedEpisodes.Any(WatchedEpisodesInSeason => WatchedEpisodesInSeason.Key == seasonNumber))
                        {
                            showToBeToggled.WatchedEpisodes.Add(seasonNumber, new SortedSet<int>());
                        }
                        SortedSet<int> watchedEpisodesInSeason = showToBeToggled.WatchedEpisodes[seasonNumber];
                        if (!watchedEpisodesInSeason.Any(EpisodeNum => EpisodeNum == episodeNumber))
                        {
                            watchedEpisodesInSeason.Add(episodeNumber);
                        }
                        else
                        {
                            watchedEpisodesInSeason.Remove(episodeNumber);
                        }
                        SaveDataFile();
                    }
                    pnlEpisode.Controls.Add(lblEpisode);
                    pnlEpisode.Controls.Add(lblStatus);
                    pnlEpisode.Controls.Add(chkEpisode);
                    flpNewEpisodesInShow.Controls.Add(pnlEpisode);
                }
                ThreadHelperClass.AddToParent(this, flpNewEpisodes, flpNewEpisodesInShow);
            }
        }

        private void DisplayUpcomingEpisodes(SortedDictionary<DateTime, Dictionary<int, List<Episode>>> upcomingEpisodes)
        {
            foreach (DateTime date in upcomingEpisodes.Keys)
            {
                string airDate = date.ToString($"ddd {shortDateFormat} {shortTimeFormat}");
                Label lblDate = new Label
                {
                    Text = $"{airDate}:",
                    Font = lblTop.Font,
                    ForeColor = lblTop.ForeColor,
                    Width = 120,
                    Height = 13
                };
                FlowLayoutPanel flpNewEpisodesOnDate = new FlowLayoutPanel
                {
                    Width = lblDate.Width + 10,
                    Height = lblDate.Height,
                    FlowDirection = FlowDirection.TopDown,
                    BorderStyle = BorderStyle.FixedSingle
                };
                flpNewEpisodesOnDate.Controls.Add(lblDate);
                foreach (int showID in upcomingEpisodes[date].Keys)
                {
                    flpNewEpisodesOnDate.Height += (13 * 3 + 6) * upcomingEpisodes[date][showID].Count;
                    foreach (Episode episode in upcomingEpisodes[date][showID])
                    {
                        string episodeSerialised = episode.Serialised;
                        Label lblEpisode = new Label
                        {
                            Name = "lbl_epi_" + episodeSerialised,
                            Text = $"{Settings.ShowNames[showID]} {episodeSerialised} - {episode.Name}",
                            Font = lblTop.Font,
                            ForeColor = lblTop.ForeColor,
                            Width = 120,
                            Height = 13 * 3
                        };
                        flpNewEpisodesOnDate.Controls.Add(lblEpisode);
                    }
                }
                ThreadHelperClass.AddToParent(this, flpFutureEpisodes, flpNewEpisodesOnDate);
            }
        }

        /// <summary>
        /// Middleware function that runs on each window change. Updates the window title and resets control visibility.
        /// </summary>
        /// <param name="WindowName">The partial title that the application should display for this window.</param>
        private void ChangeWindow(string WindowName)
        {
            flpShowsList.Controls.Clear();
            btnMostPopular.Visible = flpShowsList.Visible = btnSearch.Visible = lblTop.Visible = tlpNewEpisodes.Visible = txtSearchBar.Visible = lblCentre.Visible = timer.Enabled = false;

            CurrentlyViewingSubscriptions = WindowName == "Subscriptions";

            Text = $"{WindowName} - LiveSeries";

            switch (WindowName)
            {
                case "Home":
                    btnHome.Text = "Settings";
                    break;
                default:
                    btnHome.Text = "Home";
                    break;
            }
            // delete settings menu controls as well as episodes list navigation buttons
            foreach (Control c in Controls.Find("delete_me_on_menu_change", false))
                c.Dispose();
        }

        private void GoShowDetails(int showID, ProgramMenu lastMenu, string rawSearchQuery = null, int pageNumber = 0)
        {
            void DisplayShowDetails(TvShow Show)
            {
                if (Show == null)
                {
                    lblTop.Text = "Uh oh!\nWe couldn't find that show. Please try again.";
                    lblTop.Visible = true;
                    return;
                }
                Panel pnlShowDetailsContainer = new Panel
                {
                    Visible = true,
                    Width = 205 * 4
                };
                Label lblShowName = new Label
                {
                    Text = Show.Name,
                    Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, (byte)238),
                    ForeColor = lblTop.ForeColor,
                    Width = 300,
                    Height = 13 * 4,
                    Location = new Point(335, 3),

                };
                PictureBox imgShowImage = new PictureBox
                {
                    Name = "img_show_" + Show.ID.ToString(),
                    Location = new Point(205, 65),
                    Width = 350,
                    Height = 400,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = Show.ImagePath
                };
                Label lblShowDescription = new Label
                {
                    Text = Regex.Replace(Show.Description, "<b>|</b>", "").Replace("<br>", "\n"),
                    Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, (byte)238),
                    ForeColor = lblTop.ForeColor,
                    Width = 450,
                    Height = 13 * 8,
                    Location = new Point(235, imgShowImage.Height + 100),
                };
                Button btnToggleShowSubscription = new Button
                {
                    Name = "btn_sub_" + Show.ID.ToString(),
                    Location = new Point(300, 25 + lblShowDescription.Location.Y + lblShowDescription.Height),
                    Width = 195,
                    UseVisualStyleBackColor = true
                };
                Dictionary<int, List<Episode>> seasons = new Dictionary<int, List<Episode>>();
                btnToggleShowSubscription.Click += new EventHandler(btnToggleShowSubscription_Click);
                void btnToggleShowSubscription_Click(object sender, EventArgs e)
                {
                    ToggleShowSubscription(Show, btnToggleShowSubscription);
                }
                UpdateSubscribeButtonText(btnToggleShowSubscription, Show);
                pnlShowDetailsContainer.Controls.Add(lblShowName);
                pnlShowDetailsContainer.Controls.Add(imgShowImage);
                pnlShowDetailsContainer.Controls.Add(lblShowDescription);
                pnlShowDetailsContainer.Controls.Add(btnToggleShowSubscription);
                pnlShowDetailsContainer.Height = 150 + lblShowName.Height + imgShowImage.Height + lblShowDescription.Height + btnToggleShowSubscription.Height;


                if (Show.Episodes.Count > 0)
                {
                    foreach (Episode episode in Show.Episodes)
                    {
                        // Populates the dictionary which holds the season number and a list of the season's episodes
                        if (seasons.ContainsKey(episode.SeasonNumber))
                            seasons[episode.SeasonNumber].Add(episode);
                        else
                            seasons.Add(episode.SeasonNumber, new List<Episode>() { episode });
                    }
                    TabControl tabShowSeasons = new TabControl
                    {
                        Location = new Point(245, btnToggleShowSubscription.Location.Y + btnToggleShowSubscription.Height + 50),
                        Width = lblShowDescription.Width - 20
                    };
                    const int numPixelsBeforeEpisodes = 10 + 30;
                    string getCheckAllButtonText(int seasonNumber, bool forceUnCheckAllValue = false)
                    {
                        if (Settings.ShowLibrary.Any(SavedShow => SavedShow.ID == Show.ID) || forceUnCheckAllValue)
                        {
                            ShowHistory savedShow = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == Show.ID);
                            if (savedShow.WatchedEpisodes.ContainsKey(seasonNumber.ToString()) || forceUnCheckAllValue)
                            {
                                List<Episode> episodesInSeason = seasons[seasonNumber];
                                if (savedShow.WatchedEpisodes[seasonNumber.ToString()].Count == episodesInSeason.Count || forceUnCheckAllValue)
                                {
                                    return "Uncheck all";
                                }
                            }
                        }
                        return "Check all";
                    }
                    Label lblOrderSelector = new Label
                    {
                        Name = "lbl_ord_" + Show.Name,
                        Size = new Size(71, 13),
                        Location = new Point(3 + 100, 10),
                        Text = "Display order:"
                    };
                    ComboBox cmbOrderSelector = new ComboBox
                    {
                        Name = "cmb_" + Show.Name,
                        Size = new Size(160, 13),
                        Location = new Point(lblOrderSelector.Location.X + lblOrderSelector.Width, 6),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmbOrderSelector.Items.AddRange(new string[] { "Ascending", "Descending" });
                    cmbOrderSelector.SelectedItem = cmbOrderSelector.Items[Settings.DefaultSortByAscending ? 0 : 1];
                    cmbOrderSelector.SelectedIndexChanged += new EventHandler(cmbOrderSelector_SelectedIndexChanged);
                    void cmbOrderSelector_SelectedIndexChanged(object sender, EventArgs e)
                    {
                        Settings.DefaultSortByAscending = cmbOrderSelector.Text == cmbOrderSelector.Items[0].ToString();
                        foreach (int seasonNumber in seasons.Keys)
                        {
                            TabPage currentTabPage = tabShowSeasons.TabPages[seasonNumber.ToString()];
                            currentTabPage.Controls.Clear();
                            populateTabPage(seasonNumber);
                        }
                        tabShowSeasons.SelectedTab.Controls.Add(lblOrderSelector);
                        tabShowSeasons.SelectedTab.Controls.Add(cmbOrderSelector);
                        SaveDataFile();
                    }
                    void populateTabPage(int seasonNumber)
                    {
                        List<Episode> episodesInSeason;
                        try
                        {
                            if (Settings.SortEpisodesByIndex)
                                episodesInSeason = seasons[seasonNumber].OrderBy(episode => episode.EpisodeNumber).ToList();
                            else
                                episodesInSeason = seasons[seasonNumber].OrderBy(episode => episode.AirDate).ToList();
                            if (!Settings.DefaultSortByAscending)
                                episodesInSeason.Reverse();
                        }
                        catch (ArgumentNullException)
                        {
                            episodesInSeason = seasons[seasonNumber];
                        }
                        catch (FormatException)
                        {
                            episodesInSeason = seasons[seasonNumber];
                        }
                        TabPage tp = tabShowSeasons.TabPages[seasonNumber.ToString()];
                        Label lblTotalEpisodes = new Label
                        {
                            Name = "lbl_ttl_" + seasonNumber.ToString(),
                            Size = new Size(100, 13),
                            Location = new Point(3, 10),
                            Text = "Total episodes: " + episodesInSeason.Count
                        };
                        Button btnCheckAll = new Button
                        {
                            Name = "btn_chk_" + seasonNumber.ToString(),
                            Size = new Size(75, 23),
                            Location = new Point(tabShowSeasons.Width - 85, 5)
                        };
                        btnCheckAll.Click += new EventHandler(btnCheckAll_Click);
                        tp.Controls.Add(lblTotalEpisodes);
                        tp.Controls.Add(btnCheckAll);
                        btnCheckAll.Text = getCheckAllButtonText(seasonNumber);
                        string msg = "";
                        for (int currentEpisode = 0; currentEpisode < episodesInSeason.Count; currentEpisode++)
                        {
                            Episode episode = episodesInSeason.ElementAt(currentEpisode);
                            msg += episode.Serialised + "\n";
                            int controlPixelOffset = currentEpisode * 13 * 5 + numPixelsBeforeEpisodes;
                            Label lblWatchedEpisode = new Label
                            {
                                Location = new Point(tabShowSeasons.Width - 74, controlPixelOffset),
                                Name = "epinfo_lbl_wch_" + episode.Serialised,
                                Size = new Size(60, 13),
                                Text = "Watched?"
                            };
                            CheckBox chkWatchedEpisode = new CheckBox
                            {
                                Name = "epinfo_chk_" + episode.Serialised,
                                Location = new Point(tabShowSeasons.Width - 52, 13 + controlPixelOffset)
                            };
                            ShowHistory savedShow = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == Show.ID);
                            if (savedShow != null && savedShow.WatchedEpisodes != null)
                            {
                                try
                                {
                                    chkWatchedEpisode.Checked = savedShow.WatchedEpisodes[episode.SeasonNumber.ToString()].Contains(episode.EpisodeNumber);
                                }
                                catch (KeyNotFoundException)
                                {
                                    chkWatchedEpisode.Checked = false;
                                }
                            }
                            Label lblEpisodeText = new Label
                            {
                                Location = new Point(10, controlPixelOffset),
                                Name = "epinfo_lbl_txt_" + episode.Serialised,
                                Size = new Size(lblWatchedEpisode.Location.X - 20, 13 * 2),
                                Text = episode.Serialised + " — " + episode.Name
                            };
                            string airDate = episode.AirDate.ToString($"dddd, {longDateFormat} {shortTimeFormat}");
                            Label lblEpisodeAirDate = new Label
                            {
                                Location = new Point(10, lblEpisodeText.Height + controlPixelOffset),
                                Name = "epinfo_lbl_tx2_" + episode.Serialised,
                                Size = new Size(lblWatchedEpisode.Location.X - 20, 13),
                                Text = $"Air date: {airDate}"
                            };
                            chkWatchedEpisode.Click += new EventHandler(chkWatchedEpisode_Click);
                            lblWatchedEpisode.Click += new EventHandler(lblWatchedEpisode_Click);
                            void chkWatchedEpisode_Click(object sender, EventArgs e)
                            {
                                if (!Settings.ShowLibrary.Any(SavedShow => SavedShow.ID == Show.ID))
                                    Settings.ShowLibrary.Add(new ShowHistory { ID = Show.ID });
                                ShowHistory showToBeToggled = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == Show.ID);
                                if (showToBeToggled.WatchedEpisodes is null || !showToBeToggled.WatchedEpisodes.Any(WatchedEpisodesInSeason => WatchedEpisodesInSeason.Key == episode.SeasonNumber.ToString()))
                                {
                                    showToBeToggled.WatchedEpisodes.Add(episode.SeasonNumber.ToString(), new SortedSet<int>());
                                }
                                SortedSet<int> watchedEpisodesInSeason = showToBeToggled.WatchedEpisodes[episode.SeasonNumber.ToString()];
                                if (!watchedEpisodesInSeason.Any(EpisodeNum => EpisodeNum == episode.EpisodeNumber))
                                {
                                    watchedEpisodesInSeason.Add(episode.EpisodeNumber);
                                }
                                else
                                {
                                    watchedEpisodesInSeason.Remove(episode.EpisodeNumber);
                                }
                                SaveDataFile();
                                btnCheckAll.Text = getCheckAllButtonText(seasonNumber);
                            }
                            void lblWatchedEpisode_Click(object sender, EventArgs e)
                            {
                                chkWatchedEpisode.Checked = !chkWatchedEpisode.Checked;
                                chkWatchedEpisode_Click(sender, e);
                            }
                            tp.Controls.Add(lblEpisodeText);
                            tp.Controls.Add(lblEpisodeAirDate);
                            tp.Controls.Add(lblWatchedEpisode);
                            tp.Controls.Add(chkWatchedEpisode);
                        }
                        void btnCheckAll_Click(object sender, EventArgs e)
                        {
                            bool shouldMarkAllEpisodesAsWatched = true;
                            if (!Settings.ShowLibrary.Any(SavedShow => SavedShow.ID == Show.ID))
                                Settings.ShowLibrary.Add(new ShowHistory() { ID = Show.ID });
                            ShowHistory savedShow = Settings.ShowLibrary.Find(SavedShow => SavedShow.ID == Show.ID);
                            if (!savedShow.WatchedEpisodes.Any(watchedEpisodesInSeason => watchedEpisodesInSeason.Key == seasonNumber.ToString()))
                                savedShow.WatchedEpisodes.Add(seasonNumber.ToString(), new SortedSet<int>());
                            else if (savedShow.WatchedEpisodes[seasonNumber.ToString()].Count == episodesInSeason.Count)
                            {
                                // Mark all episodes as unwatched
                                shouldMarkAllEpisodesAsWatched = false;
                            }
                            if (shouldMarkAllEpisodesAsWatched)
                            {
                                // Mark all episodes as watched
                                foreach (Episode episode in episodesInSeason)
                                    if (!savedShow.WatchedEpisodes[seasonNumber.ToString()].Contains(episode.EpisodeNumber))
                                        savedShow.WatchedEpisodes[seasonNumber.ToString()].Add(episode.EpisodeNumber);
                            }
                            else
                            {
                                savedShow.WatchedEpisodes.Remove(seasonNumber.ToString());
                            }
                            SaveDataFile();
                            foreach (Control control in tp.Controls)
                                if (control is CheckBox box)
                                    box.Checked = shouldMarkAllEpisodesAsWatched;
                            btnCheckAll.Text = getCheckAllButtonText(seasonNumber, forceUnCheckAllValue: shouldMarkAllEpisodesAsWatched);
                        }
                    }
                    foreach (int seasonNumber in seasons.Keys)
                    {
                        TabPage tpEpisodes = new TabPage
                        {
                            Name = seasonNumber.ToString(),
                            Text = "Season " + seasonNumber.ToString(),
                            Width = 274
                        };
                        tabShowSeasons.TabPages.Add(tpEpisodes);
                        populateTabPage(seasonNumber);
                        if (seasonNumber == seasons.Keys.First())
                        {
                            tpEpisodes.Controls.Add(lblOrderSelector);
                            tpEpisodes.Controls.Add(cmbOrderSelector);
                        }
                    }
                    tabShowSeasons.Selected += new TabControlEventHandler(tabShowSeasons_Selected);
                    void tabShowSeasons_Selected(object sender, TabControlEventArgs e)
                    {
                        setCorrectContainerHeight(e.TabPage);
                        e.TabPage.Controls.Add(lblOrderSelector);
                        e.TabPage.Controls.Add(cmbOrderSelector);
                    }
                    void setCorrectContainerHeight(TabPage selectedTabPage)
                    {
                        string seasonName = selectedTabPage.Text;
                        int seasonNumber = int.Parse(seasonName.Substring(7, seasonName.Length - 7)); // 7 == "Season ".Length
                        int episodesCount = seasons[seasonNumber].Count;
                        int tabShowSeasonsHeight = episodesCount * 13 * 5; // num of episodes * 13px * 5 rows of text
                        tabShowSeasons.Height = tabShowSeasonsHeight + numPixelsBeforeEpisodes + 13; // 10px of space before and 13px after
                        pnlShowDetailsContainer.Height = 150 + lblShowName.Height + imgShowImage.Height + lblShowDescription.Height + tabShowSeasons.Height;
                    }
                    if (tabShowSeasons.TabPages.Count > 0)
                        setCorrectContainerHeight(tabShowSeasons.TabPages[0]);
                    pnlShowDetailsContainer.Controls.Add(tabShowSeasons);
                    pnlShowDetailsContainer.Height += tabShowSeasons.Height;
                }
                flpShowsList.Controls.Add(pnlShowDetailsContainer);
            }

            TvShow ShowToDisplay = GetShowFromID(showID);
            ChangeWindow($@"Viewing ""{ShowToDisplay.Name}""");
            flpShowsList.Visible = true;
            switch (lastMenu)
            {
                case ProgramMenu.Search:
                    btnHome.Text = $"Page {pageNumber} of search results for: \"{rawSearchQuery}\"";
                    break;
                case ProgramMenu.Subscriptions:
                    btnHome.Text = "Subscriptions";
                    break;
                case ProgramMenu.MostPopular:
                    btnHome.Text = $"Most Popular Shows | Page {pageNumber}";
                    break;
                default:
                    break;
            }
            DisplayShowDetails(ShowToDisplay);
        }

        private void GoSearch(string rawSearchQuery, int pageNumber = 1)
        {
            ChangeWindow($"Searching \"{rawSearchQuery}\"");
            DisplayShows(ProgramMenu.Search, rawSearchQuery, pageNumber);
        }

        private void GoMostPopular(int pageNumber = 1)
        {
            ChangeWindow("Most Popular");
            DisplayShows(ProgramMenu.MostPopular, null, pageNumber);
        }

        public void GoHome()
        {
            ChangeWindow("Home");
            lblTop.Text = "";
            txtSearchBar.Visible = true;
            btnSearch.Visible = true;
            lblCentre.Visible = true;
            lblCentre.Location = new Point(lblCentre.Location.X, 140);
            tlpNewEpisodes.Visible = true;
            btnMostPopular.Visible = true;
            btnHome.Text = "Settings";
            txtSearchBar.Focus();
            txtSearchBar.SelectAll();
            KeyValuePair<Dictionary<int, SortedSet<string>>, int> newEpisodes = mainChecker.GetUnwatchedEpisodes(ignoreDownloadedEpisodes: false);
            SortedDictionary<DateTime, Dictionary<int, List<Episode>>> upcomingEpisodes = mainChecker.GetUpcomingEpisodes();
            flpNewEpisodes.Controls.Clear();
            flpFutureEpisodes.Controls.Clear();
            if (newEpisodes.Value > 0)
            {
                lblCentre.Text = $"New unwatched episodes ({newEpisodes.Value}):";
                Thread populateNewEpsThread = new Thread(t => DisplayNewEpisodes(newEpisodes.Key));
                populateNewEpsThread.Start();
            }
            else
            {
                void updateLabelText()
                {
                    currentTime = DateTime.Now;
                    TimeSpan t = currentTime - Settings.LastEpisodesCheckDate;
                    lblCentre.Text = $"No new episodes to watch.\nLast checked: {t.Minutes:D2}m {t.Seconds:D2}s ago.";
                }
                void updateLabelTextHandler(object sender, EventArgs e)
                {
                    updateLabelText();
                }
                updateLabelText();
                timer.Tick += updateLabelTextHandler;
                timer.Interval = 1000;
                timer.Enabled = true;
            }
            Thread populateUpcomingEpsThread = new Thread(t => DisplayUpcomingEpisodes(upcomingEpisodes));
            populateUpcomingEpsThread.Start();
        }

        private void GoSettings()
        {
            ChangeWindow("Settings");
            lblCentre.Visible = true;
            lblCentre.Text = "Preferences";
            lblCentre.Location = new Point(lblCentre.Location.X, 40);

            Label lblOrderSelector = new Label
            {
                Name = "delete_me_on_menu_change",
                AutoSize = true,
                Location = new Point(lblCentre.Location.X - 127, 74),
                ForeColor = lblCentre.ForeColor,
                Text = "Default episodes display order:",
                Anchor = AnchorStyles.Top
            };
            ComboBox cmbOrderSelector = new ComboBox
            {
                Name = "delete_me_on_menu_change",
                Size = new Size(160, 13),
                Location = new Point(lblOrderSelector.Location.X + 2, 90),
                Anchor = AnchorStyles.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Label lblSortType = new Label
            {
                Name = "delete_me_on_menu_change",
                AutoSize = true,
                Location = new Point(cmbOrderSelector.Location.X + cmbOrderSelector.Width + 20, 74),
                ForeColor = lblCentre.ForeColor,
                Text = "Sort episodes by:",
                Anchor = AnchorStyles.Top
            };
            ComboBox cmbSortType = new ComboBox
            {
                Name = "delete_me_on_menu_change",
                Size = new Size(160, 13),
                Location = new Point(lblSortType.Location.X + 2, 90),
                Anchor = AnchorStyles.Top,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Label lblAutomaticDownloads = new Label
            {
                Name = "delete_me_on_menu_change",
                AutoSize = true,
                Location = new Point(lblCentre.Location.X - 110, 124),
                ForeColor = lblCentre.ForeColor,
                Text = "Automatically download new episodes as soon as they air",
                Anchor = AnchorStyles.Top
            };
            Label lblAskAfterDownloads = new Label
            {
                Name = "delete_me_on_menu_change",
                AutoSize = true,
                Location = new Point(lblCentre.Location.X - 110, 204),
                ForeColor = lblCentre.ForeColor,
                Text = "Ask to open the downloads folder after downloading new episodes",
                Anchor = AnchorStyles.Top
            };
            Label lblDesc1 = new Label
            {
                Name = "delete_me_on_menu_change",
                Size = new Size(365, 13 * 3),
                Location = new Point(lblCentre.Location.X - 127, 144),
                ForeColor = Color.LightGray,
                Text = "When deselected, LiveSeries won't automatically schedule any detected new episodes for download. Instead, it will send a notification asking you if you want to download them.",
                Anchor = AnchorStyles.Top
            };
            Label lblDesc2 = new Label
            {
                Name = "delete_me_on_menu_change",
                Size = new Size(365, 13 * 3),
                Location = new Point(lblCentre.Location.X - 127, 224),
                ForeColor = Color.LightGray,
                Text = "When selected, LiveSeries will ask you if you want to open the folder where downloaded episodes' information is stored after each download session. If any download fails, it will ask you if you wish to retry that download instead.",
                Anchor = AnchorStyles.Top
            };
            CheckBox chkAutomaticDownloads = new CheckBox
            {
                Name = "delete_me_on_menu_change",
                Location = new Point(lblCentre.Location.X - 125, lblAutomaticDownloads.Location.Y - 4),
                Anchor = AnchorStyles.Top
            };
            CheckBox chkAskAfterDownloads = new CheckBox
            {
                Name = "delete_me_on_menu_change",
                Location = new Point(lblCentre.Location.X - 125, lblAskAfterDownloads.Location.Y - 4),
                Anchor = AnchorStyles.Top
            };

            cmbOrderSelector.Items.AddRange(new string[] { "Ascending", "Descending" });
            cmbOrderSelector.SelectedItem = cmbOrderSelector.Items[Settings.DefaultSortByAscending ? 0 : 1];
            cmbSortType.Items.AddRange(new string[] { "Number in series", "Release date" });
            cmbSortType.SelectedItem = cmbSortType.Items[Settings.SortEpisodesByIndex ? 0 : 1];
            chkAutomaticDownloads.Checked = Settings.AutomaticDownloads;
            chkAskAfterDownloads.Checked = Settings.AskToOpenFolderAfterDownloading;

            cmbOrderSelector.SelectedIndexChanged += new EventHandler(cmbOrderSelector_SelectedIndexChanged);
            cmbSortType.SelectedIndexChanged += new EventHandler(cmbSortType_SelectedIndexChanged);
            chkAutomaticDownloads.Click += new EventHandler(chkAutomaticDownloads_Click);
            chkAskAfterDownloads.Click += new EventHandler(chkAskAfterDownloads_Click);
            lblAutomaticDownloads.Click += new EventHandler(lblAutomaticDownloads_Click);
            lblAskAfterDownloads.Click += new EventHandler(lblAskAfterDownloads_Click);

            void chkAutomaticDownloads_Click(object sender, EventArgs e)
            {
                Settings.AutomaticDownloads = !Settings.AutomaticDownloads;
                SaveDataFile();
            }
            void chkAskAfterDownloads_Click(object sender, EventArgs e)
            {
                Settings.AskToOpenFolderAfterDownloading = !Settings.AskToOpenFolderAfterDownloading;
                SaveDataFile();
            }
            void lblAutomaticDownloads_Click(object sender, EventArgs e)
            {
                chkAutomaticDownloads.Checked = !chkAutomaticDownloads.Checked;
                chkAutomaticDownloads_Click(sender, e);
            }
            void lblAskAfterDownloads_Click(object sender, EventArgs e)
            {
                chkAskAfterDownloads.Checked = !chkAskAfterDownloads.Checked;
                chkAskAfterDownloads_Click(sender, e);
            }
            void cmbOrderSelector_SelectedIndexChanged(object sender, EventArgs e)
            {
                Settings.DefaultSortByAscending = cmbOrderSelector.Text == cmbOrderSelector.Items[0].ToString();
                SaveDataFile();
            }
            void cmbSortType_SelectedIndexChanged(object sender, EventArgs e)
            {
                Settings.SortEpisodesByIndex = cmbSortType.Text == cmbSortType.Items[0].ToString();
                SaveDataFile();
            }

            Controls.Add(lblOrderSelector);
            Controls.Add(lblSortType);
            Controls.Add(cmbOrderSelector);
            Controls.Add(cmbSortType);

            Controls.Add(lblAutomaticDownloads);
            Controls.Add(chkAutomaticDownloads);
            Controls.Add(lblDesc1);

            Controls.Add(lblAskAfterDownloads);
            Controls.Add(chkAskAfterDownloads);
            Controls.Add(lblDesc2);
        }

        private void GoSubscriptions()
        {
            ChangeWindow("Subscriptions");
            flpShowsList.Visible = true;
            lblTop.Visible = true;
            btnHome.Focus();
            List<TvShow> subscriptionsTvShowObjects = new List<TvShow>();
            foreach (ShowHistory SavedShow in Settings.ShowLibrary.Where(SavedShow => SavedShow.Subscribed))
            {
                TvShow show = GetShowFromID(SavedShow.ID);
                if (show is null)
                    continue;
                subscriptionsTvShowObjects.Add(show);
            }
            ShowList shows = new ShowList
            {
                TotalShows = Settings.ShowLibrary.Where(SavedShow => SavedShow.Subscribed).Count().ToString(),
                TvShows = subscriptionsTvShowObjects
            };

            if (DisplayShowsList(shows, ProgramMenu.Subscriptions))
            {
                lblTop.Text = "Subscribed shows: " + shows.TotalShows;
                lblTop.Location = new Point(205 * 2, 10);
            }
            else
            {
                lblTop.Text = "You have no subscribed shows.\n";
                lblTop.Text += "Subscribe to a show by searching for it in the home screen and pressing the 'subscribe' button.";
                lblTop.Location = new Point(205, 10);
            }
        }

        public void RestoreWindow()
        {
            // Set the WindowState to normal if the form is minimized
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            // Activate the form
            Show();
            Activate();

            // If currently on homepage, reload it
            if (Text.StartsWith("Home"))
                GoHome();
        }

        /// <summary>
        /// Process the message instructing the instance to restore its window (used when app is launched while already running).
        /// </summary>
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                RestoreWindow();
            }
            base.WndProc(ref message);
        }

        #region Control event handlers
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            // When the user performs a search query
            GoSearch(txtSearchBar.Text);
        }

        private void LblNumSubscriptions_Click(object sender, EventArgs e)
        {
            GoSubscriptions();
        }

        private void BtnMostPopular_Click(object sender, EventArgs e)
        {
            GoMostPopular();
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            // Regex pattern:
            // group 1 (\d+) refers to one or more numeric characters (e.g. 7 or 17)
            // group 2 (.+) refers to one or more characters (i.e. one or multiple words)
            string searchResultsPattern = @"^Page (\d+) of search results for: ""(.+)""$";
            string mostPopularPattern = @"^Most Popular Shows \| Page (\d+)$";
            GroupCollection group;
            int pageNumber;
            if (btnHome.Text == "Settings")
                GoSettings();
            else if (btnHome.Text == "Subscriptions")
                GoSubscriptions();
            else if (Regex.IsMatch(btnHome.Text, searchResultsPattern))
            {
                group = Regex.Match(btnHome.Text, searchResultsPattern).Groups;
                pageNumber = int.Parse(group[1].Value);
                GoSearch(group[2].Value, pageNumber);
            }
            else if (Regex.IsMatch(btnHome.Text, mostPopularPattern))
            {
                group = Regex.Match(btnHome.Text, mostPopularPattern).Groups;
                pageNumber = int.Parse(group[1].Value);
                GoMostPopular(pageNumber);
            }
            else
                GoHome();
        }

        private void BtnHome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                BtnHome_Click(sender, e);
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            switch (Name.Split(' ')[0])
            {
                case "Home":
                    break;
            }
        }

        private void ItmRestore_Click(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        private void ItmHome_Click(object sender, EventArgs e)
        {
            RestoreWindow();
            GoHome();
        }

        private void ItmSettings_Click(object sender, EventArgs e)
        {
            RestoreWindow();
            GoSettings();
        }

        private void ItmSubscriptions_Click(object sender, EventArgs e)
        {
            RestoreWindow();
            GoSubscriptions();
        }

        private void ItmExit_Click(object sender, EventArgs e)
        {
            OptimiseSettings();
            SaveDataFile(instantly: true);
            mainChecker.shouldCheckForNewEpisodes = false;
            Log(new string[] {
                "----------------------------------",
                $"Program exiting (taskbar option).",
                "----------------------------------"
            });
            Close();
        }

        private void IcnTaskBar_MouseClick(object sender, MouseEventArgs e)
        {
            // Only handle left-click events
            if (e.Button != MouseButtons.Left)
                return;
            // Minimise to system tray if the application is open and visible
            if (Visible)
                Hide();
            // Restore the application window if it was previously minimised
            else
                RestoreWindow();
        }

        private void FormLiveSeries_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                Log(new string[] {
                    "---------------------------",
                    $"Program exiting (shift+X).",
                    "---------------------------"
                });
                return;
            }
            Hide();
            e.Cancel = mainChecker.shouldCheckForNewEpisodes;
        }
        #endregion
    }
}
