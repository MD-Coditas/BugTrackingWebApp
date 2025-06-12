using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Infrastructure.Models;

public partial class Comment
{
    [Key]
    public Guid Id { get; set; }

    public Guid BugId { get; set; }

    public Guid UserId { get; set; }

    public string Message { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("BugId")]
    [InverseProperty("Comments")]
    public virtual Bug Bug { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Comments")]
    public virtual User User { get; set; } = null!;
}
