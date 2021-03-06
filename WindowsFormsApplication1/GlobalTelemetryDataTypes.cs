﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Office.Web.Datacenter.Telemetry
{

    public class DataSeries
    {
        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public TimeSpan Interval { get; set; }

        [DataMember]
        public double[] Values { get; set; }
    }


    [DataContract]
    public class Instance
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] AdditionalDimensions { get; set; }

        [DataMember]
        public DateTime LastDataPointTime { get; set; }

        [DataMember]
        public double LastDataPointValue { get; set; }
    }

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

    public class Dimension
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Dictionary<string, string> ValueNames { get; set; }

        // This is not included in the original web service response; we fill it in on demand
        private Dictionary<string, string> _inverseValueNames;
        public Dictionary<string, string> InverseValueNames
        {
            get
            {
                if (_inverseValueNames != null)
                    return _inverseValueNames;

                _inverseValueNames = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kvp in ValueNames)
                {
                    // We presume there is a 1-1 mapping between keys and values
                    _inverseValueNames[kvp.Value] = kvp.Key;
                }
                return _inverseValueNames;
            }
        }
    }

}


