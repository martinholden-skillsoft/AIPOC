using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPOC.Exceptions
{
    /// <summary>
    /// Thrown when the web service request could not be authenticated
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class OlsaSecurityException : Exception
    {
        // Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaSecurityException" /> class.
        /// </summary>
        public OlsaSecurityException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaSecurityException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OlsaSecurityException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaSecurityException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public OlsaSecurityException(string message, Exception inner) : base(message, inner) { }

        // Serialization
        /// <summary>
        /// Initializes a new instance of the <see cref="OlsaSecurityException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OlsaSecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
