using UnityEngine;

namespace Zeff.Extensions
{
    public static class TimeExtension
    {
        public static string ToTimeFormat(this float time)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
