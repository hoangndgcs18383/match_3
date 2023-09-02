using System.Collections.Generic;
// This file is auto-generated. Do not modify it manually.
namespace Zeff.Core.Parser
{
	public class ZParserStatic
	{
		private static Dictionary<string, IBaseParser> _parsers = new Dictionary<string, IBaseParser>();
		private static ZParserStatic _instance;
		public static ZParserStatic Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ZParserStatic();
				}
				return _instance;
			}
			private set => _instance = value;
		}
		public void AddParser(string key, IBaseParser parser)
		{
			if (_parsers.ContainsKey(key))
			{
				_parsers[key] = parser;
			}
			else
			{
				_parsers.Add(key, parser);
			}
		}
		private static T Get<T>(string key) where T : IBaseParser, new()
		{
			if (_parsers.ContainsKey(key))
			{
				return (T)_parsers[key];
			}
			return new T();
		}
		public ZBaseParser RegisterParser(string key)
		{
			switch (key)
			{
				case "GachaDesign": return new GachaDesignParser();
				case "ShopDesign": return new ShopDesignParser();
			}
			return null;
		}
		public GachaDesignParser GachaDesign => Get<GachaDesignParser>("GachaDesign");
		public ShopDesignParser ShopDesign => Get<ShopDesignParser>("ShopDesign");
	}
}
