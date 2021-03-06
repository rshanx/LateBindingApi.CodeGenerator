﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.VB
{
    internal static class SolutionApi
    {
        internal static readonly string _coreLine = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"%Name%Api\",\"%Name%\\%Name%Api.csproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";

        internal static readonly string _projectLine = "Project(\"{F184B08F-C81C-45F6-A57F-5ABD9991F28F}\") = \"%Name%Api\",\"%Name%\\%Name%Api.vbproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";

        internal static readonly string _buildConfig = "\t\t{%Key%}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n"
                                                     + "\t\t{%Key%}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n";

        internal static string ReplaceSolutionAttributes(Settings settings, string solutionFile, XElement solution)
        {
            string projects = "";
            string configs = "";

            if (settings.Framework == "4.0")
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "11.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual Basic Express 2010");
            }
            else
            {
                solutionFile = solutionFile.Replace("%FormatVersion%", "10.00");
                solutionFile = solutionFile.Replace("%VisualStudio%", "Visual Studio 2008");
            }

            if (true == settings.AddTestApp)
            {
                string testProjectLine = "Project(\"{F184B08F-C81C-45F6-A57F-5ABD9991F28F}\") = \"%Name%\",\"%Name%\\%Name%.vbproj\", \"{%Key%}\"\r\n%Depend%EndProject\r\n";
                string newProjectLine = testProjectLine.Replace("%Name%", "ClientApplication");
                newProjectLine = newProjectLine.Replace("%Key%", "C2C0EDB2-88DC-4A32-82C3-6378009D96D7");

                string depends = "\tProjectSection(ProjectDependencies) = postProject\r\n";
                foreach (var item in solution.Element("Projects").Elements("Project"))                
                {
                    string line = "\t\t" + "{%Key%} = {%Key%}" + "\r\n";
                    depends += line.Replace("%Key%", VBGenerator.ValidateGuid(item.Attribute("Key").Value));
                }
                depends += "\tEndProjectSection\r\n";
                newProjectLine = newProjectLine.Replace("%Depend%", depends);
                projects += newProjectLine;

                string customBuildConfig = "\t\t{%Key%}.Debug|x86.ActiveCfg = Debug|x86\r\n" +
                                           "\t\t{%Key%}.Debug|x86.Build.0 = Debug|x86\r\n" +
                                           "\t\t{%Key%}.Release|x86.ActiveCfg = Release|x86\r\n" +
                                           "\t\t{%Key%}.Release|x86.Build.0 = Release|x86\r\n";

                string newConfig = customBuildConfig.Replace("%Key%", "C2C0EDB2-88DC-4A32-82C3-6378009D96D7");
                configs += newConfig; 
            }

            foreach (var project in solution.Element("Projects").Elements("Project"))
            {
                if ("true" == project.Attribute("Ignore").Value)
                    continue;

                string newProjectLine = _projectLine.Replace("%Name%", project.Attribute("Name").Value);
                newProjectLine = newProjectLine.Replace("%Key%", VBGenerator.ValidateGuid(project.Attribute("Key").Value));
                string depends = "";
                if(project.Element("RefProjects").Elements("RefProject").Count() > 0)
                    depends += "\tProjectSection(ProjectDependencies) = postProject\r\n";

                foreach (var item in project.Element("RefProjects").Elements("RefProject"))
                {
                    string projKey = item.Attribute("Key").Value;
                    XElement projNode = (from a in solution.Element("Projects").Elements("Project")
                                        where a.Attribute("Key").Value.Equals(projKey)
                                        select a).FirstOrDefault();
                    string line = "\t\t" + "{%Key%} = {%Key%}" + "\r\n";
                    depends += line.Replace("%Key%", VBGenerator.ValidateGuid(projNode.Attribute("Key").Value));               
                }
                 
                if (project.Element("RefProjects").Elements("RefProject").Count() > 0)
                    depends += "\tEndProjectSection\r\n";

                newProjectLine = newProjectLine.Replace("%Depend%", depends);
                projects += newProjectLine;

                string newConfig = _buildConfig.Replace("%Key%", VBGenerator.ValidateGuid(project.Attribute("Key").Value));
                configs += newConfig; 
            }

            string newProjectLine2 = _coreLine.Replace("Api", "").Replace("%Name%", "NetOffice").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F").Replace("%Depend%", "");
            projects += newProjectLine2;

            string newConfig2 = _buildConfig.Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
            configs += newConfig2;

            solutionFile = solutionFile.Replace("%Projects%", projects);
            solutionFile = solutionFile.Replace("%Config%", configs);
            return solutionFile;
        }

        internal static void SaveSolutionFile(Settings settings, string path, string solutionFile, XElement solution)
        {
            if (true == settings.AddTestApp)
                SaveTestClient(settings, solution, path);

            string solutionName = solution.Attribute("Name").Value;
            PathApi.CreateFolder(path);
            string solutionFilePath = System.IO.Path.Combine(path, solutionName + ".sln");
            System.IO.File.WriteAllText(solutionFilePath, solutionFile, Encoding.UTF8);
        }

        internal static void SaveApiProject(Settings settings, string projectApiPath, string path)
        {
            path = System.IO.Path.Combine(path, "NetOffice");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string[] files = System.IO.Directory.GetFiles(projectApiPath);
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path, fileName);
                System.IO.File.Copy(file, newFilePath);
            }

            if (!System.IO.Directory.Exists(path + "\\Interfaces"))
                System.IO.Directory.CreateDirectory(path + "\\Interfaces");
            files = System.IO.Directory.GetFiles(projectApiPath + "\\Interfaces");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path + "\\Interfaces", fileName);
                System.IO.File.Copy(file, newFilePath);
            }

            if (!System.IO.Directory.Exists(path + "\\Attributes"))
                System.IO.Directory.CreateDirectory(path + "\\Attributes");
            files = System.IO.Directory.GetFiles(projectApiPath + "\\Attributes");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path + "\\Attributes", fileName);
                System.IO.File.Copy(file, newFilePath);
            }


            if (!System.IO.Directory.Exists(path + "\\Properties"))
                System.IO.Directory.CreateDirectory(path + "\\Properties");
            files = System.IO.Directory.GetFiles(projectApiPath + "\\Properties");
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string newFilePath = System.IO.Path.Combine(path + "\\Properties", fileName);
                System.IO.File.Copy(file, newFilePath);
            }
        }

        internal static void SaveApiBinary(Settings settings, string path)
        {
            PathApi.CreateFolder(path);
            string binrayFilePath = System.IO.Path.Combine(path, "NetOffice.dll");
            byte[] ressourceDll = RessourceApi.ReadBinaryFromResource("Api.NetOffice" + "_v" + settings.Framework + ".dll");
            RessourceApi.WriteBinaryToFile(ressourceDll, binrayFilePath);
        }

        internal static void SaveTestClient(Settings settings, XElement solution, string path)
        {
            string appConfigFile = RessourceApi.ReadString("TestClient.App.config");
            string appInfoFile = RessourceApi.ReadString("TestClient.AssemblyInfo.vb");
            string projectFile  = RessourceApi.ReadString("TestClient.ClientApplication.vbproj");
            string formDesignerFile = RessourceApi.ReadString("TestClient.Form1.Designer.vb");
            string formFile = RessourceApi.ReadString("TestClient.Form1.vb");

            string projectRef = "    <ProjectReference Include=\"..\\%Name%\\%Name%Api.vbproj\">\r\n"
                                                   + "      <Project>{%Key%}</Project>\r\n"
                                                   + "      <Name>%Name%Api</Name>\r\n"
                                                   + "    </ProjectReference>\r\n";

            string formUsings = "";
            string projectInclude = "";
            foreach (var item in solution.Element("Projects").Elements("Project"))
            {
                if ("true" == item.Attribute("Ignore").Value)
                    continue;

                string newRefProject = projectRef.Replace("%Key%", VBGenerator.ValidateGuid(item.Attribute("Key").Value));
                newRefProject = newRefProject.Replace("%Name%", item.Attribute("Name").Value);
                projectInclude += newRefProject;

                string newUsing = "Imports " + item.Attribute("Name").Value + " = " + item.Attribute("Namespace").Value + "\r\n";
                formUsings += newUsing;
            }

            string newRefProject2 = projectRef.Replace("Api", "").Replace("%Key%", "65442327-D01F-4ECB-8C39-6D5C7622A80F");
            newRefProject2 = newRefProject2.Replace("%Name%", "NetOffice");
            projectInclude += newRefProject2;
            projectFile = projectFile.Replace("%ApiRefDll%", "");

            formFile = formFile.Replace("Imports xyz", formUsings);

            string refProjectInclude = "  <ItemGroup>\r\n" + projectInclude + "  </ItemGroup>";
            projectFile = projectFile.Replace("%ProjectRefInclude%", refProjectInclude);

            if ("4.0" == settings.Framework)
                projectFile = projectFile.Replace("%ToolsVersion%", "4.0");
            else
                projectFile = projectFile.Replace("%ToolsVersion%", "3.5");

            projectFile = projectFile.Replace("%Framework%", "v" + settings.Framework);

            string projectPath = System.IO.Path.Combine(path, "ClientApplication");
            PathApi.CreateFolder(projectPath);

            string appConfigFilePath = System.IO.Path.Combine(projectPath, "App.config");
            System.IO.File.WriteAllText(appConfigFilePath, appConfigFile, Encoding.UTF8);

            string appInfoFilePath = System.IO.Path.Combine(projectPath, "AssemblyInfo.vb");
            System.IO.File.WriteAllText(appInfoFilePath, appInfoFile, Encoding.UTF8);

            string projectFilePath = System.IO.Path.Combine(projectPath, "ClientApplication.vbproj");
            System.IO.File.WriteAllText(projectFilePath, projectFile, Encoding.UTF8);

            string formDesignerFilePath = System.IO.Path.Combine(projectPath, "Form1.Designer.vb");
            System.IO.File.WriteAllText(formDesignerFilePath, formDesignerFile, Encoding.UTF8);

            string formFilePath = System.IO.Path.Combine(projectPath, "Form1.vb");
            System.IO.File.WriteAllText(formFilePath, formFile, Encoding.UTF8);
        }
    }
}
