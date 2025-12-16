using CodeMedic.Utilities;
using CodeMedic.Commands;

namespace CodeMedic.Commands;

/// <summary>
/// Handles configuration-related commands.
/// </summary>
public class ConfigurationCommandHandler
{
	private PluginLoader _PluginLoader;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConfigurationCommandHandler"/> class.
	/// </summary>
	/// <param name="pluginLoader">The plugin loader instance used to manage plugins.</param>
	public ConfigurationCommandHandler(PluginLoader pluginLoader)
	{
		_PluginLoader = pluginLoader;
	}

	/// <summary>
	/// Handles the configuration file command.
	/// </summary>
	/// <param name="configFilePath">The path to the configuration file.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the exit code.</returns>
	internal async Task<int> HandleConfigurationFileAsync(string configFilePath)
	{

		// Check if the configuration file exists
		if (!File.Exists(configFilePath))
		{
			RootCommandHandler.Console.RenderError($"Configuration file not found: {configFilePath}");
			return 1; // Return a non-zero exit code to indicate failure
		}

		// Load the specified configuration file - we need to identify the file type and load accordingly
		try {
		var config = LoadConfigurationFromFile(configFilePath);
		if (config == null)
		{
			RootCommandHandler.Console.RenderError($"Failed to load configuration from file: {configFilePath}");
			return 1; // Return a non-zero exit code to indicate failure
		}
		} catch (Exception ex)
		{
			RootCommandHandler.Console.RenderError($"Error loading configuration file: {ex.Message}");
			return 1; // Return a non-zero exit code to indicate failure
		}

		// TODO: Use the loaded configuration to perform further actions
		return 0; // Return zero to indicate success

	}

	private CodeMedicRunConfiguration LoadConfigurationFromFile(string configFilePath)
	{
		// detect if the file is json or yaml based on extension
		var extension = Path.GetExtension(configFilePath).ToLower();
		var fileContents = File.ReadAllText(configFilePath);
		if (extension == ".json")
		{
			var config = System.Text.Json.JsonSerializer.Deserialize<CodeMedicRunConfiguration>(fileContents);
			if (config == null)
			{
				throw new InvalidOperationException("Failed to deserialize JSON configuration file.");
			}
			return config;
		}
		else if (extension == ".yaml" || extension == ".yml")
		{
			var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
			var config = deserializer.Deserialize<CodeMedicRunConfiguration>(fileContents);
			if (config == null)
			{
				throw new InvalidOperationException("Failed to deserialize YAML configuration file.");
			}
			return config;
		}
		else
		{
			throw new InvalidOperationException("Unsupported configuration file format. Only JSON and YAML are supported.");
		}
	}

}
