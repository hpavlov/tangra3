using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Globalization;

namespace AutoUpdate.Schema
{
    class ModuleUpdate : UpdateObject 
    {
        internal readonly int VerReq = 0;        
        internal readonly DateTime DateCreated = DateTime.MaxValue;
        internal readonly string m_Created = null;
        internal readonly bool m_NonEnglishOnly = false;

		//<ModuleUpdate VerReq="1" File="Software.LangRes.dll" MustExist="false" Version="30000" ReleaseDate="21 Mar 2008" ModuleName="Translation Resources">
		//    <File Path="/Software_3_0_0_0/Software.LangRes.zip" LocalPath="Software.LangRes.dll" Archived="true"/>
        //</ModuleUpdate>
        //
		//<ModuleUpdate VerReq="2" File="/Documentation/Software.pdf" MustExist="false" Created="17/03/2008" ReleaseDate="21 Mar 2008" ModuleName="Documentation">
		//    <File Path="/Software_3_0_0_0/Software.EN.pdf" LocalPath="/Documentation/Software.pdf" Action="ShellExecute">
		//      <Language Id="1" Path="/Software_3_0_0_0/Software.EN.pdf" />
		//      <Language Id="2" Path="/Software_3_0_0_0/Software.DE.pdf" />
        //    </File>
        //</ModuleUpdate>
        public ModuleUpdate(XmlElement node)
            : base(node)
        {
            VerReq = int.Parse(node.Attributes["VerReq"].Value, CultureInfo.InvariantCulture);
            if (node.Attributes["Version"] != null)
                m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture);
            else if (node.Attributes["Created"] != null)
                m_Created = node.Attributes["Created"].Value;

            m_File = node.Attributes["File"].Value;
            m_ReleaseDate = node.Attributes["ReleaseDate"].Value;
            m_ModuleName = node.Attributes["ModuleName"].Value;

            if (node.Attributes["NativeFileLength"] != null)
                m_NativeFileLength = Convert.ToInt64(node.Attributes["NativeFileLength"].Value, CultureInfo.InvariantCulture);
            else
                m_NativeFileLength = -1;

            if (node.Attributes["MustExist"] != null)
                m_MustExist = Convert.ToBoolean(node.Attributes["MustExist"].Value, CultureInfo.InvariantCulture);
            else
                m_MustExist = true;

            if (node.Attributes["NonEnglishOnly"] != null)
                m_NonEnglishOnly = Convert.ToBoolean(node.Attributes["NonEnglishOnly"].Value, CultureInfo.InvariantCulture);
            else
                m_NonEnglishOnly = false;

            if (node.Attributes["VersionStr"] != null)
                m_VersionStr = node.Attributes["VersionStr"].Value;
        }

        public override bool NewUpdatesAvailable(string tangra3Path)
        {
            if (VerReq > 1 &&
                !string.IsNullOrEmpty(m_Created))
            {
				string fullLocalFileName = System.IO.Path.GetFullPath(tangra3Path + "\\" + this.File);
                if (!System.IO.File.Exists(fullLocalFileName) &&
                    !m_MustExist)
                {
                    // The file doesn't have to exist and because it actually doesn't 
                    // this is why it must be downloaded i.e. a newer version is available
                    Trace.WriteLine(string.Format("Update required for '{0}': The file is not found locally", File));
                    return true;
                }				
				else
				{
					DateTime localModifiedDate = System.IO.File.GetLastWriteTime(fullLocalFileName);
					DateTime serverModifiedUTCDate = DateTime.Parse(m_Created, CultureInfo.InvariantCulture);

					if (localModifiedDate.ToUniversalTime().CompareTo(serverModifiedUTCDate) < 0)
					{
						Trace.WriteLine(string.Format("Update required for '{0}': local last modified: {1}; server last modified: {2}",
						                              File, localModifiedDate.ToUniversalTime(), serverModifiedUTCDate));
						return true;
					}
					else
						return false;
				}
            }
            else
                return base.NewUpdatesAvailable(tangra3Path);
        }

        protected override void OnFileUpdated(Schema.File file, string localFilePath)
        {
            if (!string.IsNullOrEmpty(this.m_Created))
            {
                // Set the file time. All updates files will get the date & time of the check file, which is fine!
                DateTime utcModifiedTime = DateTime.Parse(m_Created, CultureInfo.InvariantCulture);
                System.IO.File.SetLastWriteTimeUtc(localFilePath, utcModifiedTime);
            }
        }
    }
}
