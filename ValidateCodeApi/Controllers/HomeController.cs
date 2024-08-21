using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

[Route("/")]
public class HomeController : ControllerBase
{
	[Route("/")]
	[HttpGet]
	[Description("Home")]
	[AllowAnonymous]
	public async Task<IActionResult> Index()
	{
		return Ok("Validate Code API");
	}
}