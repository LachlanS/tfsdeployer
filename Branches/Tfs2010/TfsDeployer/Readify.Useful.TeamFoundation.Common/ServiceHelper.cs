using System.IO;
using System.Xml.Serialization;

namespace Readify.Useful.TeamFoundation.Common
{
    /// <summary>
    /// This class contains utilities for in the communication 
    /// with a Team Foundation Server
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// This method deserializes and event that has been received into a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventXml"></param>
        /// <returns></returns>
        public static T DeserializeEvent<T>(string eventXml)
        {
            TraceHelper.TraceVerbose(Constants.CommonSwitch,"Deserializing Event of type {0} from XML:\n{1}", typeof(T), eventXml);
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(eventXml)) 
            {
                return (T)serializer.Deserialize(reader);
            }
            
        }
    }
}
