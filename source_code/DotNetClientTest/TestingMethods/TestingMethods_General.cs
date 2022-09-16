using Dlubal.WS.Common.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

#if RFEM
using Dlubal.WS.Rfem6.Model;
#elif RSTAB
using Dlubal.WS.Rstab9.Model;
#elif RSECTION
using Dlubal.WS.RSection1.Model;
#endif

namespace Dlubal.WS.Clients.DotNetClientTest
{
    public static partial class TestingMethods
    {
        const double DefaultDoubleComparisonTolerance = 0.000000001;

        static List<object_types> TypesWithParent = new List<object_types>()
        {
#if RFEM || RSTAB
            object_types.E_OBJECT_TYPE_DXF_MODEL_OBJECT,
            object_types.E_OBJECT_TYPE_IMPOSED_NODAL_DEFORMATION,
            object_types.E_OBJECT_TYPE_MEMBER_IMPERFECTION,
            object_types.E_OBJECT_TYPE_MEMBER_LOAD,
            object_types.E_OBJECT_TYPE_MEMBER_SET_IMPERFECTION,
            object_types.E_OBJECT_TYPE_MEMBER_SET_LOAD,
            object_types.E_OBJECT_TYPE_NODAL_LOAD,
#endif
#if RFEM
            object_types.E_OBJECT_TYPE_FREE_CIRCULAR_LOAD,
            object_types.E_OBJECT_TYPE_FREE_CONCENTRATED_LOAD,
            object_types.E_OBJECT_TYPE_FREE_LINE_LOAD,
            object_types.E_OBJECT_TYPE_FREE_POLYGON_LOAD,
            object_types.E_OBJECT_TYPE_FREE_RECTANGULAR_LOAD,
            object_types.E_OBJECT_TYPE_IMPOSED_LINE_DEFORMATION,
            object_types.E_OBJECT_TYPE_LINE_LOAD,
            object_types.E_OBJECT_TYPE_LINE_SET_LOAD,
            object_types.E_OBJECT_TYPE_OPENING_LOAD,
            object_types.E_OBJECT_TYPE_SOLID_LOAD,
            object_types.E_OBJECT_TYPE_SOLID_SET_LOAD,
            object_types.E_OBJECT_TYPE_SURFACE_IMPERFECTION,
            object_types.E_OBJECT_TYPE_SURFACE_LOAD,
            object_types.E_OBJECT_TYPE_SURFACE_SET_IMPERFECTION,
            object_types.E_OBJECT_TYPE_SURFACE_SET_LOAD
#endif // RFEM
        };

