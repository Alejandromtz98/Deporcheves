using Deporcheves.Domain.Entities;

namespace Deporcheves.UnitTests.Domain;

public class SaleTests
{
    [Fact]
    public void AddItem_ProductoValido_ActualizaTotalYNoModificaStock()
    {
        // Arrange
        var product = new Product { Name = "Sabritas", Price = 18.50m, Stock = 10 };
        var sale = new Sale { AmountCash = 37.00m }; // Inicializamos con el pago en efectivo

        // Act
        sale.AddItem(product, 2);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(10, product.Stock); // El stock ya no se modifica en la entidad Sale
        
        Assert.Equal(37.00m, sale.Total); // 18.50 * 2
        
        var item = sale.Items.First();
        Assert.Equal("Sabritas", item.ProductName);
        Assert.Equal(18.50m, item.UnitPrice);
    }

    [Fact]
    public void AddItem_ProductoNulo_LanzaExcepcion()
    {
        var sale = new Sale { AmountCash = 0 };
        
        Assert.Throws<ArgumentNullException>(() => sale.AddItem(null!, 1));
    }
}