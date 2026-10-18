using Deporcheves.Domain.Entities;
using Deporcheves.Domain.Enums;

namespace Deporcheves.UnitTests.Domain;

public class SaleTests
{
    [Fact]
    public void AddItem_ProductoValido_ActualizaTotalYStock()
    {
        // Arrange
        var product = new Product { Name = "Sabritas", Price = 18.50m, Stock = 10 };
        var sale = new Sale { PaymentMethod = PaymentMethod.Cash };

        // Act
        sale.AddItem(product, 2);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(8, product.Stock); // Stock disminuido
        Assert.Equal(37.00m, sale.Total); // 18.50 * 2
        
        var item = sale.Items.First();
        Assert.Equal("Sabritas", item.ProductName); // Snapshot
        Assert.Equal(18.50m, item.UnitPrice);       // Snapshot
    }

    [Fact]
    public void AddItem_ProductoNulo_LanzaExcepcion()
    {
        var sale = new Sale { PaymentMethod = PaymentMethod.Cash };
        
        Assert.Throws<ArgumentNullException>(() => sale.AddItem(null!, 1));
    }
}