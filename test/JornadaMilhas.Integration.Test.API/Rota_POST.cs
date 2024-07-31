﻿using JornadaMilhas.Dominio.Entidades;
using System.Net.Http.Json;
using System.Net;

namespace JornadaMilhas.Integration.Test.API;

public class Rota_POST : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public Rota_POST(JornadaMilhasWebApplicationFactory app) => _app = app;

    [Fact]
    public async Task Cadastra_Rota()
    {
        //Arrange
        using var client = await _app.GetClientWithAccessTokenAsync();

        var rotaViagem = new Rota { Origem = "Origem", Destino = "Destino" };

        //Act
        var response = await client.PostAsJsonAsync("/rota-viagem", rotaViagem);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Cadastra_Rota_SemAutorizacao()
    {
        //Arrange
        using var client = _app.CreateClient();

        var rotaViagem = new Rota { Origem = "Origem", Destino = "Destino" };

        //Act
        var response = await client.PostAsJsonAsync("/rota-viagem", rotaViagem);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}