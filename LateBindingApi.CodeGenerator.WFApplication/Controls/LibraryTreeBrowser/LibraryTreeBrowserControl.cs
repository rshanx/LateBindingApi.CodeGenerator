﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryTreeBrowser
{
    /// <summary>
    /// Shows document types in a grid
    /// </summary>
    public partial class LibraryTreeBrowserControl : UserControl
    {
        #region Fields
        
        private string _currentNodePath;

        #endregion

        #region Events
        
        /// <summary>
        /// Grid Trigger sfter select 
        /// </summary>
        public event TreeViewEventHandler AfterSelect;

        #endregion

        #region Construction

        public LibraryTreeBrowserControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        private void ShowElements(XElement project, TreeNode treeProject, string iteratorName, string elementName)
        {
            TreeNode treeIterator = treeProject.Nodes.Add(iteratorName, iteratorName);
            foreach (var itemEnum in project.Element(iteratorName).Elements(elementName))
            {
                string enumName = itemEnum.Attribute("Name").Value;
                if (true == FilterPassed(enumName))
                {
                    TreeNode treeElement = treeIterator.Nodes.Add(enumName, enumName);
                    treeElement.Tag = itemEnum;
                }
            }
        }

        public void Show(XElement node)
        {
            Clear();
            treeViewComponents.Tag = node;

            // show libraries
            TreeNode treeLibraries = treeViewComponents.Nodes.Add("Libraries");
            treeLibraries.Tag = node.Element("Libraries");

            // show projects
            TreeNode treeSolution = treeViewComponents.Nodes.Add("Solution");
            treeSolution.Tag = node.Element("Solution");
            treeSolution.Expand();
            foreach (var item in node.Element("Solution").Element("Projects").Elements("Project"))
            {
                string projectName = item.Attribute("Name").Value;
                TreeNode treeProject = treeSolution.Nodes.Add(projectName, projectName);
                treeProject.Tag = item;
                ShowElements(item, treeProject, "Constants", "Constant");
                ShowElements(item, treeProject, "Enums", "Enum");
                ShowElements(item, treeProject, "DispatchInterfaces", "Interface");
                ShowElements(item, treeProject, "Interfaces", "Interface");
                ShowElements(item, treeProject, "CoClasses", "CoClass");
                ShowElements(item, treeProject, "Modules", "Modul");
                ShowElements(item, treeProject, "Records", "Record");
                ShowElements(item, treeProject, "TypeDefs", "Alias");
            }
        }

        public void Clear()
        {
            treeViewComponents.Nodes.Clear();
        }
        
        #endregion

        #region Methods

        private XElement GetChildElementByKey(XElement root, string key, string type)
        {
            var dispFaces = root.Descendants(type);

            XElement node = (from a in dispFaces.Elements()
                             where a.Attribute("Key").Value.Equals(key)
                             select a).FirstOrDefault();

            return node;
        }

        private void UpdateNodeInfo(ref TreeViewEventArgs e)
        {
            XElement node = e.Node.Tag as XElement;
            if (null != node)
            {
                XAttribute keyAttribute = node.Attribute("Key");
                if (null != keyAttribute)
                {
                    string key = keyAttribute.Value;
                    XElement root = treeViewComponents.Tag as XElement;

                    node = GetChildElementByKey(root, key, "DispatchInterfaces");
                    if (null != node)
                    {
                        e.Node.Tag = node;
                        return;
                    }

                    node = GetChildElementByKey(root, key, "Interfaces");
                    if (null != node)
                    {
                        e.Node.Tag = node;
                        return;
                    }

                    node = GetChildElementByKey(root, key, "CoClasses");
                    if (null != node)
                    {
                        e.Node.Tag = node;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Filter Gui Trigger

        private bool FilterPassed(string expression)
        {
            string filterText = textBoxFilter.Text.Trim();
            if (filterText == "")
                return true;

            string[] filterArray = filterText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string filter in filterArray)
            {
                int stringPosition = expression.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase);
                if (stringPosition > -1)
                    return true;
            }

            return false;   
        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            XElement documentNode = treeViewComponents.Tag as XElement;
            if ((null != documentNode) && (e.KeyCode == Keys.Return))
            {
                SaveCurrentNodePath();
                Show(documentNode);
                RestoreExpandState();
            }
        }

        public void SaveCurrentNodePath()
        {
            if (null == treeViewComponents.SelectedNode)
                _currentNodePath = "";
            else
                _currentNodePath = treeViewComponents.SelectedNode.FullPath;
        }

        public void RestoreExpandState()
        {
            TreeNode node = null;
            string[] splitArray = _currentNodePath.Split(new string[] { treeViewComponents.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string nodeName in splitArray)
            {
                if (node == null)
                    node = SearchChildTree(treeViewComponents, nodeName);
                else
                    node = SearchChildTree(node, nodeName);

                if (node != null)
                    node.Expand();
            }
            treeViewComponents.SelectedNode = node;
        }

        private TreeNode SearchChildTree(TreeView treeView, string name)
        {
            foreach (TreeNode tn in treeViewComponents.Nodes)
            {
                if (tn.Text == name)
                    return tn;
            }
            return null;
        }

        private TreeNode SearchChildTree(TreeNode treeNode, string name)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Text == name)
                    return tn;
            }
            return null;
        } 

        #endregion

        #region Tree Gui Trigger

        private void treeViewComponents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null != AfterSelect)
            {
                UpdateNodeInfo(ref e);
                AfterSelect(sender, e);
            }
        }

        #endregion
    }
}
