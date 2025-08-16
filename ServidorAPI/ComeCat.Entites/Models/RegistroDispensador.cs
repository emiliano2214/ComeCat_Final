using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ComeCat.Entites.Models;

[Table("RegistroDispensador")]
public partial class RegistroDispensador
{
    [Key]
    public int Id { get; set; }

    public double Proximidad { get; set; }

    public int ServoActivo { get; set; }

    public string FechaDispensacion { get; set; } = null!;

    public string HoraDispensacion { get; set; } = null!;
}
