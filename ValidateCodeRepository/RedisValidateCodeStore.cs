using StackExchange.Redis;
using TGS.Services.ValidateCode;

namespace TGS.Repositories.ValidateCode;

internal class RedisValidateCodeStore(RedisConnectionFactory redisConnectionFactory) : IValidateCodeStore
{
	private static readonly TimeSpan _Expire = TimeSpan.FromMinutes(10);
	private static readonly string _KeyPrefix = "TGS:ValidateCodes";
	private readonly IDatabase _redisDatabase = redisConnectionFactory.Database;

	public async ValueTask<string?> GetDataAsync(string validateCode, string token)
	{
		var dataKey = $"{_KeyPrefix}:{token}:{validateCode}";
		var value = await _redisDatabase
			.StringGetAsync(dataKey)
			.ConfigureAwait(false);

		_ = await _redisDatabase
			.KeyDeleteAsync(dataKey, flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);

		return value;
	}

	public async ValueTask<bool> SaveDataAsync(string validateCode, string token, string content)
	{
		return await _redisDatabase.StringSetAsync(
			$"{_KeyPrefix}:{token}:{validateCode}",
			content,
			_Expire,
			when: When.NotExists).ConfigureAwait(false);
	}
}