        public static bool DeleteObjects(object_types objectType, int parentNo, string text)
        {
            DataLogger.AddLogStart($"Deleting {text}...");

            try
            {
                int[] numbers = SoapModelClient.get_all_object_numbers(objectType, parentNo);

                DataLogger.InitializeProgressBar(0, numbers.Length, 0);

                SoapModelClient.begin_modification($"delete {text}");

                for (int i = 0; i < numbers.Length; i++)
                {
                    SoapModelClient.delete_object(objectType, numbers[i], parentNo);

                    DataLogger.SetProgressBarValue(i);
                }

                SoapModelClient.finish_modification();

                numbers = SoapModelClient.get_all_object_numbers(objectType, parentNo);

                if (numbers.Length != 0)
                {
                    if (parentNo > 0)
                    {
                        throw new Exception($"{text} are not empty in LC{parentNo}.");
                    }
                    else
                    {
                        throw new Exception($"{text} are not empty.");
                    }

                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool DeleteAllLoads(object_types objectType, object_types caseType, string text)
        {
            bool succeeded = true;

            int[] caseNumbers = SoapModelClient.get_all_object_numbers(caseType, 0);
            foreach (int caseNo in caseNumbers)
            {
                succeeded &= DeleteObjects(objectType, caseNo, text);
            }

            return succeeded;
        }

        public static bool Test_General_Get\u2040Objects\u2040Information()
        {
            DataLogger.AddLogStart("Getting object information...");

            bool succeeded = true;

            Array types = Enum.GetValues(typeof(object_types));
            DataLogger.InitializeProgressBar(0, types.Length, 0);

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    object_types type = (object_types)types.GetValue(i);

                    DataLogger.AddLogStart($"Getting {type} information...");

                    object_information info = SoapModelClient.get_object_information(type);

                    DataLogger.AddText($"Singular Name = '{info.singular_name}'");
                    DataLogger.AddText($"Plural Name = '{info.plural_name}'");
                    DataLogger.AddText("------------------------------");

                    foreach (attribute attributeInfo in info.attributes)
                    {
                        DataLogger.AddText($"Attribute Id = '{attributeInfo.id}'");
                        DataLogger.AddText($"Name = '{attributeInfo.name}'");
                        DataLogger.AddText($"Symbol = '{attributeInfo.symbol}'");
                        DataLogger.AddText($"Plane Symbol = '{attributeInfo.plane_symbol}'");

                        if (attributeInfo.table_column != null)
                        {
                            DataLogger.AddText($"Column Index = {attributeInfo.table_column.index}");
                            DataLogger.AddText($"Column Text = '{attributeInfo.table_column.header_text}'");
                            DataLogger.AddText($"Column Top Text = '{attributeInfo.table_column.header_top_text}'");
                            DataLogger.AddText($"Column Width = {attributeInfo.table_column.width}");
                            DataLogger.AddText($"Column Merged Cells = {attributeInfo.table_column.header_merged_cells}");
                            DataLogger.AddText($"Column H Alignment = {attributeInfo.table_column.horizontal_alignment}");
                            DataLogger.AddText($"Column V Alignment = {attributeInfo.table_column.vertical_alignment}");
                        }
                        DataLogger.AddText("------------------------------");
                    }

                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    DataLogger.SetProgressBarValue(i);
                }
                catch (Exception exception)
                {
                    succeeded &= false;
                    ModelWsExceptionHandler(exception);
                }
            }

            DataLogger.ResetProgressBar();
            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        public static bool Test_General_Copy\u2040And\u2040Verify\u2040All\u2040Objects()
        {
            bool succeeded = false;
            DataLogger.AddLogStart("Copy and verify all objects...");
            DataLogger.InitializeProgressBar(0, 50, 0);

            try
            {
                DataLogger.AddLogStart("Getting all objects...");
                List<WsObjectClass> readData = GetObjects();
                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                DataLogger.SetProgressBarValue(10);

                DataLogger.AddText("Deleting all objects");
                SoapModelClient.delete_all();

                DataLogger.SetProgressBarValue(20);

                DataLogger.AddLogStart("Setting all objects...");
                SetObjects(readData);
                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                DataLogger.SetProgressBarValue(30);

                DataLogger.AddLogStart("Getting all objects again...");
                List<WsObjectClass> dataToCompare = GetObjects();
                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                DataLogger.SetProgressBarValue(40);

                DataLogger.AddLogStart("Comparing all objects...");
                succeeded = AreListsEqual(readData, dataToCompare);
                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);

                DataLogger.SetProgressBarValue(50);
            }
            catch (FaultException ex)
            {
                DataLogger.ReportError(ex, true);
                Console.WriteLine(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
            catch (TimeoutException ex)
            {
                ModelWsExceptionHandler(ex);
            }
            catch (CommunicationException ex)
            {
                ModelWsExceptionHandler(ex);
            }
            catch (Exception ex)
            {
                DataLogger.ReportError(ex, true);
                Console.WriteLine(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            return succeeded;
        }

        public static bool Test_General_Delete\u2040All()
        {
            DataLogger.AddLogStart("Deleting all data...");
            DataLogger.InitializeProgressBar(0, 100, 50);

            try
            {
                SoapModelClient.reset();
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

#if RFEM || RSTAB
        public static bool Test_General_Get\u2040First\u2040Free\u2040UserId()
        {
            bool succeeded = true;
            int loadNumber = 0;
            int imperfectionNumber = 0;
            Array types = Enum.GetValues(typeof(object_types));
            try
            {
                if (SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_LOAD_CASE, 0) == 0)
                {
                    load_case loadCase = new load_case
                    {
                        no = 1,
                        name = "Testing Load Case"
                    };
                    SoapModelClient.set_load_case(loadCase);
                }
                loadNumber = SoapModelClient.get_nth_object_number(object_types.E_OBJECT_TYPE_LOAD_CASE, 0, 0);

                if (SoapModelClient.get_object_count(object_types.E_OBJECT_TYPE_IMPERFECTION_CASE, 0) == 0)
                {
                    imperfection_case imperfectionCase = new imperfection_case
                    {
                        no = 1,
                        name = "Testing Imperfection Case"
                    };
                    SoapModelClient.set_imperfection_case(imperfectionCase);
                }
                imperfectionNumber = SoapModelClient.get_nth_object_number(object_types.E_OBJECT_TYPE_IMPERFECTION_CASE, 0, 0);

                DataLogger.AddLogStart("Getting first free numbers for each type...");
                DataLogger.InitializeProgressBar(0, types.Length, 0);

                for (int i = 0; i < types.Length; i++)
                {
                    object_types type = (object_types)types.GetValue(i);
                    int firstFreeNumberForType;

                    DataLogger.AddLogStart($"Getting for type {type}...");
                    if (type.ToString().EndsWith("_IMPERFECTION", StringComparison.OrdinalIgnoreCase))
                    {
                        firstFreeNumberForType = SoapModelClient.get_first_free_number(type, imperfectionNumber);
                    }
                    else
                    {
                        firstFreeNumberForType = SoapModelClient.get_first_free_number(type, loadNumber);
                    }
                    DataLogger.AddText($"The first free number is '{firstFreeNumberForType}'");

                    DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
                    DataLogger.SetProgressBarValue(i);

                }
            }
            catch (Exception exception)
            {
                succeeded &= false;
                ModelWsExceptionHandler(exception);
            }
            finally
            {
                DataLogger.ResetProgressBar();
                DataLogger.AddLogEnd(succeeded ? DataLogger.LogResultType.DONE : DataLogger.LogResultType.FAILED);
            }
            return succeeded;
        }
#endif

#if RFEM
        public static bool Test_General_Get\u2040Main\u2040Objects()
        {
            DataLogger.AddLogStart("Getting activations of main objects...");
            DataLogger.InitializeProgressBar(0, 8, 0);

            try
            {
                main_objects_to_activate main_objects = SoapModelClient.get_main_objects_to_activate();

                DataLogger.AddText($"Members: {main_objects.activate_members}");
                DataLogger.AddText($"Surfaces: {main_objects.activate_surfaces}");
                DataLogger.AddText($"Solids: {main_objects.activate_solids}");
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }

        public static bool Test_General_Set\u2040Main\u2040Objects()
        {
            DataLogger.AddLogStart("Setting activations of main objects...");
            DataLogger.InitializeProgressBar(0, 8, 0);

            try
            {
                main_objects_to_activate main_objects = new main_objects_to_activate
                {
                    activate_members = true,
                    activate_membersSpecified = true,
                    activate_surfaces = true,
                    activate_solidsSpecified = true,
                    activate_solids = true,
                    activate_surfacesSpecified = true
                };

                SoapModelClient.set_main_objects_to_activate(main_objects);
                main_objects = SoapModelClient.get_main_objects_to_activate();

                if (main_objects.activate_members  != true
                 || main_objects.activate_surfaces != true
                 || main_objects.activate_solids   != true
                    )
                {
                    throw new Exception("Main objects were not activated.");
                }

                //check members
                main_objects.activate_members = false;
                SoapModelClient.set_main_objects_to_activate(main_objects);
                main_objects = SoapModelClient.get_main_objects_to_activate();
                if (main_objects.activate_members != false)
                {
                    throw new Exception("Members were not deactivated.");
                }

                //check solids
                main_objects.activate_solids = false;
                SoapModelClient.set_main_objects_to_activate(main_objects);
                main_objects = SoapModelClient.get_main_objects_to_activate();
                if (main_objects.activate_solids != false)
                {
                    throw new Exception("Solids were not deactivated.");
                }

                //check surfaces
                main_objects.activate_surfaces = false;
                main_objects.activate_members = true;
                SoapModelClient.set_main_objects_to_activate(main_objects);
                main_objects = SoapModelClient.get_main_objects_to_activate();
                if (main_objects.activate_surfaces != false)
                {
                    throw new Exception("Surfaces were not deactivated.");
                }

                main_objects.activate_surfaces = true;
                main_objects.activate_solids = true;
                SoapModelClient.set_main_objects_to_activate(main_objects);
                main_objects = SoapModelClient.get_main_objects_to_activate();
                if (main_objects.activate_surfaces != true)
                {
                    throw new Exception("Surfaces were not activated.");
                }
                if (main_objects.activate_solids != true)
                {
                    throw new Exception("Solids were not activated.");
                }
            }
            catch (Exception exception)
            {
                ModelWsExceptionHandler(exception);
                return false;
            }
            finally
            {
                DataLogger.ResetProgressBar();
            }

            DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            return true;
        }
#endif //RFEM

        #region Model data comparison

        private class WsObjectClass
        {
            public string Name;
            public object_types Type;
            public int No;
            public int ParrentNo;
            public int Priority;
            public object Object;
        }

        private static List<WsObjectClass> GetObjects()
        {
            List<WsObjectClass> readObjects = new List<WsObjectClass>();

            void ReadObject(object_types type, int parentNo)
            {
                int[] numbers = null;

                try
                {
                    numbers = SoapModelClient.get_all_object_numbers(type, parentNo);
                }
                catch (FaultException ex)
                {
                    // fails in case that an object type was removed in the application but RfemSoapWsClient was not updated
                    DataLogger.WriteToLogAndConsole($"Get {type} numbers error - {ex.Message}");
                }

                if (numbers != null && numbers.Length > 0)
                {
                    string name = type.ToString().Replace("E_OBJECT_TYPE_", string.Empty).ToLower();

                    MethodInfo method = SoapModelClient.GetType().GetMethod($"get_{name}");
                    if (method == null)
                    {
                        // structure can be a member of other structure so it does not need own get method
                        DataLogger.WriteToLogAndConsole($"Object {name} does not contain method get_{name}.", false);
                    }
                    else
                    {
                        foreach (int no in numbers)
                        {
                            try
                            {

#if RFEM || RSTAB
                                void AddNewModificationBlock(object_types objectType)
                                {
                                    if (!NewModificationBlock.Contains(objectType))
                                    {
                                        NewModificationBlock.Add(objectType);
                                    }
                                }
#endif // RFEM RSTAB

                                int GetPriority(object_types objectType)
                                {
                                    switch (objectType)
                                    {
#if RFEM || RSTAB
                                        case object_types.E_OBJECT_TYPE_MEMBER:
                                        case object_types.E_OBJECT_TYPE_DESIGN_SITUATION:
                                        {
                                            return 1;
                                        }
                                        case object_types.E_OBJECT_TYPE_ACTION:
                                        case object_types.E_OBJECT_TYPE_ACTION_COMBINATION:
                                        {
                                            return 2;
                                        }
                                        case object_types.E_OBJECT_TYPE_NODAL_SUPPORT:
                                        {
                                            AddNewModificationBlock(objectType);
                                            return 3;
                                        }
#elif RSECTION
                                        case object_types.E_OBJECT_TYPE_LINE:
                                        {
                                            return 1;
                                        }
                                        case object_types.E_OBJECT_TYPE_PART:
                                        {
                                            return 2;
                                        }
                                        case object_types.E_OBJECT_TYPE_STRESS_POINT:
                                        {
                                            return 3;
                                        }
#endif
                                        default:
                                        {
                                            return 0;
                                        }
                                    }
                                }

                                readObjects.Add(new WsObjectClass
                                {
                                    Name = name,
                                    Type = type,
                                    No = no,
                                    ParrentNo = parentNo,
                                    Priority = GetPriority(type),
                                    Object = method.Invoke(SoapModelClient, parentNo > 0 ? new object[] { no, parentNo } : new object[] { no })
                                });
                            }
                            catch
                            {
                                DataLogger.WriteToLogAndConsole($"{name} No.{no}{(parentNo > 0 ? $" parent No.{parentNo}" : string.Empty)}", true);
                                throw;
                            }
                        }
                    }
                }
            }

            // read objects without parent
            Array objectTypes = Enum.GetValues(typeof(object_types));
            foreach (object_types type in objectTypes)
            {
#if RFEM || RSTAB
                // ToDo: Steel Design: Enable E_OBJECT_TYPE_STEEL_DESIGN_SEISMIC_CONFIGURATION after add support for work with standards.
                // Steel design seismic configuration is available only for some standards, e.g US and Canadian.
                if (type == object_types.E_OBJECT_TYPE_STEEL_DESIGN_SEISMIC_CONFIGURATION)
                {
                    continue;
                }
#endif
                if (!TypesWithParent.Contains(type))
                {
                    ReadObject(type, 0);
                }
            }

#if RFEM || RSTAB
            // read objects with parent
            foreach (object_types type in TypesWithParent)
            {
                if (type == object_types.E_OBJECT_TYPE_DXF_MODEL_OBJECT)
                {
                    readObjects.Where(o => o.Type == object_types.E_OBJECT_TYPE_DXF_FILE_MODEL_OBJECT).ToList().ForEach(o =>
                    {
                        ReadObject(type, o.No);
                    });
                }
                else if (type.ToString().EndsWith("_IMPERFECTION", StringComparison.OrdinalIgnoreCase))
                {
                    readObjects.Where(o => o.Type == object_types.E_OBJECT_TYPE_IMPERFECTION_CASE).ToList().ForEach(o =>
                    {
                        ReadObject(type, o.No);
                    });
                }
                else
                {
                    readObjects.Where(o => o.Type == object_types.E_OBJECT_TYPE_LOAD_CASE).ToList().ForEach(o =>
                    {
                        ReadObject(type, o.No);
                    });
                }
            }
#endif

            return readObjects;
        }

        /// <summary>
        /// Defines type of objects before which a new modification block have to be called;
        /// </summary>
        private static List<object_types> NewModificationBlock = new List<object_types>();

        private static void SetObjects(List<WsObjectClass> objects)
        {
#if RFEM || RSTAB
            bool combinationWizardExists = false;
#endif

            bool canceledModification = false;

            try
            {
                SoapModelClient.begin_modification("Set all objects");

                objects.OrderBy(o => o.Priority).ToList().ForEach(wsObject =>
                   {
                       MethodInfo method = SoapModelClient.GetType().GetMethod($"set_{wsObject.Name}");
                       if (method == null)
                       {
                           // some of objects can be read only so no set method is needed
                           DataLogger.WriteToLogAndConsole($"Object {wsObject.Name} No.{wsObject.No} does not contain method set_{wsObject.Name}.", false);
                       }
                       else
                       {
                           PropertyInfo isGeneratedProperty = wsObject.Object.GetType().GetProperty("is_generated");
                           if (isGeneratedProperty != null)
                           {
                               bool isGenerated = (bool)isGeneratedProperty.GetValue(wsObject.Object);
                               if (isGenerated)
                               {
                                   DataLogger.AddText($"Object {wsObject.Name} No.{wsObject.No} is skipped because it is a generated object.");
                                   return;
                               }
                           }
#if RFEM || RSTAB
                           if (wsObject.Type == object_types.E_OBJECT_TYPE_COMBINATION_WIZARD)
                           {
                               combinationWizardExists = true;
                           }
#endif

                           try
                           {
                               if (NewModificationBlock.Contains(wsObject.Type))
                               {
                                   SoapModelClient.finish_modification();
                                   SoapModelClient.begin_modification("Set all objects");
                                   NewModificationBlock.Remove(wsObject.Type);
                               }

                               _ = method.Invoke(SoapModelClient, wsObject.ParrentNo > 0 ? new object[] { wsObject.ParrentNo, wsObject.Object } : new object[] { wsObject.Object });
                           }
                           catch (TargetInvocationException ex)
                           {
                               throw ex.InnerException;
                           }
                       }
                   });
            }
            catch
            {
                canceledModification = true;
                SoapModelClient.cancel_modification();
                throw;
            }
            finally
            {
                if (!canceledModification)
                {
                    SoapModelClient.finish_modification();
                }
            }

#if !RSECTION
            if (combinationWizardExists)
            {
                DataLogger.AddLogStart("Generating load cases and combinations...");
                SoapModelClient.generate_load_cases_and_combinations();
                DataLogger.AddLogEnd(DataLogger.LogResultType.DONE);
            }
#endif
        }

        private static bool IsAssignableFrom(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type);
        }

        private static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static bool AreListsEqual(List<WsObjectClass> list1, List<WsObjectClass> list2)
        {
            bool equals = true;

            /* modify_stiffness_timber_members_due_moisture_classSpecified
             * -----------------------------------------------------------
             * If we choose standard "EN 1990 | Timber" then it creates Structure Modification object with a special internal flag "Default".
             * This flag has some special internal usage and means "True if this is default modification created by the program for certain standards".
             * It causes e.g. that attribute modify_stiffness_timber_members_due_moisture_class is available although add-on Timber Design is not activated.
             * When we create new Structure Modification (via GUI or WS) this flag is not active and the attribute is not available.
             * It is causing problem in read/write test because after we run loadCasesAndCombinationsPrepare.js there is activated standard "EN 1990 | Timber"
             * and Structure Modification object contains atribute modify_stiffness_timber_members_due_moisture_class but after we delete the model and write
             * the object again via WS then this attribute is not available.
             */
            string[] ignorePropertiesList = new string[] { "id_for_export_import", "modify_stiffness_timber_members_due_moisture_classSpecified" };

            foreach (WsObjectClass object1 in list1)
            {
                WsObjectClass object2 = list2.Where(o => o.Name == object1.Name && o.No == object1.No && o.ParrentNo == object1.ParrentNo)?.FirstOrDefault();
                if (object2 == null)
                {
                    equals = false;
                    DataLogger.WriteToLogAndConsole($"Object {object1.Name} with Id {object1.No} is missing");
                }
                else
                {
                    if (object1.ParrentNo != object2.ParrentNo)
                    {
                        equals = false;
                        DataLogger.WriteToLogAndConsole($"Object {object1.Name} - ParrentId {object1.ParrentNo} != {object2.ParrentNo}");
                    }

                    equals &= AreObjectsEqual(object1.Object, object2.Object, $"{object1.Name.ToUpper()}[{object1.No}]", ignorePropertiesList);
                }
            }

            return equals;
        }

        public static bool AreObjectsEqual(object object1, object object2, string name, string[] ignorePropertiesList)
        {
            bool equals = true;

            if (object1 == null && object2 != null || object1 != null && object2 == null)
            {
                equals = false;
            }
            else if (object1 != null && object2 != null)
            {
                // get all public properties of the object using reflection
                PropertyInfo[] properties = object1.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    // if it is not a readable property or if it is a ignorable property then continue
                    if (!propertyInfo.CanRead || ignorePropertiesList.Contains(propertyInfo.Name))
                    {
                        continue;
                    }

                    // get the property values of both objects
                    object value1 = propertyInfo.GetValue(object1, null);
                    object value2 = propertyInfo.GetValue(object2, null);

                    // if the objects are primitive types such as (integer, string etc) that implement IComparable, we can just directly try and compare the value
                    if (IsAssignableFrom(propertyInfo.PropertyType) || propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType)
                    {
                        // compare both values
                        if (!AreValuesEqual(value1, value2))
                        {
                            equals = false;
                            DataLogger.WriteToLogAndConsole($"Property {name}/{propertyInfo.Name} --> {value1} != {value2}");
                        }
                    }
                    // if the property is a collection (or something that implements IEnumerable) then we have to iterate through all items and compare values
                    else if (IsEnumerableType(propertyInfo.PropertyType))
                    {
                        if (!AreEnumerationsEqual(value1, value2, name + "/" + propertyInfo.Name, ignorePropertiesList))
                        {
                            equals = false;
                        }
                    }
                    // if it is a class object, call the same function recursively again
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsEqual(propertyInfo.GetValue(object1, null), propertyInfo.GetValue(object2, null), name + "/" + propertyInfo.Name, ignorePropertiesList))
                        {
                            equals = false;
                        }
                    }
                    else
                    {
                        equals = false;
                    }
                }
            }

            return equals;
        }

        /// <summary>
        /// Compares two values and returns true if they are the same.
        /// </summary>
        private static bool AreValuesEqual(object value1, object value2)
        {
            if (value1 == null && value2 != null || value1 != null && value2 == null)
            {
                return false;
            }

            if (value1 is IComparable selfValueComparer)
            {
                bool equals = selfValueComparer.CompareTo(value2) == 0;
                if (!equals && value1 is double doubleValue1 && value2 is double doubleValue2)
                {
                    equals = Math.Abs(doubleValue1 - doubleValue2) < DefaultDoubleComparisonTolerance;
                }
                return equals;
            }

            return Equals(value1, value2);
        }

        private static bool AreEnumerationsEqual(object value1, object value2, string propertyName, string[] ignorePropertiesList)
        {
            // if one of the values is null and the second is not then return false
            if (value1 == null && value2 != null || value1 != null && value2 == null)
            {
                DataLogger.WriteToLogAndConsole($"Property {propertyName} --> {value1 ?? "Null"} != {value2 ?? "Null"}");
                return false;
            }

            if (value1 != null && value2 != null)
            {
                IEnumerable<object> enumValue1 = ((IEnumerable)value1).Cast<object>();
                IEnumerable<object> enumValue2 = ((IEnumerable)value2).Cast<object>();

                // if the items count are different return false
                if (enumValue1.Count() != enumValue2.Count())
                {
                    DataLogger.WriteToLogAndConsole($"Property {propertyName} items count --> {enumValue1.Count()} != {enumValue2.Count()}");
                    return false;
                }

                // if the count is the same, compare individual item
                for (int itemIndex = 0; itemIndex < enumValue1.Count(); itemIndex++)
                {
                    object enumValue1Item = enumValue1.ElementAt(itemIndex);
                    object enumValue2Item = enumValue2.ElementAt(itemIndex);
                    Type enumValue1ItemType = enumValue1Item.GetType();

                    if (IsAssignableFrom(enumValue1ItemType) || enumValue1ItemType.IsPrimitive || enumValue1ItemType.IsValueType)
                    {
                        if (!AreValuesEqual(enumValue1Item, enumValue2Item))
                        {
                            DataLogger.WriteToLogAndConsole($"Property {propertyName} items count --> {enumValue1.Count()} != {enumValue2.Count()}");
                            return false;
                        }
                    }
                    else if (!AreObjectsEqual(enumValue1Item, enumValue2Item, propertyName, ignorePropertiesList))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

#endregion Model data comparison
    }
}
