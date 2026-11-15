using System.Diagnostics;
using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deporcheves.Web.Controllers;

[Authorize(Roles = "Admin")]
public class FinanceController : Controller
{
    private readonly IFinancialService _financialService;
    private readonly IFixedCostService _fixedCostService;
    private readonly ISaleRepository _saleRepository;

    public FinanceController(IFinancialService financialService, IFixedCostService fixedCostService, ISaleRepository saleRepository)
    {
        _financialService = financialService;
        _fixedCostService = fixedCostService;
        _saleRepository = saleRepository;
    }

    public async Task<IActionResult> Dashboard(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        //1. Forzar fecha actual UTC
        var now = DateTime.UtcNow;
        //2. Construir start especificando Kind para postgress
        var start = startDate.HasValue ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) : new DateTime(now.Year, now.Month,1,0,0,0,DateTimeKind.Utc);
        //3. Contruir end
        DateTime end;
        if(endDate.HasValue)
        {
            var endDateValue = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            end = endDateValue.Date.AddDays(1).AddTicks(-1);
        }
        else
            end = DateTime.SpecifyKind(now.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var model = await _financialService.GetDashBoardAsync(start, end, cancellationToken);
        
        // Pasamos las fechas a la vista para mantener el estado del filtro
        ViewData["StartDate"] = start.ToString("yyyy-MM-dd");
        ViewData["EndDate"] = end.ToString("yyyy-MM-dd");

        return View(model);
    }
    public async Task<IActionResult> FixedCosts(CancellationToken cancellationToken)
    {
        var costs = await _fixedCostService.GetAllAsync(cancellationToken);
        return View(costs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFixedCost(string name, decimal amount, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(name) && amount > 0)
        {
            await _fixedCostService.CreateAsync(name, amount, cancellationToken);
        }
        return RedirectToAction(nameof(FixedCosts));
    }
        [HttpGet]
    public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var start = startDate.HasValue 
            ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) 
            : new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            
        DateTime end;
        if (endDate.HasValue)
        {
            var endDateValue = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            end = endDateValue.Date.AddDays(1).AddTicks(-1);
        }
        else
        {
            end = DateTime.SpecifyKind(now.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
        }

        var sales = await _saleRepository.GetByDateRangeAsync(start, end, cancellationToken);

        var builder = new System.Text.StringBuilder();
        
        // NUEVAS CABECERAS PARA PAGOS MIXTOS
        builder.AppendLine("Ticket,Fecha,Efectivo,Transferencia,Tarjeta,Fiado,Total");

        foreach (var sale in sales)
        {
            var cash = sale.AmountCash.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var transfer = sale.AmountTransfer.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var card = sale.AmountCard.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var fiado = sale.AmountFiado.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var total = sale.Total.ToString(System.Globalization.CultureInfo.InvariantCulture);
            
            builder.AppendLine($"{sale.SaleNumber},{sale.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm},{cash},{transfer},{card},{fiado},{total}");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
        var fileName = $"Ventas_{start:yyyyMMdd}_{end:yyyyMMdd}.csv";
        
        return File(bytes, "text/csv", fileName);
    }
}