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

            Console.WriteLine("Results of inspecting image: " + args[0]);

            await InspectImageAsync(args[0]);
        }

        static async Task InspectImageAsync(string imageName)
        {
            var imageInspectResponse = await client.Images.InspectImageAsync(imageName);

            Console.WriteLine();
            Console.WriteLine("    RepTags: "
                + string.Join(" ", imageInspectResponse.RepoTags ?? Array.Empty<string>()));
            Console.WriteLine("        Cmd: "
                + string.Join(" ", imageInspectResponse.ContainerConfig.Cmd ?? Array.Empty<string>()));
            Console.WriteLine("Entry point: "
                + string.Join(" ", imageInspectResponse.ContainerConfig.Entrypoint ?? Array.Empty<string>()));
            Console.WriteLine("       Size: " + FormatBytes(imageInspectResponse.Size));
            Console.WriteLine("VirtualSize: " + FormatBytes(imageInspectResponse.VirtualSize));

            if (!string.IsNullOrEmpty(imageInspectResponse.Parent))
            {
                await InspectImageAsync(imageInspectResponse.Parent);
            }
        }

        private static string FormatBytes(long bytes)
        {
            var suffixes = new[] { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < suffixes.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, suffixes[i]);
        }
    }
}
