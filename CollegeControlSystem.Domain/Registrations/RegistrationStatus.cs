namespace CollegeControlSystem.Domain.Registrations
{
    public enum RegistrationStatus
    {
        /// <summary>
        /// Initial state. [cite_start]Waiting for Advisor approval (Rule 5, Article 5)[cite: 121].
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Approved by advisor or auto-approved. The student is officially in the class.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Student removed the course during the Add/Drop period.
        /// </summary>
        Dropped = 2,

        /// <summary>
        /// Student withdrew after the deadline (Result is 'W').
        /// </summary>
        Withdrawn = 3,

        /// <summary>
        /// The semester is over, and a final grade has been assigned.
        /// </summary>
        Completed = 4
    }
}
