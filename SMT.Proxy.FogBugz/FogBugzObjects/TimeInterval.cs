using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    public interface ITimeInterval
    {
        int UserId { get; }
        int CaseId { get; }
        string CaseTitle { get; }
        int TimeIntervalId { get; }
        DateTime Start { get; }
        DateTime? End { get; }
        bool Deleted { get; }
    }

    [XmlRoot("interval")]
    public class TimeInterval : ITimeInterval
    {
        [XmlElement("ixPerson")]
        public int UserId { get; set; }

        [XmlElement("ixBug")]
        public int CaseId { get; set; }

        [XmlElement("sTitle")]
        public string CaseTitle { get; set; }

        [XmlElement("ixInterval")]
        public int TimeIntervalId { get; set; }

        [XmlElement("dtStart")]
        public string _startStr;
        private DateTime? _start;
        public DateTime Start
        {
            get
            {
                return _start ?? (_start = DateTime.Parse(_startStr)).Value;
            }
        }

        [XmlElement("dtEnd")]
        public string _endStr;
        private DateTime? _end;
        public DateTime? End
        {
            get
            {
                if (!_end.HasValue)
                {
                    DateTime testDate;
                    if (DateTime.TryParse(_endStr, out testDate))
                    {
                        _end = testDate;
                    }
                }

                return _end;
            }
        }

        [XmlElement("fDeleted")]
        public bool Deleted { get; set; }
    }
}
