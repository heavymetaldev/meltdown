using Meltdown.UI.SignalR;

namespace Meltdown.UI;

public enum ProgressState
{
    Unknown,
    Pending,
    Ready,
    Running,
    Done,
    Error,
}

/// <summary>
/// The progress reporter is used to report progress to the node.js side.
/// </summary>
public interface IProgressReporter
{
    /// <summary>
    /// Log a message to the node.js side.
    /// </summary>
    Task Log(string fullPath, string message);

    /// <summary>
    /// Report progress to the node.js side.
    /// </summary>
    Task ReportProgress(string fullPath, ProgressState state, string status, int progress);
}
