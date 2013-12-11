using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Tangra.Model.Image;
using Tangra.Model.Video;
using Tangra.PInvoke;

namespace Tangra.Video
{
	public class FramePlayer : IFramePlayer, IDisposable
	{
		private IFrameStream m_VideoStream;

		private int m_MillisecondsPerFrame;
		private bool m_IsRunning;
		private uint m_Step = 1;

		private IVideoFrameRenderer m_FrameRenderer;

		private Thread m_PlayerThread;

		public delegate void SimpleDelegate();
		
		public IFrameStream Video
		{
			get { return m_VideoStream; }
		}

		/// <summary>Returns the current playback status</summary>
		public bool IsRunning
		{
			get { return m_IsRunning; }
		}

		public void DisposeResources()
		{
			Dispose();
		}

		private FrameIntegratingMode m_FrameIntegration;
		private PixelIntegrationType m_PixelIntegrationMode;
		private int m_FramesToIntegrate;

		public PixelIntegrationType PixelIntegrationMode
		{
			get { return m_PixelIntegrationMode; }
			set { m_PixelIntegrationMode = value; }
		}

		public void SetupFrameIntegration(int framesToIntegrate, FrameIntegratingMode frameMode, PixelIntegrationType pixelIntegrationType)
		{
			m_FramesToIntegrate = framesToIntegrate;
			m_FrameIntegration = frameMode;
			m_PixelIntegrationMode = pixelIntegrationType;
		}

	    public FrameIntegratingMode FrameIntegratingMode
	    {
	        get
	        {
	            return m_FrameIntegration;
	        }
	    }

	    public int FramesToIntegrate
	    {
            get
            {
                return m_FramesToIntegrate;
            }	        
	    }

		public void SetFrameRenderer(IVideoFrameRenderer frameRenderer)
		{
			m_FrameRenderer = frameRenderer;
		}

		public void OpenVideo(IFrameStream frameStream)
		{
			EnsureClosed();

			m_VideoStream = frameStream;

			this.m_IsRunning = false;

			m_MillisecondsPerFrame = (int)m_VideoStream.MillisecondsPerFrame;
			m_CurrentFrameIndex = m_VideoStream.FirstFrame - 1;
			m_FramesToIntegrate = 0;
			m_FrameIntegration = FrameIntegratingMode.NoIntegration;
		}

		public void CloseVideo()
		{
			EnsureClosed();
		}

		public int FrameStep
		{
			get { return (int)m_Step; }
		}

		/// <summary>Start the video playback</summary>
		public void Start(FramePlaySpeed mode, uint step)
		{
			if (m_VideoStream != null)
			{
				m_Step = step;
				m_IsRunning = true;

				int bufferSize = m_VideoStream.RecommendedBufferSize;

				if (mode == FramePlaySpeed.Fastest)
					m_MillisecondsPerFrame = 0;
				else if (mode == FramePlaySpeed.Slower)
					m_MillisecondsPerFrame = m_MillisecondsPerFrame * 2;

				if (bufferSize < 2)
				{
					if (m_FrameIntegration == FrameIntegratingMode.SteppedAverage)
						throw new NotSupportedException("Buffer size must be larger than 2 for stepped averaging software integration to work properly");
					else
						m_PlayerThread = new Thread(new ThreadStart(Run));
				}
				else
				{
					m_PlayerThread = new Thread(new ThreadStart(RunBufferred));
					m_PlayerThread.IsBackground = true;
					m_PlayerThread.SetApartmentState(ApartmentState.MTA);

					m_BufferNextFrameThread = new Thread(new ParameterizedThreadStart(BufferNextFrame));
					m_BufferNextFrameThread.IsBackground = true;
					m_BufferNextFrameThread.SetApartmentState(ApartmentState.MTA);

					m_BufferNextFrameThread.Start(new FrameBufferContext()
					{
						BufferSize = bufferSize,
						FirstFrameNo = m_CurrentFrameIndex
					});
				}

				m_PlayerThread.Start();
			}
		}

