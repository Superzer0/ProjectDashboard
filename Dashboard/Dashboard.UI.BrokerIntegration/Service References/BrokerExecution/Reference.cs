﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dashboard.UI.BrokerIntegration.BrokerExecution {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PluginExecutionInfo", Namespace="http://schemas.datacontract.org/2004/07/Dashboard.Broker.Objects.DataObjects.Data" +
        "Contracts")]
    [System.SerializableAttribute()]
    public partial class PluginExecutionInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ConfigurationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MethodNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ParametersField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PluginIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string VersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Configuration {
            get {
                return this.ConfigurationField;
            }
            set {
                if ((object.ReferenceEquals(this.ConfigurationField, value) != true)) {
                    this.ConfigurationField = value;
                    this.RaisePropertyChanged("Configuration");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MethodName {
            get {
                return this.MethodNameField;
            }
            set {
                if ((object.ReferenceEquals(this.MethodNameField, value) != true)) {
                    this.MethodNameField = value;
                    this.RaisePropertyChanged("MethodName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Parameters {
            get {
                return this.ParametersField;
            }
            set {
                if ((object.ReferenceEquals(this.ParametersField, value) != true)) {
                    this.ParametersField = value;
                    this.RaisePropertyChanged("Parameters");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PluginId {
            get {
                return this.PluginIdField;
            }
            set {
                if ((object.ReferenceEquals(this.PluginIdField, value) != true)) {
                    this.PluginIdField = value;
                    this.RaisePropertyChanged("PluginId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Version {
            get {
                return this.VersionField;
            }
            set {
                if ((object.ReferenceEquals(this.VersionField, value) != true)) {
                    this.VersionField = value;
                    this.RaisePropertyChanged("Version");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BrokerExecution.ILaunchPluginsService")]
    public interface ILaunchPluginsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILaunchPluginsService/Execute", ReplyAction="http://tempuri.org/ILaunchPluginsService/ExecuteResponse")]
        string Execute(Dashboard.UI.BrokerIntegration.BrokerExecution.PluginExecutionInfo pluginExecutionInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILaunchPluginsService/Execute", ReplyAction="http://tempuri.org/ILaunchPluginsService/ExecuteResponse")]
        System.Threading.Tasks.Task<string> ExecuteAsync(Dashboard.UI.BrokerIntegration.BrokerExecution.PluginExecutionInfo pluginExecutionInfo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILaunchPluginsServiceChannel : Dashboard.UI.BrokerIntegration.BrokerExecution.ILaunchPluginsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LaunchPluginsServiceClient : System.ServiceModel.ClientBase<Dashboard.UI.BrokerIntegration.BrokerExecution.ILaunchPluginsService>, Dashboard.UI.BrokerIntegration.BrokerExecution.ILaunchPluginsService {
        
        public LaunchPluginsServiceClient() {
        }
        
        public LaunchPluginsServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LaunchPluginsServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LaunchPluginsServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LaunchPluginsServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Execute(Dashboard.UI.BrokerIntegration.BrokerExecution.PluginExecutionInfo pluginExecutionInfo) {
            return base.Channel.Execute(pluginExecutionInfo);
        }
        
        public System.Threading.Tasks.Task<string> ExecuteAsync(Dashboard.UI.BrokerIntegration.BrokerExecution.PluginExecutionInfo pluginExecutionInfo) {
            return base.Channel.ExecuteAsync(pluginExecutionInfo);
        }
    }
}
