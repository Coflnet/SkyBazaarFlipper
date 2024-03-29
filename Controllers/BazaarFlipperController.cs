using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Coflnet.Sky.Bazaar.Flipper.Models;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Coflnet.Sky.Bazaar.Flipper.Services;

namespace Coflnet.Sky.Bazaar.Flipper.Controllers;

/// <summary>
/// Main Controller handling tracking
/// </summary>
[ApiController]
[Route("[controller]")]
public class BazaarFlipperController : ControllerBase
{
    private readonly BazaarFlipperService service;
    private readonly BookFlipService bookFlipService;

    /// <summary>
    /// Creates a new instance of <see cref="BazaarFlipperController"/>
    /// </summary>
    /// <param name="service"></param>
    public BazaarFlipperController(BazaarFlipperService service, BookFlipService bookFlipService)
    {
        this.service = service;
        this.bookFlipService = bookFlipService;
    }

    /// <summary>
    /// Tracks a flip
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/flips")]
    public async Task<List<BazaarFlip>> GetFlips()
    {
        return await service.GetFlips();
    }

    /// <summary>
    /// Gets book flips
    /// </summary>
    [HttpGet]
    [Route("/books")]
    public async Task<List<BookFlip>> GetBooks()
    {
        return await bookFlipService.GetFlips();
    }

    [HttpGet]
    [Route("/ready")]
    public bool Ready()
    {
        return service.Ready();
    }
}
