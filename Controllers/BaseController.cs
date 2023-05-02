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

    /// <summary>
    /// Creates a new instance of <see cref="BazaarFlipperController"/>
    /// </summary>
    /// <param name="service"></param>
    public BazaarFlipperController(BazaarFlipperService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Tracks a flip
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/flips")]
    public async Task<Flip> GetFlips()
    {
        return await service.GetFlips();
    }
}
