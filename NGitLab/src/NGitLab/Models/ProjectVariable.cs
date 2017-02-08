using System;
using System.Runtime.Serialization;

namespace NGitLab.Models
{
    [DataContract]
    public class ProjectVariable
    {
        public const string Url = "/projects/{0}/variables";

        [DataMember(Name = "key")]
        public string Key;

        [DataMember(Name = "value")]
        public string Value;
    }
}