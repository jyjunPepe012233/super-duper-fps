using System;
using TMPro;
using Object = UnityEngine.Object;

namespace SDFPS.UI.Utils
{

	[Serializable]
	public struct FormatableTextMeshProUGUI
	{
		public TextMeshProUGUI textMeshPro;
		public string format;

		public FormatableTextMeshProUGUI(string format)
		{
			this.textMeshPro = null;
			this.format = format;
		}

		public void SetText(params object[] args)
		{
			textMeshPro.text = String.Format(format, args);
		}
	}

}