using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Data;

[SuppressMessage("Usage", "CS8168", Justification = "Only constructed in deserialization")]
public class Config {
	
	public const string PATH = "config.json";

	public string ConName { get; set; }
	public ulong GuildId { get; set; }

	public static Config Read() {
		string json = File.ReadAllText(PATH);
		var value = JsonSerializer.Deserialize<Config>(json);
		if (value is null) {
			throw new NullReferenceException("Config cannot be null");
		}
		return (Config) value;
	}
}