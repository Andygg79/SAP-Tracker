using System.ComponentModel;

namespace SAPTracker.Models
{
    public partial class MetricEntry : INotifyPropertyChanged
    {
        private string _metricName = string.Empty;
        public string MetricName
        {
            get => _metricName;
            set { _metricName = value; OnPropertyChanged(nameof(MetricName)); }
        }

        private DateTime _lastUpdatedDate = DateTime.Today;
        public DateTime LastUpdatedDate
        {
            get => _lastUpdatedDate;
            set { _lastUpdatedDate = value; OnPropertyChanged(nameof(LastUpdatedDate)); }
        }

        private string _statusColor = "Gray";
        public string StatusColor
        {
            get => _statusColor;
            set { _statusColor = value; OnPropertyChanged(nameof(StatusColor)); }
        }

        private string _statusName = "Gray";
        public string StatusName
        {
            get => _statusName;
            set { _statusName = value; OnPropertyChanged(nameof(StatusName)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
