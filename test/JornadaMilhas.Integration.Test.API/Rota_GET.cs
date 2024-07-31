using JornadaMilhas.Dominio.Entidades;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

public class Rota_GET : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public Rota_GET(JornadaMilhasWebApplicationFactory app) => _app = app;

    [Fact]
    public async Task Recupera_Rota_PorId()
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
        var response = await client.GetFromJsonAsync<Rota>("/rota-viagem/" + rotaExistente.Id);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(rotaExistente.Origem, response.Origem);
        Assert.Equal(rotaExistente.Destino, response.Destino);
    }
}