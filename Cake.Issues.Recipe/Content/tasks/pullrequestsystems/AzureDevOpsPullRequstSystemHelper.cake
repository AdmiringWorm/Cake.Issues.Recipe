/// <summary>
/// Support for Azure DevOps / Azure Repository hosted code.
/// </summary>
public static class AzureDevOpsPullRequstSystemHelper
{
    /// <inheritdoc />
    public static void ReportIssuesToPullRequest(ICakeContext context, IssuesData data)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (string.IsNullOrWhiteSpace(context.EnvironmentVariable("SYSTEM_ACCESSTOKEN")))
        {
            context.Warning("SYSTEM_ACCESSTOKEN environment variable not set. Make sure the 'Allow Scripts to access OAuth token' option is enabled on the build definition.");
            return;
        }

        context.ReportIssuesToPullRequest(
            data.Issues,
            context.TfsPullRequests(
                data.RepositoryUrl,
                data.PullRequestId.Value,
                context.TfsAuthenticationOAuth(context.EnvironmentVariable("SYSTEM_ACCESSTOKEN"))),
            data.RepositoryRootDirectory);
    }

    /// <inheritdoc />
    public static void SetPullRequestIssuesState(ICakeContext context, IssuesData data)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (string.IsNullOrWhiteSpace(context.EnvironmentVariable("SYSTEM_ACCESSTOKEN")))
        {
            context.Warning("SYSTEM_ACCESSTOKEN environment variable not set. Make sure the 'Allow Scripts to access OAuth token' option is enabled on the build definition.");
            return;
        }

        var pullRequestSettings =
            new TfsPullRequestSettings(
                data.RepositoryUrl,
                data.PullRequestId.Value,
                context.TfsAuthenticationOAuth(context.EnvironmentVariable("SYSTEM_ACCESSTOKEN")));

        var pullRequstStatus =
            new TfsPullRequestStatus("Issues")
            {
                Genre = "Cake.Issues.Recipe",
                State = data.Issues.Any() ? TfsPullRequestStatusState.Failed : TfsPullRequestStatusState.Succeeded,
                Description = data.Issues.Any() ? $"Found {data.Issues.Count()} issues" : "No issues found"
            };

        context.TfsSetPullRequestStatus(
            pullRequestSettings,
            pullRequstStatus);
        }
}