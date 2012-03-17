﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ModuleApi
    {
        private static string _fileHeader = "//Generated by LateBindingApi.CodeGenerator\r\n"
                                     + "using System;\r\n"
                                     + "using LateBindingApi.Core;\r\n"
                                     + "namespace %namespace%\r\n"
                                     + "{\r\n";

        private static string _classDesc = "\t///<summary>\r\n\t/// Module %name%\r\n\t///</summary>\r\n";

        private static string _classHeader = "\t[EntityTypeAttribute(EntityType.IsModule)]\r\n" + "\tpublic class %name% : COMObject\r\n\t{";

        internal static string ConvertModulesToFiles(XElement projectNode, XElement facesNode, Settings settings, string solutionFolder)
        {
            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Modules");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in facesNode.Elements("Module"))
                result += ConvertModuleToFile(settings, projectNode, faceNode, faceFolder) + "\r\n";
            
            return result;
        }

        private static string ConvertModuleToFile(Settings settings, XElement projectNode, XElement faceNode, string faceFolder)
        {
            string fileName = System.IO.Path.Combine(faceFolder, faceNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertModuleToString(settings, projectNode, faceNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + faceNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }
        
        private static string ConvertModuleToString(Settings settings, XElement projectNode, XElement moduleNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string attributes = "\t" + CSharpGenerator.GetSupportByVersionAttribute(moduleNode);
            string header = _classHeader.Replace("%name%", moduleNode.Attribute("Name").Value);
            string classDesc = _classDesc.Replace("%name%", moduleNode.Attribute("Name").Value);
            string methods = MethodApi.ConvertMethodsLateBindToString(settings, moduleNode.Element("Methods"));

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += methods;
            result += "\t}\r\n}";
            return result;
        }
    }
}
