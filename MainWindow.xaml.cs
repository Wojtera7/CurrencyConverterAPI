using System;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace CurrencyConverterAPI
{

    public partial class MainWindow : Window
    {
        Root val = new Root();

        public class Root
        {
            public Rate rates { get; set; }
        }

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            GetValue();
        }

        private async void GetValue()
        {
            val = await GetDataGetMethod<Root>("https://openexchangerates.org/api/latest.json?app_id=69cb235f2fe74f03baeec270066587cf");
            BindCurrency();
        }

        public static async Task<Root> GetDataGetMethod<T>(string url)
        {
            var ss = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync();


                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);
                        return ResponseObject;
                    }
                    return ss;
                }
            }
            catch
            {
                return ss;
            }
        }

        private void BindCurrency()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Text");

            dataTable.Columns.Add("Rate");

            dataTable.Rows.Add("--SELECT--", 0);
            dataTable.Rows.Add("INR", val.rates.INR);
            dataTable.Rows.Add("USD", val.rates.USD);
            dataTable.Rows.Add("NZD", val.rates.NZD);
            dataTable.Rows.Add("JPY", val.rates.JPY);
            dataTable.Rows.Add("EUR", val.rates.EUR);
            dataTable.Rows.Add("CAD", val.rates.CAD);
            dataTable.Rows.Add("ISK", val.rates.ISK);
            dataTable.Rows.Add("PHP", val.rates.PHP);
            dataTable.Rows.Add("DKK", val.rates.DKK);
            dataTable.Rows.Add("CZK", val.rates.CZK);

            comboboxFromCurrency.ItemsSource = dataTable.DefaultView;

            comboboxFromCurrency.DisplayMemberPath = "Text";

            comboboxFromCurrency.SelectedValuePath = "Rate";

            comboboxFromCurrency.SelectedIndex = 0;

            comboboxToCurrency.ItemsSource = dataTable.DefaultView;
            comboboxToCurrency.DisplayMemberPath = "Text";
            comboboxToCurrency.SelectedValuePath = "Rate";
            comboboxToCurrency.SelectedIndex = 0;
        }


        private void ClearControls()
        {
            textCurrency.Text = string.Empty;
            if (comboboxFromCurrency.Items.Count > 0)
                comboboxFromCurrency.SelectedIndex = 0;
            if (comboboxToCurrency.Items.Count > 0)
                comboboxToCurrency.SelectedIndex = 0;
            labelCurrency.Content = "";
            textCurrency.Focus();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (textCurrency.Text == null || textCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                textCurrency.Focus();
                return;
            }
            else if (comboboxFromCurrency.SelectedValue == null || comboboxFromCurrency.SelectedIndex == 0 || comboboxFromCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                comboboxFromCurrency.Focus();
                return;
            }
            else if (comboboxToCurrency.SelectedValue == null || comboboxToCurrency.SelectedIndex == 0 || comboboxToCurrency.Text == "--SELECT--")
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                comboboxToCurrency.Focus();
                return;
            }

            if (comboboxFromCurrency.Text == comboboxToCurrency.Text)
            {
                ConvertedValue = double.Parse(textCurrency.Text);

                labelCurrency.Content = comboboxToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(comboboxToCurrency.SelectedValue.ToString()) * double.Parse(textCurrency.Text)) / double.Parse(comboboxFromCurrency.SelectedValue.ToString());

                labelCurrency.Content = comboboxToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
    }
}