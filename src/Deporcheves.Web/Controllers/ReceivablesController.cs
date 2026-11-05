using Deporcheves.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

[Authorize]
public class ReceivablesController : Controller
{
    private readonly IReceivableService _service;

    public ReceivablesController(IReceivableService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var debts = await _service.GetPendingAsync(cancellationToken);
        return View(debts);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int id, decimal amount, CancellationToken cancellationToken)
    {
        try
        {
            await _service.RegisterPaymentAsync(id, amount, cancellationToken);
            TempData["Success"] = "Cobro registrado exitosamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}