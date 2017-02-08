using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using NGitLab.Models;

namespace NGitLab.Impl
{
    public class ProjectVariablesClient : IProjectVariablesClient
    {
        private readonly HttpRequestor _httpRequestor;
        private readonly string _projectPath;

        string IProjectVariablesClient.this[string key]
        {
            get { return _httpRequestor.Get<ProjectVariable>(this._projectPath + "/variables/" + key).Result.Value; }
            set {
                var url = this._projectPath + "/variables/" + key;
                var requestor = _httpRequestor.With(new ProjectVariable { Key = key, Value = value });
                var allTask = _httpRequestor.GetAll<ProjectVariable>(this._projectPath + "/variables");
                allTask.Wait();
                var task = allTask.Result.FirstOrDefault(v => v.Key == key);
                if (task == null)
                    requestor.Post<ProjectVariable>(this._projectPath + "/variables").Wait();
                else
                    requestor.Put<ProjectVariable>(url).Wait();
            }
        }

        public ProjectVariablesClient(HttpRequestor httpRequestor, string projectPath)
        {
            _httpRequestor = httpRequestor;
            _projectPath = projectPath;
        }

        public async Task<IEnumerable<ProjectVariable>> All() => await _httpRequestor.GetAll<ProjectVariable>(this._projectPath + "/variables");
       
    }
}