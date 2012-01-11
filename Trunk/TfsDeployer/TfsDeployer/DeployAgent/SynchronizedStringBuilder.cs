using System.Runtime.CompilerServices;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    class SynchronizedStringBuilder
    {
        private readonly StringBuilder _builder;

        public SynchronizedStringBuilder(StringBuilder builder)
        {
            _builder = builder;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SynchronizedStringBuilder Append(char[] value, int startIndex, int count)
        {
            _builder.Append(value, startIndex, count);
            return this;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}