/********************************************************************++
Copyright (c) Microsoft Corporation.  All rights reserved.
--********************************************************************/
using System;
using System.Management.Automation;
using Dbg = System.Management.Automation;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// A command to move a property on an item to another item
    /// </summary>
    [Cmdlet (VerbsCommon.Move, "ItemProperty", SupportsShouldProcess = true, DefaultParameterSetName = "Path", SupportsTransactions = true,
        HelpUri = "http://go.microsoft.com/fwlink/?LinkID=113351")]
    public class MoveItemPropertyCommand : PassThroughItemPropertyCommandBase
    {
        #region Parameters

        /// <summary>
        /// Gets or sets the path parameter to the command
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = "Path", 
                   Mandatory = true, ValueFromPipeline=true, ValueFromPipelineByPropertyName = true)]
        public string[] Path
        {
            get
            {
                return paths;
            } // get

            set
            {
                paths = value;
            } // set
        } // Path

        /// <summary>
        /// Gets or sets the literal path parameter to the command
        /// </summary>
        [Parameter(ParameterSetName = "LiteralPath",
                   Mandatory = true, ValueFromPipeline = false, ValueFromPipelineByPropertyName = true)]
        [Alias("PSPath")]
        public string[] LiteralPath
        {
            get
            {
                return paths;
            } // get

            set
            {
                base.SuppressWildcardExpansion = true;
                paths = value;
            } // set
        } // LiteralPath

        /// <summary>
        /// The name of the property to create on the item
        /// </summary>
        ///
        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("PSProperty")]
        public string[] Name
        {
            get
            {
                return property;
            } // get

            set
            {
                if (value == null)
                {
                    value = new string[0];
                }
                property = value;
            }
        } // Property

        /// <summary>
        /// The path to the destination item to copy the property to.
        /// </summary>
        /// 
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Destination
        {
            get
            {
                return destination;
            } // get

            set
            {
                destination = value;
            }
        } // Destination
            
        /// <summary>
        /// A virtual method for retrieving the dynamic parameters for a cmdlet. Derived cmdlets
        /// that require dynamic parameters should override this method and return the
        /// dynamic parameter object.
        /// </summary>
        /// 
        /// <param name="context">
        /// The context under which the command is running.
        /// </param>
        /// 
        /// <returns>
        /// An object representing the dynamic parameters for the cmdlet or null if there
        /// are none.
        /// </returns>
        /// 
        internal override object GetDynamicParameters(CmdletProviderContext context)
        {
            string propertyName = String.Empty;
            if (Name != null && Name.Length > 0)
            {
                propertyName = Name[0];
            }

            if (Path != null && Path.Length > 0)
            {
                return InvokeProvider.Property.MovePropertyDynamicParameters(Path[0], propertyName, Destination, propertyName, context);
            }
            return InvokeProvider.Property.MovePropertyDynamicParameters(
                ".",
                propertyName,
                Destination,
                propertyName,
                context);
        } // GetDynamicParameters
        
        #endregion Parameters

        #region parameter data
        
        /// <summary>
        /// The property to be created.
        /// </summary>
        private string[] property = new string[0];

        /// <summary>
        /// The destination path of the item to copy the property to.
        /// </summary>
        private string destination;

        #endregion parameter data

        #region Command code

        /// <summary>
        /// Creates the property on the item
        /// </summary>
        protected override void ProcessRecord ()
        {
            foreach (string path in Path)
            {
                foreach (string propertyName in Name)
                {
                    try
                    {
                        InvokeProvider.Property.Move(path, propertyName, Destination, propertyName, GetCurrentContext());
                    }
                    catch (PSNotSupportedException notSupported)
                    {
                        WriteError(
                            new ErrorRecord(
                                notSupported.ErrorRecord,
                                notSupported));
                        continue;
                    }
                    catch (DriveNotFoundException driveNotFound)
                    {
                        WriteError(
                            new ErrorRecord(
                                driveNotFound.ErrorRecord,
                                driveNotFound));
                        continue;
                    }
                    catch (ProviderNotFoundException providerNotFound)
                    {
                        WriteError(
                            new ErrorRecord(
                                providerNotFound.ErrorRecord,
                                providerNotFound));
                        continue;
                    }
                    catch (ItemNotFoundException pathNotFound)
                    {
                        WriteError(
                            new ErrorRecord(
                                pathNotFound.ErrorRecord,
                                pathNotFound));
                        continue;
                    }

                }
            }
        } // ProcessRecord
        #endregion Command code


    } // MoveItemPropertyCommand

} // namespace Microsoft.PowerShell.Commands