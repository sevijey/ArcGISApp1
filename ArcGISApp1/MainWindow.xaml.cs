using ArcGISApp1.Models;
using Esri.ArcGISRuntime.UI;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Html2pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArcGISApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region VARIABLES
        DocuSignApi _docuSignApi = new DocuSignApi();
        List<Stakeholder> _stakeholdersList = new List<Stakeholder>();
        SignProcess _signProcess = new SignProcess();
        string _appBaseDir = AppDomain.CurrentDomain.BaseDirectory;
        DispatcherTimer _timer = new DispatcherTimer();
        #endregion

        #region EVENTS
        private void timer_Tick(object sender, EventArgs e)
        {
            if (lblState.Content.ToString() != "State:" && lblState.Content.ToString() != "State: completed")
            {
                string envelopeState = _docuSignApi.GetEnvelopeStatus(_signProcess.EnvelopeId);
                _signProcess.State = string.IsNullOrEmpty(envelopeState) ? _signProcess.State : envelopeState;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblState.Content = $"State: {_signProcess.State}";
                }));
            }
        }

        private void ButtonAddStakeholder_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtName.Text) && !lstNames.Items.Contains($"{ txtName.Text} - {txtEmail.Text}"))
            {
                lstNames.Items.Add($"{txtName.Text} - {txtEmail.Text}");
                _stakeholdersList.Add(new Stakeholder() { Name = txtName.Text, Email = txtEmail.Text });
                txtName.Clear();
                txtEmail.Clear();
            }
        }

        private async void ButtonSendToSign_ClickAsync(object sender, RoutedEventArgs e)
        {
            await GeneratePdfMap();

            _signProcess = _docuSignApi.SendEnvelope(_stakeholdersList);
            if (!string.IsNullOrEmpty(_signProcess?.EnvelopeId))
            {
                _signProcess.CreatedMapDate = Convert.ToString(mapView.Map.Item.Created);
                _signProcess.ModifiedMapDate = Convert.ToString(mapView.Map.Item.Modified);

                //serialize
                string serializedObject = JsonConvert.SerializeObject(_signProcess);
                File.WriteAllText($"{_appBaseDir}\\Resources\\signProcess.json", serializedObject);

                //update labels
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblEnvelopeId.Content = $"EnvelopeId: {_signProcess.EnvelopeId}";
                    lblState.Content = $"State: {_signProcess.State}";
                    lblSendDate.Content = $"Send DateTime:  {_signProcess.SendDateTime.Substring(0, 19).Replace("T", " ")}";
                }));
            }
        }

        private void ButtonViewReume_Click(object sender, RoutedEventArgs e)
        {
            if (_signProcess.State != "" && _signProcess.State != "completed")
            {
                string envelopeState = _docuSignApi.GetEnvelopeStatus(_signProcess.EnvelopeId);
                _signProcess.State = string.IsNullOrEmpty(envelopeState) ? _signProcess.State : envelopeState;
            }
            if (_signProcess.State == "completed" && !_signProcess.IsDownloaded)
            {
                _signProcess.IsDownloaded = _docuSignApi.DownloadEnvelopeDocuments(_signProcess.EnvelopeId);
            }
            if (_signProcess.State == "completed" && _signProcess.IsDownloaded)
            {
                ViewSignedDocument();
            }
        }
        #endregion

        #region PRIVATE METHODS
        private void Builder()
        {
            if (File.Exists($"{_appBaseDir}\\Resources\\signProcess.json"))
            {
                string serializedObject = File.ReadAllText($"{_appBaseDir}\\Resources\\signProcess.json");
                _signProcess = JsonConvert.DeserializeObject<SignProcess>(serializedObject);

                if (!string.IsNullOrEmpty(_signProcess?.EnvelopeId))
                {
                    if (_signProcess.State != "completed")
                    {
                        string envelopeState = _docuSignApi.GetEnvelopeStatus(_signProcess.EnvelopeId);
                        if (envelopeState == "completed")
                        {
                            _signProcess.State = envelopeState;
                            //serialice
                            serializedObject = JsonConvert.SerializeObject(_signProcess);
                            File.WriteAllText($"{_appBaseDir}\\Resources\\signProcess.json", serializedObject);
                        }
                    }

                    //update labels
                    foreach (Stakeholder stakeholder in _signProcess.StakeholderList)
                    {
                        lstNames.Items.Add($"{stakeholder.Name} - {stakeholder.Email}");
                    }
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblEnvelopeId.Content = $"EnvelopeId: {_signProcess.EnvelopeId}";
                        lblState.Content = $"State: {_signProcess.State}";
                        lblSendDate.Content = $"Send DateTime:  {_signProcess.SendDateTime.Substring(0, 19).Replace("T", " ")}";
                    }));
                }
            }

            _timer.Interval = TimeSpan.FromMinutes(3);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        private async Task GeneratePdfMap()
        {
            await GenerateMapImg();

            GenerateHtmlMap();

            Utils.HtmlToPdf(htmlOrig: $"{_appBaseDir}\\Resources\\Map.html", pdfDest: $"{_appBaseDir}\\Resources\\Map.pdf");
        }

        private async Task GenerateMapImg()
        {
            Esri.ArcGISRuntime.UI.RuntimeImage mapViewImage = await mapView.ExportImageAsync();
            Stream pngImgStream = await mapViewImage.GetEncodedBufferAsync();

            Utils.StreamToFile(streamOrig: pngImgStream, pathFileDest: $"{_appBaseDir}\\Resources\\MapImg.png");
        }

        private void GenerateHtmlMap()
        {
            string baseHtmlMap = File.ReadAllText($"{_appBaseDir}\\Resources\\BaseMap.html");
            baseHtmlMap = baseHtmlMap.Replace("{TITLE}", mapView.Map.Item.Title).Replace("{ID}", mapView.Map.Item.ItemId).Replace("{URL}", "https://arcg.is/0i9GS5");
            baseHtmlMap = baseHtmlMap.Replace("{CREATED}", Convert.ToString(mapView.Map.Item.Created)).Replace("{MODIFIED}", Convert.ToString(mapView.Map.Item.Modified));

            File.WriteAllText($"{_appBaseDir}\\Resources\\Map.html", baseHtmlMap);
        }

        private void ViewSignedDocument()
        {
            DocumentViewer documentViewer = new DocumentViewer();
            documentViewer.Show();
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Builder();
        }
    }
}
