/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tangra.Helpers;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tangra 3")]
[assembly: AssemblyDescription(
	"Tangra 3 - software package for astronomical video data reduction is maintained by Hristo Pavlov.\r\n\r\n" + 
	"Project contributors: Hristo Pavlov, Anthony Mallama\r\n\r\n" +
	"Tangra 3 uses a number of open source and/or free components which may have their respective copyright owners.\r\n\r\n" +
	"CCharpFITS is a C# port of Tom McGlynn's nom.tam.fits Java package, initially ported by  Samuel Carliles. " +
    "nom.tam.fits is Copyright by Thomas McGlynn 1997-2007; " +
	"CCharpFITS is Copyright by Virtual Observatory - India 2007.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Hristo Pavlov")]
[assembly: AssemblyProduct("Tangra")]
[assembly: AssemblyCopyright("Tangra is developed in 2012-2020 by Hristo Pavlov and other contributors")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("b3b73223-050c-453d-842c-e0b5e07ec0cc")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.6.0.23")]
[assembly: AssemblyFileVersion("3.6.23")]
[assembly: ReleaseDate("2020-02-14")]
//[assembly: BetaReleaseAttribute()]
[assembly: TangraCoreVersionRequired("3.0.122")]
[assembly: TangraVideoVersionRequired("3.0.29")]
[assembly: TangraVideoLinuxVersionRequiredAttribute("3.0.11")]
[assembly: TangraVideoOSXVersionRequiredAttribute("3.0.15")]
