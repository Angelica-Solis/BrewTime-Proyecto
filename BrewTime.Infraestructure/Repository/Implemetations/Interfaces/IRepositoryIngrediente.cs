using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Models;

namespace BrewTime.Infraestructure.Repository.Implemetations.Interfaces
{
    public interface IRepositoryIngrediente
    {
        // Catálogo completo de ingredientes activos (para los checkboxes del formulario)
        Task<ICollection<Ingrediente>> ListActivasAsync();

        // Trae los ingredientes existentes que el usuario marcó en el formulario
        Task<ICollection<Ingrediente>> FindByIdsAsync(IEnumerable<int> ids);

        // Busca un ingrediente por nombre
        // Si no existe, lo crea con un INSERT independiente y lo devuelve.
        // De esta forma cada ingrediente nuevo que escribe el usuario se guarda
        // en su propia operación, uno por uno, y nunca como texto libre dentro de Producto.
        Task<Ingrediente> ObtenerOCrearAsync(string nombre);
    }
}
