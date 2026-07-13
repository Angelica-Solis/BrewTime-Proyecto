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
            System.Diagnostics.Debug.WriteLine("========= CREATE =========");

            foreach (var e in dto.Estaciones)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"{e.Nombre} | Sel:{e.Seleccionada} | Orden:{e.Orden} | Tiempo:{e.TiempoEstimadoMin}");
            }

            try
            {
                // Ignorar validaciones de campos vacios en estaciones no seleccionadas
                for (int i = 0; i < dto.Estaciones.Count; i++)
                {
                    if (!dto.Estaciones[i].Seleccionada)
                    {
                        ModelState.Remove($"Estaciones[{i}].Orden");
                        ModelState.Remove($"Estaciones[{i}].TiempoEstimadoMin");
                    }
                }


                if (!ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState inválido");

                    foreach (var error in ModelState)
                    {
                        foreach (var e in error.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"{error.Key}: {e.ErrorMessage}"
                            );
                        }
                    }

                    await CargarDatosFormulario(dto);
                    return View(dto);
                }

                System.Diagnostics.Debug.WriteLine("Llamando al Service...");

                await _serviceProcesoPreparacion.CreateAsync(dto);

                System.Diagnostics.Debug.WriteLine("Service terminó correctamente");

                TempData["Success"] = "Proceso de preparación creado correctamente.";

                return RedirectToAction(nameof(Maintenance));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Entró al catch");
                System.Diagnostics.Debug.WriteLine(ex.Message);

                foreach (var error in ex.Message.Split('|'))
                {
                    ModelState.AddModelError("", error);
                }

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
            try
            {
                ModelState.Remove("Estaciones");

                if (!ModelState.IsValid)
                {
                    await CargarDatosFormularioEdit(dto, true);
                    return View(dto);
                }

                await _serviceProcesoPreparacion.UpdateAsync(dto);

                TempData["Success"] = "Proceso de preparación actualizado correctamente.";

                return RedirectToAction(nameof(Maintenance));
            }
            catch (Exception ex)
            {
                foreach (var error in ex.Message.Split('|'))
                {
                    ModelState.AddModelError("", error);
                }

                await CargarDatosFormularioEdit(dto, true);

                return View(dto);
            }
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
