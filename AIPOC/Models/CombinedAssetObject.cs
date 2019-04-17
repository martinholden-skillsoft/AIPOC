using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AIPOC.Models
{
    /// <summary>
    /// This respresents the AICC metadata files as strings
    /// </summary>
    public class CombinedAssetObject
    {
        /// <summary>
        /// Gets or sets the string representation of the AICC AU file.
        /// </summary>
        /// <value>
        /// The au.
        /// </value>
        public string AU { get; set; }
        /// <summary>
        /// Gets or sets the string representation of the AICC CRS file.
        /// </summary>
        /// <value>
        /// The CRS.
        /// </value>
        public string CRS { get; set; }
        /// <summary>
        /// Gets or sets the string representation of the AICC CST file
        /// </summary>
        /// <value>
        /// The CST.
        /// </value>
        public string CST { get; set; }
        /// <summary>
        /// Gets or sets the string representation of the AICC DES file
        /// </summary>
        /// <value>
        /// The DES.
        /// </value>
        public string DES { get; set; }
        /// <summary>
        /// Gets or sets the string representation of the AICC ORT file
        /// </summary>
        /// <value>
        /// The ort.
        /// </value>
        public string ORT { get; set; }

        /// <summary>
        /// Gets or sets the assetid this is from the _ss_entitlements.xml
        /// </summary>
        /// <value>
        /// The assetid.
        /// </value>
        public string ASSETID { get; set; }

        /// <summary>
        /// Gets or sets the status this is from the _ss_entitlements.xml
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string STATUS { get; set; }

        /// <summary>
        /// Gets or sets the language this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string LANGUAGE { get; set; }

        /// <summary>
        /// Gets or sets the duration this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string DURATION { get; set; }

        /// <summary>
        /// Gets or sets the description this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string DESCRIPTION { get; set; }

        /// <summary>
        /// Gets or sets the imageurl this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string IMAGEURL { get; set; }

        /// <summary>
        /// Gets or sets the mobilestatus this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string MOBILESTATUS { get; set; }

        /// <summary>
        /// Gets or sets the list of keywords this is from the XML metadata
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public List<string> KEYWORDS { get; set; }
    }
}
