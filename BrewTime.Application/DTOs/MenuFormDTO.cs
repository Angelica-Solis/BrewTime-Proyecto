using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewTime.Application.DTOs
{
    public class MenuFormDTO : IValidatableObject
    {
        public int MenuId { get; set; }

        [Required(ErrorMessage = "El nombre del menú es obligatorio")]
        [StringLength(
            100,
            MinimumLength = 3,
            ErrorMessage = "El nombre debe contener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre del menú")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(
            500,
            ErrorMessage = "La descripción no puede superar los 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateOnly? FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha final es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha final")]
        public DateOnly? FechaFin { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de inicio")]
        public TimeOnly? HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora final es obligatoria")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora final")]
        public TimeOnly? HoraFin { get; set; }

        [Display(Name = "Menú activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Días disponibles")]
        public List<byte> DiasSeleccionados { get; set; } = new();


        // Valores enviados por los checkboxes
        public List<int> ProductosSeleccionados { get; set; } = new();

        public List<int> CombosSeleccionados { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (FechaInicio.HasValue &&
                FechaFin.HasValue &&
                FechaInicio.Value > FechaFin.Value)
            {
                yield return new ValidationResult(
                    "La fecha de inicio debe ser anterior o igual a la fecha final",
                    new[]
                    {
                        nameof(FechaInicio),
                        nameof(FechaFin)
                    });
            }

            if (HoraInicio.HasValue &&
                HoraFin.HasValue &&
                HoraInicio.Value >= HoraFin.Value)
            {
                yield return new ValidationResult(
                    "La hora final debe ser posterior a la hora de inicio",
                    new[]
                    {
                        nameof(HoraInicio),
                        nameof(HoraFin)
                    });
            }

            if (!ProductosSeleccionados.Any() &&
                !CombosSeleccionados.Any())
            {
                yield return new ValidationResult(
                    "Debe seleccionar al menos una opción",
                    new[]
                    {
                        nameof(ProductosSeleccionados),
                        nameof(CombosSeleccionados)
                    });
            }

            if (!DiasSeleccionados.Any())
            {
                yield return new ValidationResult(
                    "Debe seleccionar al menos un día disponible",
                    new[] { nameof(DiasSeleccionados) });
            }
        }
    }
}