		internal class FrameBufferContext
		{
			public int BufferSize;
			public int FirstFrameNo;
		}

		internal class BufferedFrame
		{
			public Pixelmap Image;
			public int FrameNo;
            public int FirstFrameInIntegrationPeriod;
		}

		private int m_CurrentFrameIndex = -1;

		public void StepForward()
		{
			if (m_VideoStream != null)
			{
				if (m_IsRunning)
					// No single frame movement or refresh when the video is 'playing'
					return;

				m_CurrentFrameIndex++;
				if (m_CurrentFrameIndex >= m_VideoStream.LastFrame) m_CurrentFrameIndex = m_VideoStream.LastFrame;

				DisplayCurrentFrame(MovementType.Step);
			}
		}

		public void StepForward(int secondsForward)
		{
			if (m_VideoStream != null)
			{
				if (m_IsRunning)
					// No single frame movement or refresh when the video is 'playing'
					return;

				var advStream = m_VideoStream as AstroDigitalVideoStream;
				if (advStream != null)
				{
					AdvFrameInfo currStatusChannel = advStream.GetStatusChannel(m_CurrentFrameIndex);
					if (currStatusChannel.HasTimeStamp)
					{
						int targetFrame = m_CurrentFrameIndex + 1;

						while (targetFrame <= m_VideoStream.LastFrame)
						{
							AdvFrameInfo statusChannel = advStream.GetStatusChannel(targetFrame);
							if (statusChannel.HasTimeStamp)
							{
								TimeSpan ts =
									new TimeSpan(statusChannel.MiddleExposureTimeStamp.Ticks - currStatusChannel.MiddleExposureTimeStamp.Ticks);
								if (ts.TotalSeconds >= secondsForward)
								{
									m_CurrentFrameIndex = targetFrame;
									break;
								}
							}
							else if (targetFrame == m_VideoStream.LastFrame)
							{
								// We have reached the end of the video
								m_CurrentFrameIndex = targetFrame;
								break;
							}
							targetFrame++;
						}
					}
					else if (advStream.FrameRate > 0)
					{
						m_CurrentFrameIndex += (int) Math.Round(secondsForward*advStream.FrameRate);
					}
					else
						m_CurrentFrameIndex++;
				}
				else
				{
					m_CurrentFrameIndex += (int) Math.Round(secondsForward*m_VideoStream.FrameRate);
				}

				if (m_CurrentFrameIndex >= m_VideoStream.LastFrame) m_CurrentFrameIndex = m_VideoStream.LastFrame;
				DisplayCurrentFrame(MovementType.Jump);
			}
		}

		public void RefreshCurrentFrame()
		{
			if (m_IsRunning)
				// No single frame movement or refresh when the video is 'playing'
				return;

			DisplayCurrentFrame(MovementType.Refresh);
		}

        public void MoveToFrame(int frameId)
        {
	        if (m_IsRunning)
		        // No single frame movement or refresh when the video is 'playing'
		        return;

			m_CurrentFrameIndex = frameId;

			if (m_VideoStream != null)
			{
				if (m_CurrentFrameIndex >= m_VideoStream.LastFrame) m_CurrentFrameIndex = m_VideoStream.LastFrame;
				if (m_CurrentFrameIndex < m_VideoStream.FirstFrame) m_CurrentFrameIndex = m_VideoStream.FirstFrame;

				DisplayCurrentFrame(MovementType.Jump);				
			}
		}

