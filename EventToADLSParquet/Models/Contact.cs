using System;
using System.ComponentModel.DataAnnotations;

namespace EventToADLSParquet.Models
{
    public class Contact
    {
        /// <summary>
        /// Gets or sets the contact identifier
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the contacts's first name
        /// </summary>
        /// <value>The contacts's first name.</value>
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the contacts's last name
        /// </summary>
        /// <value>The contacts's last name.</value>
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the contacts's display name
        /// </summary>
        /// <value>The contacts's display name.</value>
        public string displayName { get; set; }

        /// <summary>
        /// Contact created by name/service
        /// </summary>
        public string createdBy { get; set; }

        /// <summary>
        /// Date contact created
        /// </summary>
        public DateTime createdUtcDate { get; set; }

        /// <summary>
        /// Contact last modified by name/service
        /// </summary>
        public string lastModifiedBy { get; set; }

        /// <summary>
        /// Date contact last modified
        /// </summary>
        public DateTime lastModifiedUtcDate { get; set; }
    }
}