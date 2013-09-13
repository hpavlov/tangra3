using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AutoUpdateSelfUpdate;

namespace AutoUpdate.Schema
{
    class UpdateSchema
    {
        internal List<UpdateObject> AllUpdateObjects = new List<UpdateObject>();

        internal Schema.SoftwareUpdate OccuRecUpdate;
        internal Schema.SoftwareMainUpdate OccuRec;

        public UpdateSchema(XmlDocument xml)
        {
            foreach(XmlNode el in xml.DocumentElement.ChildNodes)
            {
				if (SharedUpdateConstants.UPDATER_PROGRAM_XML_ELEMENT.Equals(el.Name))
                {
                    OccuRecUpdate = new Schema.SoftwareUpdate(el as XmlElement);
                    AllUpdateObjects.Add(OccuRecUpdate);
                }
                else if ("Update".Equals(el.Name))
                {
                    OccuRec = new Schema.SoftwareMainUpdate(el as XmlElement);
                    AllUpdateObjects.Add(OccuRec);
                }
                else if ("ModuleUpdate".Equals(el.Name))
                    AllUpdateObjects.Add(new Schema.ModuleUpdate(el as XmlElement));
            }
        }

        public bool NewUpdatesAvailable(string occuRecPath)
        {
            foreach (UpdateObject obj in AllUpdateObjects)
				if (obj.NewUpdatesAvailable(occuRecPath)) return true;

            return false;
        }
    }
}
