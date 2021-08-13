using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISApp1
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {
            _ = SetupMap();
        }

        private Map _map = new Map(Basemap.CreateStreetsVector());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get => _map;
            set { _map = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;


        private async Task SetupMap()
        {

            // Create a new map 
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK6e54e50e7fd143f4bf0788c62c1239c4FA3eOUdJVfkn5tXvU3vcEPNGJEvj9YUuEvL1wRVXgpbCly0I-zy_1l7W_Qeh7yWU";

            // Create a portal. If a URI is not specified, www.arcgis.com is used by default.
            ArcGISPortal portal = await ArcGISPortal.CreateAsync();

            // Get the portal item for a web map using its unique item id.
            PortalItem mapItem = await PortalItem.CreateAsync(portal, "b2cdbf3cabc74a658b0f394a8ee4f381");

            // Create the map from the item.
            Map map = new Map(mapItem);

            // To display the map, set the MapViewModel.Map property, which is bound to the map view.
            this.Map = map;

            
        }
    }
}
