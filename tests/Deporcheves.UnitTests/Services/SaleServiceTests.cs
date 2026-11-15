using Deporcheves.Application.Abstractions.Persistence;
using Deporcheves.Application.Dtos.Sales;
using Deporcheves.Application.Services;
using Deporcheves.Domain.Entities;
using Moq;

namespace Deporcheves.UnitTests.Services;

public class SaleServiceTests
{
    [Fact]
    public async Task RegisterSaleAsync_SinCajaAbierta_LanzaExcepcion()
    {
        // Arrange
        var mockSaleRepo = new Mock<ISaleRepository>();
        var mockProductRepo = new Mock<IProductRepository>();
        var mockCashRepo = new Mock<ICashSessionRepository>();
        var mockInventoryService = new Mock<IInventoryService>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockReceivableRepo = new Mock<IReceivableRepository>();

        mockCashRepo.Setup(r => r.GetActiveSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CashSession?)null);

        var service = new SaleService(mockSaleRepo.Object, mockProductRepo.Object, mockCashRepo.Object, 
                                      mockInventoryService.Object, mockCustomerRepo.Object, mockReceivableRepo.Object);
        
        var request = new CreateSaleRequest 
        { 
            AmountCash = 100m, // Pago asignado
            Items = new List<CreateSaleItemRequest> { new() { ProductId = 1, Quantity = 1 } }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterSaleAsync(request, 1));

        Assert.Contains("Debe abrir la caja", exception.Message);
    }

    [Fact]
    public async Task RegisterSaleAsync_ConCajaAbiertaYProductoValido_RetornaVenta()
    {
        // Arrange
        var mockSaleRepo = new Mock<ISaleRepository>();
        var mockProductRepo = new Mock<IProductRepository>();
        var mockCashRepo = new Mock<ICashSessionRepository>();
        var mockInventoryService = new Mock<IInventoryService>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockReceivableRepo = new Mock<IReceivableRepository>();

        var activeSession = new CashSession { Id = 1 };
        var product = new Product { Id = 1, Name = "Test", Price = 100, Stock = 10 };

        mockCashRepo.Setup(r => r.GetActiveSessionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeSession);
        mockProductRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        mockSaleRepo.Setup(r => r.GetLastSaleNumberForDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var service = new SaleService(mockSaleRepo.Object, mockProductRepo.Object, mockCashRepo.Object, 
                                      mockInventoryService.Object, mockCustomerRepo.Object, mockReceivableRepo.Object);
        
        var request = new CreateSaleRequest 
        { 
            AmountCash = 200m, // El total será 200
            Items = new List<CreateSaleItemRequest> { new() { ProductId = 1, Quantity = 2 } }
        };

        // Act
        var result = await service.RegisterSaleAsync(request, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200m, result.Total);
        Assert.Equal("V-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-0001", result.SaleNumber);
        
        mockSaleRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}