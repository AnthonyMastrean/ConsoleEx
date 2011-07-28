using System;
using System.Reflection;

[assembly: AssemblyTitle("ConsoleEx")]
[assembly: AssemblyDescription("Helper class for writing console applications")]
[assembly: AssemblyCompany("Microsoft Ltd.")]
[assembly: AssemblyCopyright("(C) 2002 Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyProduct("ConsoleEx")]
[assembly: AssemblyVersion("1.0.4")]
[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif