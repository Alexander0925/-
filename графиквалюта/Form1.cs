using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Net.Http; // подключил библиотеку
using Newtonsoft.Json.Linq; // подключил библиотеку

namespace графиквалюта
{
    public partial class Form1 : Form
    {
        private JObject exchangeRates;
        public Form1()
        {
            InitializeComponent();
            LoadData(); 

        }
        private async void LoadData()
        {
            try
            {
                await GetExchangeRates();

            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузки данных: " + ex.Message);
            }


        }
        public async Task GetExchangeRates()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://api.exchangerate-api.com/v4/latest/USD";
                    string response = await client.GetStringAsync(url);
                    JObject data = JObject.Parse(response);

                    exchangeRates = (JObject)data["rates"];

                    comboBox1.Items.Clear(); // очищаю список курсов валют
                    JObject rates = (JObject)data["rates"]; // получаю список курсов валют

                    foreach (var rate in exchangeRates)
                    {
                        comboBox1.Items.Add(rate.Key);
                    }

                    comboBox1.SelectedIndex = 0;


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }


            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await GetExchangeRates();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (exchangeRates == null) return;

            string selectedCurrency = comboBox1.SelectedItem.ToString(); // получаю валюту
            double rate = exchangeRates[selectedCurrency].Value<double>(); // получаю курс

            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[0].Points.AddXY(DateTime.Now, rate); //

            if (chart1.Series[0].Points.Count > 20)
            {
                chart1.Series[0].Points.RemoveAt(0);
            }

            chart1.ChartAreas[0].RecalculateAxesScale();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear(); // очищаю график
        }
    }
}
