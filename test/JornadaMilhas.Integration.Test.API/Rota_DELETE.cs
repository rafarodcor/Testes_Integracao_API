using JornadaMilhas.Dominio.Entidades;
using System.Net;

namespace JornadaMilhas.Integration.Test.API;

public class Rota_DELETE : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public Rota_DELETE(JornadaMilhasWebApplicationFactory app) => _app = app;

    [Fact]
    public async Task Deletar_Rota_PorId()
    {
        //Arrange
        var rotaExistente = _app.Context.Rota.FirstOrDefault();
        if (rotaExistente is null)
        {
            rotaExistente = new Rota()
            {
                Origem = "Origem",
                Destino = "Destino"
            };
            _app.Context.Add(rotaExistente);
            _app.Context.SaveChanges();
        }

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.DeleteAsync("/rota-viagem/" + rotaExistente.Id);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}