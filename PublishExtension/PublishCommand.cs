using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading.Tasks;
using PublishLib;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace PublishExtension;

internal static class PublishCommand
{
    public const int PublishSolutionId = 0x0100;
    public const int PublishSelectionId = 0x0101;
    public static readonly Guid CommandSet = new("4bcb48eb-606d-4f7d-a43f-9aba3a099035");

    private static AsyncPackage? _package;
    private static OleMenuCommandService? _commandService;
    private static uint _eventsCookie;
    private static bool _solutionOpen;
    private static readonly Publisher _publisher = new();

    public static async Task InitializeAsync(AsyncPackage package)
    {
        _package = package;
        _commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        await InitializeCommandsAsync();
        await AdviseSolutionEventsAsync();
    }

    private static async Task InitializeCommandsAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (_commandService is null)
            return;

        var publishSolutionCmd = new CommandID(CommandSet, PublishSolutionId);
        var publishSolutionItem = new OleMenuCommand(async (_, __) => await ExecutePublishSolutionAsync(), publishSolutionCmd);
        publishSolutionItem.BeforeQueryStatus += UpdateStatus;
        _commandService.AddCommand(publishSolutionItem);

        var publishSelectionCmd = new CommandID(CommandSet, PublishSelectionId);
        var publishSelectionItem = new OleMenuCommand(async (_, __) => await ExecutePublishSelectionAsync(), publishSelectionCmd);
        publishSelectionItem.BeforeQueryStatus += UpdateStatus;
        _commandService.AddCommand(publishSelectionItem);
    }

    private static void UpdateStatus(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        if (sender is OleMenuCommand command)
        {
            command.Enabled = _solutionOpen;
        }
    }

    private static async Task AdviseSolutionEventsAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (_package is null)
            return;
        var solution = await _package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
        if (solution is null)
            return;
        solution.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value);
        _solutionOpen = value is bool b && b;
        var events = new SolutionEvents();
        solution.AdviseSolutionEvents(events, out _eventsCookie);
    }

    private static async Task ExecutePublishSolutionAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (_package is null)
            return;
        var buildManager = await _package.GetServiceAsync(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
        buildManager?.GetType().InvokeMember(
            "StartSimpleUpdateProjectConfigurations",
            BindingFlags.InvokeMethod,
            binder: null,
            buildManager,
            new object?[] { null, null, 0, 0, 0 });

        var solution = await _package.GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
        string solutionFile = string.Empty;
        solution?.GetSolutionInfo(out _, out solutionFile, out _);
        var message = await _publisher.PublishSolutionAsync(solutionFile);
        VsShellUtilities.ShowMessageBox(_package, message, "Publish", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
    }

    private static async Task ExecutePublishSelectionAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        if (_package is null)
            return;
        var monitorSelection = await _package.GetServiceAsync(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
        // TODO: use monitorSelection to detect active project path
        var message = await _publisher.PublishProjectAsync("selected-project");
        VsShellUtilities.ShowMessageBox(_package, message + " - Not implemented", "Publish", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
    }

    private sealed class SolutionEvents : IVsSolutionEvents
    {
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            _solutionOpen = false;
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) => VSConstants.S_OK;
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) => VSConstants.S_OK;
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _solutionOpen = true;
            return VSConstants.S_OK;
        }
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) => VSConstants.S_OK;
        public int OnBeforeCloseSolution(object pUnkReserved) => VSConstants.S_OK;
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) => VSConstants.S_OK;
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) => VSConstants.S_OK;
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) => VSConstants.S_OK;
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) => VSConstants.S_OK;
    }
}
