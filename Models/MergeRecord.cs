using System.ComponentModel.DataAnnotations;

namespace MergeApi.Models;

public class MergeRecord
{
    [Key]
    public int Id { get; set; }

    public string Array1 { get; set; }
    public string Array2 { get; set; }
    public string Result { get; set; }

    public int Length { get; set; }
    public DateTime Timestamp { get; set; }
}
