using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotNext.Net.Cluster.Consensus.Raft.Http
{
    using Messaging;
    using Replication;

    internal partial class RaftHttpCluster
    {
        private static IServiceCollection AddClusterAsSingleton<TCluster>(IServiceCollection services)
            where TCluster : RaftHttpCluster
        {
            Func<IServiceProvider, RaftHttpCluster> clusterNodeCast = ServiceProviderServiceExtensions.GetRequiredService<TCluster>;

            return services.AddSingleton<TCluster>()
                .AddSingleton(clusterNodeCast)
                .AddSingleton<IHostedService>(clusterNodeCast)
                .AddSingleton<ICluster>(clusterNodeCast)
                .AddSingleton<IRaftCluster>(clusterNodeCast)
                .AddSingleton<IMessageBus>(clusterNodeCast)
                .AddSingleton<IReplicationCluster>(clusterNodeCast)
                .AddSingleton<IReplicationCluster<IRaftLogEntry>>(clusterNodeCast)
                .AddSingleton<IExpandableCluster>(clusterNodeCast)
                .AddSingleton<IPeerMesh<IClusterMember>>(clusterNodeCast)
                .AddSingleton<IPeerMesh<IRaftClusterMember>>(clusterNodeCast);
        }

        // TODO: Add support of Action<HttpClusterMemberConfiguration> and merge it with RaftEmbeddedClusterMemberConfiguration
        internal static IServiceCollection AddClusterAsSingleton<TCluster, TConfig>(IServiceCollection services, IConfiguration memberConfig)
            where TCluster : RaftHttpCluster
            where TConfig : HttpClusterMemberConfiguration, new()
        {
            Func<IServiceProvider, IOptionsMonitor<HttpClusterMemberConfiguration>> configCast = ServiceProviderServiceExtensions.GetRequiredService<IOptionsMonitor<TConfig>>;
            return AddClusterAsSingleton<TCluster>(services.Configure<TConfig>(memberConfig).AddSingleton(configCast));
        }
    }
}