using Microsoft.Office.Web.Datacenter.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private List<Metric> Metrics { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadMetrics_Click(object sender, EventArgs e)
        {
            var serverAInfo = new ServerInfo();
            serverAInfo.FullName = ServerA.Text;
            serverAInfo.WebServiceEndpointHint = "http://" + serverAInfo.FullName;

            BeginGetMetrics(serverAInfo);
        }

        public void BeginGetMetrics(ServerInfo _server)
        {
            HttpWebRequest request = WebRequest.Create(_server.GetFullRequestUrl("/metrics")) as HttpWebRequest;
            request.BeginGetResponse(EndGetMetrics, new GetMetricsRequest { HttpWebRequest = request });
        }

        private void EndGetMetrics(IAsyncResult async)
        {
            GetMetricsRequest request = async.AsyncState as GetMetricsRequest;
            try
            {
                HttpWebResponse response = request.HttpWebRequest.EndGetResponse(async) as HttpWebResponse;
                Stream stream = response.GetResponseStream();

                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Metric>));
                Metrics = (List<Metric>)serializer.ReadObject(stream);

                MetricsCombo.DisplayMember = "Name";
                MetricsCombo.DataSource = Metrics;
            }
            catch (Exception e)
            {
                // _dispatcher.BeginInvoke(() => _reportError(String.Format(CultureInfo.InvariantCulture, "Failed to retrieve metrics from {0}!\n\n{1}", request.HttpWebRequest.RequestUri, e)));
            }
        }

        private class GetMetricsRequest
        {
            public HttpWebRequest HttpWebRequest { get; set; }
            public Action<List<Metric>> Callback { get; set; }
        }

        private void MetricsCombo_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
