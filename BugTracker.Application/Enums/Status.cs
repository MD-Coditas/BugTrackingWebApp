using System;
using System.ComponentModel.DataAnnotations;


namespace BugTracker.Application.Enums
{
    public enum Status
    {
        Open,
        [Display(Name = "In Progress")]
        InProgress,
        Resolved,
        Closed
    }
}
