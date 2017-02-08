using System;
using System.Collections.Generic;
using System.IO;
using NGitLab.Models;
using System.Threading.Tasks;

namespace NGitLab.Impl
{
    public class RepositoryClient : IRepositoryClient
    {
        private readonly HttpRequestor _httpRequestor;
        private readonly string _repoPath;
        private readonly string _projectPath;

        public RepositoryClient(HttpRequestor httpRequestor, int projectId)
        {
            _httpRequestor = httpRequestor;

            _projectPath = Project.Url + "/" + projectId;
            _repoPath = _projectPath + "/repository";
        }

        public async Task<IEnumerable<Tag>> Tags() => await _httpRequestor.GetAll<Tag>(_repoPath + "/tags");

        public async Task<IEnumerable<TreeOrBlob>> Tree() => await _httpRequestor.GetAll<TreeOrBlob>(_repoPath + "/tree");

        public async Task<IEnumerable<Commit>> Commits() => await _httpRequestor.GetAll<Commit>(_repoPath + "/commits");

        public async Task<SingleCommit> GetCommitAsync(Sha1 sha) => await _httpRequestor.Get<SingleCommit>(_repoPath + "/commits/" + sha);

        public async Task<IEnumerable<Diff>> GetCommitDiff(Sha1 sha) => await _httpRequestor.GetAll<Diff>(_repoPath + "/commits/" + sha + "/diff");

        public IFilesClient Files => new FileClient(_httpRequestor, _repoPath);

        public IBranchClient Branches => new BranchClient(_httpRequestor, _repoPath);

        public IProjectHooksClient ProjectHooks => new ProjectHooksClient(_httpRequestor, _projectPath);

        public IProjectVariablesClient ProjectVariables => new ProjectVariablesClient(_httpRequestor, _projectPath);
    }
}