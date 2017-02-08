using System.Collections.Generic;
using System.Threading.Tasks;
using NGitLab.Models;

namespace NGitLab
{
    public interface IProjectVariablesClient
    {
        /// <summary>
        /// Get a list of projects variables
        /// </summary>
        Task<IEnumerable<ProjectVariable>> All();

        /// <summary>
        /// Get or set a project variable
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string this[string key] { get; set; }
    }
}