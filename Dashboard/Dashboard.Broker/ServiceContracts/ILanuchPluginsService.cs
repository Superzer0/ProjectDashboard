﻿using System.ServiceModel;
using Dashboard.Broker.Objects.DataObjects.DataContracts;

namespace Dashboard.Broker.ServiceContracts
{
    [ServiceContract]
    public interface ILaunchPluginsService
    {
        [OperationContract]
        string Execute(PluginExecutionInfo pluginExecutionInfo);
    }
   
}
