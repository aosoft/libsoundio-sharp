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
			using (var api = new SoundIO())
			{
				api.Connect();
				api.FlushEvents();
				var device = api.GetOutputDevice(api.DefaultOutputDeviceIndex);
				try
				{
					using (var waiter = new ManualResetEventSlim())
					using (var outstream = device.CreateOutStream())
					{
						using (var en = stream.GetEnumerator())
						{
							outstream.WriteCallback = (frameCountMin, frameCountMax) =>
							{
								int frameCount = frameCountMax;
								var layout2 = outstream.Layout;
								var results = outstream.BeginWrite(ref frameCount);
								try
								{
									for (int i = 0; i < frameCount; i++)
									{
										if (!en.MoveNext())
										{
											waiter.Set();
											return;
										}

										var value = en.Current;

										for (int c = 0; c < layout2.ChannelCount; c++)
										{
											unsafe
											{
												var area = results.GetArea(c);
												*((float*)area.Pointer) = value;
												area.Pointer += area.Step;
											}
										}
									}
								}
								catch
								{
									waiter.Set();
									throw;
								}
								finally
								{
									outstream.EndWrite();
								}
							};

							outstream.Format = SoundIODevice.Float32NE;
							outstream.SampleRate = sampleRate;
							outstream.SoftwareLatency = 0.0;

							outstream.Open();
							outstream.Start();

							waiter.Wait();
						}
					}
				}
				finally
				{
					device.RemoveReference();
				}
			}
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
