using Newtonsoft.Json;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        string abbrev = "";
        List<string> dates = new List<string>();
        List<double> valueCourses = new List<double>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CharInit(string abbrev, List<string> dates, List<double> valueCourses)
        {
            double[] values = valueCourses.ToArray();

            double[] positions = DataGen.Consecutive(values.Length);
            Chart.Plot.AddScatter(positions, values, Color.Black);

            string[] labels = dates.ToArray();
            Chart.Plot.XAxis.ManualTickPositions(positions, labels);
            Chart.Plot.XAxis.TickLabelStyle(rotation: 45);

            Chart.Plot.Title("Анализ курсов");
            Chart.Plot.YLabel($"Курс ({abbrev})");
            Chart.Plot.XLabel("Дата");

            Chart.Plot.SaveFig("ticks_nonLinearX.png");
            Chart.Refresh();
        }

        private void Button_CreateChart_Click(object sender, RoutedEventArgs e)
        {
            
            if (!String.IsNullOrEmpty(DP1.Text) && !String.IsNullOrEmpty(DP2.Text) && !String.IsNullOrEmpty(currency.Text) && DateTime.Parse(DP1.Text) < DateTime.Parse(DP2.Text))
            {
                Chart.Reset();
                string startDate = DateTime.Parse(DP1.Text).ToString("yyyy-MM-ddTHH:mm:ss");
                string endDate = DateTime.Parse(DP2.Text).ToString("yyyy-MM-ddTHH:mm:ss");
                string curr = currency.Text;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:19237/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync($"values/getCourses?abbr={curr}&startDate={startDate}&endDate={endDate}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var s = JsonConvert.DeserializeObject(result);



                    List<Courses> courses = System.Text.Json.JsonSerializer.Deserialize<List<Courses>>(s.ToString());
                    abbrev = courses[0].Currency;
                    foreach (var item in courses)
                    {
                        dates.Add(item.Date.ToString("dd/MM/yyyy"));
                        valueCourses.Add(item.Value);

                    }
                }
                CharInit(abbrev, dates, valueCourses);
            }
        }

        private void DP1_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string d = DP1.Text;
            if(!String.IsNullOrEmpty(d))
            {
                DateTime dt = DateTime.Parse(d);
                if (dt < DateTime.Now.AddYears(-5))
                {
                    DP1.Text = "";
                }
            }   
        }

        private void DP2_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string d = DP2.Text;
            if (!String.IsNullOrEmpty(d))
            {
                DateTime dt = DateTime.Parse(d);
                if (dt < DateTime.Now.AddYears(-5))
                {
                    DP2.Text = "";
                }
            }
        }
    }
}
