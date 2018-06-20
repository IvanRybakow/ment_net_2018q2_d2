using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_2
{
    public partial class Form1 : Form
    {
        BindingList<DownloadModel> downloads = new BindingList<DownloadModel>();
        Dictionary<int, CancellationTokenSource> tokenSources = new Dictionary<int, CancellationTokenSource>();
        public Form1()
        {
            InitializeComponent();
            dgvDownloads.DataSource = bindingSource;
            bindingSource.DataSource = downloads;
            var ActionButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Cancel",
                UseColumnTextForButtonValue = true
            };
            dgvDownloads.Columns.Add(ActionButtonColumn);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtURL.Text))
            {
                MessageBox.Show("Please fill URL text box");
            }
            int id = downloads.Select(d => d.Id).OrderByDescending(i => i).FirstOrDefault() + 1;
            var download = new DownloadModel()
            {
                Id = id,
                Status = DownloadStatus.Loading,
                URL = txtURL.Text
            };
            var source = new CancellationTokenSource();
            tokenSources.Add(id, source);
            downloads.Add(download);
            using (var downloader = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotImplemented
                };
                try
                {
                    response = await downloader.GetAsync(download.URL, source.Token);
                }
                catch (TaskCanceledException)
                {
                    download.Status = DownloadStatus.Canceled;
                    bindingSource.ResetBindings(true);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);                    
                }
                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    download.Data = await response.Content.ReadAsStringAsync();
                    download.Status = DownloadStatus.Ready;
                }
                else
                {
                    download.Status = DownloadStatus.Error;
                }
            }
            bindingSource.ResetBindings(true);
        }

        private void dgvDownloads_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDownloads.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var item = (DownloadModel)dgvDownloads.Rows[e.RowIndex].DataBoundItem;
                var token = tokenSources[item.Id];
                if (token.IsCancellationRequested)
                {
                    return;
                }
                token.Cancel();
            }
        }
    }
}
