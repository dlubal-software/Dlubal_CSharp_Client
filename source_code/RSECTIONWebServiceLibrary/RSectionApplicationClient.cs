//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dlubal.WS.RSection1.Application
{
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RSectionApplicationClient : System.ServiceModel.ClientBase<Dlubal.WS.RSection1.Application.IRSectionApplication>, Dlubal.WS.RSection1.Application.IRSectionApplication
    {
        
        public RSectionApplicationClient()
        {
        }
        
        public RSectionApplicationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public RSectionApplicationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public RSectionApplicationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public RSectionApplicationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.close_applicationResponse Dlubal.WS.RSection1.Application.IRSectionApplication.close_application(Dlubal.WS.RSection1.Application.close_applicationRequest request)
        {
            return base.Channel.close_application(request);
        }
        
        public void close_application()
        {
            Dlubal.WS.RSection1.Application.close_applicationRequest inValue = new Dlubal.WS.RSection1.Application.close_applicationRequest();
            Dlubal.WS.RSection1.Application.close_applicationResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).close_application(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.close_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.close_model(Dlubal.WS.RSection1.Application.close_modelRequest request)
        {
            return base.Channel.close_model(request);
        }
        
        public void close_model(int index, bool save_changes)
        {
            Dlubal.WS.RSection1.Application.close_modelRequest inValue = new Dlubal.WS.RSection1.Application.close_modelRequest();
            inValue.index = index;
            inValue.save_changes = save_changes;
            Dlubal.WS.RSection1.Application.close_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).close_model(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.delete_projectResponse Dlubal.WS.RSection1.Application.IRSectionApplication.delete_project(Dlubal.WS.RSection1.Application.delete_projectRequest request)
        {
            return base.Channel.delete_project(request);
        }
        
        public void delete_project(string project_path)
        {
            Dlubal.WS.RSection1.Application.delete_projectRequest inValue = new Dlubal.WS.RSection1.Application.delete_projectRequest();
            inValue.project_path = project_path;
            Dlubal.WS.RSection1.Application.delete_projectResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).delete_project(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_active_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_active_model(Dlubal.WS.RSection1.Application.get_active_modelRequest request)
        {
            return base.Channel.get_active_model(request);
        }
        
        public string get_active_model()
        {
            Dlubal.WS.RSection1.Application.get_active_modelRequest inValue = new Dlubal.WS.RSection1.Application.get_active_modelRequest();
            Dlubal.WS.RSection1.Application.get_active_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_active_model(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_current_projectResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_current_project(Dlubal.WS.RSection1.Application.get_current_projectRequest request)
        {
            return base.Channel.get_current_project(request);
        }
        
        public Dlubal.WS.RSection1.Application.project_info get_current_project()
        {
            Dlubal.WS.RSection1.Application.get_current_projectRequest inValue = new Dlubal.WS.RSection1.Application.get_current_projectRequest();
            Dlubal.WS.RSection1.Application.get_current_projectResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_current_project(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_detailed_loggingResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_detailed_logging(Dlubal.WS.RSection1.Application.get_detailed_loggingRequest request)
        {
            return base.Channel.get_detailed_logging(request);
        }
        
        public bool get_detailed_logging()
        {
            Dlubal.WS.RSection1.Application.get_detailed_loggingRequest inValue = new Dlubal.WS.RSection1.Application.get_detailed_loggingRequest();
            Dlubal.WS.RSection1.Application.get_detailed_loggingResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_detailed_logging(inValue);
            return retVal.value;
        }
        
        public Dlubal.WS.RSection1.Application.application_information get_information()
        {
            Dlubal.WS.RSection1.Application.get_informationRequest inValue = new Dlubal.WS.RSection1.Application.get_informationRequest();
            Dlubal.WS.RSection1.Application.get_informationResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_information(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_informationResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_information(Dlubal.WS.RSection1.Application.get_informationRequest request)
        {
            return base.Channel.get_information(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_list_of_existing_projectsResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_list_of_existing_projects(Dlubal.WS.RSection1.Application.get_list_of_existing_projectsRequest request)
        {
            return base.Channel.get_list_of_existing_projects(request);
        }
        
        public Dlubal.WS.RSection1.Application.project_info[] get_list_of_existing_projects()
        {
            Dlubal.WS.RSection1.Application.get_list_of_existing_projectsRequest inValue = new Dlubal.WS.RSection1.Application.get_list_of_existing_projectsRequest();
            Dlubal.WS.RSection1.Application.get_list_of_existing_projectsResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_list_of_existing_projects(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_model(Dlubal.WS.RSection1.Application.get_modelRequest request)
        {
            return base.Channel.get_model(request);
        }
        
        public string get_model(int index)
        {
            Dlubal.WS.RSection1.Application.get_modelRequest inValue = new Dlubal.WS.RSection1.Application.get_modelRequest();
            inValue.index = index;
            Dlubal.WS.RSection1.Application.get_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_model(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_model_listResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_model_list(Dlubal.WS.RSection1.Application.get_model_listRequest request)
        {
            return base.Channel.get_model_list(request);
        }
        
        public string[] get_model_list()
        {
            Dlubal.WS.RSection1.Application.get_model_listRequest inValue = new Dlubal.WS.RSection1.Application.get_model_listRequest();
            Dlubal.WS.RSection1.Application.get_model_listResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_model_list(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_model_list_with_indexesResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_model_list_with_indexes(Dlubal.WS.RSection1.Application.get_model_list_with_indexesRequest request)
        {
            return base.Channel.get_model_list_with_indexes(request);
        }
        
        public Dlubal.WS.RSection1.Application.model_name_and_index[] get_model_list_with_indexes()
        {
            Dlubal.WS.RSection1.Application.get_model_list_with_indexesRequest inValue = new Dlubal.WS.RSection1.Application.get_model_list_with_indexesRequest();
            Dlubal.WS.RSection1.Application.get_model_list_with_indexesResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_model_list_with_indexes(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_program_optionsResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_program_options(Dlubal.WS.RSection1.Application.get_program_optionsRequest request)
        {
            return base.Channel.get_program_options(request);
        }
        
        public Dlubal.WS.RSection1.Application.settings_program_options get_program_options()
        {
            Dlubal.WS.RSection1.Application.get_program_optionsRequest inValue = new Dlubal.WS.RSection1.Application.get_program_optionsRequest();
            Dlubal.WS.RSection1.Application.get_program_optionsResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_program_options(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_program_settingsResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_program_settings(Dlubal.WS.RSection1.Application.get_program_settingsRequest request)
        {
            return base.Channel.get_program_settings(request);
        }
        
        public Dlubal.WS.RSection1.Application.program_settings get_program_settings()
        {
            Dlubal.WS.RSection1.Application.get_program_settingsRequest inValue = new Dlubal.WS.RSection1.Application.get_program_settingsRequest();
            Dlubal.WS.RSection1.Application.get_program_settingsResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_program_settings(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_projectResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_project(Dlubal.WS.RSection1.Application.get_projectRequest request)
        {
            return base.Channel.get_project(request);
        }
        
        public Dlubal.WS.RSection1.Application.project_info get_project(string project_path)
        {
            Dlubal.WS.RSection1.Application.get_projectRequest inValue = new Dlubal.WS.RSection1.Application.get_projectRequest();
            inValue.project_path = project_path;
            Dlubal.WS.RSection1.Application.get_projectResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_project(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_session_idResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_session_id(Dlubal.WS.RSection1.Application.get_session_idRequest request)
        {
            return base.Channel.get_session_id(request);
        }
        
        public string get_session_id()
        {
            Dlubal.WS.RSection1.Application.get_session_idRequest inValue = new Dlubal.WS.RSection1.Application.get_session_idRequest();
            Dlubal.WS.RSection1.Application.get_session_idResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_session_id(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.get_settings_program_languageResponse Dlubal.WS.RSection1.Application.IRSectionApplication.get_settings_program_language(Dlubal.WS.RSection1.Application.get_settings_program_languageRequest request)
        {
            return base.Channel.get_settings_program_language(request);
        }
        
        public Dlubal.WS.RSection1.Application.settings_program_language get_settings_program_language()
        {
            Dlubal.WS.RSection1.Application.get_settings_program_languageRequest inValue = new Dlubal.WS.RSection1.Application.get_settings_program_languageRequest();
            Dlubal.WS.RSection1.Application.get_settings_program_languageResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).get_settings_program_language(inValue);
            return retVal.value;
        }
        
        public Dlubal.WS.RSection1.Application.import_from_output import_from(string file_path)
        {
            Dlubal.WS.RSection1.Application.import_fromRequest inValue = new Dlubal.WS.RSection1.Application.import_fromRequest();
            inValue.file_path = file_path;
            Dlubal.WS.RSection1.Application.import_fromResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).import_from(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.import_fromResponse Dlubal.WS.RSection1.Application.IRSectionApplication.import_from(Dlubal.WS.RSection1.Application.import_fromRequest request)
        {
            return base.Channel.import_from(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.new_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.new_model(Dlubal.WS.RSection1.Application.new_modelRequest request)
        {
            return base.Channel.new_model(request);
        }
        
        public string new_model(string model_name)
        {
            Dlubal.WS.RSection1.Application.new_modelRequest inValue = new Dlubal.WS.RSection1.Application.new_modelRequest();
            inValue.model_name = model_name;
            Dlubal.WS.RSection1.Application.new_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).new_model(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.new_model_as_copyResponse Dlubal.WS.RSection1.Application.IRSectionApplication.new_model_as_copy(Dlubal.WS.RSection1.Application.new_model_as_copyRequest request)
        {
            return base.Channel.new_model_as_copy(request);
        }
        
        public string new_model_as_copy(string model_name, string file_path)
        {
            Dlubal.WS.RSection1.Application.new_model_as_copyRequest inValue = new Dlubal.WS.RSection1.Application.new_model_as_copyRequest();
            inValue.model_name = model_name;
            inValue.file_path = file_path;
            Dlubal.WS.RSection1.Application.new_model_as_copyResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).new_model_as_copy(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.new_model_from_templateResponse Dlubal.WS.RSection1.Application.IRSectionApplication.new_model_from_template(Dlubal.WS.RSection1.Application.new_model_from_templateRequest request)
        {
            return base.Channel.new_model_from_template(request);
        }
        
        public string new_model_from_template(string model_name, string file_path)
        {
            Dlubal.WS.RSection1.Application.new_model_from_templateRequest inValue = new Dlubal.WS.RSection1.Application.new_model_from_templateRequest();
            inValue.model_name = model_name;
            inValue.file_path = file_path;
            Dlubal.WS.RSection1.Application.new_model_from_templateResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).new_model_from_template(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.new_projectResponse Dlubal.WS.RSection1.Application.IRSectionApplication.new_project(Dlubal.WS.RSection1.Application.new_projectRequest request)
        {
            return base.Channel.new_project(request);
        }
        
        public void new_project(Dlubal.WS.RSection1.Application.project_info project_info)
        {
            Dlubal.WS.RSection1.Application.new_projectRequest inValue = new Dlubal.WS.RSection1.Application.new_projectRequest();
            inValue.project_info = project_info;
            Dlubal.WS.RSection1.Application.new_projectResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).new_project(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.open_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.open_model(Dlubal.WS.RSection1.Application.open_modelRequest request)
        {
            return base.Channel.open_model(request);
        }
        
        public string open_model(string model_path)
        {
            Dlubal.WS.RSection1.Application.open_modelRequest inValue = new Dlubal.WS.RSection1.Application.open_modelRequest();
            inValue.model_path = model_path;
            Dlubal.WS.RSection1.Application.open_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).open_model(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.save_modelResponse Dlubal.WS.RSection1.Application.IRSectionApplication.save_model(Dlubal.WS.RSection1.Application.save_modelRequest request)
        {
            return base.Channel.save_model(request);
        }
        
        public void save_model(int index)
        {
            Dlubal.WS.RSection1.Application.save_modelRequest inValue = new Dlubal.WS.RSection1.Application.save_modelRequest();
            inValue.index = index;
            Dlubal.WS.RSection1.Application.save_modelResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).save_model(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.set_as_current_projectResponse Dlubal.WS.RSection1.Application.IRSectionApplication.set_as_current_project(Dlubal.WS.RSection1.Application.set_as_current_projectRequest request)
        {
            return base.Channel.set_as_current_project(request);
        }
        
        public void set_as_current_project(string project_path)
        {
            Dlubal.WS.RSection1.Application.set_as_current_projectRequest inValue = new Dlubal.WS.RSection1.Application.set_as_current_projectRequest();
            inValue.project_path = project_path;
            Dlubal.WS.RSection1.Application.set_as_current_projectResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).set_as_current_project(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.set_detailed_loggingResponse Dlubal.WS.RSection1.Application.IRSectionApplication.set_detailed_logging(Dlubal.WS.RSection1.Application.set_detailed_loggingRequest request)
        {
            return base.Channel.set_detailed_logging(request);
        }
        
        public void set_detailed_logging(bool value)
        {
            Dlubal.WS.RSection1.Application.set_detailed_loggingRequest inValue = new Dlubal.WS.RSection1.Application.set_detailed_loggingRequest();
            inValue.value = value;
            Dlubal.WS.RSection1.Application.set_detailed_loggingResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).set_detailed_logging(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.set_program_optionsResponse Dlubal.WS.RSection1.Application.IRSectionApplication.set_program_options(Dlubal.WS.RSection1.Application.set_program_optionsRequest request)
        {
            return base.Channel.set_program_options(request);
        }
        
        public void set_program_options(Dlubal.WS.RSection1.Application.settings_program_options program_options)
        {
            Dlubal.WS.RSection1.Application.set_program_optionsRequest inValue = new Dlubal.WS.RSection1.Application.set_program_optionsRequest();
            inValue.program_options = program_options;
            Dlubal.WS.RSection1.Application.set_program_optionsResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).set_program_options(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.set_program_settingsResponse Dlubal.WS.RSection1.Application.IRSectionApplication.set_program_settings(Dlubal.WS.RSection1.Application.set_program_settingsRequest request)
        {
            return base.Channel.set_program_settings(request);
        }
        
        public string set_program_settings(Dlubal.WS.RSection1.Application.program_settings program_settings)
        {
            Dlubal.WS.RSection1.Application.set_program_settingsRequest inValue = new Dlubal.WS.RSection1.Application.set_program_settingsRequest();
            inValue.program_settings = program_settings;
            Dlubal.WS.RSection1.Application.set_program_settingsResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).set_program_settings(inValue);
            return retVal.value;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Dlubal.WS.RSection1.Application.set_settings_program_languageResponse Dlubal.WS.RSection1.Application.IRSectionApplication.set_settings_program_language(Dlubal.WS.RSection1.Application.set_settings_program_languageRequest request)
        {
            return base.Channel.set_settings_program_language(request);
        }
        
        public string set_settings_program_language(Dlubal.WS.RSection1.Application.settings_program_language settings_program_language)
        {
            Dlubal.WS.RSection1.Application.set_settings_program_languageRequest inValue = new Dlubal.WS.RSection1.Application.set_settings_program_languageRequest();
            inValue.settings_program_language = settings_program_language;
            Dlubal.WS.RSection1.Application.set_settings_program_languageResponse retVal = ((Dlubal.WS.RSection1.Application.IRSectionApplication)(this)).set_settings_program_language(inValue);
            return retVal.value;
        }
    }
}
