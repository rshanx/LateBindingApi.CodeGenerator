﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public class Settings
    {
        #region Fields

        string _folder;
        bool   _convertOptionalsToObject;
        bool   _convertParamNamesToCamelCase;
        bool   _removeRefAttribute;
        bool   _createDocu;
        string _framework;
        bool   _addTestApp;
        bool   _openFolder;
        bool  _useSigning;
        bool _addDocumentationLinks = true;
        string _linkFilePath;
        string _signPath;

        #endregion

        #region Properties

        public bool VBOptimization{get;internal set;}

        public string SignPath
        {
            get
            {
                return _signPath;
            }
            internal set
            {
                _signPath = value;
            }
        }

        public string LinkFilePath
        {
            get
            {
                return _linkFilePath;
            }
            internal set
            {
                _linkFilePath = value;
            }

        }
        public bool AddDocumentationLinks
        {
            get
            {
                return _addDocumentationLinks;
            }
            internal set
            {
                _addDocumentationLinks = value;
            }
        }

        public bool UseSigning
        {
            get
            {
                return _useSigning;
            }
            internal set
            {
                _useSigning = value;
            }
        }

        public bool AddTestApp
        {
            get
            {
                return _addTestApp;
            }
            internal set
            {
                _addTestApp = value;
            }
        }

        public bool CreateXmlDocumentation
        {
            get
            {
                return _createDocu;
            }
            internal set
            {
                _createDocu = value;
            }
        }

        public bool ConvertOptionalsToObject
        {
            get
            {
                return _convertOptionalsToObject;
            }
            internal set
            {
                _convertOptionalsToObject = value;
            }
        }

        public bool ConvertParamNamesToCamelCase
        {
            get
            {
                return _convertParamNamesToCamelCase;
            }
            internal set
            {
                _convertParamNamesToCamelCase = value;
            }
        }

        public bool RemoveRefAttribute
        {
            get
            {
                return _removeRefAttribute;
            }
            internal set
            {
                _removeRefAttribute = value;
            }
        }

        public string Framework
        {
            get
            {
                return _framework;
            }
            internal set
            {
                _framework = value;
            }
        }

        public string Folder
        {
            get 
            {
                return _folder;
            }
            internal set
            {
                _folder = value;
            }
        }

        public bool OpenFolder
        {
            get
            {
                return _openFolder;
            }
            internal set
            {
                _openFolder = value;
            }
        }

        #endregion
    }
}
