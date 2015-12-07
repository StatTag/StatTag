namespace AnalysisManager.Core.Models
{
    public class ValueFormat
    {
        public string FormatType { get; set; }
        public int DecimalPlaces { get; set; }
        public bool UseThousands { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
    }
}