		public Pixelmap GetFrame(int frameNo, bool noIntegrate)
		{
			Pixelmap currentBitmap = null;

			if (m_VideoStream != null)
			{
				if (frameNo >= m_VideoStream.LastFrame) frameNo = m_VideoStream.LastFrame;
				if (frameNo < m_VideoStream.FirstFrame) frameNo = m_VideoStream.FirstFrame;

				try
				{
					if (noIntegrate)
						currentBitmap = m_VideoStream.GetPixelmap(frameNo);
					else
						currentBitmap = m_VideoStream.GetIntegratedFrame(frameNo, m_FramesToIntegrate,
						                                                 m_FrameIntegration == FrameIntegratingMode.SlidingAverage,
						                                                 m_PixelIntegrationMode == PixelIntegrationType.Median);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			}

			return currentBitmap;
		}

        public Pixelmap GetIntegratedFrame(int startFrameNo, int framesToIntegrate, bool isSlidingIntegration, bool isMedianAveraging)
        {
			return m_VideoStream != null
				? m_VideoStream.GetIntegratedFrame(startFrameNo, framesToIntegrate, isSlidingIntegration, isMedianAveraging)
				: null;
        }

		public FrameStateData GetFrameStatusChannel(int index)
		{
			if (m_VideoStream is AstroDigitalVideoStream)
				return ((AstroDigitalVideoStream) m_VideoStream).GetFrameStatusChannel(index);
			else
				return new FrameStateData();
		}

	    public void StepBackward()
		{
			if (m_VideoStream != null)
			{
				if (m_IsRunning)
					// No single frame movement or refresh when the video is 'playing'
					return;

				m_CurrentFrameIndex--;

				if (m_CurrentFrameIndex < m_VideoStream.FirstFrame) m_CurrentFrameIndex = m_VideoStream.FirstFrame;

				DisplayCurrentFrame(MovementType.StepBackwards);				
			}
		}

		public void StepBackward(int secondsBackward)
		{
			if (m_VideoStream != null)
			{
				if (m_IsRunning)
					// No single frame movement or refresh when the video is 'playing'
					return;

				AstroDigitalVideoStream advStream = m_VideoStream as AstroDigitalVideoStream;
				if (advStream != null)
				{
					AdvFrameInfo currStatusChannel = advStream.GetStatusChannel(m_CurrentFrameIndex);
					if (currStatusChannel.HasTimeStamp)
					{
						int targetFrame = m_CurrentFrameIndex - 1;

						while (targetFrame >= m_VideoStream.FirstFrame)
						{
							AdvFrameInfo statusChannel = advStream.GetStatusChannel(targetFrame);
							if (statusChannel.HasTimeStamp)
							{
								TimeSpan ts =
									new TimeSpan(currStatusChannel.MiddleExposureTimeStamp.Ticks - statusChannel.MiddleExposureTimeStamp.Ticks);
								if (ts.TotalSeconds >= secondsBackward)
								{
									m_CurrentFrameIndex = targetFrame;
									break;
								}
							}
							else if (targetFrame == m_VideoStream.FirstFrame)
							{
								// We have reached the beginning of the video
								m_CurrentFrameIndex = targetFrame;
								break;
							}
							targetFrame--;
						}
					}
					else if (advStream.FrameRate > 0)
					{
						m_CurrentFrameIndex -= (int) Math.Round(secondsBackward*advStream.FrameRate);
					}
					else
						m_CurrentFrameIndex--;
				}
				else
				{
					m_CurrentFrameIndex -= (int)Math.Round(secondsBackward * m_VideoStream.FrameRate);
				}

				if (m_CurrentFrameIndex < m_VideoStream.FirstFrame) m_CurrentFrameIndex = m_VideoStream.FirstFrame;

				DisplayCurrentFrame(MovementType.Jump);				
			}
		}

		private void DisplayCurrentFrame(MovementType movementType)
		{
			if (m_VideoStream != null)
            {
                Pixelmap currentBitmap = null;

                if (m_FrameIntegration == FrameIntegratingMode.NoIntegration)
				{
					currentBitmap = m_VideoStream.GetPixelmap(m_CurrentFrameIndex);
				}
				else if (m_FrameIntegration == FrameIntegratingMode.SlidingAverage)
				{
                    currentBitmap = ProduceRunningAverageIntegratedFrame(m_CurrentFrameIndex);
				}
				else if (m_FrameIntegration == FrameIntegratingMode.SteppedAverage)
				{
                    currentBitmap = ProduceBinningIntegratedFrame(m_CurrentFrameIndex);
				}
				else
				    throw new NotSupportedException();

				DisplayCurrentFrameInternal(movementType, currentBitmap);
			}
		}

        private void DisplayCurrentFrameNoIntegrate(MovementType movementType)
        {
            if (m_VideoStream != null)
            {
                Pixelmap currentBitmap = m_VideoStream.GetPixelmap(m_CurrentFrameIndex);

                DisplayCurrentFrameInternal(movementType, currentBitmap);
            }
        }

		private void DisplayCurrentFrameInternal(MovementType movementType, Pixelmap currentPixelmap)
		{
			if (m_VideoStream != null)
			{
				if (m_CurrentFrameIndex >= m_VideoStream.FirstFrame &&
				    m_CurrentFrameIndex <= m_VideoStream.LastFrame)
				{
					m_FrameRenderer.RenderFrame(m_CurrentFrameIndex, currentPixelmap, movementType, false, 0, m_CurrentFrameIndex);
				}
			}
		}

	    public int CurrentFrameIndex
	    {
	        get
	        {
	            if (!m_IsRunning &&
                    m_CurrentFrameIndex >= m_VideoStream.FirstFrame &&
	                m_CurrentFrameIndex <= m_VideoStream.LastFrame)
	            {
	                return m_CurrentFrameIndex;
	            }

	            return -1;
	        }
	    }

	    private Thread m_BufferNextFrameThread;
		private object m_FrameBitmapLock = new object();
		private Queue<BufferedFrame> m_FramesBufferQueue = new Queue<BufferedFrame>();

		private void BufferNextFrame(object state)
		{
			FrameBufferContext context = (FrameBufferContext)state;
			context.BufferSize = Math.Min(context.BufferSize, 64);

			Trace.WriteLine(string.Format("Frame Player: Bufferring {0} frames starting from {1}", context.BufferSize, context.FirstFrameNo));

			if (m_FramesBufferQueue.Count > 0)
			{
				lock (m_FrameBitmapLock)
				{
					m_FramesBufferQueue.Clear();
				}
			}

			int nextFrameIdToBuffer = context.FirstFrameNo;

			while (m_IsRunning)
			{
				if (nextFrameIdToBuffer > -1 &&
					m_FramesBufferQueue.Count < context.BufferSize)
				{
					if (nextFrameIdToBuffer < m_VideoStream.LastFrame)
					{
						if (m_FrameIntegration == FrameIntegratingMode.NoIntegration)
						{
							BufferNonIntegratedFrame(nextFrameIdToBuffer);
							nextFrameIdToBuffer += (int)m_Step;
						}
						else if (m_FrameIntegration == FrameIntegratingMode.SlidingAverage)
						{
							BufferRunningAverageIntegratedFrame(nextFrameIdToBuffer);
							nextFrameIdToBuffer += (int)m_Step;
						}
						else if (m_FrameIntegration == FrameIntegratingMode.SteppedAverage)
						{
							BufferBinningIntegratedFrame(nextFrameIdToBuffer);
							nextFrameIdToBuffer += m_FramesToIntegrate;
						}
					}
				}

				Thread.Sleep(1);
			}
		}

		private void BufferNonIntegratedFrame(int nextFrameIdToBuffer)
		{
			Pixelmap bmp = m_VideoStream.GetPixelmap(nextFrameIdToBuffer);

			lock (m_FrameBitmapLock)
			{
			    var bufferedFrame = new BufferedFrame()
			    {
				    FrameNo = nextFrameIdToBuffer,
                    FirstFrameInIntegrationPeriod = nextFrameIdToBuffer,
				    Image = bmp
			    };

				m_FramesBufferQueue.Enqueue(bufferedFrame);				
			}
		}

		private Pixelmap ProduceRunningAverageIntegratedFrame(int firstFrameNoToIntegrate)
		{
			return GetIntegratedFrame(firstFrameNoToIntegrate, m_FramesToIntegrate, true, m_PixelIntegrationMode == PixelIntegrationType.Median);
		}

		private void BufferRunningAverageIntegratedFrame(int nextFrameIdToBuffer)
		{
			Pixelmap thisFrame = ProduceRunningAverageIntegratedFrame(nextFrameIdToBuffer);

			// 4) Produce the integrated bitmap
			lock(m_FrameBitmapLock)
			{
				var bufferedFrame = new BufferedFrame()
				{
					FrameNo = nextFrameIdToBuffer,
                    FirstFrameInIntegrationPeriod = nextFrameIdToBuffer,
					Image = thisFrame
				};

				m_FramesBufferQueue.Enqueue(bufferedFrame);
			}
		}

		private Pixelmap ProduceBinningIntegratedFrame(int firstFrameNoToIntegrate)
		{
			return GetIntegratedFrame(firstFrameNoToIntegrate, m_FramesToIntegrate, false, m_PixelIntegrationMode == PixelIntegrationType.Median);
		}

		private void BufferBinningIntegratedFrame(int nextFrameIdToBuffer)
		{
			for (int i = 0; i < m_FramesToIntegrate; i++)
			{
				Pixelmap thisFrame = ProduceBinningIntegratedFrame(nextFrameIdToBuffer);

				lock (m_FrameBitmapLock)
				{
					var bufferedFrame = new BufferedFrame()
					{
						FrameNo = nextFrameIdToBuffer + i,
                        FirstFrameInIntegrationPeriod = nextFrameIdToBuffer,
						Image = thisFrame
					};

					m_FramesBufferQueue.Enqueue(bufferedFrame);
				}
			}
		}

		/// <summary>Extract and display the frames</summary>
		private void RunBufferred()
		{
			m_FramesBufferQueue.Clear();
			try
			{
				m_FrameRenderer.PlayerStarted();
				int lastFrame = m_VideoStream.LastFrame;

				Stopwatch sw = new Stopwatch();
				for (; (m_CurrentFrameIndex < lastFrame) && m_IsRunning; m_CurrentFrameIndex += (int)m_Step)
				{
					if (m_CurrentFrameIndex >= lastFrame)
						break;

					if (m_MillisecondsPerFrame != 0)
						sw.Start();

					BufferedFrame currentFrame = null;

					while (m_FramesBufferQueue.Count == 0)
					{
						Thread.Sleep(1);
					}

					bool taken = false;
					lock (m_FrameBitmapLock)
					{
						try
						{
							currentFrame = m_FramesBufferQueue.Dequeue();
						}
						catch (InvalidOperationException)
						{
							// Queue is empty
							currentFrame = null;
						}						
					}
					
					if (currentFrame == null) continue;
					if (currentFrame.FrameNo < m_CurrentFrameIndex) continue;
					if (currentFrame.FrameNo > m_CurrentFrameIndex)
					{
						Trace.WriteLine(string.Format("Frame Player: {0} frame(s) dropped by the rendering engine.", currentFrame.FrameNo - m_CurrentFrameIndex));

						// This will potentially skip a frame
						m_CurrentFrameIndex = currentFrame.FrameNo;
					}

					Pixelmap currentPixelmap =
						currentFrame.FrameNo > m_CurrentFrameIndex ?
						null : currentFrame.Image;

					int msToWait = -1;
					if (m_MillisecondsPerFrame != 0)
					{
						sw.Stop();
						msToWait = m_MillisecondsPerFrame - (int)sw.ElapsedMilliseconds;
						sw.Reset();
					}

				    Debug.Assert(currentFrame.FrameNo == m_CurrentFrameIndex);

					//show frame
					m_FrameRenderer.RenderFrame(
                        currentFrame.FrameNo, 
                        currentPixelmap, 
                        MovementType.Step,
						m_CurrentFrameIndex + m_Step >= lastFrame, 
                        msToWait,
                        currentFrame.FirstFrameInIntegrationPeriod);

					Thread.Sleep(1);
				}
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			finally
			{
				m_IsRunning = false;
			}

			m_FrameRenderer.PlayerStopped();
		}

		/// <summary>Extract and display the frames</summary>
		private void Run()
		{
			try
			{
				m_FrameRenderer.PlayerStarted();
				int lastFrame = m_VideoStream.LastFrame;

				Stopwatch sw = new Stopwatch();
				for (; (m_CurrentFrameIndex < lastFrame) && m_IsRunning; m_CurrentFrameIndex += (int)m_Step)
				{
					if (m_CurrentFrameIndex >= lastFrame)
						break;

					if (m_MillisecondsPerFrame != 0)
						sw.Start();

					Pixelmap currentPixelmap = m_VideoStream != null ? m_VideoStream.GetPixelmap(m_CurrentFrameIndex) : null;

					int msToWait = -1;
					if (m_MillisecondsPerFrame != 0)
					{
						sw.Stop();
						msToWait = m_MillisecondsPerFrame - (int)sw.ElapsedMilliseconds;
						sw.Reset();
					}

					//show frame
					m_FrameRenderer.RenderFrame(
								m_CurrentFrameIndex, 
								currentPixelmap, 
								MovementType.Step,
								m_CurrentFrameIndex + m_Step >= lastFrame,
								msToWait,
                                m_CurrentFrameIndex);
				}
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("FramePlayer:Run() -> " + ex.ToString());
			}
			finally
			{
				m_IsRunning = false;
			}

			m_FrameRenderer.PlayerStopped();
		}

		/// <summary>Stop the video playback</summary>
		public void Stop()
		{
			m_IsRunning = false;
		}

		public void Dispose()
		{
			EnsureClosed();
		}

		private void EnsureClosed()
		{
			if (m_VideoStream != null)
			{
				IDisposable disp = m_VideoStream as IDisposable;
				if (disp != null) disp.Dispose();

				m_VideoStream = null;
			}
		}

		public bool IsAstroDigitalVideo
		{
			get
			{
				return m_VideoStream is AstroDigitalVideoStream && ((AstroDigitalVideoStream)m_VideoStream).Engine == "ADV";
			}
		}

		public GeoLocationInfo GeoLocation
		{
			get
			{
				return m_VideoStream != null
					       ? ((AstroDigitalVideoStream) m_VideoStream).GeoLocation
					       : null;
			}
		}

		public bool IsAstroAnalogueVideo
		{
			get
			{
				return m_VideoStream is AstroDigitalVideoStream && ((AstroDigitalVideoStream)m_VideoStream).Engine == "AAV";
			}
		}

	    public bool AstroAnalogueVideoHasOcrData
	    {
	        get
	        {
                return IsAstroAnalogueVideo && ((AstroDigitalVideoStream)m_VideoStream).OcrDataAvailable;
	        }
	    }

        public int AstroAnalogueVideoIntegratedAAVFrames
	    {
	        get
	        {
                return IsAstroAnalogueVideo ? ((AstroDigitalVideoStream)m_VideoStream).IntegratedAAVFrames : -1;
	        }
	    }

		public string AstroVideoCameraModel
		{
			get
			{
				if (IsAstroAnalogueVideo || IsAstroDigitalVideo)
					return ((AstroDigitalVideoStream) m_VideoStream).CameraModel;
				else
					return null;
			}
		}

		public string AstroVideoNativeVideoStandard
		{
			get
			{
				if (IsAstroAnalogueVideo)
					return ((AstroDigitalVideoStream)m_VideoStream).VideoStandard;
				else
					return null;
			}
		}
	}
}
