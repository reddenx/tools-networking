using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SMT.Utilities.FogBugz.Json.ObjectInterfaces
{
	public interface IFogbugzCase
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

	internal class FogbugzCaseDto : IFogbugzCase
	{
		public int CaseId { get; }
		public int? ParentCaseId { get; }
		public int[] ChildrenCaseIds { get; }
		public string[] Tags { get; }
		public bool IsOpen { get; }
		public string Title { get; }
		public int? ProjectId { get; }
		public string ProjectName { get; }
		public int StatusId { get; }
		public int MilestoneId { get; }
		public int? StoryPoints { get; }
		public DateTime DateOpened { get; }
		public DateTime? DateResolved { get; }
		public DateTime? DateClosed { get; }

		[JsonConstructor]
		public FogbugzCaseDto(
			int ixBug, //case id
			string ixBugParent, //parent id
			string ixBugChildren, //children string csv
			FogbugzCaseTagDto[] tags, //case tags
			bool fOpen, //is case open
			string sTitle, //case title
			string ixProject, //project id
			string sProject, //project name
			int ixStatus, //status id
			int ixFixFor, //milestone id
			string dtOpened, //date opened string
			string dtResolved, //date resolved string
			string dtClosed //date closed string
			)
		{
			CaseId = ixBug;
		}
	}

	internal class FogbugzCaseTagDto { }
}
