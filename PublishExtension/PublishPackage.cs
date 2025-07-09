using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace PublishExtension;

[Guid(PublishPackage.PackageGuidString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class PublishPackage : AsyncPackage
{
    public const string PackageGuidString = "d9b3b76d-4d2d-4c8e-9e0b-05f4b3a6c0a1";

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        await PublishCommand.InitializeAsync(this);
    }
}
