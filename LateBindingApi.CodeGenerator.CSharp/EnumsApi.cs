﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class EnumsApi
    {
        private static string _fileHeader = ""
                                            + "using System;\r\n"
                                            + "using NetOffice;\r\n"
                                            + "namespace %namespace%\r\n"
                                            + "{\r\n";

        internal static string ConvertEnumsToFiles(XElement projectNode, XElement enumsNode, Settings settings, string solutionFolder)
        {
            string enumFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            enumFolder = System.IO.Path.Combine(enumFolder, "Enums");
            if (false == System.IO.Directory.Exists(enumFolder))
                System.IO.Directory.CreateDirectory(enumFolder);
            
            string result = "";
            foreach (XElement enumNode in enumsNode.Elements("Enum"))
                result += ConvertEnumToFile(settings, projectNode, enumNode, enumFolder) + "\r\n";

            return result;
        }

        private static string ConvertEnumToFile(Settings settings, XElement projectNode, XElement enumNode, string enumFolder)
        {
            string fileName = System.IO.Path.Combine(enumFolder, enumNode.Attribute("Name").Value + ".cs");

            string newEnum = ConvertEnumToString(settings, projectNode, enumNode);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = enumFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + enumFolder.Substring(i + 1) + "\\" + enumNode.Attribute("Name").Value + ".cs" + "\" />";
            return result;
        }

        private static string ConvertEnumToString(Settings settings, XElement projectNode, XElement enumNode)
        {
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value + ".Enums");
            string enumAttributes = CSharpGenerator.GetSupportByVersionAttribute(enumNode);

            string name = enumNode.Attribute("Name").Value;

            if(true == settings.CreateXmlDocumentation)
                result += CSharpGenerator.GetSupportByVersionSummary("\t", enumNode);
             
            string between2 = "";
            if (CSharpGenerator.Settings.AddDocumentationLinks)
            {
                string projectName = projectNode.Attribute("Name").Value;
                if (null != projectName && CSharpGenerator.IsRootProjectName(projectName))
                {
                    XElement linkNode = (from a in CSharpGenerator.LinkFileDocument.Element("NOBuildTools.ReferenceAnalyzer").Element(projectName).Element("Enums").Elements("Enum")
                                         where a.Element("Name").Value.Equals(enumNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase)
                                         select a).FirstOrDefault();
                    if (null != linkNode)
                        between2 = "\t" + " ///<remarks> MSDN Online Documentation: " + linkNode.Element("Link").Value + " </remarks>\r\n";
                }
            }

            result += between2;
            result += "\t" + enumAttributes + Environment.NewLine;
            result += "\t[EntityTypeAttribute(EntityType.IsEnum)]\r\n" + "\tpublic enum " + name + Environment.NewLine + "\t{" + Environment.NewLine;
            
            int countOfMembers =  enumNode.Element("Members").Elements("Member").Count();
            int i = 1;
            foreach (var itemMember in enumNode.Element("Members").Elements("Member"))
            {
                string memberAttribute = CSharpGenerator.GetSupportByVersionAttribute(itemMember);
                string memberName = itemMember.Attribute("Name").Value;
                string memberValue = itemMember.Attribute("Value").Value;

                if (true == settings.CreateXmlDocumentation)
                { 
                    result += CSharpGenerator.GetSupportByVersionSummary("\t\t", itemMember);
                    result += "\t\t /// <remarks>" + memberValue + "</remarks>\r\n";
                }

                result += "\t\t " + memberAttribute + "\r\n";
                result += "\t\t " + memberName + " = " + memberValue;
                
                if(i<countOfMembers)
                    result += ",\r\n\r\n";
                else
                    result += "\r\n";
                
                i++;
            }
            
            result += "\t}" + Environment.NewLine;
            result += "}";
            return result;
        }
    }
}
