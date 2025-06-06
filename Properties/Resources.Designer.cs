﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Weekly.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Weekly.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Got an unexpected response from the [olive]{0}[/] API..
        /// </summary>
        internal static string ApiErrorMessage {
            get {
                return ResourceManager.GetString("ApiErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The API returned an unexpected response. To get more info, enable logging in your config file by setting [olive]LogExceptionsToConsole[/] to [olive]true[/] at [olive]{0}[/]..
        /// </summary>
        internal static string ApiHelpMessage {
            get {
                return ResourceManager.GetString("ApiHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [olive]{0}[/] API access was denied..
        /// </summary>
        internal static string ApiUnauthorizedErrorMessage {
            get {
                return ResourceManager.GetString("ApiUnauthorizedErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Check your [olive]{0}[/] API token. It may be expired or missing. Run [olive]wk token add[/] to fix it..
        /// </summary>
        internal static string ApiUnauthorizedHelpMessage {
            get {
                return ResourceManager.GetString("ApiUnauthorizedHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A work log with the name [olive]{0}[/] already exists..
        /// </summary>
        internal static string CreateConflictErrorMessage {
            get {
                return ResourceManager.GetString("CreateConflictErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can try passing in a different work log name via the [olive]--workLog[/] option, or if you know for sure you don&apos;t need the existing file you can overwrite it with the [olive]--force[/] option..
        /// </summary>
        internal static string CreateConflictHelpMessage {
            get {
                return ResourceManager.GetString("CreateConflictHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] Default_Template {
            get {
                object obj = ResourceManager.GetObject("Default.Template", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Need more help? Run [olive]wk --help[/]. To turn off these help messages, set [olive]ShowHelpMessages[/] to [olive]false[/] in your config file..
        /// </summary>
        internal static string HelpInstructionMessage {
            get {
                return ResourceManager.GetString("HelpInstructionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn’t initialize [olive]{0}[/] as the work log folder..
        /// </summary>
        internal static string InitializationErrorMessage {
            get {
                return ResourceManager.GetString("InitializationErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [teal]Weekly[/] needs read/write access to the work log directory, as well as your config file at [olive]{0}[/]. Check permissions and try [olive]wk init[/] again..
        /// </summary>
        internal static string InitializationHelpMessage {
            get {
                return ResourceManager.GetString("InitializationHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Double check the parameters you&apos;re passing to [teal]Weekly[/]. Run [olive]wk --help[/] to double check the values for formats that [teal]Weekly[/] expects..
        /// </summary>
        internal static string InputHelpMessage {
            get {
                return ResourceManager.GetString("InputHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something went wrong inside [teal]Weekly[/]..
        /// </summary>
        internal static string InternalErrorMessage {
            get {
                return ResourceManager.GetString("InternalErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something failed unexpectedly. To see error details, set [olive]LogExceptionsToConsole[/] to [olive]true[/] in your config file at [olive]{0}[/]..
        /// </summary>
        internal static string InternalHelpMessage {
            get {
                return ResourceManager.GetString("InternalHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn’t find issue key [olive]{0}[/]..
        /// </summary>
        internal static string IssueNotFoundErrorMessage {
            get {
                return ResourceManager.GetString("IssueNotFoundErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [teal]Weekly[/] checks issue keys using the Jira API. Check for typos. If it looks right, the API may be down. Enable logging in your config file at [olive]{0}[/] by setting [olive]LogExceptionsToConsole[/] to [olive]true[/]..
        /// </summary>
        internal static string IssueNotFoundHelpMessage {
            get {
                return ResourceManager.GetString("IssueNotFoundHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t find your template folder..
        /// </summary>
        internal static string MissingTemplateDirectoryErrorMessage {
            get {
                return ResourceManager.GetString("MissingTemplateDirectoryErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Set [olive]TemplateDirectory[/] in your config file at [olive]{0}[/] to a valid folder in your work log direcotry, or run [olive]wk init[/] from within your work log directory to let [teal]Weekly[/] set things up for you. .
        /// </summary>
        internal static string MissingTemplateDirectoryHelpMessage {
            get {
                return ResourceManager.GetString("MissingTemplateDirectoryHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing [olive]{0}[/] API token..
        /// </summary>
        internal static string MissingTokenErrorMessage {
            get {
                return ResourceManager.GetString("MissingTokenErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To post logs, [teal]Weekly[/] needs API tokens for [olive]Jira[/] and [olive]Tempo[/]. The tokens will need to be created individually on each service. Once you have them, you can them to [teal]Weekly[/] by using the [olive]wk token add[/] command. Examples are below. Replace the [teal]fake[/] values below with your data.
        ///
        ///[olive]wk token add jira[/] [teal]you@example.com abc123...[/]
        ///[olive]wk token add tempo[/] [teal]you@example.com xyz456...[/].
        /// </summary>
        internal static string MissingTokenHelpMessage {
            get {
                return ResourceManager.GetString("MissingTokenHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can&apos;t find your work log folder..
        /// </summary>
        internal static string MissingWorkLogDirectoryErrorMessage {
            get {
                return ResourceManager.GetString("MissingWorkLogDirectoryErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Set [olive]WorkLogDirectory[/] in your config file at [olive]{0}[/] to a valid folder, or run [olive]wk init[/] from within the directory you&apos;d like to use, to let [teal]Weekly[/] set things up for you..
        /// </summary>
        internal static string MissingWorkLogDirectoryHelpMessage {
            get {
                return ResourceManager.GetString("MissingWorkLogDirectoryHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are already time entries for this period..
        /// </summary>
        internal static string PushConflictErrorMessage {
            get {
                return ResourceManager.GetString("PushConflictErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you&apos;re sure your log is correct, use [olive]--force[/] with [olive]wk push[/] to overwrite what’s on the server..
        /// </summary>
        internal static string PushConflictHelpMessage {
            get {
                return ResourceManager.GetString("PushConflictHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Template not found..
        /// </summary>
        internal static string TemplateNotFoundErrorMessage {
            get {
                return ResourceManager.GetString("TemplateNotFoundErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The template name you used doesn’t match any files in your template folder. Double-check the name and file format..
        /// </summary>
        internal static string TemplateNotFoundHelpMessage {
            get {
                return ResourceManager.GetString("TemplateNotFoundHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The work log wasn&apos;t found..
        /// </summary>
        internal static string WorkLogNotFoundErrorMessage {
            get {
                return ResourceManager.GetString("WorkLogNotFoundErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The work log name you used doesn&apos;t match any files in your work log directory, or the current work log hasn’t been created yet. Use [olive]wk create[/] or just run [olive]wk add[/] to start one automatically..
        /// </summary>
        internal static string WorkLogNotFoundHelpMessage {
            get {
                return ResourceManager.GetString("WorkLogNotFoundHelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There’s a problem reading the work log file format..
        /// </summary>
        internal static string WorkLogParseErrorMessage {
            get {
                return ResourceManager.GetString("WorkLogParseErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you edited the file manually, something may have broken it. Change it&apos;s file extension or delete it, then run [olive]wk create[/] to start fresh..
        /// </summary>
        internal static string WorkLogParseHelpMessage {
            get {
                return ResourceManager.GetString("WorkLogParseHelpMessage", resourceCulture);
            }
        }
    }
}
