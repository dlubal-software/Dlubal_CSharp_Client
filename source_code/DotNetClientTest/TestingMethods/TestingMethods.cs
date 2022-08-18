using Dlubal.WS.Common.Tools;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

#if RFEM
using Dlubal.WS.Rfem6.Model;
using ApplicationClient = Dlubal.WS.Rfem6.Application.RfemApplicationClient;
using ModelClient = Dlubal.WS.Rfem6.Model.RfemModelClient;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
using ApplicationClient = Dlubal.WS.Rstab9.Application.RstabApplicationClient;
using ModelClient = Dlubal.WS.Rstab9.Model.RstabModelClient;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        public enum TestResultType
        {
            PASSED = 0,
            FAILED = 1,
            SKIPPED = 2
        }

        public static DataLogger DataLogger
        {
            get
            {
                return Session.DataLogger;
            }
        }

        public static ApplicationClient SoapApplicationClient
        {
            get
            {
                return Session.SoapApplicationClient;
            }
        }

        public static ModelClient SoapModelClient
        {
            get
            {
                return Session.SoapModelClient;
            }
        }

        // Shared objects and parameters of tests should be placed here.
        private const string MATERIAL_STEEL = "S235"; // (EN 10025-2:2004-11) - zatim bez pouziti normy, aplikace s tim ma problemy
        private const string SECTION_I = "IPE 100";
        private const string SECTION_RECTANGLE = "R_M1 200/300"; // Rectangle 200/300

        //private static string materialTimber = "Hardwood Timber D30";

        public static void ApplicationWsExceptionHandler(Exception exception)
        {
            Session.ApplicationWsExceptionHandler(exception);
        }

        public static void ModelWsExceptionHandler(Exception exception)
        {
            Session.ModelWsExceptionHandler(exception);
        }

        public static string GetDataPath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Properties.Settings.Default.DataFolder);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static double DegToRad(double angle)
        {
            return angle / 180 * Math.PI;
        }

        public static bool InitializeTest()
        {
            if (!Test_General_Delete⁀All() || SoapModelClient == null)
            {
                DataLogger.AddLogEnd(DataLogger.LogResultType.FAILED);
                return false;
            }
            return true;
        }

        private delegate T GetObject<T>(int id);

        /// <summary>
        /// Gets all objects of specified type.
        /// </summary>
        /// <typeparam name="T">Template type of objects to read.</typeparam>
        /// <param name="handler">Delegate to reading function.</param>
        /// <param name="type">Type of object</param>
        /// <returns></returns>
        private static T[] GetAllObjects<T>(ref GetObject<T> handler, object_types type)
        {
            int[] numbers = SoapModelClient.get_all_object_numbers(type, 0);

            DataLogger.AddText($"Reading {numbers.Length} {typeof(T).Name}s...");

            int i = 0;
            T[] objects = new T[numbers.Length];
            foreach (int no in numbers)
            {
                objects[i++] = handler(no);
            }

            DataLogger.AddText("...done");
            return objects;
        }

        private delegate void SetObject<T>(T obj);

        /// <summary>
        /// Sets all objects to model.
        /// </summary>
        /// <typeparam name="T">Template type of objects to write.</typeparam>
        /// <param name="handler">Delegate to writing function.</param>
        /// <param name="objects">Array of object to write.</param>
        private static void SetAllObjects<T>(ref SetObject<T> handler, T[] objects)
        {
            DataLogger.AddText($"Writing {objects.Length} {typeof(T).Name}s...");

            foreach (T obj in objects)
            {
                handler(obj);
            }

            DataLogger.AddText("...done");
        }

        public static void DumpObject(object obj)
        {
            DataLogger.AddLogStart(obj.GetType().Name);

            PropertyInfo[] infos = obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (!info.Name.EndsWith("Specified", StringComparison.OrdinalIgnoreCase))
                {
                    if (info.PropertyType.BaseType == typeof(Array))
                    {
                        if (info.GetValue(obj) is Array array)
                        {
                            foreach (object item in array)
                            {
                                DumpObject(item);
                            }
                        }
                    }
                    else if (!info.PropertyType.IsValueType &&
                             !info.PropertyType.IsEnum &&
                             info.PropertyType != typeof(string))
                    {
                        DumpObject(info.GetValue(obj));
                    }
                    else
                    {
                        object value = info.GetValue(obj);
                        if (value != null)
                        {
                            DataLogger.AddText($"{info.Name} = {value}");
                        }
                    }
                }
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
        }

        private class PropertiesHelper<T> where T : class, new()
        {
            public (string name, object value)[] Properties { get; set; }

            public static bool IsIntegralButNotULong(Type type)
            {
                return
                    type == typeof(sbyte)
                    || type == typeof(byte)
                    || type == typeof(short)
                    || type == typeof(ushort)
                    || type == typeof(int)
                    || type == typeof(uint)
                    || type == typeof(long)
                    || type == typeof(bool)
                ;
            }

            public void AssertEqual(in object lhs, in object rhs)
            {
                var objectType = lhs.GetType();
                Debug.Assert(objectType == rhs.GetType());
                foreach (var (name, value) in Properties)
                {
                    var propertyInfo = objectType.GetProperty(name);
                    var lproperty = propertyInfo.GetValue(lhs);
                    var rproperty = propertyInfo.GetValue(rhs);
                    var exception = new Exception(
                        $"Properties are not equal (type {lhs.GetType().Name}, property {name}, value {lproperty} != {rproperty}"
                    );
                    var type = lproperty.GetType();
                    if (type.IsEnum || IsIntegralButNotULong(type))
                    {
                        if (lproperty as long? != rproperty as long?)
                        {
                            throw exception;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        if (lproperty as ulong? != rproperty as ulong?)
                        {
                            throw exception;
                        }
                    }
                    else if (type == typeof(float) || type == typeof(double))
                    {
                        if (
                            lproperty as double? != rproperty as double?
                            && ( !double.IsNaN(lproperty as double? ?? 0) && !double.IsNaN(rproperty as double? ?? 0))
                        )
                        {
                            throw exception;
                        }
                    }
                    else if (type == typeof(string))
                    {
                        if (lproperty as string != rproperty as string)
                        {
                            throw exception;
                        }
                    }
                    else if (
                        !value.GetType().IsArray
                        && value.GetType().IsGenericType
                        && value.GetType().GetGenericTypeDefinition() == typeof(PropertiesHelper<>)
                    )
                    {
                        try
                        {
                            value.GetType()
                            .GetMethod(nameof(PropertiesHelper<T>.AssertEqual))
                            .Invoke(value, new object[] { lproperty, rproperty });
                        }
                        catch (Exception invokeException)
                        {
                            throw invokeException.GetBaseException();
                        }
                    }
                    else if (
                        value.GetType().IsArray
                        && value.GetType().GetElementType().IsGenericType
                        && value.GetType().GetElementType().GetGenericTypeDefinition() == typeof(PropertiesHelper<>)
                    )
                    {
                        try
                        {
                            value.GetType().GetElementType()
                            .GetMethod(nameof(PropertiesHelper<T>.AssertArraysEqual))
                            .Invoke(null, new object[] { value, lproperty, rproperty });
                        }
                        catch (Exception invokeException)
                        {
                            throw invokeException.GetBaseException();
                        }
                    }
                }
            }

            public static void AssertArraysEqual(object helpersWrapper, in object lhs, in object rhs)
            {
                Debug.Assert(helpersWrapper.GetType().IsArray);
                Debug.Assert(helpersWrapper.GetType().GetElementType() == typeof(PropertiesHelper<T>));
                var helpers = helpersWrapper as PropertiesHelper<T>[];
                Debug.Assert(lhs.GetType().IsArray && lhs.GetType().GetElementType() == typeof(T));
                Debug.Assert(rhs.GetType().IsArray && rhs.GetType().GetElementType() == typeof(T));
                var lhs_ = lhs as T[];
                var rhs_ = rhs as T[];
                Debug.Assert(lhs_.Length == rhs_.Length);
                for (int i = 0; i != lhs_.Length; ++i)
                {
                    helpers[i].AssertEqual(lhs_[i], rhs_[i]);
                }
            }

            public T CreateObject()
            {
                object obj = new T();
                foreach (var (name, value) in Properties)
                {
                    var propertyInfo = typeof(T).GetProperty(name + "Specified");
                    propertyInfo?.SetValue(obj, true);
                    propertyInfo = typeof(T).GetProperty(name);
                    if (
                        !value.GetType().IsArray
                        && value.GetType().IsGenericType
                        && value.GetType().GetGenericTypeDefinition() == typeof(PropertiesHelper<>)
                    )
                    {
                        propertyInfo.SetValue(
                            obj
                            , value.GetType()
                              .GetMethod(nameof(PropertiesHelper<T>.CreateObject))
                              .Invoke(value, null)
                        );
                    }
                    else if (
                        value.GetType().IsArray
                        && value.GetType().GetElementType().IsGenericType
                        && value.GetType().GetElementType().GetGenericTypeDefinition() == typeof(PropertiesHelper<>)
                    )
                    {
                        propertyInfo.SetValue(
                            obj
                            , value.GetType().GetElementType()
                              .GetMethod(nameof(PropertiesHelper<T>.CreateArray))
                              .Invoke(null, new object[] { value })
                        );
                    }
                    else
                    {
                        propertyInfo.SetValue(obj, value);
                    }
                }
                return obj as T;
            }
            public static T[] CreateArray(object helpersWrapper)
            {
                Debug.Assert(helpersWrapper.GetType().IsArray);
                Debug.Assert(helpersWrapper.GetType().GetElementType() == typeof(PropertiesHelper<T>));
                var helpers = helpersWrapper as PropertiesHelper<T>[];
                Debug.Assert(!(helpers is null));
                var array = new T[helpers.Length];
                for (int i = 0; i != helpers.Length; ++i)
                {
                    array[i] = helpers[i].CreateObject();
                }
                return array;
            }
        }

        private static object Clone(object obj)
        {
            var ret = obj.GetType().GetConstructor(Array.Empty<Type>()).Invoke(null);
            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.PropertyType.IsValueType)
                {
                    property.SetValue(ret, property.GetValue(obj));
                }
                else
                {
                    property.SetValue(ret, Clone(property.GetValue(obj)));
                }
            }
            return ret;
        }

        private class AddonEnabler
        {
            private addon_list_type backup = null;
            public void EnableAddons(in string[] addons)
            {
                if (addons is null || addons.Length == 0)
                {
                    return;
                }
                var statuses = SoapModelClient.get_addon_statuses();
                backup = Clone(statuses) as addon_list_type;
                bool noAddonsToEnable = true;
                foreach (string addon in addons)
                {
                    var exception = new Exception($"'{addon}' is invalid addon name");

                    Action<PropertyInfo, object> processLeaf =
                        (PropertyInfo info, object obj) =>
                        {
                            var val_ = info.GetValue(obj);
                            if (val_.GetType() != typeof(bool))
                            {
                                throw exception;
                            }
                            if (!(bool)val_)
                            {
                                DataLogger.AddText($"Addon to enable: {addon}.");
                                noAddonsToEnable = false;
                                info.SetValue(obj, true);
                            }
                        }
                    ;

                    var path = addon.Split('.');
                    if (path.Length < 1 || path.Length > 2)
                    {
                        throw exception;
                    }
                    string s1 = path[0];
                    string s2 = path.Length == 2 ? path[1] : null;
                    var property = statuses.GetType().GetProperty(s1);
                    if (property is null || s1.EndsWith("Specified"))
                    {
                        throw exception;
                    }
                    if (!(s2 is null))
                    {
                        object val = property.GetValue(statuses);
                        var property_ = val.GetType().GetProperty(s2);
                        if (property is null || s2.EndsWith("Specified"))
                        {
                            throw exception;
                        }
                        processLeaf(property_, val);
                        property.SetValue(statuses, val);
                    }
                    else
                    {
                        processLeaf(property, statuses);
                    }
                }
                if (noAddonsToEnable)
                {
                    backup = null;
                    return;
                }
                DataLogger.AddText("Enabling addons...");
                SoapModelClient.set_addon_statuses(statuses);
            }

            public void RestoreBackup()
            {
                if (backup is null)
                {
                    return;
                }
                DataLogger.AddText("Disabling addons...");
                SoapModelClient.set_addon_statuses(backup);
            }
        }

        private static bool RunSetterGetterTest<T>(
            PropertiesHelper<T> setterTest1Helper
            , PropertiesHelper<T> setterTest2Helper
            , in string[] addonsUsed = null
        ) where T : class, new()
        {
            Debug.Assert(setterTest1Helper.Properties[0].name == "no");
            Debug.Assert(setterTest2Helper.Properties[0].name == "no");
            Debug.Assert(setterTest1Helper.Properties[0].value.GetType() == typeof(int));
            Debug.Assert(setterTest2Helper.Properties[0].value.GetType() == typeof(int));
            Debug.Assert(setterTest1Helper.Properties[0].value as int? != null);
            Debug.Assert(setterTest1Helper.Properties[0].value as int? == setterTest2Helper.Properties[0].value as int?);
            Debug.Assert(setterTest1Helper.Properties[1].name != "parent_no"); // objects with parent are not implemented here

            Type type = typeof(T);
            DataLogger.AddLogStart($"Testing {type.Name}...");
            DataLogger.InitializeProgressBar(0, 3, 0);

            bool succeeded = true;
            T backup = null;
            bool cleanupNeeded = false;

            var getter = SoapModelClient.GetType().GetMethod("get_" + type.Name);
            var setter = SoapModelClient.GetType().GetMethod("set_" + type.Name);

            int id = setterTest1Helper.Properties[0].value as int? ?? -1;
            object_types objectType =
                Enum.Parse(typeof(object_types), "E_OBJECT_TYPE_" + type.Name.ToUpper())
                    as object_types?
                ?? object_types.E_OBJECT_TYPE_ACTION
            ;

            var addonEnabler = new AddonEnabler();

            try
            {
                addonEnabler.EnableAddons(addonsUsed);

                if (Array.Exists(SoapModelClient.get_all_object_numbers(type: objectType, parent_no: 0), id_ => id_ == id))
                {
                    DataLogger.AddText("Make a backup.");
                    backup = getter.Invoke(SoapModelClient, new object[] { id }) as T;
                }
                else
                {
                    DataLogger.AddText("Nothing to backup.");
                    cleanupNeeded = true;
                }

                var value = setterTest1Helper.CreateObject();
                DataLogger.AddText("Setter test #1.");
                setter.Invoke(SoapModelClient, new object[] { value });
                var value_ = getter.Invoke(SoapModelClient, new object[] { id }) as T;
                setterTest1Helper.AssertEqual(value, value_);
                DataLogger.SetProgressBarValue(1);

                value = setterTest2Helper.CreateObject();
                DataLogger.AddText("Setter test #2.");
                setter.Invoke(SoapModelClient, new object[] { value });
                value_ = getter.Invoke(SoapModelClient, new object[] { id }) as T;
                setterTest2Helper.AssertEqual(value, value_);
                DataLogger.SetProgressBarValue(2);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                succeeded = false;
            }
            finally
            {
                if (!(backup is null))
                {
                    try
                    {
                        DataLogger.AddText("Restore backup.");
                        setter.Invoke(SoapModelClient, new object[] { backup });
                    }
                    catch (Exception exception)
                    {
                        DataLogger.AddText($"Failed to restore backup object: {exception.Message}");
                    }
                }
                if (cleanupNeeded)
                {
                    DataLogger.AddText("Delete temporary object.");
                    SoapModelClient.delete_object(type: objectType, no: id, parent_no: 0);
                }
                addonEnabler.RestoreBackup();
                DataLogger.SetProgressBarValue(3);
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        private static bool RunSetterGetterTestForConfig<T>(
            Func<T> getter
            , Action<T> setter
            , PropertiesHelper<T> setterTest1Helper
            , PropertiesHelper<T> setterTest2Helper
        ) where T : class, new()
        {
            Type type = typeof(T);
            DataLogger.AddLogStart($"Testing {type.Name}...");
            DataLogger.InitializeProgressBar(0, 3, 0);

            bool succeeded = true;
            T backup = null;

            try
            {
                DataLogger.AddText("Make a backup.");
                backup = getter();

                var value = setterTest1Helper.CreateObject();
                DataLogger.AddText("Setter test #1.");
                setter(value);
                var value_ = getter();
                setterTest1Helper.AssertEqual(value, value_);
                DataLogger.SetProgressBarValue(1);

                value = setterTest2Helper.CreateObject();
                DataLogger.AddText("Setter test #2.");
                setter(value);
                value_ = getter();
                setterTest2Helper.AssertEqual(value, value_);
                DataLogger.SetProgressBarValue(2);
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                succeeded = false;
            }
            finally
            {
                if (!(backup is null))
                {
                    try
                    {
                        DataLogger.AddText("Restore backup.");
                        setter(backup);
                    }
                    catch (Exception exception)
                    {
                        DataLogger.AddText($"Failed to restore backup config: {exception.Message}");
                    }
                }
                DataLogger.SetProgressBarValue(3);
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        /* *** Example of testing method ***
        /// <summary>
        /// This is example of testing method.
        /// Name of testing method has to start with word "Test" followed by group, subgroups and name of test at the end.
        /// Method is placed into the tree according to group and subgroups attached before the name of test.
        /// Each section of method name is separated by "_". Character \u2040 is used as a "space",
        /// because it is not possible to use real space character within the name of method.
        /// </summary>
        /// <returns>Each testing method must have boolean return value which contains result of test.</returns>
        public static bool Test_Group_Sub\u2040Group_Testing()
        {
            // This opens named section within the log.
            DataLogger.AddLogStart("Testing something...");

            // This initialize ProgressBar parameters (min, max, current).
            DataLogger.InitializeProgressBar(0, 100, 0);

            // SOAP WS calls should be placed within the try block followed by specific catch blocks.
            try
            {
                // Core of testing method should be placed here.
                SoapClient.delete_all();

                // If we want to write some data then we have to call begin_modification.
                SoapClient.begin_modification("caption");

                //...

                // This sets value of ProgressBar.
                DataLogger.SetProgressBarValue(50);

                //...

                // At the end we have to call finish_modification.
                SoapClient.finish_modification();
            }
            catch (Exception exception)
            {
                // This catches TimeoutException, FaultException, CommunicationException and also general Exception.
                WsExceptionHandler(exception);
                return false;
            }
            finally
            {
                // Sometimes we need to perform some operations before exiting.

                // This resets ProgressBar to its default state.
                DataLogger.ResetProgressBar();
            }

            // This closes the last opened named section within the log.
            // It contains one of three results [DONE, FAILED, CANCELED].
            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
        */
    }
}