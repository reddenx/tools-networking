using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Utilities.FogBugz.Json.ObjectInterfaces
{
	internal enum FogbugzEventCodes
	{
		evtOpened = 1,
		evtEdited = 2,
		evtAssigned = 3,
		evtReactivated = 4,
		evtReopened = 5,
		evtClosed = 6,
		evtMoved = 7,// ‘ 2.0 or earlier.From 3.0 on this was recorded as an Edit,
		evtUnknown = 8,// ‘ Not quite sure what happened; display sVerb in the UI,
		evtReplied = 9,
		evtForwarded = 10,
		evtReceived = 11,
		evtSorted = 12,
		evtNotSorted = 13,
		evtResolved = 14,
		evtEmailed = 15,
		evtReleaseNoted = 16,
		evtDeletedAttachment = 17,
	}
}
