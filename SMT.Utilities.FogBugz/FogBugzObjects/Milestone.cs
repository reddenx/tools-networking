using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Utilities.FogBugz.FogBugzObjects
{
    public interface IMilestone
    {
        int MilestoneId { get; }
        string Name { get; }
        bool Deleted { get; }
        //DateTime? EndDate { get; }
        //DateTime? StartDate { get; }
        //int? ProjectId { get; }
        string ProjectName { get; }
        int[] MilestoneDependencyList { get; }
    }

    [XmlRoot("fixfor")]
    public class Milestone : IMilestone
    {
        [XmlElement("ixFixFor")]
        public int MilestoneId { get; set; }

        [XmlElement("sFixFor")]
        public string Name { get; set; }

        [XmlElement("fDeleted")]
        public bool Deleted { get; set; }

        [XmlElement("sStartNote")]
        public string StartNote { get; set; }

        [XmlElement("dt")]
        public string _endDateStr;
        private DateTime? _endDate;
        public DateTime? EndDate { get { return XmlUtilities.LazyParseDate(_endDateStr, ref _endDate); } }

        [XmlElement("dtStart")]
        public string _startDateStr;
        private DateTime? _startDate;
        public DateTime? StartDate { get { return XmlUtilities.LazyParseDate(_startDateStr, ref _startDate); } }

        [XmlElement("ixProject")]
        public string _projectIdStr;
        private int? _projectId;
        public int? ProjectId { get { return XmlUtilities.LazyParseInt(_projectIdStr, ref _projectId); } }

        [XmlElement("sProject")]
        public string ProjectName { get; set; }

        [XmlElement("setixFixForDependency")]
        public string _dependencyListStr;
        private int[] _dependencyList;
        public int[] MilestoneDependencyList
        {
            get
            {
                return _dependencyList ??
                    (_dependencyList = string.IsNullOrWhiteSpace(_dependencyListStr)
                    ? _dependencyListStr.Split(',').Select(str => int.Parse(str)).ToArray()
                    : new int[] { });
            }
        }

        [XmlElement("fReallyDeleted")]
        public bool ReallyDeleted { get; set; }
    }
}
