using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
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
}
