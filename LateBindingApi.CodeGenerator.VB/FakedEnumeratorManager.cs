﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal class FakedEnumeratorManager
    {
        VBGenerator _parent;
        XDocument _document;
        XDocument _derived;

        internal FakedEnumeratorManager(VBGenerator parent, XDocument document)
        {
            _parent = parent;
            _document = document;
        }

        public void ScanForMissedEnumerators()
        {
            _derived = new XDocument();
            _derived.Add(new XElement("Document"));

            ScanForDerived("DispatchInterfaces", "Interface");
            ScanForDerived("Interfaces", "Interface");
        }

        private void ScanForDerived(string elements, string element)
        {

            var interfaces = (from a in _document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project").
                                 Elements(elements).Elements(element)
                              select a);

            foreach (XElement itemFace in interfaces)
            {
                XElement countNode = GetCountNode(itemFace);
                XElement itemNode = GetDefaultItemNode(itemFace);
                XElement enumNode = GetEnumeratorNode(itemFace);
                if ((null != countNode) && (null != itemNode) && (null == enumNode) && (itemNode.Element("Parameters").Element("Parameter").Attribute("IsEnum").Value == "false"))
                {
                    XElement projectNode = itemFace;
                    while (projectNode.Name != "Project")
                        projectNode = projectNode.Parent;
 
                    XElement fakedEnum = new XElement("Property", 
                                                new XAttribute("Name", "_NewEnum"),
                                                new XAttribute("InvokeKind", "INVOKE_PROPERTYGET"),
                                                new XAttribute("Key", XmlConvert.EncodeLocalName(Guid.NewGuid().ToString())),
                                                new XAttribute("IsCustom", "true")
                                        );
                    fakedEnum.Add(new XElement("RefLibraries"));
                    fakedEnum.Add(new XElement("Parameters", new XElement("RefLibraries")));
                    fakedEnum.Element("Parameters").Add(new XElement("ReturnValue",
                                                    new XAttribute("Type", itemNode.Element("Parameters").Element("ReturnValue").Attribute("Type").Value),
                                                    new XAttribute("VarType", itemNode.Element("Parameters").Element("ReturnValue").Attribute("VarType").Value),
                                                    new XAttribute("MarshalAs", itemNode.Element("Parameters").Element("ReturnValue").Attribute("MarshalAs").Value),
                                                    new XAttribute("TypeKind", itemNode.Element("Parameters").Element("ReturnValue").Attribute("TypeKind").Value),
                                                    new XAttribute("IsComProxy", itemNode.Element("Parameters").Element("ReturnValue").Attribute("IsComProxy").Value),
                                                    new XAttribute("IsExternal", itemNode.Element("Parameters").Element("ReturnValue").Attribute("IsExternal").Value),
                                                    new XAttribute("IsEnum", itemNode.Element("Parameters").Element("ReturnValue").Attribute("IsEnum").Value),
                                                    new XAttribute("IsArray", itemNode.Element("Parameters").Element("ReturnValue").Attribute("IsArray").Value),
                                                    new XAttribute("IsNative", itemNode.Element("Parameters").Element("ReturnValue").Attribute("IsNative").Value),
                                                    new XAttribute("TypeKey", itemNode.Element("Parameters").Element("ReturnValue").Attribute("TypeKey").Value),
                                                    new XAttribute("ProjectKey", itemNode.Element("Parameters").Element("ReturnValue").Attribute("ProjectKey").Value),
                                                    new XAttribute("LibraryKey", itemNode.Element("Parameters").Element("ReturnValue").Attribute("LibraryKey").Value)
                                                    )); 

                    foreach (XElement itemRef in itemNode.Element("Parameters").Element("RefLibraries").Elements("Ref"))
                        fakedEnum.Element("Parameters").Element("RefLibraries").Add(new XElement("Ref", new XAttribute("Key", itemRef.Attribute("Key").Value)));

                    foreach (XElement itemRef in itemNode.Element("RefLibraries").Elements("Ref"))
                        fakedEnum.Element("RefLibraries").Add(new XElement("Ref", new XAttribute("Key", itemRef.Attribute("Key").Value)));

                    XElement dispNode = new XElement("DispIds", new XElement("DispId", new XAttribute("Id", "-999")));
                    dispNode.Element("DispId").Add(new XElement("RefLibraries"));
                    foreach (XElement itemRef in itemNode.Element("Parameters").Element("RefLibraries").Elements("Ref"))
                    {
                        dispNode.Element("DispId").Element("RefLibraries").Add(new XElement("Ref", new XAttribute("Key", itemRef.Attribute("Key").Value)));
                    }
                    fakedEnum.Add(dispNode);
                    Console.WriteLine(projectNode.Attribute("Name").Value + "." + itemFace.Attribute("Name").Value);
                    itemFace.Element("Properties").Add(fakedEnum);
                }
            }
        }

        private XElement GetCountNode(XElement itemFace)
        {
            XElement node = (from a in itemFace.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("Count", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null != node)
            {
                string type = (node.Element("Parameters").Element("ReturnValue").Attribute("Type").Value) ;
                if ("Int32" == type)
                    return node;
            }

            node = (from a in itemFace.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("Count", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (null != node)
                return node;

            return null;
        }

        private XElement GetDefaultItemNode(XElement itemFace)
        {
            XElement node = null;

            foreach (XElement itemMethod in itemFace.Element("Properties").Elements("Property"))
            {
                node = (from a in itemMethod.Element("DispIds").Elements("DispId")
                        where a.Attribute("Id").Value.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();
                if (null != node)
                    return itemMethod;
            }

            foreach (XElement itemMethod in itemFace.Element("Methods").Elements("Method"))
            {
                node = (from a in itemMethod.Element("DispIds").Elements("DispId")
                        where a.Attribute("Id").Value.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                        select a).FirstOrDefault();
                if (null != node)
                    return itemMethod;
            }

            return null;
        }

        private XElement GetEnumeratorNode(XElement itemFace)
        {
            XElement node = (from a in itemFace.Element("Properties").Elements("Property")
                             where a.Attribute("Name").Value.Equals("_NewEnum", StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null != node)
                return node;

            node = (from a in itemFace.Element("Methods").Elements("Method")
                    where a.Attribute("Name").Value.Equals("_NewEnum", StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();
            if (null != node)
                return node;

            return null;
        }

        private void AddType(XElement type)
        {
            string id = type.Attribute("Key").Value;
            XElement node = (from a in _derived.Element("Document").Elements("Type")
                             where a.Attribute("Key").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            if (null == node)
            {
                string name = type.Attribute("Name").Value;
                _derived.Element("Document").Add(new XElement("Type", new XAttribute("Name", name), new XAttribute("Key", id)));
            }
        }

        private XElement GetTypeByName(XElement projectNode, string name)
        {

            XElement node = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                             where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in projectNode.Element("Interfaces").Elements("Interface")
                    where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;

            node = (from a in projectNode.Element("CoClasses").Elements("CoClass")
                    where a.Attribute("Name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    select a).FirstOrDefault();

            if (null != node)
                return node;

            throw new Exception("name not found " + name);
        }
   
        private bool HasAttriute(XElement node, string attributeName)
        {
            foreach (XAttribute item in node.Attributes())
            {
                if (item.Name == attributeName)
                    return true;
            }

            return false;
        }

        private XElement GetProjectNode(XElement returnValue)
        {
            XElement projectNode = returnValue.Parent;
            while (projectNode.Name != "Project")
                projectNode = projectNode.Parent;

            return projectNode;
        }

        public bool IsDerivedReturnValue(XElement returnValue)
        {
            string typeKey = returnValue.Attribute("TypeKey").Value;
            if (string.IsNullOrEmpty(typeKey))
            {
                if (returnValue.Attribute("IsExternal").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                XElement projectNode = GetProjectNode(returnValue);
                XElement interfaceNode = GetTypeByName(projectNode, returnValue.Attribute("Type").Value as string);
                string id = interfaceNode.Attribute("Key").Value;
                return IsDerived(id);
            }
            else
            {
                XElement interfaceNode = VBGenerator.GetInterfaceOrClassFromKey(typeKey);
                string id = interfaceNode.Attribute("Key").Value;
                return IsDerived(id);
            }
        }

        public bool IsDerived(string id)
        {
            XElement node = (from a in _derived.Element("Document").Elements("Type")
                             where a.Attribute("Key").Value.Equals(id, StringComparison.InvariantCultureIgnoreCase)
                             select a).FirstOrDefault();
            return (node != null);
        }
    }
}
