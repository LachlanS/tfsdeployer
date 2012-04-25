using System.IO;

namespace Tests.TfsDeployer.Resources
{
    class Resource
    {
        public static string AsString(string resourceName)
        {
            using (var stream = typeof(Resource).Assembly.GetManifestResourceStream(typeof(Resource), resourceName))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
