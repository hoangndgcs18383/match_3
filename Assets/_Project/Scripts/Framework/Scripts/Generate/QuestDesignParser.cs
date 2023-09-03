using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Zeff.Extensions;

namespace Zeff.Core.Parser
{
	[Serializable]
	public struct QuestDesignData
	{
		public string ID;
		public string Name;
		public string Description;
		public string Collection;
		public string Rewards;
	}
	public class QuestDesignParser : ZBaseParser
	{
		private Dictionary<string, QuestDesignData> _questDesigns;

		public override void LoadData(string data)
		{
			base.LoadData(data);
			// TODO: Parse data
			_questDesigns = new Dictionary<string, QuestDesignData>();

			try
			{
				_questDesigns = JsonConvert.DeserializeObject<Dictionary<string, QuestDesignData>>(data);
				
				DebugFormat.ToString(_questDesigns);
			}
			catch (Exception e)
			{
				Debug.LogError("Error while parsing QuestDesign: " + e.Message);
			}
		}
	}
}
