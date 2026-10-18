using Deporcheves.Domain.Entities;

namespace Deporcheves.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void DecreaseStock_ConStockSuficiente_DisminuyeElStock()
    {
        // Arrange (Preparar)
        var product = new Product { Name = "Cerveza", Stock = 10 };

        // Act (Actuar)
        product.DecreaseStock(3);

        // Assert (Verificar)
        Assert.Equal(7, product.Stock);
    }

    [Fact]
    public void DecreaseStock_ConStockInsuficiente_LanzaExcepcion()
    {
        // Arrange
        var product = new Product { Name = "Cerveza", Stock = 2 };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(5));
        
        Assert.Contains("Stock insuficiente", exception.Message);
        Assert.Equal(2, product.Stock); // El stock no debe haber cambiado
    }

    [Fact]
    public void DecreaseStock_ConCantidadNegativa_LanzaExcepcion()
    {
        // Arrange
        var product = new Product { Name = "Cerveza", Stock = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.DecreaseStock(-1));
    }
}