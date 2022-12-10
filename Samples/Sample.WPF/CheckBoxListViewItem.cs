using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.OpenMapTiles;
using System;
using System.ComponentModel;

// Found at https://www.licensespot.com/blog/wpf-listview-checkbox

namespace Sample.WPF
{
    public class CheckBoxListViewItem : INotifyPropertyChanged 
    {
        private OMTVectorTileStyle _style;
        private bool _isChecked; 
        private string _text; 

        public OMTVectorTileStyle Style
        { 
            get 
            { 
                return _style; 
            } 
        }

        public bool IsChecked 
        { 
            get 
            { 
                return _isChecked; 
            } 
            set 
            { 
                if (_isChecked == value) 
                    return; 
                _isChecked = value; 
                RaisePropertyChanged("IsChecked"); 
            } 
        } 
        
        public String Text 
        { 
            get 
            { 
                return _text; 
            } 
        } 
        
        public CheckBoxListViewItem(OMTVectorTileStyle s, string t, bool c) 
        {
            _style = s;
            _text = t; 
            this.IsChecked = c; 
        } 
        
        public event PropertyChangedEventHandler PropertyChanged; 
        
        private void RaisePropertyChanged(string propName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
