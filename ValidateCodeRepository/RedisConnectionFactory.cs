using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace TGS.Repositories.ValidateCode
{
	internal class RedisConnectionFactory : IDisposable
	{
		private readonly ConnectionMultiplexer _connectionMultiplexer;

		private bool _disposedValue;

		public IDatabase Database { get; }

		public RedisConnectionFactory(IOptions<RedisOptions> optionsAccessor)
		{
			var options = optionsAccessor.Value;

			var connectionOptions = ConfigurationOptions.Parse(options.ConnectionString);

			_connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);

			Database = _connectionMultiplexer.GetDatabase();
		}

		public void Dispose()
		{
			// 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_connectionMultiplexer?.Dispose();
				}

				_disposedValue = true;
			}
		}
	}
}
