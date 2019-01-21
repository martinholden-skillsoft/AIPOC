using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPOC.Exceptions
{
    /// <summary>
    /// Thrown when the poll cycle times out
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class OlsaPollTimeOutException : Exception
    {
        // Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaPollTimeOutException" /> class.
        /// </summary>
        public OlsaPollTimeOutException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaPollTimeOutException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OlsaPollTimeOutException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaPollTimeOutException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public OlsaPollTimeOutException(string message, Exception inner) : base(message, inner) { }

        // Serialization
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaPollTimeOutException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OlsaPollTimeOutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
