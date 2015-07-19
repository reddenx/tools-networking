using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMT.Proxy.FogBugz.FogBugzObjects
{
    public interface ICase
    {
        int CaseId { get; }
        int? ParentCaseId { get; }
        int[] ChildrenCaseIds { get; }
        string[] Tags { get; }
        bool IsOpen { get; }
        string Title { get; }
        int? ProjectId { get; }
        string ProjectName { get; }
        int StatusId { get; }
        int MilestoneId { get; }
        int? StoryPoints { get; }
        DateTime DateOpened { get; }
        DateTime? DateResolved { get; }
        DateTime? DateClosed { get; }
    }

    [XmlRootAttribute("case")]
    public class Case : ICase
    {
        [XmlElement("ixBug")]
        public int CaseId { get; set; }

        [XmlElement("ixBugParent")]
        public string _parentCaseIdStr;
        public int? _parentCaseId;
        public int? ParentCaseId { get { return XmlUtilities.LazyParseInt(_parentCaseIdStr, ref _parentCaseId); } }

        [XmlElement("ixBugChildren")]
        public string _childrenString;
        private int[] _childIds;
        public int[] ChildrenCaseIds
        {
            get
            {
                return _childIds ?? (_childIds =
                    string.IsNullOrWhiteSpace(_childrenString)
                        ? new int[] { }
                        : _childrenString
                                    .Split(',')
                                    .Select(c => int.Parse(c))
                                    .ToArray());
            }
        }

        [XmlElement("tags")]
        public Tags _tags;
        public string[] Tags { get { return _tags.Tag; } }

        [XmlElement("fOpen")]
        public bool IsOpen { get; set; }

        [XmlElement("sTitle")]
        public string Title { get; set; }

        [XmlElement("ixProject")]
        public string _projectIdStr;
        private int? _projectId;
        public int? ProjectId { get { return XmlUtilities.LazyParseInt(_projectIdStr, ref _projectId); } }

        [XmlElement("sProject")]
        public string ProjectName { get; set; }

        [XmlElement("ixStatus")]
        public int StatusId { get; set; }

        [XmlElement("ixFixFor")]
        public int MilestoneId { get; set; }

        [XmlElement("quarterxinitiativesa12")]
        public string _storyPointStr { get; set; }
        public int? _storyPoints;
        public int? StoryPoints { get { return XmlUtilities.LazyParseInt(_storyPointStr, ref _storyPoints); } }

        [XmlElement("dtOpened")]
        public string _dateOpenedStr;
        private DateTime? _dateOpened;
        public DateTime DateOpened { get { return XmlUtilities.LazyParseDate(_dateOpenedStr, ref _dateOpened).Value; } }

        [XmlElement("dtResolved")]
        public string _dateResolvedStr;
        private DateTime? _dateResolved;
        public DateTime? DateResolved { get { return XmlUtilities.LazyParseDate(_dateResolvedStr, ref _dateResolved); } }

        [XmlElement("dtClosed")]
        public string _dateClosedStr;
        private DateTime? _dateClosed;
        public DateTime? DateClosed { get { return XmlUtilities.LazyParseDate(_dateClosedStr, ref _dateClosed); } }
    }
}
