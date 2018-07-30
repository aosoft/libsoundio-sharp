using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SoundIOSharp.Extensions
{
	public static class StreamPlayExtension
	{
		public static void PlaySound(this IEnumerable<float> stream, int sampleRate)
		{
			SoundIOOutStreamUtil.Play(stream, sampleRate);
		}

		public static void PlaySoundTest()
		{
			const int sampleRate = 48000;
			const int pitch = 440;
			Enumerable.Range(0, sampleRate * 2)
				.Select(x => (float)Math.Sin(2.0 * Math.PI * pitch * x / sampleRate))
				.PlaySound(sampleRate);
		}
	}
}
