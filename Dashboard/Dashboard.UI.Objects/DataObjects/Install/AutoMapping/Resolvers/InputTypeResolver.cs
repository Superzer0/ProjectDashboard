using System.Collections.Generic;
using AutoMapper;

namespace Dashboard.UI.Objects.DataObjects.Install.AutoMapping.Resolvers
{
    public class InputTypeResolver : ValueResolver<string, InputType>
    {
        protected override InputType ResolveCore(string mappedValue)
        {
            return MappingValues[mappedValue];
        }

        private static readonly Dictionary<string, InputType> MappingValues = new Dictionary
            <string, InputType>
        {
            {"String", InputType.String},
            {"Json", InputType.Json}
        };
    }
}
