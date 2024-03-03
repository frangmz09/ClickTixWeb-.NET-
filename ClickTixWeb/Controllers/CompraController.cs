﻿using ClickTixWeb.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClickTixWeb.Controllers
{
    public class CompraController : Controller
    {
        private readonly ClicktixContext _context;
        private readonly ILogger<DetalleController> _logger;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;


        public CompraController(ILogger<DetalleController> logger, ClicktixContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost]
        public ActionResult ConfirmarCompra(IFormCollection formCollection)
        {
            var funcionId = int.Parse(formCollection["funcionId"]);
            var precioMomento = decimal.Parse(formCollection["precioMomento"]);
            var email = formCollection["emailHidden"];
            var asientosIds = formCollection["asientosId"].Select(id => int.Parse(id)).ToList();

            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine(funcionId);
            Console.WriteLine(email);
            Console.WriteLine(precioMomento);
            Console.WriteLine(asientosIds);
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("-----------------------------------");

            string UserId = ObtenerUid(email);
            int idAsetear = _context.Tickets.OrderByDescending(ticket => ticket.Id).Select(ticket => ticket.Id).FirstOrDefault();
            string abreviatura = ObtenerAbreviaturaSucursalPorIdFuncion(funcionId, _context);
            foreach (var idAsiento in asientosIds)
            {
                idAsetear++;

                DateTime fechaActual = DateTime.Now;
                var nuevoTicket = new Ticket
                {
                        Id = idAsetear,
                        IdFuncion = funcionId,
                        Fecha = fechaActual,
                        Fila = ObtenerFilaPorId(idAsiento), 
                        Columna = ObtenerColumnaPorId(idAsiento), 
                        PrecioAlMomento = (double)precioMomento,
                        UidFb = UserId,
                        IdLabel = abreviatura + idAsetear.ToString("D10"),
                        IsWithdrawn = false,
                    };
                var entry = _context.Entry(nuevoTicket);
                if (entry.State != EntityState.Detached)
                {
                    entry.State = EntityState.Detached;
                }

                _context.Tickets.Add(nuevoTicket);
            }
            _context.SaveChanges();

            foreach (var asientoId in asientosIds)
            {
                var asiento = _context.Asientos.SingleOrDefault(a => a.Id == asientoId);

                if (asiento != null)
                {
                    asiento.Disponible = 0;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); 
        }
        public static string ObtenerAbreviaturaSucursalPorIdFuncion(int idFuncion, ClicktixContext dbContext)
        {
            var funcion = dbContext.Funcions
            .Include(f => f.IdSalaNavigation)
                .ThenInclude(s => s.IdSucursalNavigation)
            .FirstOrDefault(f => f.Id == idFuncion);

            if (funcion != null && funcion.IdSalaNavigation != null && funcion.IdSalaNavigation.IdSucursalNavigation != null)
            {
                return funcion.IdSalaNavigation.IdSucursalNavigation.Abreviatura;
            }
            else
            {
                return "";
            }
        }
        public int ObtenerFilaPorId(int asientoId)
        {
            var fila = _context.Asientos
                .Where(asiento => asiento.Id == asientoId)
                .Select(asiento => asiento.Fila)
                .FirstOrDefault();

            return (int)fila;
        }
        public int ObtenerColumnaPorId(int asientoId)
        {
            var fila = _context.Asientos
                .Where(asiento => asiento.Id == asientoId)
                .Select(asiento => asiento.Columna)
                .FirstOrDefault();

            return (int)fila;
        }
        public string ObtenerUid(string email)
        {
            var userRecord = auth.GetUserByEmailAsync(email).Result;

            return userRecord.Uid;

        }

        // GET: CompraController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CompraController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompraController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompraController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompraController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CompraController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompraController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CompraController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
