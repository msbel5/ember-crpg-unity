using System;

// Design note:
// JobId is Ember's stable identity primitive for production jobs.
// It is string-backed to support save/load, imported settlement queues, and readable replay traces.
namespace EmberCrpg.Domain.Production
{
    /// <summary>
    /// Stable handle to a production job.
    /// </summary>
    public readonly struct JobId : IEquatable<JobId>
    {
        private readonly string _value;

        /// <summary>
        /// Creates a job identifier from a stable value.
        /// </summary>
        public JobId(string value)
        {
            _value = value ?? string.Empty;
        }

        /// <summary>
        /// Stable job identifier value.
        /// </summary>
        public string Value
        {
            get { return _value ?? string.Empty; }
        }

        /// <summary>
        /// True when this handle does not point to a production job.
        /// </summary>
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Value); }
        }

        /// <summary>
        /// Returns true when both job ids carry the same stable value.
        /// </summary>
        public bool Equals(JobId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when the object is a job id with the same stable value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is JobId other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code derived only from the stable job id.
        /// </summary>
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        /// <summary>
        /// Returns a compact debug label for this job id.
        /// </summary>
        public override string ToString()
        {
            return IsEmpty ? "JobId.Empty" : $"JobId({Value})";
        }

        /// <summary>
        /// Returns true when both job ids carry the same stable value.
        /// </summary>
        public static bool operator ==(JobId left, JobId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true when job ids carry different stable values.
        /// </summary>
        public static bool operator !=(JobId left, JobId right)
        {
            return !left.Equals(right);
        }
    }
}