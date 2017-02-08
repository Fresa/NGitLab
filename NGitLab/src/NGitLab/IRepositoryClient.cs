using NGitLab.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NGitLab
{
    public interface IRepositoryClient
    {
        Task<IEnumerable<Tag>> Tags();

        Task<IEnumerable<TreeOrBlob>> Tree();

        Task<IEnumerable<Commit>> Commits();

        Task<SingleCommit> GetCommitAsync(Sha1 sha);

        Task<IEnumerable<Diff>> GetCommitDiff(Sha1 sha);

        IFilesClient Files { get; }

        IBranchClient Branches { get; }

        IProjectHooksClient ProjectHooks { get; }

        IProjectVariablesClient ProjectVariables { get; }
    }
}