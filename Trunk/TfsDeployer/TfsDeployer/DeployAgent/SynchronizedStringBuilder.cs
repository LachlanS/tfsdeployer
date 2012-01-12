using System.Runtime.CompilerServices;
using System.Text;

namespace TfsDeployer.DeployAgent
{
    public class SynchronizedStringBuilder
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

        public int Length
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _builder.Length; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { _builder.Length = value; }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}