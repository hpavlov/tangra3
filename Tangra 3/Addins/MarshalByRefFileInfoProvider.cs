using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangra.SDK;

namespace Tangra.Addins
{
    [Serializable]
    public class MarshalByRefFileInfoProvider : IFileInfoProvider
    {
        private string m_FileName;

        public MarshalByRefFileInfoProvider(IFileInfoProvider localProvider)
        {
            if (localProvider != null)
                m_FileName = localProvider.FileName;
            else
                m_FileName = null;
        }

        public string FileName
        {
            get { return m_FileName; }
        }
    }
}
