using System.ComponentModel.DataAnnotations.Schema;

namespace MicroRabbit.Transfer.Domain.Models;

public class TransferLog
{
    public int Id { get; set; }
    public int AccountFrom { get; set; }
    public int AccountTo { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal TransferAmount { get; set; }
}
