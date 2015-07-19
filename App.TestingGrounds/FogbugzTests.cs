using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace App.TestingGrounds
{
    class FogbugzTests
    {

        public void Test()
        {
            //XmlSerializer serialzer = new XmlSerializer(typeof(CaseResponseRoot));
            //var test = serialzer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            return;
        }
        private static string xml = @"<cases count=""1""> -- count is included in the cases attribute
  <case ixBug=""123"" operations=""edit,assign,resolve,reactivate,close,reopen,reply,forward,email,move,spam,remind"">
  <ixBug>123</ixBug> -- case number
    <ixBugParent>234</ixBugParent> -- parent case number
    <ixBugChildren>456,876</ixBugChildren> -- subcase numbers
    <tags> -- tags
      <tag><![CDATA[first]]></tag>
      <tag><![CDATA[second]]></tag>
      <tag><![CDATA[third]]></tag>
    </tags>
    <fOpen>true</fOpen> -- true if open, false if closed
    <sTitle>Duck, Duck... but No Goose!</sTitle> -- title
    <sLatestTextSummary>I searched the docs, but no goose!</sLatestTextSummary> -- short string with case's latest comment
    <ixBugEventLatestText>1151</ixBugEventLatestText> -- ixBugEvent for latest event with actual text comment
    <ixProject>22</ixProject> -- project id
    <sProject>The Farm</sProject> -- project name
    <ixArea>35</ixArea> -- area id
    <sArea>Pond</sArea> -- area name
    <ixGroup>6</ixGroup> -- group id (deprecated as of FogBugz 8--will always return 0)
    <ixPersonAssignedTo>1</ixPersonAssignedTo> -- person case is assigned to (id)
    <sPersonAssignedTo>Old MacDonald</sPersonAssignedTo> -- person case is assigned to (name)
    <sEmailAssignedTo>grandpa@oldmacdonald.com</sEmailAssignedTo> -- email of person case is assigned to
    <ixPersonOpenedBy>2</ixPersonOpenedBy> -- person case was opened by (id)
    <ixPersonResolvedBy>2</ixPersonResolvedBy> -- person case was resolved by (id)
    <ixPersonClosedBy></ixPersonClosedBy> -- person case was closed by (id)
    <ixPersonLastEditedBy>0</ixPersonLastEditedBy> -- person case was last edited by (id)
    <ixStatus>2</ixStatus> -- status (id)
    <ixBugDuplicates>321</ixBugDuplicates> -- cases that are closed as duplicates of this one (id)
    <ixBugOriginal>654</ixBugOriginal> -- the case which this one was a duplicate of (id)
    <sStatus>Geschlossen (Fixed)</sStatus> -- status (name)
    <ixPriority>3</ixPriority> -- priority (id)
    <sPriority>Must Fix</sPriority> -- priority (name)
    <ixFixFor>3</ixFixFor> -- fixfor (id)
    <sFixFor>Test</sFixFor> -- fixfor (name)
    <dtFixFor>2007-05-06T22:47:59Z</dtFixFor> -- date of fixfor (date)
    <sVersion></sVersion> -- version field (custom field #1)
    <sComputer></sComputer> -- computer field (custom field #2)
    <hrsOrigEst>0</hrsOrigEst> -- hours of original estimate (0 if no estimate)
    <hrsCurrEst>0</hrsCurrEst> -- hours of current estimate
    <hrsElapsed>0</hrsElapsed> -- total elapsed hours -- includes all time from time intervals PLUS hrsElapsedExtra time
    <c>0</c> -- number of occurrences (minus 1) of this bug (increased via bugzscout)
      -- to display the actual number of occurrences, add 1 to this number  
    <sCustomerEmail></sCustomerEmail> -- if there is a customer contact for this case, this is their email
    <ixMailbox>0</ixMailbox> -- if this case came in via dispatcho, the mailbox it came in on
    <ixCategory>1</ixCategory> -- category (id)
    <sCategory>Feature</sCategory> -- category (name)
    <dtOpened>2007-05-06T22:47:59Z</dtOpened> -- date case was opened
    <dtResolved>2007-05-06T22:47:59Z</dtResolved> -- date case was resolved
    <dtClosed>2007-05-06T22:47:59Z</dtClosed> -- date case was closed
    <ixBugEventLatest>1151</ixBugEventLatest> -- latest bugevent
    <dtLastUpdated>2007-05-06T22:47:59Z</dtLastUpdated> -- the date when this case was last updated
    <fReplied>false</fReplied> -- has this case been replied to?
    <fForwarded>false</fForwarded> -- has this case been forwarded?
    <sTicket></sTicket> -- id for customer to view bug (bug number + 8 letters e.g. 4003_XFLFFFCS)
    <ixDiscussTopic>0</ixDiscussTopic> -- id of discussion topic if case is related
    <dtDue></dtDue> -- date this case is due (empty if no due date)
    <sReleaseNotes></sReleaseNotes> -- release notes
    <ixBugEventLastView>1151</ixBugEventLastView> -- the ixBugEventLatest when you last viewed this case
    <dtLastView>2007-05-06T22:47:59Z</dtLastView> -- the date when you last viewed this case
    <ixRelatedBugs>345,267,2920</ixRelatedBugs> -- comma separated list of other related case numbers
    <sScoutDescription>Main.cpp:165</sScoutDescription> -- if this case is a Scout case, this ID is the unique identifier
    <sScoutMessage>Please contact us or visit our knowledge base to resolve.</sScoutMessage> -- this is the message
      -- displayed to users when they submit a case that matches this sScoutDescription  
    <fScoutStopReporting>false</fScoutStopReporting> -- whether we are still recording occurrences of this crash or not
    <dtLastOccurrence>2007-05-06T22:47:59Z</dtLastOccurrence> -- most recent occurrence of this crash, if this is a BugzScout case
    <fSubscribed>true</fSubscribed> -- true if you are subscribed to this case, otherwise false
  </case>
</cases> ";
    }
}
