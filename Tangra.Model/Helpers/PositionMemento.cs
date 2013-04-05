using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Tangra.Model.Helpers
{
    public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        public new TValue this[TKey key]
        {
            get
            {
                TValue val;

                if (base.TryGetValue(key, out val))
                    return val;
                else
                    return default(TValue);
            }
            set
            {
                base[key] = value;
            }
        }

        public new void Add(TKey key, TValue value)
        {
            TValue val;

            if (base.TryGetValue(key, out val))
            {
                base[key] = value;
            }
            else
                base.Add(key, value);
        }
    }

    public static class PositionMemento
    {
        public static string DATABASE_PATH
        {
            get
            {
                string rootDirectory = null;
                try
                {
                    rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }
                catch (Exception)
                {
                    rootDirectory = null;
                }

                if (rootDirectory != null &&
                    System.IO.Directory.Exists(rootDirectory))
                {
                    rootDirectory = System.IO.Path.GetFullPath(string.Concat(rootDirectory, "\\Tangra"));
                }
                else
                {
                    rootDirectory = "C:\\Tangra";
                }

                if (!System.IO.Directory.Exists(rootDirectory))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(rootDirectory);
                        System.IO.File.SetAttributes(rootDirectory, System.IO.FileAttributes.Hidden);
                    }
                    catch (Exception)
                    {
                        rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }

                return rootDirectory;
            }
        }

        private static PositionMementoCache m_Cache = new PositionMementoCache();

        public static bool CacheReset
        {
            get { return m_Cache.m_CacheReset; }
        }

        public static void SaveControlPosition(Control ctl)
        {
            SaveControlPosition(ctl, null);
        }

        public static void SaveControlPosition(Control ctl, object argument)
        {
            if (ctl != null)
            {
                ControlMemento memento = m_Cache.GetControlMemento(ctl);

                if (ctl is Form)
                {
                    SaveForm(memento, ctl as Form);
                }
                else if (ctl is ListView)
                {
                    SaveListView(memento, ctl as ListView, argument);
                }
                else if (ctl is DataGridView)
                {
                    SaveDataGridView(memento, ctl as DataGridView, argument);
                }
                else
                {
                    Debug.Assert(false, "Control type not supported.");
                }

                m_Cache.Save();
            }
        }

        public static bool LoadControlPosition(Control ctl)
        {
            if (ctl != null)
            {
                ControlMemento memento = m_Cache.GetControlMemento(ctl);

                if (ctl is Form)
                {
                    return LoadForm(memento, ctl as Form);
                }
                else if (ctl is ListView)
                {
                    return LoadListView(memento, ctl as ListView);
                }
                else if (ctl is DataGridView)
                {
                    return LoadDataGridView(memento, ctl as DataGridView);
                }
                else
                {
                    Debug.Assert(false, "Control type not supported.");
                }
            }

            return false;
        }

        private static void SaveForm(ControlMemento memento, Form form)
        {
            memento.Properties.Clear();

            memento.Properties.Add("Version", 1);

            memento.Properties.Add("Height", form.Height);
            memento.Properties.Add("Width", form.Width);
            memento.Properties.Add("Top", form.Top);
            memento.Properties.Add("Left", form.Left);
        }

        private static bool LoadForm(ControlMemento memento, Form form)
        {
            if (memento.IsNew)
            {
                memento.Properties["Height"] = form.Height;
                memento.Properties["Width"] = form.Width;
                memento.Properties["Top"] = form.Top;
                memento.Properties["Left"] = form.Left;
                memento.IsNew = false;
            }

            form.Height = memento.Properties["Height"];
            form.Width = memento.Properties["Width"];
            form.Top = memento.Properties["Top"];
            form.Left = memento.Properties["Left"];

            return true;
        }

        private static void SaveDataGridView(ControlMemento memento, DataGridView dgv, object argument)
        {
            memento.Properties.Clear();

            memento.Properties.Add("Version", 1);

            int idx = 0;
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                string key = string.Format("Width-{0}", idx);
                int val = column.Width;

                memento.Properties.Add(key, val);
                idx++;
            }
        }

        private static void SaveListView(ControlMemento memento, ListView lv, object argument)
        {
            memento.Properties.Clear();

            memento.Properties.Add("Version", 1);

            int idx = 0;
            foreach (ColumnHeader col in lv.Columns)
            {
                string key = string.Format("Width-{0}", idx);
                int val = col.Width;

                memento.Properties.Add(key, val);
                idx++;
            }
        }


        private static bool LoadListView(ControlMemento memento, ListView lv)
        {
            if (memento.IsNew)
            {
                SaveListView(memento, lv, null);
                memento.IsNew = false;
            }

            if (memento.Properties.Count <= lv.Columns.Count + 1)
            {
                int idx = 0;
                foreach (ColumnHeader col in lv.Columns)
                {
                    string key = string.Format("Width-{0}", idx);

                    if (memento.Properties.ContainsKey(key))
                    {
                        int val = memento.Properties[key];

                        col.Width = val;
                    }
                    else
                        idx++;
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        private static bool LoadDataGridView(ControlMemento memento, DataGridView dgv)
        {
            if (memento.IsNew)
            {
                SaveDataGridView(memento, dgv, null);
                memento.IsNew = false;
            }

            if (memento.Properties.Count == dgv.Columns.Count + 1)
            {
                int idx = 0;
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    string key = string.Format("Width-{0}", idx);
                    int val = memento.Properties[key];

                    column.Width = val;
                    idx++;
                }

                return true;
            }
            else
            {
                //Trace.WriteLineIf(Config.TraceSwitches.UIEvents.TraceVerbose, "FAILED: '" + memento.Properties.Count.ToString() + "' props and '" + dgv.Columns.Count.ToString() + "' columns", "ControlMemento");
                return false;
            }

        }

    }

    [Serializable()]
    internal class ControlMemento : ISerializable
    {
        private string m_ControlType = null;
        private SafeDictionary<string, int> m_Properties = new SafeDictionary<string, int>();
        internal bool IsNew = false;

        public string ControlType
        {
            get { return m_ControlType; }
        }

        public SafeDictionary<string, int> Properties
        {
            get { return m_Properties; }
        }

        public ControlMemento(string controlType)
        {
            m_ControlType = controlType;
            IsNew = true;
        }

        public ControlMemento(SerializationInfo info, StreamingContext context)
        {
            m_ControlType = info.GetString("ControlType");
            int count = info.GetInt32("AllItems");

            m_Properties.Clear();

            for (int i = 0; i < count; i++)
            {
                string key = info.GetString(string.Format("{0}-{1}-0", m_ControlType, i));
                int val = info.GetInt32(string.Format("{0}-{1}-1", m_ControlType, i));
                m_Properties.Add(key, val);
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ControlType", m_ControlType);
            info.AddValue("AllItems", m_Properties.Keys.Count);

            int i = 0;
            foreach (string key in m_Properties.Keys)
            {
                info.AddValue(string.Format("{0}-{1}-0", m_ControlType, i), key);
                info.AddValue(string.Format("{0}-{1}-1", m_ControlType, i), m_Properties[key]);
                i++;
            }
        }
    }

    internal class PositionMementoCache
    {
        private object m_SyncRoot = new object();
        private Dictionary<string, ControlMemento> m_SavedControl = new Dictionary<string, ControlMemento>();

        public ControlMemento GetControlMemento(Control ctl)
        {
            string key = ctl.GetType().ToString() + "*" + ctl.Name;

            ControlMemento outVal;
            if (m_SavedControl.TryGetValue(key, out outVal))
                return outVal;
            else
            {
                ControlMemento newMemento = new ControlMemento(key);
                m_SavedControl.Add(key, newMemento);
                return newMemento;
            }
        }

        internal PositionMementoCache()
        {
            LoadCache();
        }

        internal bool m_CacheReset = false;

        private void LoadCache()
        {
            lock (m_SyncRoot)
            {
                string cacheFile = System.IO.Path.GetFullPath(PositionMemento.DATABASE_PATH + "\\Controls.cache");
                if (File.Exists(cacheFile))
                {
                    m_SavedControl.Clear();
                    try
                    {
                        using (FileStream cachFile = new FileStream(cacheFile, FileMode.Open, FileAccess.Read))
                        {
                            BinaryFormatter frm = new BinaryFormatter();
                            int NumEvents = (int)frm.Deserialize(cachFile);
                            for (int i = 0; i < NumEvents; i++)
                            {
                                ControlMemento entry = (ControlMemento)frm.Deserialize(cachFile);
                                m_SavedControl.Add(entry.ControlType, entry);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.FullExceptionInfo());
                        m_CacheReset = true;
                    }
                }
                else
                    m_CacheReset = true;
            }
        }

        private void SaveCache()
        {
            lock (m_SyncRoot)
            {
                string cacheFile = System.IO.Path.GetFullPath(PositionMemento.DATABASE_PATH + "\\Controls.cache");
                try
                {

                    using (FileStream cachFile = new FileStream(cacheFile, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter frm = new BinaryFormatter();
                        frm.Serialize(cachFile, m_SavedControl.Count);

                        foreach (ControlMemento entry in m_SavedControl.Values)
                        {
                            frm.Serialize(cachFile, entry);
                        }

                        cachFile.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.FullExceptionInfo());

                    if (File.Exists(cacheFile))
                        File.Delete(cacheFile);
                }
            }
        }

        public void Save()
        {
            SaveCache();
        }
    }
}
