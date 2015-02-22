using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Win32;

namespace Tangra.Model.Controls
{
    public class PersistableDropDown : ComboBox
    {
        private string m_RegistryKey;
        private string m_PersistanceKey;
        private int m_NumberRememberedValues = 10;

        [Category("Persistance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("The registry key where to save the values")]
        [DefaultValue(@"Software\HristoPavlov")]
        public string RegistryKey
        {
            get { return m_RegistryKey; }
            set { m_RegistryKey = value; }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Persistance")]
        [Description("Unique ID corresponding to the persisted values of this control.")]
        [DefaultValue("")]
        public string PersistanceKey
        {
            get { return m_PersistanceKey; }
            set { m_PersistanceKey = value; }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category("Persistance")]
        [Description("Maximum number of values to be remembered.")]
        [DefaultValue(10)]
        public int RememberedValues
        {
            get { return m_NumberRememberedValues; }
            set { m_NumberRememberedValues = value; }
        }

        private List<string> m_OrderedItems = new List<string>();

        public PersistableDropDown()
            : base()
        {
            m_PersistanceKey = this.Name;
            DropDownStyle = ComboBoxStyle.DropDown;
        }

        public void Persist()
        {
            if (Text != null)
            {
                string selection = Text;

                if (!string.IsNullOrEmpty(selection))
                {
                    LoadSavedValues();

                    int selectedIndex = m_OrderedItems.IndexOf(selection);
                    if (selectedIndex > -1)
                    {
                        m_OrderedItems.Remove(selection);
                    }
                    else
                    {
                        while (m_OrderedItems.Count >= m_NumberRememberedValues)
                            m_OrderedItems.RemoveAt(m_PersistanceKey.Length - 1);
                    }

                    m_OrderedItems.Insert(0, selection);
                    BindItems();

                    SavedValues();
                }
            }
        }

        protected override void OnDropDown(EventArgs e)
        {
            LoadSavedValues();

            BindItems();

            base.OnDropDown(e);
        }

        private void BindItems()
        {
            Items.Clear();
            foreach (string item in m_OrderedItems)
                Items.Add(item);
        }

        private void LoadSavedValues()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_PersistanceKey) &&
                    !string.IsNullOrEmpty(m_RegistryKey))
                {
                    string fullKeyName = m_RegistryKey.TrimEnd('\\') + @"\Persistance";
                    RegistryKey regKeyRoot = Registry.CurrentUser.OpenSubKey(fullKeyName, true);
                    if (regKeyRoot == null)
                        regKeyRoot = Registry.CurrentUser.CreateSubKey(fullKeyName);

                    byte[] value = (byte[])regKeyRoot.GetValue(m_PersistanceKey, null);
                    if (value != null)
                    {
                        DecodeValues(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private void SavedValues()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_PersistanceKey) &&
                    !string.IsNullOrEmpty(m_RegistryKey))
                {
                    string fullKeyName = m_RegistryKey.TrimEnd('\\') + @"\Persistance";
                    RegistryKey regKeyRoot = Registry.CurrentUser.OpenSubKey(fullKeyName, true);
                    if (regKeyRoot == null)
                        regKeyRoot = Registry.CurrentUser.CreateSubKey(fullKeyName);

                    if (m_OrderedItems != null &&
                        m_OrderedItems.Count > 0)
                    {
                        byte[] encodedValues = EncodeValues();
                        regKeyRoot.SetValue(m_PersistanceKey, encodedValues);
                    }
                    else
                        regKeyRoot.SetValue(m_PersistanceKey, new byte[0]);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private byte[] EncodeValues()
        {
            List<Byte> bytes = new List<byte>();

            foreach (string item in m_OrderedItems)
            {
                bytes.AddRange(Encoding.UTF7.GetBytes(item));
                bytes.Add(0);
            }

            return bytes.ToArray();
        }

        private void DecodeValues(byte[] rawData)
        {
            m_OrderedItems.Clear();
            List<Byte> bytes = new List<byte>();
            foreach (byte bt in rawData)
            {
                if (bt == 0)
                {
                    string item = Encoding.UTF7.GetString(bytes.ToArray());
                    m_OrderedItems.Add(item);
                    bytes.Clear();
                }
                else
                    bytes.Add(bt);
            }
        }
    }
}
