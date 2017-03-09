﻿using NGitLab.Impl;

namespace NGitLab
{
    public class GitLabClient
    {
        private readonly API _api;

        public readonly IUserClient Users;
        public readonly IProjectClient Projects;
        public readonly IIssueClient Issues;
        public readonly INamespaceClient Groups;

        private GitLabClient(string hostUrl, string apiToken, IHttpRequestorFactory httpRequestorFactory)
        {
            _api = new API(hostUrl, apiToken, httpRequestorFactory);
            Users = new UserClient(_api);
            Projects = new ProjectClient(_api);
            Issues = new IssueClient(_api);
            Groups = new NamespaceClient(_api);
        }

        public static GitLabClient Connect(string hostUrl, string apiToken)
        {
            return new GitLabClient(hostUrl, apiToken, new WebRequestHttpRequestorFactory());
        }

        public static GitLabClient Connect(string hostUrl, string apiToken, IHttpRequestorFactory httpRequestorFactory)
        {
            return new GitLabClient(hostUrl, apiToken, httpRequestorFactory);
        }

        public IRepositoryClient GetRepository(int projectId)
        {
            return new RepositoryClient(_api, projectId);
        }

        public IMergeRequestClient GetMergeRequest(int projectId)
        {
            return new MergeRequestClient(_api, projectId);
        }

    }
}