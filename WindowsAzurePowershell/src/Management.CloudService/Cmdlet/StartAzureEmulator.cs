﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.CloudService.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.Security.Permissions;
    using System.Text;
    using Model;
    using Properties;
    using Services;

    /// <summary>
    /// Runs the service in the emulator
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AzureEmulator")]
    public class StartAzureEmulatorCommand : DeploymentServiceManagementCmdletBase
    {
        [Parameter(Mandatory = false)]
        [Alias("ln")]
        public SwitchParameter Launch { get; set; }

        public StartAzureEmulatorCommand()
        {
            // This instantiation will throw if user is running with incompatible Windows Azure SDK version.
            new AzureTools.AzureTool();
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public string StartAzureEmulatorProcess(string rootPath)
        {
            string standardOutput;
            string standardError;

            StringBuilder message = new StringBuilder();
            AzureService service = new AzureService(rootPath ,null);
            SafeWriteObject(string.Format(Resources.CreatingPackageMessage, "local"));
            service.CreatePackage(DevEnv.Local, out standardOutput, out standardError);
            SafeWriteObject(Resources.StartingEmulator);
            service.StartEmulator(Launch.ToBool(), out standardOutput, out standardError);
            SafeWriteObject(standardOutput);
            SafeWriteObject(Resources.StartedEmulator);
            return message.ToString();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void ProcessRecord()
        {
            try
            {
                SkipChannelInit = true;
                base.ProcessRecord();
                string result = this.StartAzureEmulatorProcess(base.GetServiceRootPath());
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}