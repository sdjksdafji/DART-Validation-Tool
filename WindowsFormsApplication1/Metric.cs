using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Office.Web.Datacenter.Telemetry
{
    [DataContract]
    public class Metric : IComparable<Metric>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Stream { get; set; }

        [DataMember]
        public string[] StreamFilter { get; set; }

        [DataMember]
        public string[] DimensionNames { get; set; }

        // This is not included in the original web service response; we fill it in later
        public Dimension[] Dimensions { get; set; }

        [DataMember]
        public double MinValue { get; set; }

        [DataMember]
        public double MaxValue { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Description { get; set; }

        // This really ought to just be part of the structure we get from the server; for now infer it
        public bool DisplayValuesAsPercentages { get { return MinValue == 0 && MaxValue == 1; } }

        public int CompareTo(Metric other)
        {
            // sort case insensitive within category path depth

            string sourceCategory = Category ?? string.Empty;
            string otherCategory = other.Category ?? string.Empty;

            return String.Compare(
                sourceCategory + " " + Name,
                otherCategory + " " + other.Name,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
