using System;

namespace Dashboard.UI.Objects.DataObjects
{
    public class Plugin
    {
        public Guid Id { get; set; }
        public string Xml { get; set; }
        public DateTime Added { get; set; }
        public string AddedBy { get; set; }        
    }
}
