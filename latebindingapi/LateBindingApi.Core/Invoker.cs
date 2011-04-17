﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace LateBindingApi.Core
{
    /// <summary>
    /// invoke helper functions
    /// </summary>
    public static class Invoker
    { 
        #region Method

        public static void Method(COMObject comObject, string name)
        {
            Method(comObject, name, null);
        }

        public static void Method(COMObject comObject, string name, object[] paramsArray)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
        }

        public static void Method(COMObject comObject, string name, object[] paramsArray, ParameterModifier[] paramModifiers)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, paramModifiers, Settings.ThreadCulture, null);
        }

        public static object MethodReturn(COMObject comObject, string name)
        {
            return MethodReturn(comObject, name);
        }

        public static object MethodReturn(COMObject comObject, string name, object[] paramsArray)
        {
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static object MethodReturn(COMObject comObject, string name, object[] paramsArray, ParameterModifier[] paramModifiers)
        {
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.InvokeMethod, null, comObject.UnderlyingObject, paramsArray, paramModifiers, Settings.ThreadCulture, null);
            return returnValue;
        }

        #endregion

        #region Property

        public static object PropertyGet(COMObject comObject, string name)
        { 
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, null, Settings.ThreadCulture);
            return returnValue;
        }

        public static object PropertyGet(COMObject comObject, string name, object[] paramsArray)
        {
            object returnValue = comObject.InstanceType.InvokeMember(name, BindingFlags.GetProperty, null, comObject.UnderlyingObject, paramsArray, Settings.ThreadCulture);
            return returnValue;
        }

        public static void PropertySet(COMObject comObject, string name, object value)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[]{value}, Settings.ThreadCulture);
        }

        public static void PropertySet(COMObject comObject, string name, object value, ParameterModifier[] paramModifiers)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, new object[] { value }, paramModifiers, Settings.ThreadCulture, null);
        }

        public static void PropertySet(COMObject comObject, string name, object[] value, ParameterModifier[] paramModifiers)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, value, paramModifiers, Settings.ThreadCulture, null);
        }

        public static void PropertySet(COMObject comObject, string name, object[] value)
        {
            comObject.InstanceType.InvokeMember(name, BindingFlags.SetProperty, null, comObject.UnderlyingObject, value, Settings.ThreadCulture);
        }

        #endregion

        #region Parameters

        public static ParameterModifier[] CreateParamModifiers(params bool[] isRef)
        {
            if (null != isRef)
            {
                int parramArrayCount = isRef.Length;
                ParameterModifier newModifiers = new ParameterModifier(parramArrayCount);
                
                for (int i = 0; i < parramArrayCount; i++)
                    newModifiers[i] = isRef[i];

                ParameterModifier[] returnModifiers = { newModifiers };
                return returnModifiers;
            }
            else
                return null;
        }

        public static object ValidateParam(object param)
        {
            if (null != param)
            {
                IObject comObject = param as IObject;
                if (null != comObject)
                        param = comObject.UnderlyingObject;

                return param;
            }
            else
                return null;
        }

        public static object[] ValidateParamsArray(params object[] paramsArray)
        {
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i<parramArrayCount; i++)
                    paramsArray[i] = ValidateParam(paramsArray[i]);
                return paramsArray;
            }
            else
                return null;
        }
     
        public static void ReleaseParam(object param)
        {
            if (null != param)
            {
                if (param is COMObject)
                {
                    COMObject comObject = param as COMObject;
                    comObject.Dispose();
                }
                else if (param is COMVariant)
                {
                    COMVariant comVariant = param as COMVariant;
                    comVariant.Dispose();
                }
                else
                {
                    Type paramType = param.GetType();
                    if (true == paramType.IsCOMObject)
                        Marshal.ReleaseComObject(param);
                }
            }
        }

        public static void ReleaseParamsArray(object[] paramsArray)
        {
            if (null != paramsArray)
            {
                int parramArrayCount = paramsArray.Length;
                for (int i = 0; i < parramArrayCount; i++)
                    ReleaseParam(paramsArray[i]);
            }
        }

        #endregion
    }
}
