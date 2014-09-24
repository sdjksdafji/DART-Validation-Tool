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
        private List<Metric> metrics { get; set; }
        private List<Instance> instances { get; set; }
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

        private void BeginGetMetrics(ServerInfo _server)
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
                this.metrics = (List<Metric>)serializer.ReadObject(stream);

                MetricsList.Invoke(() =>
                {
                    MetricsList.DisplayMember = "Name";
                    MetricsList.DataSource = this.metrics;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private class GetMetricsRequest
        {
            public HttpWebRequest HttpWebRequest { get; set; }
           // public Action<List<Metric>> Callback { get; set; }
        }


        private void MetricsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Metric selectedMetric = MetricsList.SelectedItem as Metric;
            if (selectedMetric != null)
            {
                var serverAInfo = new ServerInfo();
                serverAInfo.FullName = ServerA.Text;
                serverAInfo.WebServiceEndpointHint = "http://" + serverAInfo.FullName;

                BeginGetInstancesForMetric(serverAInfo, selectedMetric);
            }
        }


        private void BeginGetInstancesForMetric(ServerInfo server, Metric metric)
        {
            String url = "/instances?metric=" + metric.Name;
            HttpWebRequest request = WebRequest.Create(server.GetFullRequestUrl(url)) as HttpWebRequest;
            request.BeginGetResponse(EndGetInstancesForMetric, new GetInstancesRequest { HttpWebRequest = request });
        }

        private void EndGetInstancesForMetric(IAsyncResult async)
        {
            GetInstancesRequest request = async.AsyncState as GetInstancesRequest;
            try
            {
                HttpWebResponse response = request.HttpWebRequest.EndGetResponse(async) as HttpWebResponse;
                Stream stream = response.GetResponseStream();

                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Instance>));
                this.instances = (List<Instance>)serializer.ReadObject(stream);

                InstancesList.Invoke(() =>
                {
                    InstancesList.DisplayMember = "Name";
                    InstancesList.DataSource = this.instances;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private class GetInstancesRequest
        {
            public HttpWebRequest HttpWebRequest { get; set; }
            public Action<List<Instance>> Callback { get; set; }
        }


    }

    public static class ControlExtensions
    {
        public static void Invoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(action), null);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
