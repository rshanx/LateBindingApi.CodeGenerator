﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.VB
{
    partial class FormConfigDialog : Form
    {
        #region Construction

        public FormConfigDialog()
        {
            InitializeComponent();
            textBoxFolder.Text = Application.StartupPath;
            comboBoxFramework.SelectedIndex = 0;
        }
        
        #endregion

        #region Properties

        public Settings Selected
        {
            get 
            {
                Settings newSettings = new Settings();
                newSettings.Folder = textBoxFolder.Text.Trim();
                newSettings.AddTestApp = checkBoxAddTestApplication.Checked; 
                newSettings.OpenFolder = checkBoxOpenFolder.Checked;  
                newSettings.ConvertOptionalsToObject = checkBoxConvertOptionals.Checked;
                newSettings.ConvertParamNamesToCamelCase = checkBoxConvertToCamel.Checked;
                newSettings.RemoveRefAttribute = checkBoxRemoveRef.Checked;
                newSettings.CreateXmlDocumentation = checkBoxCreateDocu.Checked;
                newSettings.UseSigning = checkBoxSignAssemblies.Checked;
                newSettings.SignPath = textBoxKeyFiles.Text;
 
                string res = "";
                switch (comboBoxFramework.SelectedIndex)
                {
                    case 0:
                        res = "2.0";
                        break;
                    case 1:
                        res = "3.0";
                        break;
                    case 2:
                        res = "3.5";
                        break;
                    case 3:
                        res = "4.0";
                        break;
                }
                newSettings.Framework = res;
                return newSettings;
            }
        }

        #endregion

        #region Trigger

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; 
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (DialogResult.OK == folderDialog.ShowDialog(this))
                textBoxFolder.Text = folderDialog.SelectedPath;
        }

        private void buttonWhyOptionals_Click(object sender, EventArgs e)
        {
            HelpBox box = new HelpBox(this);
            box.Show("#Optionals");
        }

        private void buttonWhyCamel_Click(object sender, EventArgs e)
        {
            HelpBox box = new HelpBox(this);
            box.Show("#Camel");
        }

        private void buttonWhyRef_Click(object sender, EventArgs e)
        {
            HelpBox box = new HelpBox(this);
            box.Show("#Ref");
        }

        private void buttonWhyFramework_Click(object sender, EventArgs e)
        {
            HelpBox box = new HelpBox(this);
            box.Show("#Framework");
        }

        private void buttonWhyDocu_Click(object sender, EventArgs e)
        {
            HelpBox box = new HelpBox(this);
            box.Show("#Docu");
        }

        #endregion

        private void buttonKeyFiles_Click(object sender, EventArgs e)
        {            
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (DialogResult.OK == folderDialog.ShowDialog(this))
                textBoxKeyFiles.Text = folderDialog.SelectedPath;
        }
    }
}
