/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Text;

namespace AutoUpdateSelfUpdate
{
	public class SharedUpdateConstants
	{
		public static string REGISTRY_KEY = @"SOFTWARE\Tangra3";
		public static string REG_ENTRY_COPY_FROM_FULL_FILE_NAME = "CopySelfTangra3UpdateFrom";
		public static string REG_ENTRY_COPY_TO_DIRECTORY_NAME = "CopySelfTangra3UpdateTo";
		public static string REG_ENTRY_DEFAULT_UPDATE_LOCATION = "UpdateLocation";
		public static string REG_ENTRY_SELFUPDATE_TEMP_FILE = "SelfTangra3UpdateTempFile";
		public static string REG_ENTRY_UPDATE_LOCATION = "Tangra3UpdateTangra3Location";
		public static string REG_ENTRY_ACCEPT_BETA_VERSION = "Tangra3UpdateAcceptBeta";

		public static string MAIN_EXECUTABLE_NAME = "Tangra.exe";
		public static string MAIN_UPDATER_EXECUTABLE_NAME = "Tangra3Update.exe";
		public static string MAIN_PROGRAM_NAME = "Tangra3";
		public static string UPDATER_PROGRAM_NAME = "Tangra3 Update";
		public static string UPDATER_PROGRAM_XML_ELEMENT = "AutoUpdate";
		public static string UPDATE_URL_LOCATION = "http://www.hristopavlov.net/Tangra3/";
	}
}
