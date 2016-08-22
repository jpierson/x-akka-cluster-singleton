using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("singletontest", ConfigurationFactory.Load().WithFallback(ClusterSingletonManager.DefaultConfig()));
            Console.Title = $"Cluster Node ({((ExtendedActorSystem)system).Provider.DefaultAddress})";

            var singletonManager = system.ActorOf(
                props: ClusterSingletonManager.Props(
                    singletonProps: Props.Create<MySingleton>(),
                    terminationMessage: PoisonPill.Instance,
                    //settings: ClusterSingletonManagerSettings.Create(system)),
                    settings: new ClusterSingletonManagerSettings(
                        "my-singleton",
                        "singletonrole",
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(3))),
                name: "singleton");


            var singletonProxy = system.ActorOf(ClusterSingletonProxy.Props(
                singletonManagerPath: "/user/singleton",
                //settings: ClusterSingletonProxySettings.Create(system)),
                settings: new ClusterSingletonProxySettings("my-singleton", "singletonrole", TimeSpan.FromSeconds(3), 8192)),
                name: "my-singleton-proxy");

            system.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero, 
                TimeSpan.FromSeconds(4), 
                singletonProxy, 
                $"Message from process {Process.GetCurrentProcess().Id}", 
                Nobody.Instance);

            system.WhenTerminated.Wait();
        }
    }
}
