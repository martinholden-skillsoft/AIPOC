using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPOC.Exceptions
{
    /// <summary>
    /// Thrown when the poll is for metadata not yet completed
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class OlsaDataNotReadyException : Exception
    {
        // Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaDataNotReadyException" /> class.
        /// </summary>
        public OlsaDataNotReadyException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaDataNotReadyException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OlsaDataNotReadyException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaDataNotReadyException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public OlsaDataNotReadyException(string message, Exception inner) : base(message, inner) { }

        // Serialization
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaDataNotReadyException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OlsaDataNotReadyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
