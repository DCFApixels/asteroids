namespace Unity.ProjectAuditor.Editor.Core
{
    internal interface IIssueFilter
    {
        bool Match(ReportItem issue);
        bool PackageFilterMatch(ReportItem issue);
    }
}
