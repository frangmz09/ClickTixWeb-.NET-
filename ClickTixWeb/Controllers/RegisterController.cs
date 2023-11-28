﻿using Microsoft.AspNetCore.Mvc;
using ClickTixWeb.Models; // Asegúrate de importar el espacio de nombres que contiene tu modelo de usuario

public class RegisterController : Controller
{
    private readonly ClicktixContext _context;

    public RegisterController(ClicktixContext context)
    {
        _context = context;
    }

    // GET: RegisterController
    public ActionResult Index()
    {
        return View();
    }

    // GET: RegisterController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: RegisterController/Create





    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(UsuarioWeb model)
    {
        if (ModelState.IsValid)
        {
            
            var usuarioWeb = new UsuarioWeb
            {
                
                nombre = model.nombre,
                apellido = model.apellido,
                pass = model.pass,
                email = model.email,
                fnac = model.fnac,
                genero = model.genero,
                celular = model.celular,
                sucursal_habitual = model.sucursal_habitual
            };

            _context.UsuarioWebs.Add(usuarioWeb);
            _context.SaveChanges();

            return RedirectToAction("Index", "Login");
        }

        return View(model);
    }







    // Otros métodos...

    private bool UsuarioExists(int id)
    {
        return _context.UsuarioWebs.Any(e => e.IdUsuario == id);
    }
}
