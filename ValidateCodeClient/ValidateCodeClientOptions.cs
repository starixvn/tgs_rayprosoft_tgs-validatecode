using System;

namespace TGS.Services.ValidateCode.Clients
{
	public class ValidateCodeClientOptions
	{
		public Uri ValidateCodeBaseUrl { get; set; } = new Uri("https://localhost:5001");
	}
}
