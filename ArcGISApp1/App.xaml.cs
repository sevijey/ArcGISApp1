using Esri.ArcGISRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArcGISApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
            protected override void OnStartup(StartupEventArgs e)
            {
                base.OnStartup(e);
                // Note: it is not best practice to store API keys in source code.
                // The API key is referenced here for the convenience of this tutorial.
                Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK6e54e50e7fd143f4bf0788c62c1239c4FA3eOUdJVfkn5tXvU3vcEPNGJEvj9YUuEvL1wRVXgpbCly0I-zy_1l7W_Qeh7yWU";
            }

            private void Application_Startup(object sender, StartupEventArgs e)
            {
                try
                {
                    //****************
                    //
                    // Authentication:
                    // Use of Esri location services, including basemaps and geocoding, requires either an ArcGIS identity or an API key. 
                    // For more information see https://links.esri.com/arcgis-runtime-security-auth.
                    //
                    // Licensing:
                    // Production deployment of applications built with ArcGIS Runtime requires you to license ArcGIS Runtime functionality.
                    // For more information see https://links.esri.com/arcgis-runtime-license-and-deploy.
                    //
                    //****************

                    // Initialize the ArcGIS Runtime before any components are created.
                    ArcGISRuntimeEnvironment.Initialize();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "ArcGIS Runtime initialization failed.");

                    // Exit application
                    this.Shutdown();
                }
            }
    }
}
