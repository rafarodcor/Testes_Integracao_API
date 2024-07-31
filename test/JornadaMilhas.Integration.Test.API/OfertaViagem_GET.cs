using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;
using JornadaMilhas.Integration.Test.API.DataBuilders;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace JornadaMilhas.Integration.Test.API;

public class OfertaViagem_GET : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private readonly JornadaMilhasWebApplicationFactory _app;

    public OfertaViagem_GET(JornadaMilhasWebApplicationFactory app) => _app = app;

    [Fact]
    public async Task Recupera_OfertaViagem_PorId()
    {
        //Arrange
        var ofertaExistente = _app.Context.OfertasViagem.FirstOrDefault();
        if (ofertaExistente is null)
        {
            ofertaExistente = new OfertaViagem()
            {
                Preco = 100,
                Rota = new Rota("Origem", "Destino"),
                Periodo = new Periodo(DateTime.Parse("2024-03-03"), DateTime.Parse("2024-03-06"))
            };
            _app.Context.Add(ofertaExistente);
            _app.Context.SaveChanges();
        }

        using var client = await _app.GetClientWithAccessTokenAsync();

        //Act
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/" + ofertaExistente.Id);

        //Assert
        Assert.NotNull(response);
        Assert.Equal(ofertaExistente.Preco, response.Preco, 0.001);
        Assert.Equal(ofertaExistente.Rota.Origem, response.Rota.Origem);
        Assert.Equal(ofertaExistente.Rota.Destino, response.Rota.Destino);
    }

    [Fact]
    public async Task Recupera_OfertaViagem_Na_Consulta_Paginada()
    {
        //Arrange
        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaDeOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 1;
        int tamanhoPorPagina = 80;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task Recupera_OfertaViagem_Consulta_Ultima_Pagina()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("Delete from OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(100);
        _app.Context.OfertasViagem.AddRange(listaDeOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 4;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Consulta_Com_Pagina_Inexistente()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("Delete from OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaDeOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = 5;
        int tamanhoPorPagina = 25;

        //Act
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");

        //Assert
        Assert.True(response != null);
        //Assert.Equal(0, response.Count());
        Assert.Empty(response);
    }

    [Fact]
    public async Task Recuperar_OfertasViagens_Consulta_Com_Pagina_Com_Valor_Negativo()
    {
        //Arrange
        _app.Context.Database.ExecuteSqlRaw("Delete from OfertasViagem");

        var ofertaDataBuilder = new OfertaViagemDataBuilder();
        var listaDeOfertas = ofertaDataBuilder.Generate(80);
        _app.Context.OfertasViagem.AddRange(listaDeOfertas);
        _app.Context.SaveChanges();

        using var client = await _app.GetClientWithAccessTokenAsync();

        int pagina = -5;
        int tamanhoPorPagina = 25;

        //Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem?pagina={pagina}&tamanhoPorPagina={tamanhoPorPagina}");
        });
    }
}