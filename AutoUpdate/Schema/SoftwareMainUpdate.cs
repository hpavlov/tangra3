using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using AutoUpdateSelfUpdate;

namespace AutoUpdate.Schema
{
    class SoftwareMainUpdate : UpdateObject
    {
		//<Update File="Software.exe" MustExist="true" Version="30000" ReleaseDate="21 Mar 2008" ModuleName="Occult Watcher">
		//    <File Path="/Software_3_0_0_0/Software.zip" LocalPath="Software.exe" Archived="true" />
		//    <File Path="/Software_3_0_0_0/Software.Core.zip" LocalPath="Software.Core.dll" Archived="true" />
		//    <File Path="/Software_3_0_0_0/Software.SDK.dll" />
        //</Update>
        public SoftwareMainUpdate(XmlElement node)
            : base(node)
        {
            if (node.Attributes["File"] == null)
				throw new InstallationAbortException("The update location points to an older version of " + SharedUpdateConstants.MAIN_PROGRAM_NAME + ".");

            m_File = node.Attributes["File"].Value;
            m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture);
            if (node.Attributes["MustExist"] != null)
                m_MustExist = Convert.ToBoolean(node.Attributes["MustExist"].Value, CultureInfo.InvariantCulture);
            else
                m_MustExist = true;

            m_ReleaseDate = node.Attributes["ReleaseDate"].Value;

            if (node.Attributes["ModuleName"] != null)
                m_ModuleName = node.Attributes["ModuleName"].Value;
            else
				m_ModuleName = SharedUpdateConstants.MAIN_PROGRAM_NAME;
        }
    }
}
