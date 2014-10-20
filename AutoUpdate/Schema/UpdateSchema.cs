/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

        internal Schema.SoftwareUpdate AutoUpdate;
        internal Schema.SoftwareMainUpdate Tangra3;

        public UpdateSchema(XmlDocument xml)
        {
            foreach(XmlNode el in xml.DocumentElement.ChildNodes)
            {
				if (SharedUpdateConstants.UPDATER_PROGRAM_XML_ELEMENT.Equals(el.Name))
                {
                    AutoUpdate = new Schema.SoftwareUpdate(el as XmlElement);
                    AllUpdateObjects.Add(AutoUpdate);
                }
                else if ("Update".Equals(el.Name))
                {
                    Tangra3 = new Schema.SoftwareMainUpdate(el as XmlElement);
                    AllUpdateObjects.Add(Tangra3);
                }
                else if ("ModuleUpdate".Equals(el.Name))
                    AllUpdateObjects.Add(new Schema.ModuleUpdate(el as XmlElement));
            }
        }

        public bool NewUpdatesAvailable(string tangra3Path)
        {
            foreach (UpdateObject obj in AllUpdateObjects)
				if (obj.NewUpdatesAvailable(tangra3Path)) return true;

            return false;
        }
    }
}
