using System;
using System.Threading.Tasks;
using Docker.DotNet;

namespace DockerClientLibrarySampleApp
{
    class Program
    {
        static DockerClient client = null;

        static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Provide image name to inspect");
                return;
            }

            client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();

            await InspectImageAsync(args[0]);
        }

        static async Task InspectImageAsync(string imageName)
        {
            var imageInspectResponse = await client.Images.InspectImageAsync(imageName);

            Console.WriteLine();
            Console.WriteLine("Cmd: " + string.Join(" ", imageInspectResponse.ContainerConfig.Cmd));

            if (!string.IsNullOrEmpty(imageInspectResponse.Parent))
            {
                await InspectImageAsync(imageInspectResponse.Parent);
            }
        }
    }
}
