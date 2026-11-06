using ClosedXML.Excel;
using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;
using Deporcheves.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// ============================================================
// CONFIGURACIÓN DE LA BASE DE DATOS (CAMBIA ESTO SI ES NECESARIO)
// ============================================================
var connectionString = "Host=localhost;Port=5432;Database=deporcheves;Username=postgres;Password=postgres";
// var connectionString = "TU_CADENA_DE_NEON_AQUI";

var optionsBuilder = new DbContextOptionsBuilder<DeporchevesDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var context = new DeporchevesDbContext(optionsBuilder.Options);

// Abrimos el archivo Excel UNA SOLA VEZ y lo mantenemos abierto
using var workbook = new XLWorkbook("src/Deporcheves.DataMigration/Datos.xlsx");

Console.WriteLine("Iniciando Migración ETL (Extract, Transform, Load)...");

try
{
    // 1. Cargar Configuraciones
    Console.WriteLine("Migrando Configuraciones...");
    var configSheet = workbook.Worksheet("CONFIG");
    foreach (var row in configSheet.RowsUsed().Skip(1))
    {
        var key = row.Cell(1).GetString();
        var val = row.Cell(2).GetString();
        var desc = row.Cell(3).GetString();
        if (!string.IsNullOrEmpty(key) && !context.Settings.Any(s => s.Key == key))
        {
            context.Settings.Add(new Setting { Key = key, Value = val, Description = desc });
        }
    }
    context.SaveChanges();

    // 2. Migrar Proveedores
    Console.WriteLine("Migrando Proveedores...");
    var supplierMap = new Dictionary<string, int>();
    var supplierSheet = workbook.Worksheet("CAT_PROVEEDORES");
    foreach (var row in supplierSheet.RowsUsed().Skip(1))
    {
        var idExcel = row.Cell(1).GetString();
        if (string.IsNullOrEmpty(idExcel)) continue;

        var supplier = new Supplier
        {
            Name = row.Cell(2).GetString().Trim(),
            ContactName = row.Cell(3).GetString(),
            Phone = row.Cell(4).GetString(),
            IsActive = row.Cell(6).GetString() == "ACTIVO",
            CreatedAt = DateTime.UtcNow
        };
        context.Suppliers.Add(supplier);
        context.SaveChanges();
        supplierMap[idExcel] = supplier.Id;
    }
        // 2.1 Crear Proveedor Genérico por si hay compras sin proveedor
    var genericSupplier = new Supplier { Name = "Proveedor Genérico (Migración)", IsActive = true };
    context.Suppliers.Add(genericSupplier);
    context.SaveChanges();
    var genericSupplierId = genericSupplier.Id;

    // 3. Migrar Clientes (Para los fiados)
    Console.WriteLine("Creando Cliente Histórico para Fiados...");
    var historicalCustomer = new Customer { Name = "Cliente Histórico (Migración)" };
    if (!context.Customers.Any(c => c.Name == historicalCustomer.Name))
    {
        context.Customers.Add(historicalCustomer);
        context.SaveChanges();
    }
    else
    {
        historicalCustomer = context.Customers.First(c => c.Name == historicalCustomer.Name);
    }

    // 4. Migrar Productos
    Console.WriteLine("Migrando Productos...");
    var productMap = new Dictionary<string, int>();
    var productSheet = workbook.Worksheet("CAT_PRODUCTOS");
    foreach (var row in productSheet.RowsUsed().Skip(1))
    {
        var idExcel = row.Cell(1).GetString();
        if (string.IsNullOrEmpty(idExcel)) continue;

        var product = new Product
        {
            Sku = idExcel,
            Name = row.Cell(2).GetString().Trim(),
            Description = row.Cell(17).GetString(),
            Price = ParseDecimal(row.Cell(10).GetString()),
            Cost = ParseDecimal(row.Cell(9).GetString()),
            Stock = ParseInt(row.Cell(12).GetString()),
            MinimumStock = ParseInt(row.Cell(13).GetString()),
            IsActive = row.Cell(14).GetString() == "ACTIVO",
            CreatedAt = DateTime.UtcNow
        };
        
        context.Products.Add(product);
        context.SaveChanges();
        productMap[idExcel] = product.Id;
    }

    // 5. Migrar Empleados
    Console.WriteLine("Migrando Empleados...");
    var employeeMap = new Dictionary<string, int>();
    var payrollSheet = workbook.Worksheet("NOMINA");
    foreach (var row in payrollSheet.RowsUsed().Skip(1))
    {
        var name = row.Cell(5).GetString();
        if (string.IsNullOrEmpty(name) || employeeMap.ContainsKey(name)) continue;

        var employee = new Employee { Name = name, BaseWeeklySalary = ParseDecimal(row.Cell(6).GetString()) };
        context.Employees.Add(employee);
        context.SaveChanges();
        employeeMap[name] = employee.Id;
    }

    // 6. Migrar Cajas (Turnos Históricos)
    Console.WriteLine("Migrando Cajas y Turnos...");
    var cashSessionMap = new Dictionary<string, int>();
    var cashSheet = workbook.Worksheet("CAJA");
    foreach (var row in cashSheet.RowsUsed().Skip(1))
    {
        var dateStr = row.Cell(2).GetString();
        if (string.IsNullOrEmpty(dateStr)) continue;
        var date = ParseDate(dateStr);

        var session = new CashSession 
        { 
            OpenedAt = date.AddHours(8),
            ClosedAt = date.AddHours(22), 
            OpeningCash = ParseDecimal(row.Cell(3).GetString()),
            ExpectedCash = ParseDecimal(row.Cell(9).GetString()),
            ClosingCash = ParseDecimal(row.Cell(10).GetString()),
            Difference = ParseDecimal(row.Cell(11).GetString())
        };
        context.CashSessions.Add(session);
        context.SaveChanges();
        cashSessionMap[date.ToString("yyyy-MM-dd")] = session.Id;
    }

        // 7. Migrar Compras
    Console.WriteLine("Migrando Compras...");
    var purchaseSheet = workbook.Worksheet("COMPRAS");
    var purchaseGroups = purchaseSheet.RowsUsed().Skip(1)
        .GroupBy(r => r.Cell(1).GetString())
        .ToList();

    foreach (var group in purchaseGroups)
    {
        var firstRow = group.First();
        
        // Asignamos el proveedor AQUÍ DENTRO del bucle
        var supplierId = supplierMap.ContainsKey(firstRow.Cell(3).GetString()) 
            ? supplierMap[firstRow.Cell(3).GetString()] 
            : genericSupplierId;

        var purchase = new Purchase
        {
            PurchaseNumber = group.Key,
            SupplierId = supplierId, // Usamos la variable correctamente (con I mayúscula)
            CreatedAt = ParseDate(firstRow.Cell(2).GetString()),
            Notes = "Compra migrada del Excel",
        };
        
        foreach (var row in group)
        {
            var prodIdExcel = row.Cell(5).GetString();
            if (productMap.ContainsKey(prodIdExcel))
            {
                var unidades = ParseInt(row.Cell(8).GetString());
                var totalExcel = ParseDecimal(row.Cell(10).GetString());
                var costoUnitario = ParseDecimal(row.Cell(9).GetString());
                
                var subtotal = totalExcel > 0 ? totalExcel : (unidades * costoUnitario);
                var unitCostReal = unidades > 0 ? (subtotal / unidades) : 0;

                var item = new PurchaseItem
                {
                    ProductId = productMap[prodIdExcel],
                    ProductName = context.Products.Find(productMap[prodIdExcel])?.Name ?? "Desconocido",
                    Quantity = unidades,
                    UnitCost = unitCostReal,
                    Subtotal = subtotal,
                    Purchase = purchase 
                };
                context.PurchaseItems.Add(item);
                purchase.Total += subtotal;
            }
        }
        
        var pagoExcel = ParseDecimal(firstRow.Cell(14).GetString());
        
        if (pagoExcel > purchase.Total)
        {
            purchase.Total = pagoExcel; 
        }
        
        purchase.RegisterPayment(pagoExcel);
        context.Purchases.Add(purchase);
        context.SaveChanges();
    }


    // 8. Migrar Ventas
    Console.WriteLine("Migrando Ventas...");
    var salesEncSheet = workbook.Worksheet("VENTAS_ENC");
    var salesDetSheet = workbook.Worksheet("VENTAS_DET");
    var salesDetGroups = salesDetSheet.RowsUsed().Skip(1).GroupBy(r => r.Cell(2).GetString()).ToDictionary(g => g.Key, g => g.ToList());

    foreach (var row in salesEncSheet.RowsUsed().Skip(1))
    {
        var saleNumber = row.Cell(1).GetString();
        if (string.IsNullOrEmpty(saleNumber)) continue;

        var date = ParseDate(row.Cell(2).GetString());
        var dateKey = date.ToString("yyyy-MM-dd");
        var sessionId = cashSessionMap.ContainsKey(dateKey) ? cashSessionMap[dateKey] : context.CashSessions.FirstOrDefault()?.Id;

        var sale = new Sale
        {
            SaleNumber = saleNumber,
            CreatedAt = date,
            PaymentMethod = PaymentMethod.Cash,
            CashSessionId = sessionId,
            AmountFiado = ParseDecimal(row.Cell(9).GetString()),
            Total = ParseDecimal(row.Cell(5).GetString()),
            CustomerId = ParseDecimal(row.Cell(9).GetString()) > 0 ? historicalCustomer.Id : null
        };

        if (salesDetGroups.ContainsKey(saleNumber))
        {
            foreach (var detRow in salesDetGroups[saleNumber])
            {
                var prodIdExcel = detRow.Cell(4).GetString();
                if (productMap.ContainsKey(prodIdExcel))
                {
                    sale.AddItem(context.Products.Find(productMap[prodIdExcel])!, ParseInt(detRow.Cell(5).GetString()));
                }
            }
        }

        context.Sales.Add(sale);
        context.SaveChanges();

        if (sale.AmountFiado > 0)
        {
            var receivable = new AccountsReceivable(sale, historicalCustomer.Id, sale.AmountFiado);
            context.AccountsReceivables.Add(receivable);
            context.SaveChanges();
        }
    }

    // 9. Migrar Gastos
    Console.WriteLine("Migrando Gastos...");
    var expenseSheet = workbook.Worksheet("GASTOS");
    foreach (var row in expenseSheet.RowsUsed().Skip(1))
    {
        if (string.IsNullOrEmpty(row.Cell(1).GetString())) continue;
        context.Expenses.Add(new Expense
        {
            Date = ParseDate(row.Cell(2).GetString()),
            Category = row.Cell(3).GetString(),
            Description = row.Cell(4).GetString(),
            Amount = ParseDecimal(row.Cell(5).GetString()),
            PaymentMethod = row.Cell(7).GetString(),
            Status = row.Cell(8).GetString() == "PAGADO" ? ExpenseStatus.Paid : ExpenseStatus.Pending
        });
    }
    context.SaveChanges();

    // 10. Migrar Capital
    Console.WriteLine("Migrando Capital...");
    var capitalSheet = workbook.Worksheet("CAPITAL");
    foreach (var row in capitalSheet.RowsUsed().Skip(1))
    {
        if (string.IsNullOrEmpty(row.Cell(1).GetString())) continue;
        context.CapitalMovements.Add(new CapitalMovement
        {
            Date = ParseDate(row.Cell(2).GetString()),
            Type = row.Cell(3).GetString() == "RETIRO" ? CapitalMovementType.Withdrawal : CapitalMovementType.Contribution,
            OwnerName = row.Cell(4).GetString(),
            Amount = ParseDecimal(row.Cell(5).GetString()),
            PaymentMethod = row.Cell(6).GetString(),
            AuthorizedBy = row.Cell(7).GetString()
        });
    }
    context.SaveChanges();

    // 11. Migrar Nómina
    Console.WriteLine("Migrando Nóminas...");
    foreach (var row in payrollSheet.RowsUsed().Skip(1))
    {
        if (string.IsNullOrEmpty(row.Cell(1).GetString())) continue;
        var empName = row.Cell(5).GetString();
        if (!employeeMap.ContainsKey(empName)) continue;

        context.Payrolls.Add(new Payroll
        {
            EmployeeId = employeeMap[empName],
            PeriodStart = ParseDate(row.Cell(2).GetString()),
            PeriodEnd = ParseDate(row.Cell(3).GetString()),
            PaymentDate = ParseDate(row.Cell(4).GetString()),
            BaseSalary = ParseDecimal(row.Cell(6).GetString()),
            TotalCalculated = ParseDecimal(row.Cell(10).GetString()),
            AmountPaid = ParseDecimal(row.Cell(11).GetString()),
            Balance = ParseDecimal(row.Cell(12).GetString()),
            Status = row.Cell(14).GetString()
        });
    }
    context.SaveChanges();

    // 12. Migrar Kardex Histórico
    Console.WriteLine("Migrando Kardex Histórico...");
    var invSheet = workbook.Worksheet("MOV_INVENTARIO");
    foreach (var row in invSheet.RowsUsed().Skip(1))
    {
        if (string.IsNullOrEmpty(row.Cell(1).GetString())) continue;
        var prodIdExcel = row.Cell(3).GetString();
        if (!productMap.ContainsKey(prodIdExcel)) continue;

        var typeStr = row.Cell(4).GetString();
        var type = typeStr.Contains("VENTA") ? InventoryMovementType.Sale :
                   typeStr.Contains("COMPRA") ? InventoryMovementType.Purchase :
                   typeStr.Contains("INICIAL") ? InventoryMovementType.InitialLoad : InventoryMovementType.Adjustment;

        var entrada = ParseInt(row.Cell(5).GetString());
        var salida = ParseInt(row.Cell(6).GetString());
        var quantity = entrada > 0 ? entrada : -salida;

        context.InventoryMovements.Add(new InventoryMovement(
            productMap[prodIdExcel], 
            type, 
            quantity, 
            ParseDecimal(row.Cell(8).GetString()), 
            row.Cell(10).GetString(), 
            row.Cell(11).GetString()
        ));
    }
    context.SaveChanges();

    Console.WriteLine("¡Migración completada exitosamente!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error durante la migración: {ex.Message}");
    Console.WriteLine(ex.InnerException?.Message);
}

// Helpers
decimal ParseDecimal(string val)
{
    if (string.IsNullOrWhiteSpace(val) || val.Trim().ToUpper() == "PENDIENTE") return 0;
    if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)) return result;
    if (decimal.TryParse(val, NumberStyles.Any, new CultureInfo("es-MX"), out result)) return result;
    return 0;
}
int ParseInt(string val) => (int)ParseDecimal(val);
DateTime ParseDate(string val)
{
    DateTime result;
    if (DateTime.TryParse(val, new CultureInfo("es-MX"), DateTimeStyles.None, out result) ||
        DateTime.TryParse(val, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
    {
        // Forzamos que la fecha sea UTC para que PostgreSQL no lance excepción
        return DateTime.SpecifyKind(result, DateTimeKind.Utc);
    }
    return DateTime.UtcNow;
}