﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    
    internal static class CoClassApi
    {
        private static string _fileHeader = "''Generated by LateBindingApi.CodeGenerator\r\n"
                                      + "Imports System\r\n"
                                      + "Imports NetRuntimeSystem = System\r\n"
                                      + "Imports System.ComponentModel\r\n"
                                      + "Imports LateBindingApi.Core\r\n"
                                      + "Namespace %namespace%\r\n"
                                      + "\r\n";

        private static string _disposeOverride = "\r\n\t\t''' <summary>\r\n" +
                                "\t\t''' NetOffice method: dispose instance and all child instances\r\n" +
                                "\t\t''' </summary>\r\n" +
                                "\t\t''' <param name=\"disposeEventBinding\">dispose event exported proxies with one or more event recipients</param>\r\n" +
                                "\t\tPublic Overrides Sub Dispose(disposeEventBinding As Boolean)\r\n\r\n" +
                                "\t\t\tIf Me.Equals([Globals].Instance) Then\r\n" +
                                "\t\t\t\t [Globals].Instance = Nothing\r\n" +
                                "\t\t\tEnd If\r\n\r\n" +
                                "\t\t\tMyBase.Dispose(disposeEventBinding)\r\n\r\n" +
                                "\t\tEnd Sub\r\n\r\n" +            
                                "\r\n\t\t''' <summary>\r\n" +
                                "\t\t''' NetOffice method: dispose instance and all child instances\r\n" +
                                "\t\t''' </summary>\r\n" +
                                "\t\tPublic Overrides Sub Dispose()\r\n\r\n" +
                                "\t\t\tIf Me.Equals([Globals].Instance) Then\r\n" +
                                "\t\t\t\t [Globals].Instance = Nothing\r\n" +
                                "\t\t\tEnd If\r\n\r\n" +
                                "\t\t\tMyBase.Dispose()\r\n\r\n" +
                                "\t\tEnd Sub\r\n";

        private static string _delegates = "#Region \"Delegates\"\r\n\r\n" +
                                            "%delegates%" +
                                            "\r\n#end Region\r\n\r\n";

        private static string _classDesc = "\t'''<summary>\r\n\t''' CoClass %name% %RefLibs%\r\n\t'''</summary>\r\n";

        private static string _classHeader = "\t<EntityTypeAttribute(EntityType.IsCoClass)> _\r\n" + "\tPublic Class %name% \r\n\t  Inherits %inherited% \r\n%ImplementsEventBinding%\r\n\r\n";

        private static string _classConstructor;
        
        private static string _classEventBinding;

        internal static string ConvertCoClassesToFiles(XElement projectNode, XElement classesNode, Settings settings, string solutionFolder)
        {
            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Classes");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            string result = "";
            foreach (XElement faceNode in classesNode.Elements("CoClass"))
                result += ConvertCoClassToFile(projectNode, faceNode,settings, faceFolder) + "\r\n";

            return result;
        }

        private static string ConvertCoClassToFile(XElement projectNode, XElement classNode, Settings settings, string classFolder)
        {
            string fileName = System.IO.Path.Combine(classFolder, classNode.Attribute("Name").Value + ".vb");

            string newEnum = ConvertCoClassToString(projectNode, classNode, settings);
            System.IO.File.AppendAllText(fileName, newEnum);

            int i = classFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + classFolder.Substring(i + 1) + "\\" + classNode.Attribute("Name").Value + ".vb" + "\" />";
            return result;
        }
         
        private static string ConvertCoClassToString(XElement projectNode, XElement classNode, Settings settings)
        {   
            string result = _fileHeader.Replace("%namespace%", projectNode.Attribute("Namespace").Value);
            string delegates = _delegates.Replace("%delegates%", GetDelegates(projectNode, classNode));
            result += delegates;

            string attributes = "\t" + VBGenerator.GetSupportByVersionAttribute(classNode);
            string header = _classHeader.Replace("%name%", ParameterApi.ValidateNameWithoutVarType(classNode.Attribute("Name").Value));
            header = header.Replace("%inherited%", GetInherited(projectNode, classNode));
            if (ImplentsAnEventInterface(projectNode, classNode))
                header = header.Replace("%ImplementsEventBinding%", "Implements IEventBinding");
            else
                header = header.Replace("%ImplementsEventBinding%", "");

            if (null == _classConstructor)
                _classConstructor = RessourceApi.ReadString("CoClass.Constructor.txt");
            string construct = _classConstructor.Replace("%name%", ParameterApi.ValidateNameWithoutVarType(classNode.Attribute("Name").Value));

            construct = construct.Replace("%ProgId%", projectNode.Attribute("Name").Value + "." + classNode.Attribute("Name").Value);
            if ("Application" == classNode.Attribute("Name").Value)
            {
                construct = construct.Replace("%setGlobalInstance%", "\r\n[Globals].Instance = Me");
                construct = construct.Replace("%disposeGlobalInstance%", _disposeOverride);
            }
            else
            { 
                construct = construct.Replace("%setGlobalInstance%", "");
                construct = construct.Replace("%disposeGlobalInstance%", "");
            }

            if (ImplentsAnEventInterface(projectNode, classNode))
            {
                string sinkHelperDefine = GetSinkHelperDefine(projectNode, classNode);
                construct = construct.Replace("%sinkHelperDefine%", sinkHelperDefine);
            }
            else
                construct = construct.Replace("%sinkHelperDefine%", "");

            string sinkHelperIds = GetSinkHelperIds(projectNode, classNode);
            string sinkHelperSetActive = GetSinkHelperSetActiveSink(projectNode, classNode);

            if (ImplentsAnEventInterface(projectNode, classNode))
            {
                string events = GetEvents(projectNode, classNode, settings);
                construct += events;
            }

            string classDesc = _classDesc.Replace("%name%", classNode.Attribute("Name").Value).Replace("%RefLibs%", "\r\n\t''' " + VBGenerator.GetSupportByVersion("", classNode));

            if (null == _classEventBinding)
                _classEventBinding = RessourceApi.ReadString("CoClass.EventHelper.txt");

            string sinkHelperDispose = GetSinkHelperDispose(projectNode, classNode);

            if (classNode.Attribute("AutomaticQuit").Value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase))
                construct = construct.Replace("%callQuitInDispose%", "_callQuitInDispose = true");
            else
                construct = construct.Replace("%callQuitInDispose%", "");

            result += classDesc;
            result += attributes + "\r\n";
            result += header;
            result += construct;

            if (ImplentsAnEventInterface(projectNode, classNode))
            {
                result += _classEventBinding.Replace("%sinkHelperDispose%", sinkHelperDispose);
                result = result.Replace("%SinkHelperType%", GetSinkHelperType(projectNode, classNode));
                result = result.Replace("%CompareIds%", sinkHelperIds);
                result = result.Replace("%SetActiveSink%", sinkHelperSetActive);
                result = result.Replace("%HandlerCheck%", GetHandlerChecks(projectNode, classNode));
            }

            result += "\tEnd Class\r\n\r\nEnd Namespace";
            return result;
        }

        private static bool ImplentsAnEventInterface(XElement projectNode, XElement faceNode)
        {
            return (faceNode.Element("EventInterfaces").Elements("Ref").Count() > 0);
        }

        private static string GetInherited(XElement projectNode, XElement faceNode)
        {
            if (faceNode.Element("Inherited").Elements("Ref").Count() == 0)
                return "COMObject";

            string retList = "";

            // select last default interface
            XElement refNode = faceNode.Element("DefaultInterfaces").Elements("Ref").Last();
            XElement inInterface = GetItemByKey(projectNode, refNode);
            if (inInterface.Parent.Parent == faceNode.Parent.Parent)
            {
                // same project               
                retList += inInterface.Attribute("Name").Value;
            }
            else
            {
                // extern project
                retList += inInterface.Parent.Parent.Attribute("Namespace").Value + "." + inInterface.Attribute("Name").Value;
            }

            return retList;
        }


        private static string GetSinkHelperType(XElement projectNode, XElement faceNode)
        {
            string result = "";

            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);
                result +=type;
            }
            return result;
        }

        private static string GetSinkHelperDefine(XElement projectNode, XElement faceNode)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                result += "\t\tPrivate " + " " + name + " As " + type + "\r\n";
            }
            return result;
        }

        private static string GetSinkHelperIds(XElement projectNode, XElement faceNode)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper.Id,";
                result += type;
            }

            if ("" != result)
            {
                if ("," == result.Substring(result.Length - ",".Length))
                    result = result.Substring(0, result.Length - ",".Length);
            }
            else
                result = "null";

            return result;
        }

        private static string GetSinkHelperSetActiveSink(XElement projectNode, XElement faceNode)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);

                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                string ifLine = "\r\n\t\t\t" + "If(" + type + ".Id.Equals(_activeSinkId, StringComparison.InvariantCultureIgnoreCase))\r\n";
                ifLine += "\t\t\t\t" + name + " = new " + type + "(Me, _connectPoint)\r\n";
                ifLine += "\t\t\t\treturn\r\n";
                ifLine += "\t\t\tEnd If\r\n";

                result += ifLine;
            }

            if ("" != result)
            {
                if ("\r\n" == result.Substring(result.Length - "\r\n".Length))
                    result = result.Substring(0, result.Length - "\r\n".Length);
            }

            return result;
        }

        private static string GetSinkHelperDispose(XElement projectNode, XElement faceNode)
        {
            string result = "";
            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value + "_SinkHelper";
                string name = "_" + type.Substring(0, 1).ToLower() + type.Substring(1);

                result += "\t\t\t" + "if(Not LateBindingApi.Core.Utils.IsNothing(" + name + "))\r\n";
                result += "\t\t\t\t" + name + ".Dispose()\r\n";
                result += "\t\t\t\t" + name + " = Nothing\r\n";
                result += "\t\t\t" + "End If\r\n";
            }
            return result;
        }

        private static string GetHandlerChecks(XElement projectNode, XElement faceNode)
        {
            List<string> methodNames = new List<string>();
            foreach (var refFace in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, refFace);
                foreach (var itemMethod in inInterface.Element("Methods").Elements("Method"))
                {
                    bool found = false;
                    foreach (string item in methodNames)
                    {
                        if (item == itemMethod.Attribute("Name").Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (false == found)
                    {
                        methodNames.Add("\t\t\tIf (_" + itemMethod.Attribute("Name").Value + "EventHandlers.Count > 0) Then Return True\r\n");
                    }
                }
            }

            string result = "";
            foreach (string item in methodNames.ToArray())
                result += item;
            return result;
        }

        private static string GetDelegates(XElement projectNode, XElement faceNode)
        {
            List<string> methodNames = new List<string>();
            string delegateResult ="";
            foreach (var refFace in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, refFace);
                foreach (var itemMethod in inInterface.Element("Methods").Elements("Method"))
                {
                    bool found = false;
                    foreach (string item in methodNames)
                    {
                        if (item == itemMethod.Attribute("Name").Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (false == found)
                    { 
                        delegateResult += "\t" + GetDelegateSignatur(faceNode.Attribute("Name").Value, itemMethod) + "\r\n";
                        methodNames.Add(itemMethod.Attribute("Name").Value);
                    }
                }
            }  
            return delegateResult;
        }

        private static string GetDelegateSignatur(string className, XElement methodNode)
        {
            string result = "Public Delegate Sub " + className + "_" + methodNode.Attribute("Name").Value + "EventHandler" + "(";
            foreach (XElement itemParam in methodNode.Element("Parameters").Elements("Parameter"))
            {
                string isRef = "ByVal ";
                if ("true" == itemParam.Attribute("IsRef").Value)
                    isRef = "ByRef ";

                string par = "";
                if (("true" == itemParam.Attribute("IsComProxy").Value) && ("object" == itemParam.Attribute("Type").Value))
                {
                    string typeName = VBGenerator.GetQualifiedType(itemParam);
                    if (typeName.Equals("object", StringComparison.InvariantCultureIgnoreCase))
                        typeName = "COMObject";
                    par = isRef + itemParam.Attribute("Name").Value + " As " + typeName; // +"Object";
                }
                else
                {
                    par = isRef + itemParam.Attribute("Name").Value + " As " + VBGenerator.GetQualifiedType(itemParam);
                }

                result += par + ", ";
            }
            if (", " == result.Substring(result.Length - 2))
                result = result.Substring(0, result.Length - 2);

            return result + ")";
        }

        private static string GetImplementEventSignaturParamNames(Settings settings, XElement projectNode, XElement faceNode, string eventName)
        {
            string result = "";
            XElement eventInterface = GetImplementEventInterface(projectNode, faceNode);

            XElement methodNode = (from a in eventInterface.Element("Methods").Elements("Method")
                                   where a.Attribute("Name").Value.Equals(eventName)
                                   select a).FirstOrDefault();

            foreach (XElement item in methodNode.Element("Parameters").Elements("Parameter"))
	        {
                string parName = ParameterApi.ValidateParamName(settings, (item.Attribute("Name").Value));

                if (parName.Equals(methodNode.Attribute("Name").Value, StringComparison.InvariantCultureIgnoreCase))
                    parName += "_";

                result += parName + ",";
	        }
            if (result.Length > 0)
                result = result.Substring(0, result.Length - 1);
            return result;
        }

        private static string GetImplementEventSignatur(Settings settings, XElement projectNode, XElement faceNode, string eventName)
        {
            XElement eventInterface = GetImplementEventInterface(projectNode, faceNode);

            XElement methodNode = (from a in eventInterface.Element("Methods").Elements("Method")
                                   where a.Attribute("Name").Value.Equals(eventName)
                                    select a).FirstOrDefault();
            return ParameterApi.CreateParametersPrototypeString(settings, methodNode.Element("Parameters"), true, false, false);
        }

        private static XElement GetImplementEventInterface(XElement projectNode, XElement faceNode)
        {
            XElement element = faceNode.Element("EventInterfaces").Elements("Ref").Last();
            XElement inInterface = GetItemByKey(projectNode, element);
            return inInterface;
        }

        private static XDocument GetImplementEvents(XElement projectNode, XElement faceNode)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("Methods"));

            foreach (var item in faceNode.Element("EventInterfaces").Elements("Ref"))
            {
                XElement inInterface = GetItemByKey(projectNode, item);
                string type = inInterface.Attribute("Name").Value;
                foreach (var itemMethod in inInterface.Elements("Methods").Elements("Method"))
                {
                    string methodName = itemMethod.Attribute("Name").Value;

                    var methodNode = (from a in doc.Element("Methods").Elements("Method")
                                      where a.Attribute("Name").Value.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)
                                      select a).FirstOrDefault();
                    if (null == methodNode)
                    {
                        methodNode = new XElement("Method", new XAttribute("Name", methodName));
                        doc.Element("Methods").Add(methodNode);
                    }

                    string[] versions = VBGenerator.GetSupportByVersionArray(itemMethod);
                    foreach (string version in versions)
                    {
                        XElement attribute = (from a in methodNode.Elements("Version")
                                              where a.Attribute("Name").Value.Equals(version, StringComparison.InvariantCultureIgnoreCase)
                                              select a).FirstOrDefault();
                        if (null == attribute)
                        {
                            attribute = new XElement("Version", new XAttribute("Name", version));
                            methodNode.Add(attribute);
                        }
                    }

                }

            }

            return doc;
        }

        private static string GetEvents(XElement projectNode, XElement faceNode, Settings settings)
        {
            string result = "\r\n#Region \"Events\"\r\n\r\n";
            XDocument doc = GetImplementEvents(projectNode, faceNode);
            string line = "";
            foreach (var itemNode in doc.Element("Methods").Elements("Method"))
            {
                string versionAttributeString = "";
                foreach (var itemAttribute in itemNode.Elements("Version"))
                {
                    versionAttributeString += "\"" + itemAttribute.Attribute("Name").Value + "\"";
                    versionAttributeString += ",";
                }
                versionAttributeString = versionAttributeString.Substring(0, versionAttributeString.Length - 1);

                line += "\t\t''' <summary>\r\n" + "\t\t''' SupportByVersion " + projectNode.Attribute("Name").Value + ", " + versionAttributeString.Replace("\"", "") + "\r\n" + "\t\t''' </summary>\r\n";
                line += "\t\tPrivate _" + itemNode.Attribute("Name").Value + "EventHandlers As New ArrayList()" + "\r\n\r\n";

                line += "\t\t''' <summary>\r\n" + "\t\t''' SupportByVersion " + projectNode.Attribute("Name").Value + " " + versionAttributeString.Replace(",", " ").Replace("\"", "") +
                    "\r\n" + "\t\t''' </summary>\r\n";  

                line += "\t\t<SupportByVersion(" + "\"" + projectNode.Attribute("Name").Value + "\"" + ", " + versionAttributeString.Replace("\"", "") + ")> _\r\n";

                string methodSignatur = GetImplementEventSignatur(settings, projectNode, faceNode, itemNode.Attribute("Name").Value);

                line += "\t\tPublic Custom Event " + itemNode.Attribute("Name").Value + "Event" + " As " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value + "EventHandler\r\n\r\n";

                line += "\t\t\tAddHandler(ByVal value As " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value + "EventHandler)\r\n\r\n";
                line += "\t\t\t\tCreateEventBridge()\r\n";
                line += "\t\t\t\t_" + itemNode.Attribute("Name").Value + "EventHandlers.Add(value)\r\n\r\n";
                line += "\t\t\tEnd AddHandler\r\n\r\n";


                line += "\t\t\tRemoveHandler(ByVal value As " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value + "EventHandler)\r\n\r\n";
                line += "\t\t\t\t_" + itemNode.Attribute("Name").Value + "EventHandlers.Remove(value)\r\n\r\n";
                line += "\t\t\tEnd RemoveHandler\r\n\r\n";

                line += "\t\t\tRaiseEvent(" + methodSignatur + ")\r\n\r\n";
                line += "\t\t\t\tFor Each handler As " + faceNode.Attribute("Name").Value + "_" + itemNode.Attribute("Name").Value + "EventHandler" + " In " + "_" + itemNode.Attribute("Name").Value + "EventHandlers" + "\r\n";
                line += "\t\t\t\t\thandler.Invoke(" + GetImplementEventSignaturParamNames(settings, projectNode, faceNode, itemNode.Attribute("Name").Value) + ")\r\n";
                line += "\t\t\t\tNext\r\n\r\n";
                line += "\t\t\tEnd RaiseEvent\r\n\r\n";
                 

                line += "\t\tEnd Event\r\n\r\n";
            }
            
            result += line;
            result += "#End Region\r\n";
            return result;
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
