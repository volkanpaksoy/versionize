﻿using Xunit;
using LibGit2Sharp;
using Shouldly;
using System.Linq;
using Versionize.Tests.TestSupport;

namespace Versionize.Tests
{
    public class ChangelogazureLinkBuilderFactoryTests
    {
        [Fact]
        public void ShouldCreateAnAzureUrlBuilderForHTTPSPushUrls()
        {
            var repo = SetupRepositoryWithRemote("origin", "https://dosse@dev.azure.com/dosse/DosSE.ERP.Cloud/_git/ERP.git");
            var linkBuilder = ChangelogLinkBuilderFactory.CreateFor(repo);

            linkBuilder.ShouldBeAssignableTo<AzureLinkBuilder>();
        }

        [Fact]
        public void ShouldCreateAnAzureUrlBuilderForSSHPushUrls()
        {
            var repo = SetupRepositoryWithRemote("origin", "git@ssh.dev.azure.com:v3/dosse/DosSE.ERP.Cloud/ERP.git");
            var linkBuilder = ChangelogLinkBuilderFactory.CreateFor(repo);

            linkBuilder.ShouldBeAssignableTo<AzureLinkBuilder>();
        }

        [Fact]
        public void ShouldAzurePickFirstRemoteInCaseNoOriginWasFound()
        {
            var repo = SetupRepositoryWithRemote("some", "git@ssh.dev.azure.com:v3/dosse/DosSE.ERP.Cloud/ERP.git");
            var linkBuilder = ChangelogLinkBuilderFactory.CreateFor(repo);

            linkBuilder.ShouldBeAssignableTo<AzureLinkBuilder>();
        }

        [Fact]
        public void ShouldFallbackToNoopInCaseNoAzurePushUrlWasDefined()
        {
            var repo = SetupRepositoryWithRemote("origin", "https://hostmeister.com/saintedlama/versionize.git");
            var linkBuilder = ChangelogLinkBuilderFactory.CreateFor(repo);

            linkBuilder.ShouldBeAssignableTo<PlainLinkBuilder>();
        }

        [Fact]
        public void ShouldBuildASSHCommitLink()
        {
            var commit = new ConventionalCommit {
                Sha = "734713bc047d87bf7eac9674765ae793478c50d3"
            };

            var linkBuilder = new AzureLinkBuilder("git@ssh.dev.azure.com:v3/dosse/DosSE.ERP.Cloud/ERP.git");
            var link = linkBuilder.BuildCommitLink(commit);

            link.ShouldBe("https://v3@dev.azure.com/v3/dosse/DosSE.ERP.Cloud/ERP/commit/734713bc047d87bf7eac9674765ae793478c50d3");
        }

        [Fact]
        public void ShouldBuildAHTTPSCommitLink()
        {
            var commit = new ConventionalCommit {
                Sha = "734713bc047d87bf7eac9674765ae793478c50d3"
            };

            var linkBuilder = new AzureLinkBuilder("https://dosse@dev.azure.com/dosse/DosSE.ERP.Cloud/_git/ERP.git");
            var link = linkBuilder.BuildCommitLink(commit);

            link.ShouldBe("https://dosse@dev.azure.com/dosse/DosSE.ERP.Cloud/_git/ERP/commit/734713bc047d87bf7eac9674765ae793478c50d3");
        }

        private static Repository SetupRepositoryWithRemote(string remoteName, string pushUrl)
        {
            var workingDirectory = TempDir.Create();
            var repo = TempRepository.Create(workingDirectory);

            foreach (var existingRemoteName in repo.Network.Remotes.Select(remote => remote.Name)) {
              repo.Network.Remotes.Remove(existingRemoteName);
            }

            repo.Network.Remotes.Add(remoteName, pushUrl);

            return repo;
        }
    }
}
