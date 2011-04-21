﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class  RecordsApi
    {
        private static string _fileHeader = "//Generated by LateBindingApi.CodeGenerator\r\n"
                                               + "using System;\r\n"
                                               + "using LateBindingApi.Core;\r\n"
                                               + "namespace %namespace%\r\n"
                                               + "{\r\n";

        internal static string ConvertRecordsToFiles(XElement projectNode, XElement enumsNode, Settings settings, string solutionFolder)
        {
            string enumFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            enumFolder = System.IO.Path.Combine(enumFolder, "Records");
            if (false == System.IO.Directory.Exists(enumFolder))
                System.IO.Directory.CreateDirectory(enumFolder);

            string result = "";
            foreach (XElement enumNode in enumsNode.Elements("Record"))
                result += ConvertRecordToFile(projectNode, enumNode, enumFolder) + "\r\n";

            return result;
        }

        private static string ConvertRecordToFile(XElement projectNode, XElement enumNode, string enumFolder)
        {
            string fileName = System.IO.Path.Combine(enumFolder, enumNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertRecordToString(projectNode, enumNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = enumFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + enumFolder.Substring(i + 1) + "\\" + enumNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertRecordToString(XElement projectNode, XElement enumNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value );
            string enumAttributes = CSharpGenerator.GetSupportByLibraryAttribute(enumNode);

            string name = enumNode.Attribute("Name").Value;
            result += "\t" + enumAttributes + Environment.NewLine;
            result += "\tpublic struct " + name + Environment.NewLine + "\t{" + Environment.NewLine;

            int countOfMembers = enumNode.Element("Members").Elements("Member").Count();
            int i = 1;
            foreach (var itemMember in enumNode.Element("Members").Elements("Member"))
            {
                string arr = "";
                if ("true" == itemMember.Attribute("IsArray").Value)
                    arr = "[]";

                string memberAttribute = CSharpGenerator.GetSupportByLibraryAttribute(itemMember);               
                string memberType = itemMember.Attribute("Type").Value;
                string memberName = itemMember.Attribute("Name").Value;

                result += "\t\t" + memberAttribute + "\r\n";
                result += "\t\t" + memberType + arr + " " + memberName;

                if (i < countOfMembers)
                    result += ";\r\n\r\n";
                else
                    result += ";\r\n";
               
                i++;
            }

            result += "\t}" + Environment.NewLine;
            result += "}";
            return result;
        }
    }
}
