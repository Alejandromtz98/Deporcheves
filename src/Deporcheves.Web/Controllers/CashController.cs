using Deporcheves.Application.Dtos.Cash;
using Deporcheves.Application.Services;
using Deporcheves.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

public class CashController : Controller
{
    private readonly ICashService _cashService;

    public CashController(ICashService cashService)
    {
        _cashService = cashService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            // Intenta obtener el reporte del turno activo.
            var report = await _cashService.GetActiveSessionReportAsync(cancellationToken);
            return View("ActiveSession", report);
        }
        catch (InvalidOperationException)
        {
            // Si lanza excepcion, significa que no hay turno abierto.
            return View("NoSession", new OpenCashViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(OpenCashViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("NoSession", model);

        try
        {
            await _cashService.OpenSessionAsync(model.OpeningCash, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("NoSession", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMovement(CashMovementViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos del movimiento invalidos.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _cashService.RegisterMovementAsync(new CashMovementRequest
            {
                Type = model.Type,
                Amount = model.Amount,
                Reason = model.Reason
            }, cancellationToken);
            
            TempData["Success"] = "Movimiento registrado.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(CloseCashViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "El efectivo contado es invalido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var report = await _cashService.CloseSessionAsync(model.ClosingCash, cancellationToken);
            return View("Report", report); // Muestra el ticket de Cierre Z
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}