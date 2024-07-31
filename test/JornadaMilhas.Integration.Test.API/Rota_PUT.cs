using JornadaMilhas.Dominio.Entidades;
using System.Net;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

public class Rota_PUT : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public Rota_PUT(JornadaMilhasWebApplicationFactory app) => _app = app;

    [Fact]
    public async Task Atualiza_OfertaViagem_PorId()
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

        rotaExistente.Origem = "Origem atualizada";
        rotaExistente.Destino = "Destino atualizado";

        //Act
        var response = await client.PutAsJsonAsync($"/rota-viagem/", rotaExistente);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}