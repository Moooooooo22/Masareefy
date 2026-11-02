using System.Drawing;

namespace Masareefy.Models
{
    /// <summary>One slice of a pie / donut chart.</summary>
    public class ChartSlice
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Color Color { get; set; }

        public ChartSlice(string label, double value, Color color)
        {
            Label = label; Value = value; Color = color;
        }
    }
}
