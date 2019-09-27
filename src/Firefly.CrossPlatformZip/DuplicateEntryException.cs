using System;
using System.Collections.Generic;

namespace Firefly.CrossPlatformZip
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when duplicate entries (case sensitivity) are found when targeting Windows from a linux file system.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class DuplicateEntryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntryException"/> class.
        /// </summary>
        public DuplicateEntryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntryException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicateEntryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DuplicateEntryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntryException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected DuplicateEntryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the duplicate files.
        /// </summary>
        /// <value>
        /// The duplicate files.
        /// </value>
        public List<string> DuplicateFiles { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets the duplicate directories.
        /// </summary>
        /// <value>
        /// The duplicate directories.
        /// </value>
        public List<string> DuplicateDirectories { get; internal set; } = new List<string>();
    }
}
