using System.Collections;
using Readify.Useful.TeamFoundation.Common;

namespace TfsDeployer
{
    public class NamedLockSet
    {
        private readonly IDictionary _locks = new Hashtable();

        public object GetLockObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return new object();

            lock (_locks.SyncRoot)
            {
                TraceHelper.TraceVerbose(TraceSwitches.TfsDeployer, "Providing lock object for name: {0}", name);
                if (!_locks.Contains(name)) _locks.Add(name, new object());
                return _locks[name];
            }
        }

    }
}