using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Moq;
using TranslationManagement.Api.Controlers;
using TranslationManagement.Api.Dtos;
using TranslationManagement.Api.RequestHelpers;

namespace TranslationManagement.Api.Tests.Controllers;
public class TranslatorManagementControllerTests
{
    private readonly Mock<ITranslatorManagementRepository> _translatorRepo;
    private readonly Mock<ILogger<TranslatorManagementController>> _logger;
    private readonly Fixture _fixture;
    private readonly TranslatorManagementController _controller;
    private readonly IMapper _mapper;

    public TranslatorManagementControllerTests()
    {
        _fixture = new Fixture();
        _translatorRepo = new Mock<ITranslatorManagementRepository>();

        _logger = new Mock<ILogger<TranslatorManagementController>>();

        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        _mapper = new Mapper(mockMapper);
        _controller = new TranslatorManagementController(_translatorRepo.Object, _logger.Object, _mapper);
    }

    [Fact]
    public async Task GetTranslators_WithNoParams_Returns10Translators()
    {
        // arrange
        var translators = _fixture.CreateMany<TranslatorDto>(10).ToList();
        _translatorRepo.Setup(repo => repo.GetTranslatorsAsync()).ReturnsAsync(translators);

        // act
        var result = await _controller.GetTranslators();
        var count = ((result.Result as OkObjectResult)!.Value as List<TranslatorDto>)!.Count;

        // assert
        Assert.Equal(10, count);
        Assert.IsType<ActionResult<List<TranslatorDto>>>(result);
    }

    [Fact]
    public async Task GetTranslatorByNameAsync_WithValidName_ReturnsTranslator()
    {
        // arrange
        var translator = _fixture.Create<TranslatorDto>();
        _translatorRepo.Setup(repo => repo.GetTranslatorByNameAsync(It.IsAny<string>())).ReturnsAsync(translator);

        // act
        var result = await _controller.GetTranslatorByNameAsync(translator.Name);
        var Name = ((result.Result as OkObjectResult)!.Value as TranslatorDto)!.Name;

        // assert
        Assert.Equal(translator.Name, Name);
        Assert.IsType<ActionResult<TranslatorDto>>(result);
    }

    [Fact]
    public async Task GetTranslatorByNameAsync_WithInvalidName_ReturnsNotFound()
    {
        // arrange
        _translatorRepo.Setup(repo => repo.GetTranslatorByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(value: null);

        // act
        var result = await _controller.GetTranslatorByNameAsync("John Doe");

        // assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

}