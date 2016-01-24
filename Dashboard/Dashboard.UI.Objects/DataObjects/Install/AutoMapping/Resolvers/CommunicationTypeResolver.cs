using System.Collections.Generic;
using AutoMapper;

namespace Dashboard.UI.Objects.DataObjects.Install.AutoMapping.Resolvers
{
    public class CommunicationTypeResolver : ValueResolver<string, CommunicationType>
    {
        protected override CommunicationType ResolveCore(string mappedValue)
        {
            return MappingValues[mappedValue];
        }

        private static readonly Dictionary<string, CommunicationType> MappingValues = new Dictionary
            <string, CommunicationType>
        {
            {"Plain", CommunicationType.Plain},
        };
    }
}
