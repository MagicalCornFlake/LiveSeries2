using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSeries2
{
    public partial class FormTesting : Form
    {
        public FormTesting()
        {
            InitializeComponent();
            // Foo();
            Bar();
        }

        private void Bar()
        {
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < 10; i++)
            {
                Label lbl = new Label
                {
                    Text = "Column " + (i + 1).ToString(),
                    Height = 13
                };
                FlowLayoutPanel flpInner = new FlowLayoutPanel
                {
                    Width = 150,
                    Height = 15 * (23 + 6) + 13,
                    Padding = new Padding(0, 0, 30, 0),
                    FlowDirection = FlowDirection.TopDown
                };
                flpInner.Controls.Add(lbl);
                for (int j = 0; j < 20; j++)
                {
                    Panel pnlInner = new Panel
                    {
                        Width = 150,
                        Height = 23
                    };
                    Label lblInner = new Label
                    {
                        Text = (j + 1).ToString() + ". ",
                        Width = 20
                    };
                    Button btn = new Button
                    {
                        Text = "Button " + (j + 1).ToString(),
                        Visible = true,
                        Width = 100,
                        Height = 23,
                        Location = new Point(lblInner.Width + 3, 0)
                    };
                    CheckBox chk = new CheckBox
                    {
                        Location = new Point(btn.Location.X + btn.Width + 5, 0)
                    };
                    pnlInner.Controls.Add(lblInner);
                    pnlInner.Controls.Add(btn);
                    pnlInner.Controls.Add(chk);
                    flpInner.Controls.Add(pnlInner);
                }
                flowLayoutPanel1.Controls.Add(flpInner);
            }
        }

        private void Foo()
        {
            Label lblShowName = new Label
            {
                Name = "lbl_shw_",
                Text = "Show Name",
                Font = label2.Font,
                ForeColor = label2.ForeColor,
                Visible = true,
                Width = 100,
                Height = 13 * 2
            };
            FlowLayoutPanel flpNewEpisodesInShow = new FlowLayoutPanel
            {
                Name = "flp_shw_",
                Visible = true,
                Width = 205,
                Height = lblShowName.Height + 23 * 10,
                FlowDirection = FlowDirection.TopDown
            };
            flpNewEpisodesInShow.Controls.Add(lblShowName);
            for (int i = 0; i < 10; i++)
            {
                string episodeSerialised = "Season X Episode " + i;
                int yOffset = 23 * i;
                Panel pnlEpisode = new Panel
                {
                    Name = "pnl_epi_" + episodeSerialised,
                    Visible = true,
                    Width = 205,
                    Height = 23,
                    Location = new Point(0, lblShowName.Height + yOffset)
                };
                Label lblEpisode = new Label
                {
                    Name = "lbl_epi_" + episodeSerialised,
                    Text = i + 1 + ". " + episodeSerialised,
                    Font = label2.Font,
                    ForeColor = label2.ForeColor,
                    Visible = true,
                    Width = 100,
                    Height = 13,
                    Location = new Point(0, yOffset + 5)
                };
                Button btnEpisode = new Button
                {
                    Name = "btn_epi_" + episodeSerialised,
                    Text = "Download",
                    Font = button2.Font,
                    UseVisualStyleBackColor = true,
                    Visible = true,
                    Location = new Point(lblEpisode.Width, yOffset)
                };
                CheckBox chkEpisode = new CheckBox
                {
                    Name = "chk_epi_" + episodeSerialised,
                    Location = new Point(btnEpisode.Location.X + btnEpisode.Width + 5, yOffset)
                };
                pnlEpisode.Controls.Add(lblEpisode);
                pnlEpisode.Controls.Add(btnEpisode);
                pnlEpisode.Controls.Add(chkEpisode);
                flpNewEpisodesInShow.Controls.Add(pnlEpisode);
            }
            flowLayoutPanel1.Controls.Add(flpNewEpisodesInShow);
        }
    }
}
