using BrewTime.Application.DTOs;
using BrewTime.Application.Services.Implementations;
using BrewTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BrewTime.Web.Controllers
{
    public class ProcesoPreparacionController : Controller
    {
        private readonly IServiceProcesoPreparacion _serviceProcesoPreparacion;
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceEstacionCocina _serviceEstacionCocina;
 
        public ProcesoPreparacionController(IServiceProcesoPreparacion service, IServiceProducto serviceProducto, IServiceEstacionCocina serviceEstacion)
        {
            _serviceProcesoPreparacion = service;
            _serviceProducto = serviceProducto;
            _serviceEstacionCocina = serviceEstacion;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lista = await _serviceProcesoPreparacion.ListadoProcesosAsync();

            return View(lista);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var detalle = await _serviceProcesoPreparacion.DetailByProductoAsync(id);

            if (detalle == null)
                return NotFound();

            return View(detalle);
        }

        // mantenimientos

        [HttpGet]
        public async Task<IActionResult> Maintenance()
        {
            var procesos = await _serviceProcesoPreparacion.ListadoProcesosAsync();

            return View(procesos);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var dto = new ProcesoPreparacionFormDTO();

            await CargarDatosFormulario(dto);

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProcesoPreparacionFormDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarDatosFormulario(dto);
                    return View(dto);
                }

                await _serviceProcesoPreparacion.CreateAsync(dto);

                TempData["Success"] = "Proceso de preparación creado correctamente.";

                return RedirectToAction(nameof(Maintenance));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                await CargarDatosFormulario(dto);

                return View(dto);
            }

        }


        

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // id corresponde al ProductoId
            var dto = await _serviceProcesoPreparacion
                .FindFormEditByProductoIdAsync(id);

            if (dto == null)
            {
                return NotFound();
            }

            await CargarDatosFormularioEdit(dto,true);

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProcesoPreparacionFormDTO dto)
        {
            ModelState.Remove("Estaciones");

            if (!ModelState.IsValid)
            {
                await CargarDatosFormularioEdit(dto,true);
                return View(dto);
            }

            await _serviceProcesoPreparacion.UpdateAsync(dto);
            TempData["Success"] = "Proceso de preparación actualizado correctamente.";
            return RedirectToAction(nameof(Maintenance));
        }

        // helper

        private async Task CargarDatosFormulario(ProcesoPreparacionFormDTO? dto)
        {
            dto ??= new ProcesoPreparacionFormDTO();

            // Productos
            var productos = await _serviceProducto.ProductosSinProcesoAsync();

            ViewBag.Productos = new SelectList(
                productos,
                "ProductoID",
                "Nombre",
                dto.ProductoId
            );

            // Estaciones
            var estaciones = await _serviceEstacionCocina.ListAsync();

            // solo si viene vacío (Create GET)
            if (!dto.Estaciones.Any())
            {
                dto.Estaciones = estaciones.Select(e => new EstacionProcesoDTO
                {
                    EstacionId = e.EstacionId,
                    Nombre = e.Nombre,
                    Seleccionada = false,
                    Orden = 0,
                    TiempoEstimadoMin = 0
                }).ToList();
            }
        }
        private async Task CargarDatosFormularioEdit(ProcesoPreparacionFormDTO? dto,bool esEdit = false)
        {
            dto ??= new ProcesoPreparacionFormDTO();


            var productos = esEdit
                ? await _serviceProducto.ListAsync()
                : await _serviceProducto.ProductosSinProcesoAsync();



            ViewBag.Productos = new SelectList(
                productos,
                "ProductoID",
                "Nombre",
                dto.ProductoId
            );



            var estaciones = await _serviceEstacionCocina.ListAsync();


            if (!dto.Estaciones.Any())
            {
                dto.Estaciones = estaciones.Select(e => new EstacionProcesoDTO
                {
                    EstacionId = e.EstacionId,
                    Nombre = e.Nombre,
                    Seleccionada = false,
                    Orden = 0,
                    TiempoEstimadoMin = 0
                }).ToList();
            }
        }
    }
}
