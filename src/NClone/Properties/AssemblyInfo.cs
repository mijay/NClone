using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("NClone")]
[assembly: AssemblyDescription("NClone - fast and flexible deep clone for arbitrary .NET objects")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Dmitry Kononchuk")]
[assembly: AssemblyProduct("NClone")]
[assembly: AssemblyCopyright("Copyright © Dmitry Kononchuk 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("ba9971b4-5737-44e6-8670-e67e7674ece8")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]

#if DEBUG
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("NClone.Tests")]
[assembly: InternalsVisibleTo("NClone.EntryPoint")]
#endif