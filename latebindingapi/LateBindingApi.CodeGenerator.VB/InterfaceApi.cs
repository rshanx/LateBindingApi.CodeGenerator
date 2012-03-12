﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal static class InterfaceApi
    {
        private static string _fileHeader = "'Generated by LateBindingApi.CodeGenerator\r\n"
                                       + "Imports System\r\n"
                                       + "Imports NetRuntimeSystem = System\r\n"
                                       + "Imports System.ComponentModel\r\n"
                                       + "Imports System.Runtime.CompilerServices\r\n"
                                       + "Imports System.Runtime.InteropServices\r\n"
                                       + "Imports System.Reflection\r\n"
                                       + "Imports System.Collections.Generic\r\n"
                                       + "%enumerableSpace%"
                                       + "Imports LateBindingApi.Core\r\n\r\n"
                                       + "Namespace %namespace%\r\n"
                                       + "\r\n";

        private static string _classDesc = "\t'''<summary>\r\n\t''' Interface %name% %RefLibs%\r\n\t'''</summary>\r\n";

        private static string _classHeader = "\t<EntityTypeAttribute(EntityType.IsInterface)> _\r\n" + "\tPublic Class %name% \r\n\t Inherits %inherited%%enumerable%\r\n\t\r\n";

        private static string _classConstructor;

        private static string ConvertInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            if ("true" == faceNode.Attribute("IsEarlyBind").Value)
                return ConvertEarlyBindInterfaceToString(settings, projectNode, faceNode);
            else
                return ConvertLateBindInterfaceToString(settings, projectNode, faceNode);
        }
        
        private static string ConvertEarlyBindInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value).Replace("%enumerableSpace%", "");
            string header = _classDesc.Replace("%name%", ParameterApi.ValidateNameWithoutVarType(faceNode.Attribute("Name").Value)).Replace("%RefLibs%", VBGenerator.GetSupportByLibraryString("", faceNode));

            string version = VBGenerator.GetSupportByLibraryAttribute(faceNode);
            header += "\t" + version + "\r\n";
            string guid = XmlConvert.DecodeName(faceNode.Element("DispIds").Element("DispId").Attribute("Id").Value);
            header += "\t<ComImport, Guid(\"" + guid + "\"), TypeLibType(CShort(" + faceNode.Attribute("TypeLibType").Value + "))> _\r\n";
            header += _classHeader.Replace("%name%", ParameterApi.ValidateNameWithoutVarType(faceNode.Attribute("Name").Value));
            header = header.Replace("Class", "interface").Replace(" : %inherited%", "").Replace("%enumerable%", "");
            result += header;

            string methods = MethodApi.ConvertMethodsEarlyBindToString(settings, faceNode.Element("Methods"));
            result += methods;

            string properties = PropertyApi.ConvertPropertiesEarlyBindToString(settings, faceNode.Element("Properties"));
            result += properties;

            result += "\tEnd Interface\r\nEnd Namespace";
            return result;
        }

        private static XElement GetDefaultItemNode(XElement itemFace)
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

        private static string ConvertLateBindInterfaceToString(Settings settings, XElement projectNode, XElement faceNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string attributes = "\t" + VBGenerator.GetSupportByLibraryAttribute(faceNode);
            XElement defaultItemNode = GetDefaultItemNode(faceNode);
            if (null != defaultItemNode)
                attributes += "\r\n\t<DefaultProperty(\"" + defaultItemNode.Attribute("Name").Value + "\")> _";

            if(faceNode.Attribute("IsHidden").Value.Equals("true",StringComparison.InvariantCultureIgnoreCase))
                attributes += "\r\n\t<EditorBrowsable(EditorBrowsableState.Never), Browsable(false)> _";
           
            string header = _classHeader.Replace("%name%", ParameterApi.ValidateName(faceNode.Attribute("Name").Value));
            
            header = header.Replace("%inherited%", GetInherited(projectNode, faceNode));     
            if (null == _classConstructor)
                _classConstructor = RessourceApi.ReadString("Interface.Constructor.txt");
            string construct = _classConstructor.Replace("%name%", ParameterApi.ValidateName(faceNode.Attribute("Name").Value));
            string classDesc = _classDesc.Replace("%name%", faceNode.Attribute("Name").Value).Replace("%RefLibs%", "\r\n\t''' " + VBGenerator.GetSupportByLibrary("", faceNode));

            string properties = PropertyApi.ConvertPropertiesLateBindToString(settings, faceNode.Element("Properties"), "Me");
            string methods = MethodApi.ConvertMethodsLateBindToString(settings, faceNode.Element("Methods"), "Me");

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += construct;
            result += properties;
            result += methods;

            ScanEnumerable(faceNode, ref result);

            result += "\r\n\tEnd Class\r\n\r\nEnd Namespace";
            return result;
        }

        internal static string ConvertInterfacesToFiles(XElement projectNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Interfaces");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in facesNode.Elements("Interface"))
            { 
                if("false" == faceNode.Attribute("IsEventInterface").Value)
                    result += ConvertInterfaceToFile(settings, projectNode, faceNode, faceFolder) + "\r\n";
            }
            return result;
        }

        private static string ConvertInterfaceToFile(Settings settings, XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = System.IO.Path.Combine(faceFolder, faceNode.Attribute("Name").Value + ".vb");

            string newEnum = ConvertInterfaceToString(settings, projectNode, faceNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".vb" + "\" />";
            return result;
        }

        private static void ScanEnumerable(XElement faceNode, ref string content)
        {
            bool hasEnumerator = EnumerableApi.HasEnumerator(faceNode);
            if (true == hasEnumerator)
                EnumerableApi.AddEnumerator(faceNode,ref content);
            else
                EnumerableApi.RemoveEnumeratorMarker(ref content);
        }

        private static string GetInherited(XElement projectNode, XElement faceNode)
        {
            if (faceNode.Element("Inherited").Elements("Ref").Count() == 0)
                return "COMObject";
            
            string retList ="";
            // select last interface
            XElement refNode = faceNode.Element("Inherited").Elements("Ref").Last();
            XElement inInterface = GetItemByKey(projectNode, refNode);
            if (inInterface.Parent.Parent == faceNode.Parent.Parent)
            {
                // same project               
                retList += ParameterApi.ValidateName(inInterface.Attribute("Name").Value);
            }
            else
            {
                // extern project
                retList += inInterface.Parent.Parent.Attribute("Namespace").Value + "." + ParameterApi.ValidateName(inInterface.Attribute("Name").Value);
            }

            return retList;
        }

        private static XElement GetItemByKey(XElement projectNode, XElement refEntity)
        {
            XElement solutionNode = projectNode.Parent.Parent;
            string key = refEntity.Attribute("Key").Value; 
            foreach (var project in solutionNode.Element("Projects").Elements("Project"))
            {
                var target = (from a in project.Element("Interfaces").Elements("Interface")
                              where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                               select a).FirstOrDefault();
                if (null != target)
                    return target;

                target = (from a in project.Element("DispatchInterfaces").Elements("Interface")
                          where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                          select a).FirstOrDefault();
                if (null != target)
                    return target;

                target = (from a in project.Element("CoClasses").Elements("CoClass")
                          where a.Attribute("Key").Value.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                          select a).FirstOrDefault();
                if (null != target)
                    return target;
            }

            throw (new ArgumentException("refEntity not found."));
        }
    }
}
