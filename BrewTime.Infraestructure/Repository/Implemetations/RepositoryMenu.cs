using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewTime.Infraestructure.Data;
using BrewTime.Infraestructure.Models;
using BrewTime.Infraestructure.Repository.Implemetations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrewTime.Infraestructure.Repository.Implemetations
{
    public class RepositoryMenu : IRepositoryMenu
    {
        private readonly BrewTimeContext _context;

        public RepositoryMenu(BrewTimeContext context)
        {
            _context = context;
        }

        private IQueryable<Menu> ConsultaCompleta()
        {
            return _context.Set<Menu>()
                .Include(x => x.MenuDiaSemana)

                .Include(x => x.MenuProducto)
                    .ThenInclude(x => x.Producto)
                        .ThenInclude(x => x.Categoria)

                .Include(x => x.MenuCombo)
                    .ThenInclude(x => x.Combo)
                        .ThenInclude(x => x.Categoria);
        }

        public async Task<ICollection<Menu>> ListAsync()
        {
            return await ConsultaCompleta()
                .AsNoTracking()

                // Los menús con fecha de inicio más reciente aparecen primero
                .OrderByDescending(x => x.FechaInicio)
                .ThenByDescending(x => x.FechaFin)
                .ThenByDescending(x => x.FechaCreacion)

                .ToListAsync();
        }

        public async Task<Menu?> FindByIdAsync(int id)
        {
            return await ConsultaCompleta()
                .FirstOrDefaultAsync(x => x.MenuId == id);
        }

        public async Task<Menu?> FindDisponibleAsync(
            DateOnly fechaActual,
            TimeOnly horaActual)
        {
            var menusActivos = await ConsultaCompleta()
                .AsNoTracking()
                .Where(x => x.Activo)
                .ToListAsync();

            byte diaActual = fechaActual.DayOfWeek == DayOfWeek.Sunday
                ? (byte)7
                : (byte)fechaActual.DayOfWeek;

            var menuDisponible = menusActivos

                .Where(x =>
                    x.FechaInicio.HasValue &&
                    x.FechaFin.HasValue)

                .Where(x =>
                    x.FechaInicio!.Value <= fechaActual &&
                    x.FechaFin!.Value >= fechaActual)

                .Where(x =>
                    x.HoraInicio <= horaActual &&
                    x.HoraFin >= horaActual)

                .Where(x =>
                    x.MenuDiaSemana == null ||
                    !x.MenuDiaSemana.Any() ||
                    x.MenuDiaSemana.Any(d =>
                        d.DiaSemana == diaActual))

                // Regla 1: fecha de inicio más reciente
                .OrderByDescending(x =>
                    x.FechaInicio!.Value)

                // Regla 2: rango de fechas más corto
                .ThenBy(x =>
                    x.FechaFin!.Value.DayNumber -
                    x.FechaInicio!.Value.DayNumber)

                // Regla 3: horario iniciado más recientemente
                .ThenByDescending(x =>
                    x.HoraInicio)

                // Regla 4: registro creado más recientemente
                .ThenByDescending(x =>
                    x.FechaCreacion)

                // Solo devuelve un menú
                .FirstOrDefault();

            return menuDisponible;
        }

        public async Task CreateAsync(Menu entity, ICollection<int> productoIds, ICollection<int> comboIds, ICollection<byte> dias)
        {
            await using var transaction =
       await _context.Database.BeginTransactionAsync();

            try
            {
                entity.Nombre = entity.Nombre.Trim();
                entity.Descripcion = entity.Descripcion?.Trim();
                entity.FechaCreacion = DateTime.Now;

                entity.MenuProducto = productoIds
                    .Distinct()
                    .Select(productoId => new MenuProducto
                    {
                        ProductoId = productoId
                    })
                    .ToList();

                entity.MenuCombo = comboIds
                    .Distinct()
                    .Select(comboId => new MenuCombo
                    {
                        ComboId = comboId
                    })
                    .ToList();

                entity.MenuDiaSemana = dias
            .Distinct()
            .Where(dia => dia >= 1 && dia <= 7)
            .Select(dia => new MenuDiaSemana
            {
                DiaSemana = dia
            })
            .ToList();

                await _context.Set<Menu>().AddAsync(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Menu entity, ICollection<int> productoIds, ICollection<int> comboIds, ICollection<byte> dias)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {

                //La entidad ya viene rastreada porque fue obtenida mediante FindByIdAsync.


                entity.Nombre = entity.Nombre.Trim();
                entity.Descripcion = entity.Descripcion?.Trim();

                var productoIdsUnicos = productoIds
                    .Distinct()
                    .ToHashSet();

                var comboIdsUnicos = comboIds
                    .Distinct()
                    .ToHashSet();

                // ── Productos eliminados ─────────────────────

                var productosEliminar = entity.MenuProducto
                    .Where(x =>
                        !productoIdsUnicos.Contains(x.ProductoId))
                    .ToList();

                _context.Set<MenuProducto>()
                    .RemoveRange(productosEliminar);

                // ── Productos nuevos ─────────────────────────

                var productosActuales = entity.MenuProducto
                    .Select(x => x.ProductoId)
                    .ToHashSet();

                foreach (int productoId in productoIdsUnicos
                             .Except(productosActuales))
                {
                    entity.MenuProducto.Add(new MenuProducto
                    {
                        MenuId = entity.MenuId,
                        ProductoId = productoId
                    });
                }

                // ── Combos eliminados ────────────────────────

                var combosEliminar = entity.MenuCombo
                    .Where(x =>
                        !comboIdsUnicos.Contains(x.ComboId))
                    .ToList();

                _context.Set<MenuCombo>()
                    .RemoveRange(combosEliminar);

                // ── Combos nuevos ────────────────────────────

                var combosActuales = entity.MenuCombo
                    .Select(x => x.ComboId)
                    .ToHashSet();

                foreach (int comboId in comboIdsUnicos
                             .Except(combosActuales))
                {
                    entity.MenuCombo.Add(new MenuCombo
                    {
                        MenuId = entity.MenuId,
                        ComboId = comboId
                    });
                }

                //dias
                var diasUnicos = dias
    .Where(dia => dia >= 1 && dia <= 7)
    .Distinct()
    .ToHashSet();

                // Días eliminados
                var diasEliminar = entity.MenuDiaSemana
                    .Where(x => !diasUnicos.Contains(x.DiaSemana))
                    .ToList();

                _context.Set<MenuDiaSemana>()
                    .RemoveRange(diasEliminar);

                // Días existentes
                var diasActuales = entity.MenuDiaSemana
                    .Select(x => x.DiaSemana)
                    .ToHashSet();

                // Días nuevos
                foreach (byte dia in diasUnicos.Except(diasActuales))
                {
                    entity.MenuDiaSemana.Add(new MenuDiaSemana
                    {
                        MenuId = entity.MenuId,
                        DiaSemana = dia
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ToggleActivoAsync(int id)
        {
            var entity = await _context.Set<Menu>()
                .FirstOrDefaultAsync(x => x.MenuId == id);

            if (entity == null)
                return;

            entity.Activo = !entity.Activo;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteNombreAsync( string nombre,int? menuIdExcluir = null)
        {
            string nombreNormalizado = nombre.Trim().ToLower();

            return await _context.Set<Menu>()
                .AnyAsync(x =>
                    x.Nombre.ToLower() == nombreNormalizado &&
                    (!menuIdExcluir.HasValue ||
                     x.MenuId != menuIdExcluir.Value));
        }
    }
}
