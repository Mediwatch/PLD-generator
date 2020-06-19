using DotNetEnv;

namespace PLD_generator
{
	class Configuration {
		static public string AirtableApiKey = "";

		static public void Load()
		{
			DotNetEnv.Env.Load(".env");

			AirtableApiKey = DotNetEnv.Env.GetString("AIRTABLE_API_KEY");
		}
	}
}
