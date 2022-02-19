using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using Windows.Foundation.Collections;
using Microsoft.Toolkit.Uwp.Notifications;

namespace LiveSeries2
{
    partial class FormLiveSeries
    {
        private static class ThreadHelperClass
        {
            /// <summary>
            /// Set visibility property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="visibility"></param>
            public static void SetControlVisibility(Form form, Control ctrl, bool visibility)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    SetVisCallback d = SetControlVisibility;
                    form.Invoke(d, new object[] { form, ctrl, visibility });
                }
                else
                {
                    ctrl.Visible = visibility;
                }
            }
            delegate void SetVisCallback(Form f, Control ctrl, bool visibility);

            /// <summary>
            /// Set location property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="location"></param>
            public static void SetControlLocation(Form form, Control ctrl, Point location)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    SetLocCallback d = new SetLocCallback(SetControlLocation);
                    form.Invoke(d, new object[] { form, ctrl, location });
                }
                else
                {
                    ctrl.Location = location;
                }
            }
            delegate void SetLocCallback(Form f, Control ctrl, Point location);

            /// <summary>
            /// Set size property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="size"></param>
            public static void SetControlSize(Form form, Control ctrl, Size size)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    ResizeCallback d = new ResizeCallback(SetControlSize);
                    form.Invoke(d, new object[] { form, ctrl, size });
                }
                else
                {
                    ctrl.Size = size;
                }
            }
            delegate void ResizeCallback(Form f, Control ctrl, Size size);

            /// <summary>
            /// Set value property of ProgressBar control
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="prg"></param>
            /// <param name="value"></param>
            public static void SetProgressBarValue(Form form, ProgressBar prg, int value)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (prg.InvokeRequired)
                {
                    SetValCallback d = new SetValCallback(SetProgressBarValue);
                    form.Invoke(d, new object[] { form, prg, value });
                }
                else
                {
                    prg.Value = value;
                }
            }
            delegate void SetValCallback(Form f, ProgressBar ctrl, int value);

            /// <summary>
            /// Set text property of various controls
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            /// <param name="text"></param>
            public static void SetControlText(Form form, Control ctrl, string text)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    SetTxtCallback d = new SetTxtCallback(SetControlText);
                    form.Invoke(d, new object[] { form, ctrl, text });
                }
                else
                {
                    ctrl.Text = text;
                }
            }
            delegate void SetTxtCallback(Form f, Control ctrl, string text);

            /// <summary>
            /// Bring the form to the front
            /// </summary>
            /// <param name="form">The calling form</param>
            public static void BringToFront(Form form)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (form.InvokeRequired)
                {
                    BringFrontCallback d = new BringFrontCallback(BringToFront);
                    form.Invoke(d, new object[] { form });
                }
                else
                {
                    form.BringToFront();
                    form.WindowState = FormWindowState.Minimized;
                    form.Show();
                    form.WindowState = FormWindowState.Normal;
                }
            }
            delegate void BringFrontCallback(Form f);

            /// <summary>
            /// Clear a parent control of its children
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="ctrl"></param>
            public static void ClearChildren(Form form, Control ctrl)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (ctrl.InvokeRequired)
                {
                    ClearCallback d = new ClearCallback(ClearChildren);
                    form.Invoke(d, new object[] { form, ctrl });
                }
                else
                {
                    ctrl.Controls.Clear();
                }
            }
            delegate void ClearCallback(Form f, Control ctrl);

            /// <summary>
            /// Add a child control to a parent control
            /// </summary>
            /// <param name="form">The calling form</param>
            /// <param name="parent"></param>
            /// <param name="child"></param>
            public static void AddToParent(Form form, Control parent, Control child)
            {
                // InvokeRequired compares the thread ID of the 
                // calling thread to the thread ID of the creating thread. 
                // If these threads are different, it returns true. 
                if (parent.InvokeRequired)
                {
                    AddCallback d = new AddCallback(AddToParent);
                    try
                    {
                        form.Invoke(d, new object[] { form, parent, child });
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
                else
                {
                    try
                    {
                        parent.Controls.Add(child);
                    }
                    catch (Exception e)
                    {
                        Log(e.Message);
                    }
                }
            }
            delegate void AddCallback(Form f, Control parent, Control child);

        }

        private class NewEpisodesManager
        {
            public NewEpisodesManager(FormLiveSeries form)
            {
                baseForm = form;
            }

            public string GetEpisodeTorrentPath(int showID, string episodeSerialised)
            {
                (int seasonNo, int episodeNo) = DeserialiseEpisode(episodeSerialised);
                // Encode the show name
                string showName = RemoveInvalidDirectoryChars(baseForm.Settings.ShowNames[showID]);
                return $@"torrents\{showName}\{showID}\Season {seasonNo}\Episode {episodeNo}";
            }

            public bool EpisodeHasBeenDownloaded(int showID, string episodeSerialised)
            {
                string episodeTorrentPath = GetEpisodeTorrentPath(showID, episodeSerialised);
                // Check if the file directory exists
                return Directory.Exists(episodeTorrentPath);
            }

            public KeyValuePair<Dictionary<int, SortedSet<string>>, int> GetUnwatchedEpisodes(bool ignoreDownloadedEpisodes = true)
            {
                Dictionary<int, SortedSet<string>> unwatchedEpisodes = new Dictionary<int, SortedSet<string>>();
                int totalEpisodes = 0;
                foreach (ShowHistory subscribedShow in baseForm.Settings.ShowLibrary.Where(SavedShow => SavedShow.Subscribed))
                {
                    TvShow show = GetShowFromID(subscribedShow.ID);
                    if (show is null)
                        continue;
                    foreach (Episode episode in show.Episodes)
                    {
                        // Discard watched episodes
                        bool userHasWatchedEpisodesInThisSeason = subscribedShow.WatchedEpisodes.ContainsKey(episode.SeasonNumber.ToString());
                        if (userHasWatchedEpisodesInThisSeason)
                        {
                            SortedSet<int> episodesWatchedThisSeason = subscribedShow.WatchedEpisodes[episode.SeasonNumber.ToString()];
                            bool userHasWatchedThisEpisode = episodesWatchedThisSeason.Any(watchedEpisode => episode.EpisodeNumber == watchedEpisode);
                            if (userHasWatchedThisEpisode)
                                continue;
                        }
                        // Discard future episodesbaseForm.GetShowFromID
                        if (episode.AirDate > DateTime.Now)
                            continue;
                        if (!baseForm.Settings.ShowNames.ContainsKey(show.ID))
                            baseForm.Settings.ShowNames.Add(show.ID, show.Name);
                        if (ignoreDownloadedEpisodes && EpisodeHasBeenDownloaded(show.ID, episode.Serialised))
                            continue;
                        // Add to dictionary
                        if (!unwatchedEpisodes.ContainsKey(show.ID))
                            unwatchedEpisodes.Add(show.ID, new SortedSet<string>());
                        unwatchedEpisodes[show.ID].Add(episode.Serialised);
                        totalEpisodes++;
                    }
                }
                return new KeyValuePair<Dictionary<int, SortedSet<string>>, int>(unwatchedEpisodes, totalEpisodes);
            }

            public SortedDictionary<DateTime, Dictionary<int, List<Episode>>> GetUpcomingEpisodes()
            {
                SortedDictionary<DateTime, Dictionary<int, List<Episode>>> upcomingEpisodes = new SortedDictionary<DateTime, Dictionary<int, List<Episode>>>();
                foreach (ShowHistory subscribedShow in baseForm.Settings.ShowLibrary.Where(SavedShow => SavedShow.Subscribed))
                {
                    TvShow show = GetShowFromID(subscribedShow.ID);
                    if (show is null)
                        continue;
                    foreach (Episode episode in show.Episodes)
                    {
                        if (episode.AirDate > DateTime.Now)
                        {
                            if (!upcomingEpisodes.ContainsKey(episode.AirDate))
                                upcomingEpisodes.Add(episode.AirDate, new Dictionary<int, List<Episode>>());
                            if (!upcomingEpisodes[episode.AirDate].ContainsKey(subscribedShow.ID))
                                upcomingEpisodes[episode.AirDate].Add(subscribedShow.ID, new List<Episode>());
                            upcomingEpisodes[episode.AirDate][subscribedShow.ID].Add(episode);
                        }
                    }
                }
                return upcomingEpisodes;
            }

            public bool shouldCheckForNewEpisodes = true;

            private readonly FormLiveSeries baseForm;

            private List<string> successfulDownloads, unsuccessfulDownloads;

            private KeyValuePair<Dictionary<int, SortedSet<string>>, int> downloadQueue = new KeyValuePair<Dictionary<int, SortedSet<string>>, int>();

            public static string RemoveInvalidDirectoryChars(string rawString)
            {
                if (rawString is null)
                    return null;
                string[] temp = rawString.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries);
                return string.Join("", temp);
            }

            private async Task AttemptDownload(int showID, string episodeSerialised, int downloadedSoFar)
            {
                SetAsDownloading(showID, episodeSerialised, downloadedSoFar);
                string showNameEncoded = RemoveInvalidDirectoryChars(baseForm.Settings.ShowNames[showID]);
                string json = null;

                Log("[Step 1/6] Fetching the torrent info...");
                Thread GetJsonThread = new Thread(t => json = GetJsonStringFromURL(apibayURL, "q.php", $"{showNameEncoded}+{episodeSerialised}"));
                GetJsonThread.Start();
                Log("[Step 1/6] Started the HTTP fetch thread...");
                if (!GetJsonThread.Join(TimeSpan.FromSeconds(5)))
                {
                    Log("[Aborting] The fetch thread is unresponsive for over 5 seconds. Terminating it...");
                    GetJsonThread.Abort();
                }
                Log("[Step 1/6] HTTP fetch thread terminated successfully...");
                if (json is null)
                {
                    // Search query didn't return a JSON string
                    Log("[Aborting] Could not fetch data from the torrent source.");
                    await FinishDownloadTask("Download failed!", showID, episodeSerialised);
                    string unsuccessfulDownloadMessage = $"{baseForm.Settings.ShowNames[showID]} {episodeSerialised}\n" +
                        $"Could not connect to the torrent host. Please try again later.\n" +
                        $"https://{apibayURL}/q.php?q={showNameEncoded}+{episodeSerialised}";
                    unsuccessfulDownloads.Add(unsuccessfulDownloadMessage);
                    return;
                }
                List<ApiBaySearchResult> rawResults = JsonConvert.DeserializeObject<List<ApiBaySearchResult>>(json);
                List<ApiBaySearchResult> validResults = new List<ApiBaySearchResult>();
                Log($"[Step 2/6] Determining the best torrent source for {showID} {episodeSerialised}...");
                for (int i = 0; i < rawResults.Count; i++)
                {
                    string torrentName = rawResults[i].Name;
                    if (torrentName.StartsWith(showNameEncoded.Replace(' ', '.') + "." + episodeSerialised))
                        validResults.Add(rawResults[i]);
                }
                Log($"[Step 3/6] Filtered {validResults.Count} valid torrent candidate(s) from {rawResults.Count} total result(s)...");
                if (validResults.Count == 0)
                {
                    Log("[Aborting] Cancelling download task since there were no torrent candidates to select from.");
                    // Could not find a good torrent candidate
                    await FinishDownloadTask("Download failed!", showID, episodeSerialised);
                    string unsuccessfulDownloadMessage = $"{baseForm.Settings.ShowNames[showID]} {episodeSerialised}\n";
                    unsuccessfulDownloadMessage += $"({rawResults.Count} raw results, {validResults.Count} valid results)\n" +
                        $"https://{apibayURL}/q.php?q={showNameEncoded}+{episodeSerialised}";
                    unsuccessfulDownloads.Add(unsuccessfulDownloadMessage);
                    return;
                }
                IOrderedEnumerable<ApiBaySearchResult> candidatesBySeeders = validResults.OrderByDescending(t => int.Parse(t.Seeders));
                // ApiBaySearchResult torrentInfo;
                // if (candidatesBySeeders.Any(t => int.Parse(t.Seeders) > 0 && t.NumFiles == "1"))
                //     torrentInfo = candidatesBySeeders.First(t => int.Parse(t.Seeders) > 0 && t.NumFiles == "1");
                // else
                ApiBaySearchResult torrentInfo = candidatesBySeeders.First();
                Log($"[Step 4/6] Selected torrent candidate with {torrentInfo.Seeders} seeder(s) and {torrentInfo.NumFiles} total file(s)...");
                string torrentDirectory = GetEpisodeTorrentPath(showID, episodeSerialised);
                Directory.CreateDirectory(torrentDirectory);
                string magnetLink = $"magnet:?xt=urn:btih:{torrentInfo.InfoHash}&dn={torrentInfo.Name}";
                using (FileStream fs = File.Create($@"{torrentDirectory}\torrent_details.json"))
                {
                    string details = JsonConvert.SerializeObject(torrentInfo, Formatting.Indented);
                    byte[] info = new UTF8Encoding(true).GetBytes(details);
                    fs.Write(info, 0, info.Length);
                }
                using (StreamWriter writer = new StreamWriter(torrentDirectory + $@"\{torrentInfo.Name}.url"))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine($"URL={magnetLink}");
                    writer.Flush();
                }

                DownloadTorrent(magnetLink);

                string newStatusText = $"Downloaded! ({downloadedSoFar + 1}/{downloadQueue.Value})";
                await FinishDownloadTask(newStatusText, showID, episodeSerialised);
                successfulDownloads.Add(baseForm.Settings.ShowNames[showID] + " " + episodeSerialised);
            }

            private void SetAsDownloading(int showID, string episodeSerialised, int totalDownloaded)
            {
                Log($"[Step 0/6] Commencing download process of {showID} {episodeSerialised}...");

                // Don't update any controls if the application is minimised to the system tray
                if (!baseForm.Visible)
                {
                    return;
                }

                string newStatusText = $"Downloading ({totalDownloaded}/{downloadQueue.Value})...";
                string showName = baseForm.Settings.ShowNames[showID];
                baseForm.Invoke(new Action(() =>
                {
                    baseForm.prgDownload.Value = 0;
                    baseForm.lblDownloadStatus.Text = newStatusText;
                    baseForm.lblDownloadInfo.Text = $"{showName} {episodeSerialised}";
                    baseForm.prgDownload.Visible = true;
                    baseForm.lblDownloadStatus.Visible = true;
                    baseForm.lblDownloadInfo.Visible = true;
                    if (!baseForm.Text.StartsWith("Home"))
                    {
                        baseForm.GoHome();
                        return;
                    }
                    Control showContainer = baseForm.flpNewEpisodes.Controls[$"flp_shw_{showID}"];
                    Control episodeContainer = showContainer.Controls[$"pnl_epi_{episodeSerialised}"];
                    Control statusLabel = episodeContainer.Controls[$"lbl_sts_{episodeSerialised}"];
                    statusLabel.Text = "Downloading...";
                }));
            }

            private void DownloadTorrent(string magnetLink)
            {
                Log("[Step 5/6] Launching the torrent client...");
                // For now just opens the magnet link; if the user has a torrent client installed, it will start the download there
                Process.Start(magnetLink);
            }

            private async Task FinishDownloadTask(string newStatusText, int showID, string episodeSerialised)
            {
                Log($"[Finished] Download process of {showID} {episodeSerialised} has reached completion.");
                ThreadHelperClass.SetProgressBarValue(baseForm, baseForm.prgDownload, 100);
                ThreadHelperClass.SetControlText(baseForm, baseForm.lblDownloadStatus, newStatusText);
                int delayInMilliseconds = 1000;
                await Task.Delay(delayInMilliseconds);
                ThreadHelperClass.SetControlVisibility(baseForm, baseForm.prgDownload, false);
                ThreadHelperClass.SetControlVisibility(baseForm, baseForm.lblDownloadStatus, false);
                ThreadHelperClass.SetControlVisibility(baseForm, baseForm.lblDownloadInfo, false);

                try
                {
                    Control c = baseForm.flpNewEpisodes.Controls[$"flp_shw_{showID}"].Controls[$"pnl_epi_{episodeSerialised}"].Controls[$"lbl_sts_{episodeSerialised}"];
                    ThreadHelperClass.SetControlText(baseForm, c, newStatusText == "Download failed!" ? "Failed" : "Downloaded");
                }
                catch (IndexOutOfRangeException)
                {
                    // User is not in the home menu
                }
            }

            private async Task ProcessDownloadQueue()
            {
                // Don't download new episodes if there are none
                if (downloadQueue.Value == 0)
                    return;
                // Initialise containers to hold information about the episodes that were downloaded
                successfulDownloads = new List<string>();
                unsuccessfulDownloads = new List<string>();
                int downloadedSoFar = 0;
                // Don't download new episodes if the user doesn't want to
                if (!baseForm.Settings.AutomaticDownloads && !AskIfShouldDownloadEpisodes())
                    return;
                // Refresh the homescreen to display new episodes
                if (baseForm.Name.StartsWith("Home"))
                    baseForm.GoHome();
                // Process episodes show by show
                foreach (int showID in downloadQueue.Key.Keys)
                {
                    // Process each of the show's new episodes
                    for (int i = 0; i < downloadQueue.Key[showID].Count; i++)
                    {
                        await AttemptDownload(showID, downloadQueue.Key[showID].ElementAt(i), downloadedSoFar++);
                    }
                }
                if ((unsuccessfulDownloads.Count == 0 && !baseForm.Settings.AskToOpenFolderAfterDownloading) || !AskAfterDownloadsComplete())
                    // Finish
                    return;
                // AskAfterDownloadsComplete has two modes:
                // 1. Returns true if the user selects 'yes' when prompted to open the downloads folder, else false
                // 2. Returns true if the user selects 'retry' when prompted to retry failed downloads, else false
                // Mode 1 is used when all downloads were successful, else Mode 2 is used.
                if (unsuccessfulDownloads.Count == 0)
                    // Open downloads folder
                    Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory + "torrents");
                else
                    // Retry unsuccessful downloads (recursive call)
                    await ProcessDownloadQueue();
            }

            private bool AskIfShouldDownloadEpisodes()
            {
                string caption = $"{downloadQueue.Value} new episode";
                string messageIntro = "";
                string message = "";
                int epNumber = 0;
                int maxDigits = $"{downloadQueue.Value}".Length;
                foreach (int showID in downloadQueue.Key.Keys)
                {
                    for (int i = 0; i < downloadQueue.Key[showID].Count; i++)
                    {
                        epNumber++;
                        string episode = downloadQueue.Key[showID].ElementAt(i);
                        string epNo = epNumber.ToString($"D{maxDigits}");
                        message += $"{epNo}. {baseForm.Settings.ShowNames[showID]} - {episode}\n";
                    }
                    message += "\n";
                }
                if (downloadQueue.Value == 1)
                {
                    messageIntro = "A new unwatched episode has aired";
                    message = $"{messageIntro}:\n\n{message}\nDownload this episode now?";
                }
                else
                {
                    caption += "s";
                    messageIntro = "New unwatched episodes have aired";
                    message = $"{messageIntro}:\n\n{message}\nDownload these episodes now?";
                }

                // Determines whether we should create a popup message box or a Windows application notification,
                // depending on if the program is in the foreground or not.
                if (baseForm.Visible)
                {
                    DialogResult result = MessageBox.Show(
                        message,
                        caption,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                    return result == DialogResult.Yes;
                }
                else
                {
                    new ToastContentBuilder()
                        .AddText(messageIntro + "!")
                        .AddText(caption)
                        .Show();
                    // Listen to notification activation
                    ToastNotificationManagerCompat.OnActivated += toastArgs =>
                    {
                        // Obtain the arguments from the notification
                        ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                        // Obtain any user input (text boxes, menu selections) from the notification
                        ValueSet userInput = toastArgs.UserInput;
                        ThreadHelperClass.BringToFront(baseForm);
                    };
                    return false;
                }
            }

            private bool AskAfterDownloadsComplete()
            {
                string s = downloadQueue.Value == 1 ? "" : "s";
                string caption = $"{downloadQueue.Value} download{s} complete (successful: {successfulDownloads.Count}, unsuccessful: {unsuccessfulDownloads.Count})";
                string successfulMsg = "\n";
                string unsuccessfulMsg = "\n";
                DialogResult result;
                if (successfulDownloads.Count > 0)
                {
                    successfulMsg = "Successful downloads:\n";
                    for (int i = 0; i < successfulDownloads.Count; i++)
                        successfulMsg += $"{i + 1}. {successfulDownloads[i]}\n\n";

                }
                if (unsuccessfulDownloads.Count > 0)
                {
                    unsuccessfulMsg = "Unsuccessful downloads:\n";
                    for (int i = 0; i < unsuccessfulDownloads.Count; i++)
                        unsuccessfulMsg += $"{i + 1}. {unsuccessfulDownloads[i]}\n\n";

                    result = MessageBox.Show(
                        successfulMsg + unsuccessfulMsg + "Retry failed downloads?",
                        caption,
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    result = MessageBox.Show(
                        successfulMsg + unsuccessfulMsg + "Open downloads folder?",
                        caption,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                }
                return result == DialogResult.Yes || result == DialogResult.Retry;
            }

            public async Task RunChecksForNewEpisodes()
            {
                DateTime currentDate;

                while (shouldCheckForNewEpisodes)
                {
                    currentDate = DateTime.Now;

                    // Download queue is updated every 5 seconds (5000 milliseconds)
                    downloadQueue = GetUnwatchedEpisodes();
                    if (downloadQueue.Value > 0)
                    // Performs a check for new episodes every 1 hour
                    // if (currentDate >= baseForm.Settings.LastEpisodesCheckDate.Add(new TimeSpan(hours: 1, 0, 0)))
                    {
                        Log(new string[] {
                            $"Downloading new episodes (last check: {baseForm.Settings.LastEpisodesCheckDate:HH':'mm':'ss})...",
                            $"Episodes to download: {downloadQueue.Value}"
                        });
                        baseForm.Settings.LastEpisodesCheckDate = currentDate;
                        baseForm.SaveDataFile();
                        await ProcessDownloadQueue();
                    }

                    await Task.Delay(5000);
                }
            }
        }

        private class ApiBaySearchResult
        {
            [JsonProperty("id")]
            public string ID { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("info_hash")]
            public string InfoHash { get; set; }
            [JsonProperty("leechers")]
            public string Leechers { get; set; }
            [JsonProperty("seeders")]
            public string Seeders { get; set; }
            [JsonProperty("num_files")]
            public string NumFiles { get; set; }
            [JsonProperty("size")]
            public string Size { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("added")]
            public string DateAdded { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("category")]
            public string Category { get; set; }
            [JsonProperty("imdb")]
            public string IMDB { get; set; }
        }

        private class ShowList
        {
            [JsonProperty("total")]
            public string TotalShows { get; set; }
            [JsonProperty("page")]
            public int CurrentPage { get; set; }
            [JsonProperty("pages")]
            public int TotalPages { get; set; }
            [JsonProperty("tv_shows")]
            public List<TvShow> TvShows { get; set; }
        }

        private class ShowData
        {
            [JsonProperty("tvShow")]
            public TvShow TvShowData { get; set; }
        }

        private class TvShow
        {
            [JsonProperty("id")]
            public int ID { get; set; } // Present in tv show overview on search page
            [JsonProperty("name")]
            public string Name { get; set; } // Present in tv show overview on search page
            [JsonProperty("permalink")]
            public string Permalink { get; set; } // Present in tv show overview on search page
            [JsonProperty("url")]
            public string URL { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("description_source")]
            public string DescriptionSource { get; set; }
            public DateTime? StartDate { get; set; }
            private string StartDateStr { get; set; }
            [JsonProperty("start_date")]
            public string StartDateString
            {
                get
                {
                    return StartDate is null ? StartDateStr : StartDate.ToString(); 
                }
                set
                {
                    StartDateStr = value;
                    if (value is null)
                        return;
                    try
                    {
                        StartDate = DateTime.Parse(value) + TimeZone.CurrentTimeZone.GetUtcOffset(currentTime);
                    } catch (FormatException)
                    {
                        if (value.Length == 0)
                        {
                            StartDateStr = null;
                        }
                        Log($"Invalid start date '{value}' for show '{Name}'.");
                    }
                }
            } // Present in tv show overview on search page
            private string EndDateStr { get; set; }
            public DateTime? EndDate { get; set; }
            [JsonProperty("end_date")]
            public string EndDateString
            {
                get
                {
                    return EndDate is null ? EndDateStr : EndDate.ToString();
                }
                set
                {
                    EndDateStr = value;
                    if (value is null)
                        return;
                    try
                    {
                        EndDate = DateTime.Parse(value) + TimeZone.CurrentTimeZone.GetUtcOffset(currentTime);
                    }
                    catch (FormatException)
                    {
                        if (value.Length ==  0)
                        {
                            EndDateStr = null;
                        }
                        Log($"Invalid end date '{value}' for show '{Name}'.");
                    }
                }
            } // Present in tv show overview on search page
            [JsonProperty("country")]
            public string Country { get; set; } // Present in tv show overview on search page
            [JsonProperty("status")]
            public string Status { get; set; } // Present in tv show overview on search page
            [JsonProperty("runtime")]
            public int Runtime { get; set; }
            [JsonProperty("network")]
            public string Network { get; set; } // Present in tv show overview on search page
            [JsonProperty("youtube_link")]
            public string YTLink { get; set; }
            [JsonProperty("image_path")]
            public string ImagePath { get; set; }
            [JsonProperty("image_thumbnail_path")]
            public string ThumbnailPath { get; set; } // Present in tv show overview on search page
            [JsonProperty("rating")]
            public string Rating { get; set; }
            [JsonProperty("rating_count")]
            public string RatingCount { get; set; }
            [JsonProperty("countdown")]
            public dynamic Countdown { get; set; }
            [JsonProperty("genres")]
            public List<string> Genres { get; set; }
            [JsonProperty("pictures")]
            public List<string> Pictures { get; set; }
            [JsonProperty("episodes")]
            public List<Episode> Episodes { get; set; }
        }

        private class Episode
        {
            [JsonProperty("season")]
            public int SeasonNumber { get; set; }
            [JsonProperty("episode")]
            public int EpisodeNumber { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            public DateTime AirDate { get; set; }
            [JsonProperty("air_date")]
            public string AirDateString
            {
                get { return AirDate.ToString(); }
                set { AirDate = DateTime.Parse(value) + TimeZone.CurrentTimeZone.GetUtcOffset(currentTime); }
            }
            public string Serialised => $"S{SeasonNumber:D2}E{EpisodeNumber:D2}";
        }

        private class ProgramSettings
        {
            public bool DefaultSortByAscending { get; set; } = false;
            public bool SortEpisodesByIndex { get; set; } = true;
            public bool AutomaticDownloads { get; set; } = false;
            public bool AskToOpenFolderAfterDownloading { get; set; } = true;
            public DateTime LastEpisodesCheckDate { get; set; } = DateTime.MinValue;
            public List<ShowHistory> ShowLibrary { get; set; } = new List<ShowHistory>();
            public Dictionary<int, string> ShowNames { get; set; } = new Dictionary<int, string>();
        }

        private class ShowHistory
        {
            public int ID { get; set; }
            public bool Subscribed { get; set; } = false;
            public SortedDictionary<string, SortedSet<int>> WatchedEpisodes { get; set; } = new SortedDictionary<string, SortedSet<int>>();
        }

        public static (int season, int episode) DeserialiseEpisode(string episodeSerialised)
        {
            string[] episodeInfo = episodeSerialised.Split('E');
            int seasonNo = int.Parse(episodeInfo[0].Substring(1, 2));
            int episodeNo = int.Parse(episodeInfo[1]);
            return (seasonNo, episodeNo);
        }

        private enum ProgramMenu
        {
            Home,
            Search,
            MostPopular,
            Subscriptions,
            Settings
        }

        private void UpdateSubscriptions()
        {
            List<ShowHistory> subscribedShows = Settings.ShowLibrary.Where(Show => Show.Subscribed).ToList();
            // Update the subscriptions counter text
            lblNumSubscriptions.Text = "Subscriptions: " + subscribedShows.Count().ToString();
            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
            SizeF size = graphics.MeasureString(lblNumSubscriptions.Text, new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point));
            lblNumSubscriptions.Location = new Point(this.Width - (int)size.Width - 33, 3);

            // Update the subscriptions when the app is hidden to taskbar
            itmSubscriptions.MenuItems.Clear();
            for (int i = 0; i < subscribedShows.Count; i++)
            {
                ShowHistory show = subscribedShows[i];
                string showName;
                try
                {
                    showName = Settings.ShowNames[show.ID];
                }
                catch (KeyNotFoundException)
                {
                    showName = show.ID.ToString();
                }
                MenuItem itmShow = new MenuItem
                {
                    Index = i * 2,
                    Text = showName
                };
                itmSubscriptions.MenuItems.Add(itmShow);
                if (i + 1 < subscribedShows.Count)
                {
                    MenuItem itmSeparator = new MenuItem
                    {
                        Index = i + 1,
                        Text = "-"
                    };
                    itmSubscriptions.MenuItems.Add(itmSeparator);
                }
                itmShow.Click += new EventHandler(ItmShow_Click);
                void ItmShow_Click(object sender, EventArgs e)
                {
                    RestoreWindow();
                    GoShowDetails(show.ID, ProgramMenu.Home);
                }
            }
        }

        private void UpdateSubscribeButtonText(Button btn, TvShow Show)
        {
            if (Settings.ShowLibrary.Where(SavedShow => SavedShow.Subscribed).Any(SubscribedShow => SubscribedShow.ID == Show.ID))
                btn.Text = "Unsubscribe";
            else
                btn.Text = "Subscribe";
            if (!Settings.ShowNames.ContainsKey(Show.ID))
                Settings.ShowNames.Add(Show.ID, Show.Name);
            UpdateSubscriptions();
        }

        private static string MakeWebRequest(string query)
        {
            Log($"Making a web request to '{query}'...");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Log("Received a response from the HTTP request.");
                string rawResult = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return rawResult;
            }
            catch (WebException e)
            {
                Log(e.Message);
                return null;
            }
        }

        private static string GetJsonStringFromURL(string domain, string pageName, string query, int pageNumber = 1, bool useCachedResults = true)
        {
            string queryEncoded = NewEpisodesManager.RemoveInvalidDirectoryChars(query);
            string cachePath = $@"cache\{domain.Split('.')[0]}\{pageName}\{queryEncoded}\page {pageNumber}\";
            Directory.CreateDirectory(cachePath);
            DateTime oneHourAgo = DateTime.Now.Subtract(new TimeSpan(hours: 1, minutes: 0, seconds: 0));
            if (useCachedResults && File.Exists(cachePath + "cache.json") && File.GetLastWriteTime(cachePath + "cache.json") >= oneHourAgo)
            {
                using (StreamReader r = new StreamReader(cachePath + "cache.json"))
                {
                    string content = r.ReadToEnd();
                    if (content != null && content != "" && content != "{}")
                        return content;
                }
            }
            if (query != null)
                queryEncoded = HttpUtility.HtmlEncode(query).Replace(' ', '+');
            string json = MakeWebRequest($"https://{domain}/{pageName}?q={queryEncoded}&page={pageNumber}");
            File.WriteAllText(cachePath + "cache.json", json);
            return json;
        }

        private static TvShow GetShowFromID(int showID)
        {
            string rawResult = GetJsonStringFromURL(episodateURL, "show-details", showID.ToString());
            if (rawResult is null)
                return null;
            ShowData show = JsonConvert.DeserializeObject<ShowData>(rawResult);
            return show.TvShowData;
        }

        private bool OptimiseSettings()
        {
            string originalSettings = JsonConvert.SerializeObject(Settings);
            List<int> showsToBeRemovedFromLibrary = new List<int>();
            foreach (ShowHistory show in Settings.ShowLibrary)
            {
                List<string> seasonsWithNoWatchedEpisodes = new List<string>();
                foreach (KeyValuePair<string, SortedSet<int>> watchedEpisodesInSeason in show.WatchedEpisodes)
                {
                    if (watchedEpisodesInSeason.Value.Count == 0)
                    {
                        seasonsWithNoWatchedEpisodes.Add(watchedEpisodesInSeason.Key);
                    }
                    else
                    {
                        watchedEpisodesInSeason.Value.OrderBy(episodeNumber => episodeNumber);
                    }
                }
                foreach (string seasonNumber in seasonsWithNoWatchedEpisodes)
                {
                    show.WatchedEpisodes.Remove(seasonNumber);
                }
                if (!show.Subscribed && (show.WatchedEpisodes is null || show.WatchedEpisodes.Count == 0))
                {
                    showsToBeRemovedFromLibrary.Add(show.ID);
                }
                else
                {
                    show.WatchedEpisodes.OrderBy(keyValuePair => keyValuePair.Key);
                }
            }
            Settings.ShowLibrary.RemoveAll(SavedShow => showsToBeRemovedFromLibrary.Any(ShowToBeRemoved => SavedShow.ID == ShowToBeRemoved));
            // Returns whether or not the library has been changed
            return originalSettings != JsonConvert.SerializeObject(Settings);
        }

        private void SaveDataFile(string filename = "data.json", bool instantly = false)
        {
            ThreadHelperClass.SetProgressBarValue(this, prgFileSave, 0);
            ThreadHelperClass.SetControlVisibility(this, prgFileSave, true);
            ThreadHelperClass.SetProgressBarValue(this, prgFileSave, 100);
            OptimiseSettings();
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(filename, json);
            if (instantly)
                ThreadHelperClass.SetControlVisibility(this, prgFileSave, false);
            else
                Task.Delay(600).ContinueWith(t => ThreadHelperClass.SetControlVisibility(this, prgFileSave, false));
        }

        private static void Log(string line, bool forceNewLogMessage = false)
        {
            Log(new string[] { line }, forceNewLogMessage);
        }

        private static void Log(string[] lines, bool forceNewLogMessage = false)
        {
            Directory.CreateDirectory("logs");
            string filename = $@"logs\{DateTime.Now:yyyy-MM-dd}.log";
            bool existed = File.Exists(filename);

            try
            {
                using (StreamWriter w = File.AppendText(filename))
                {
                    void _Write(string[] linesToWrite)
                    {
                        foreach (string line in linesToWrite)
                        {
                            w.WriteLine($"{DateTime.Now:yyyy-MM-dd @ HH:mm:ss}: {line}");
                        }
                    }
                    if (!existed || forceNewLogMessage)
                        _Write(new string[] {
                            "----------------",
                            "Started new log.",
                            "----------------"
                        });
                    _Write(lines);
                }
            } catch (IOException e)
            {
                // Try again in 100 ms
                Task.Delay(100).ContinueWith(t => Log(lines.Concat(new string[] { e.Message }).ToArray(), forceNewLogMessage));
            }
        }
    }
}
